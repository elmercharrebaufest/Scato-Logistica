using System;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto.WebAPI;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Newtonsoft.Json;
using Ninject.Extensions.Logging;
using RestSharp;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorMOAPayCrearModificarCPE : ProcesadorComando<MOAPayCrearModificarCPE>
    {
        public ProcesadorMOAPayCrearModificarCPE(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(MOAPayCrearModificarCPE comando)
        {
            var resultado = new Resultado();

            try
            {
                var requestBody = GenerarRequestBody(comando);

                var responseWebAPI = EnviarRequest(requestBody);

                if (responseWebAPI.IsSuccessful)
                {
                    ProcesarRespuestaExitosa(responseWebAPI, resultado, comando.NumeroDocumento);
                }
                else
                {
                    Log.Error("Error en la comunicación con la API en MOAPayCrearModificarCPE");
                    resultado.Errores.Add("Error", "Error en la comunicación con la API");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al procesar MOAPayCrearModificarCPE");
                var errorMessage = ex.InnerException != null ? ex.Message + " " + ex.InnerException.Message : ex.Message;
                resultado.Errores.Add("Error", errorMessage);
            }

            Repositorio.GuardarCambios();
            return resultado;
        }

        private object GenerarRequestBody(MOAPayCrearModificarCPE comando)
        {
            return new
            {
                comando.NumeroDocumento,
                comando.Dominio,
                comando.TipoDeVehiculo,
                comando.CuitInterviniente
            };
        }

        private IRestResponse EnviarRequest(object requestBody)
        {
            var client = WebAPIRestClientFactory.GenerarClienteWebAPI();
            var request = new RestRequest(RecursoWebAPI.MOAPAY.CrearModificar, Method.POST);
            request.AddJsonBody(requestBody);
            return client.Execute(request);
        }

        private void ProcesarRespuestaExitosa(IRestResponse responseWebAPI, Resultado resultado, string numeroDocumento)
        {
            var response = JsonConvert.DeserializeObject<ResponseWebAPIDto<ResultadoMOAPayCrearModificarCPE>>(responseWebAPI.Content);

            if (response.IsValid)
            {
                Log.Debug($"Se registro/modifico la CPE {JsonConvert.SerializeObject(response.Data)}");
            }
            else
            {
                Log.Error($"Error al no se pudo registrar/modificar la CPE {numeroDocumento} {response.Messages.FirstOrDefault()?.Message}");
                resultado.Errores.Add("Error", response.Messages.FirstOrDefault()?.Message);
            }
        }
    }
}