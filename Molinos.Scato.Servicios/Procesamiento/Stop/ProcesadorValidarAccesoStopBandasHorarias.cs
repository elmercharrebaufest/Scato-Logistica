using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Dto.WebAPI;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Newtonsoft.Json;
using Ninject.Extensions.Logging;
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
            var resultado = new Resultado();
            var fechaAcceso = comando.Fecha ?? DateTime.Now;
            var logDto = new LogValidacionAccesoStopBandasHorariasDto
            {
                CTG = comando.CTG,
                Patente = comando.Patente,
                FechaIngreso = fechaAcceso,
                Semaforo = "Blanco",
                Mensaje = Textos.Stop_Error_ServicioNoDisponible
            };

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
                    resultado.Error("Error", Textos.Stop_Error_ServicioNoDisponible);
                }
                else
                {
                    logDto.RespuestaStop = responseWebAPI.Content;
                    var response = JsonConvert.DeserializeObject<ResponseWebAPIDto<ResultadoValidarAccesoBandaHoraria>>(responseWebAPI.Content);
                    if (response == null)
                    {
                        resultado.Error("Error", Textos.Stop_Error_ServicioNoDisponible);
                    }
                    else if (!response.IsValid)
                    {
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
            try
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
            catch (Exception ex)
            {
                Log.Error(ex, "Error al registrar log de acceso a banda horaria");
            }
        }
    }
}
