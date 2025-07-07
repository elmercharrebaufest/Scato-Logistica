using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto.WebAPI;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Newtonsoft.Json;
using Ninject.Extensions.Logging;
using RestSharp;
using System;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorMOAPayInformarPagoComoConsumido : ProcesadorComando<MOAPayInformarPagoComoConsumido>
    {
        public ProcesadorMOAPayInformarPagoComoConsumido(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(MOAPayInformarPagoComoConsumido comando)
        {
            var resultado = new Resultado();

            try
            {
                var requestBody = GenerarRequestBody(comando);

                var responseWebAPI = EnviarRequest(requestBody);

                if (responseWebAPI.IsSuccessful)
                {
                    ProcesarRespuestaExitosa(responseWebAPI, resultado);
                }
                else
                {
                    Log.Error("Error en la comunicación con la API en MOAPayInformarPagoComoConsumido");
                    resultado.Errores.Add("Error", "Error en la comunicación con la API");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al procesar MOAPayInformarPagoComoConsumido");
                var errorMessage = ex.InnerException != null ? ex.Message + " " + ex.InnerException.Message : ex.Message;
                resultado.Errores.Add("Error", errorMessage);
            }

            Repositorio.GuardarCambios();
            return resultado;
        }

        private object GenerarRequestBody(MOAPayInformarPagoComoConsumido comando)
        {
            return new
            {
                comando.Id,
                comando.Disponible
            };
        }

        private IRestResponse EnviarRequest(object requestBody)
        {
            var client = WebAPIRestClientFactory.GenerarClienteWebAPI();
            var request = new RestRequest(RecursoWebAPI.MOAPAY.InformarPagoComoConsumido, Method.POST);
            request.AddJsonBody(requestBody);
            return client.Execute(request);
        }

        private void ProcesarRespuestaExitosa(IRestResponse responseWebAPI, Resultado resultado)
        {
            var response = JsonConvert.DeserializeObject<ResponseWebAPIDto<ResultadoMOAPayInformarPagoComoConsumido>>(responseWebAPI.Content);

            if (!response.IsValid)
            {
                Log.Error(response.Messages.FirstOrDefault()?.Message);
                resultado.Errores.Add("Error", response.Messages.FirstOrDefault()?.Message);
            }
        }
    }
}