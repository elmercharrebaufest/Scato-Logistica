using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Comandos.ResultadoServicio;
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
    public class ProcesadorConsultarEstadoTransmisionVisec : ProcesadorComando<ConsultarEstadoTransmisionVisec>
    {
        public ProcesadorConsultarEstadoTransmisionVisec(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ConsultarEstadoTransmisionVisec comando)
        {
            var resultado = new ResultadoConsultarEstadoTransmisionVisec();
            
            try
            {
                Log.Info($"Consultando estado de transmisión VISEC para proceso: {comando.NumeroProceso}");

                var client = WebAPIRestClientFactory.GenerarClienteWebAPI();
                var request = new RestRequest(RecursoWebAPI.VISEC.ConsultarEstadoImportacion, Method.POST);
                request.AddJsonBody(new
                {
                    NumeroProceso = comando.NumeroProceso,
                    CuitEmpresaResponsable = Constantes.ValoresPorDefecto.CuitMOA.ToString()
                });

                var responseWebAPI = client.Execute(request);
                if (!responseWebAPI.IsSuccessful)
                {
                    resultado.Error("Error", Textos.Visec_Error_Conexion);
                    return resultado;
                }

                var response = JsonConvert.DeserializeObject<ResponseWebAPIDto<ResultadoConsultarEstadoImportacionVisec>>(responseWebAPI.Content);
                if (response == null)
                {
                    resultado.Error("Error", "Respuesta nula de la API VISEC");
                    return resultado;
                }

                if (!response.IsValid)
                {
                    var errorMessage = response.Messages?.Any() == true 
                        ? string.Join(", ", response.Messages.Select(m => m.Message ?? "Error desconocido"))
                        : Textos.Visec_Error_ConexionScato;
                    
                    resultado.Error("Error", errorMessage);
                    return resultado;
                }

                if (response.Data == null)
                {
                    resultado.Error("Error", "Datos nulos en la respuesta de la API VISEC");
                    return resultado;
                }

                if (response.Data.CodigoEstado == 4)
                {
                    var detalle = response.Data.Detalles.FirstOrDefault();
                    var error = detalle.Errores.FirstOrDefault();
                    var errorMessage = error.Observaciones ?? "Error desconocido"; 
                    resultado.Error("Error", errorMessage);
                    resultado.Estado = EstadoTransmisionAVisec.ErrorVisec;
                    return resultado;
                }

                if (response.Data.CodigoEstado == 3)
                    resultado.Estado = EstadoTransmisionAVisec.Finalizado;
            }
            catch (JsonException jsonEx)
            {
                Log.Error(jsonEx, $"Error de deserialización JSON al procesar ConsultarEstadoTransmisionVisec para proceso: {comando.NumeroProceso}");
                resultado.Errores.Add("Error", "Error al procesar la respuesta de la API VISEC");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error al procesar ConsultarEstadoTransmisionVisec para proceso: {comando.NumeroProceso}");
                resultado.Errores.Add("Error", Textos.Visec_Error_ConexionScato);
            }
            return resultado;
        }
    }
}