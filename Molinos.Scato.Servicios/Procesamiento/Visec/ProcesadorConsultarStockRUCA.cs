using Molinos.Scato.Dominio.Comandos;
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
    public class ProcesadorConsultarStockRUCA : ProcesadorComando<ConsultarStockRUCA>
    {
        public ProcesadorConsultarStockRUCA(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ConsultarStockRUCA comando)
        {
            var resultado = new Resultado();
            try
            {
                var client = WebAPIRestClientFactory.GenerarClienteWebAPI();
                var request = new RestRequest(RecursoWebAPI.VISEC.ExisteStockRUCA, Method.POST);
                request.AddJsonBody(comando);

                var responseWebAPI = client.Execute(request);
                if (!responseWebAPI.IsSuccessful)
                {
                    resultado.Error("Error", Textos.Visec_Error_Conexion);
                    return resultado;
                }

                var response = JsonConvert.DeserializeObject<ResponseWebAPIDto<ResultadoConsultarStockRUCA>>(responseWebAPI.Content);
                if (response == null)
                {
                    resultado.Error("Error", Textos.Visec_Error_ConexionScato);
                    return resultado;
                }

                if (!response.IsValid)
                {
                    var errorMessage = ExtraerMensajeDeError(response.Messages?.FirstOrDefault()?.Message);
                    resultado.Error("Error", errorMessage);
                    return resultado;
                }

                if (!response.Data.StockDisponible)
                {
                    resultado.Error("Error", Textos.Visec_Error_StockNoDisponible);
                    return resultado;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al procesar ConsultarStockRUCA");
                resultado.Error("Error", Textos.Visec_Error_ConexionScato);
            }
            return resultado;
        }

        private string ExtraerMensajeDeError(string mensaje)
        {
            if (string.IsNullOrEmpty(mensaje))
                return Textos.Visec_Error_ConexionScato;

            try
            {
                var errorResponse = JsonConvert.DeserializeObject<VisecErrorResponse>(mensaje);
                if (errorResponse?.error?.validationErrors?.Any() == true)
                {
                    var firstValidationError = errorResponse.error.validationErrors.First();
                    return firstValidationError.errorMessage;
                }
                else if (errorResponse?.error != null)
                {
                    return errorResponse.error.errorMessage;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al procesar ConsultarStockRUCA ExtraerMensajeDeError");
                return Textos.Visec_Error_ConexionScato;
            }

            return mensaje;
        }
    }
}