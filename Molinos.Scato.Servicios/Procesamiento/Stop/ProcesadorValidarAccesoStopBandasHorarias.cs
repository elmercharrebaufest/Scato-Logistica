using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Dto.WebAPI;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Newtonsoft.Json;
using Ninject.Extensions.Logging;
using NPOI.SS.Formula.Functions;
using RestSharp;
using System;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorValidarAccesoStopBandasHorarias : ProcesadorComando<ValidarAccesoStopBandasHorarias>
    {
        private readonly IServicioComandos servicioComandos;

        public ProcesadorValidarAccesoStopBandasHorarias(
            IRepositorio repositorio,
            IConversor conversor,
            ILogger log,
            IServicioComandos servicioComandos)
            : base(repositorio, conversor, log)
        {
            this.servicioComandos = servicioComandos;
        }

        public override Resultado Ejecutar(ValidarAccesoStopBandasHorarias comando)
        {
            Log.Info("STOP - Paso 6: ValidarAccesoStopBandasHorarias, ProcesadorValidarAccesoStopBandasHorarias");            

            var resultado = new Resultado();

            var yaValidado = Repositorio.Existe<LogValidacionAccesoStopBandasHorarias>(x => x.CTG == comando.CTG && x.Semaforo != "Blanco");
            if (yaValidado)
            {
                resultado.Error("Error", Textos.Stop_Error_AccesoYaValidado);
                return resultado;
            }
            
            var fechaAcceso = comando.Fecha ?? DateTime.Now;

            var logDto = new LogValidacionAccesoStopBandasHorariasDto
            {
                CTG = comando.CTG,
                Patente = comando.Patente,
                FechaIngreso = fechaAcceso,
                Semaforo = "Blanco",
                Mensaje = Textos.Stop_Error_ServicioNoDisponible,
                Reintentos = comando.Reintentos
            };


            if (logDto.Reintentos >= 3)
            {
                resultado.Error("Error", "Cantidad máxima de reintentos alcanzada");
                return resultado;
            }

            try
            {
                var client = WebAPIRestClientFactory.GenerarClienteWebAPI();
                var request = new RestRequest(RecursoWebAPI.STOP.ValidarAcceso, Method.GET);
                request.AddQueryParameter("ctg", comando.CTG);
                request.AddQueryParameter("fechaHoraAcceso", fechaAcceso.ToString("yyyy-MM-ddTHH:mm:ss"));
                Log.Debug("ValidarAccesoBandaHoraria - CTG: {0}, Patente: {1}", comando.CTG, comando.Patente);

                var responseWebAPI = client.Execute(request);
                if (!responseWebAPI.IsSuccessful)
                {
                    logDto.Reintentos++;
                    resultado.Error("Error", Textos.Stop_Error_ServicioNoDisponible);
                }
                else
                {
                    logDto.RespuestaStop = responseWebAPI.Content;
                    var response = JsonConvert.DeserializeObject<ResponseWebAPIDto<ResultadoValidarAccesoBandaHoraria>>(responseWebAPI.Content);
                    if (response == null)
                    {
                        logDto.Reintentos++;
                        resultado.Error("Error", Textos.Stop_Error_ServicioNoDisponible);
                    }
                    else if (!response.IsValid)
                    {
                        logDto.Reintentos++;
                        var mensajeError = response.Messages?.FirstOrDefault()?.Message ?? Textos.Stop_Error_ServicioNoDisponible;
                        logDto.Mensaje = mensajeError;
                        resultado.Error("Error", mensajeError);
                    }
                    else
                    {
                        var datos = response.Data?.data;
                        if (datos != null)
                        {
                            logDto.Permitido = datos.permitido;
                            logDto.Semaforo = datos.semaforo;
                            logDto.Estado = datos.estado;
                            logDto.Mensaje = datos.mensaje;

                            if (datos.turnoActual != null)
                            {
                                logDto.BandaHorariaFecha = datos.turnoActual.fechaAplicacion;
                                logDto.BandaHorariaHoraDesde = datos.turnoActual.horaDesde;
                                logDto.BandaHorariaHoraHasta = datos.turnoActual.horaHasta;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al procesar ValidarAccesoBandaHoraria");
                logDto.Mensaje = Textos.Stop_Error_ServicioNoDisponible;
                resultado.Error("Error", Textos.Stop_Error_ServicioNoDisponible);
            }
            finally
            {
                RegistrarLog(logDto, comando.Usuario);
            }

            return resultado;
        }

        private void RegistrarLog(LogValidacionAccesoStopBandasHorariasDto dto, string usuario)
        {
            Log.Info("STOP - Paso 7: RegistrarLog, ProcesadorValidarAccesoStopBandasHorarias");
            Log.Info("STOP - Paso 7: RegistrarLog, Camion: "+dto.Patente);
            try
            {
                var logExistente = Repositorio.Existe<LogValidacionAccesoStopBandasHorarias>(x => x.CTG == dto.CTG);               

                if (!logExistente)
                {
                    var resultadoLog = servicioComandos.Ejecutar(new CrearLogValidacionAccesoStopBandasHorarias { Dto = dto, Usuario = usuario }) as ResultadoCrear;
                    if (resultadoLog != null && !resultadoLog.HayErrores && !string.IsNullOrEmpty(dto.RespuestaStop))
                    {
                        servicioComandos.Ejecutar(new CrearLogValidacionAccesoStopRespuesta
                        {
                            Dto = new LogValidacionAccesoStopRespuestaDto
                            {
                                LogValidacionAccesoStopBandasHorariasId = resultadoLog.Id,
                                RespuestaStop = dto.RespuestaStop
                            },
                            Usuario = usuario
                        });
                    }
                    
                }
                else
                {                    
                    var resultado = servicioComandos.Ejecutar(new ModificarLogValidacionAccesoStopBandasHorarias { Dto = dto }) as Resultado;

                    if (resultado.HayErrores)
                    {
                        Log.Error("Error en Actualizar Banda Horaria STOP");
                        if (resultado?.HayErrores == true)
                        {
                            foreach (var err in resultado.Errores)
                            {
                                Log.Error($"Error: {err}");
                            }
                        }
                    }
                        
                        
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al registrar log de acceso a banda horaria");
            }
        }
    }
}
