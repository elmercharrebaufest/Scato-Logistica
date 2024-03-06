using Molinos.Scato.Dominio.Dto.OperacionesAPI;
using Newtonsoft.Json;
using Ninject.Extensions.Logging;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;


namespace Molinos.Scato.Servicios.Impl
{
    public class ServicioOperaciones : IServicioOperaciones
    {
        private readonly ILogger log;
        private readonly IExternalServiceException externalServiceException;

        public ServicioOperaciones(ILogger log, IExternalServiceException externalServiceException)
        {
            this.log = log;
            this.externalServiceException = externalServiceException;
        }

        public IEnumerable<OrdenDeCargaDto> ObtenerOrdenesDeCarga(string patente)
        {
            const string RECURSO = "ObtenerOrdenesDeCarga1";
            const bool FASON = true;
            const bool FAS = false;

            if (string.IsNullOrWhiteSpace(patente))
                throw externalServiceException.ThrowException("La patente no puede ser nula o estar vacía.");

            var client = CrearCliente();
            var request = CrearRequest(RECURSO);
            IRestResponse<IEnumerable<OrdenDeCargaDto>> restResponse;

            request.AddParameter("patenteChasis", patente);
            request.AddParameter("fason", FASON);
            request.AddParameter("fas", FAS);

            log.Trace("Se ejecuta la consulta a la Api");

            try
            {
               restResponse = client.Get<IEnumerable<OrdenDeCargaDto>>(request);
            }
            catch (Exception ex)
            {
                throw externalServiceException.ThrowException("Error general al consumir el servicio externo MOAOperaciones.", ex.Message, ex);
            }

            if (restResponse.IsSuccessful)
            {
                return restResponse.Data;

            } else {
                
                string message = "Error al procesar la solicitud.";
                string RestMessage = "";
                
                try
                {
                    var errorContent = JsonConvert.DeserializeObject<Dictionary<string, string>>(restResponse.Content);

                    if (errorContent != null && errorContent.ContainsKey("Message"))
                    {
                        RestMessage += errorContent["Message"];
                    }
                }
                catch (JsonException jsonEx)
                {
                    log.Trace("Error: " + restResponse.Content);
                    throw externalServiceException.ThrowException("Error al deserializar la respuesta del servicio externo.", jsonEx.Message, jsonEx);
                }

                log.Trace(message + " Código de estado: " + (int)restResponse.StatusCode + ". Causa: " + RestMessage);

                switch ((HttpStatusCode)restResponse.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        throw externalServiceException.ThrowException("Parámetros de solicitud incorrectos. Verifique los parámetros enviados a MoaOperaciones");

                    case HttpStatusCode.Unauthorized:
                    case HttpStatusCode.Forbidden:
                        throw externalServiceException.ThrowException("Error de autenticación. Token inválido o falta de permisos en MoaOperaciones");

                    case HttpStatusCode.NotFound:
                        throw externalServiceException.ThrowException("El endpoint de MoaOperaciones especificado no fue encontrado o el servicio no responde. Verifique la URL del servicio.");

                    case HttpStatusCode.RequestTimeout:
                        throw externalServiceException.ThrowException("El Servicio de MoaOperaciones ha excedido el tiempo de espera de 5 segundos.");

                    default:
                        
                        if (restResponse.StatusCode == 0)
                            throw externalServiceException.ThrowException(message + " Código de estado: " + (int)restResponse.StatusCode + ". Causa: ServicioOperaciones Responde:" +  restResponse.ErrorException.Message);

                        throw externalServiceException.ThrowException(message + " Código de estado: " + (int)restResponse.StatusCode + ". Causa: ServicioOperaciones Responde:" + RestMessage);
                }
            }
 
        }

        public void InformarViajeOrdenesDeCargaFason()
        {
            throw new NotImplementedException();
        }

        public IRestClient CrearCliente()
        {
            log.Trace("Empieza el método ORDEN FASON");
            log.Trace("Se crean variables de url, token y resource");
            string url = ConfigurationManager.AppSettings["URLOperacionesAPI"]; // /externalApi/external/api/


            log.Trace("Se inicializa RestClien y parametros de configuracion");
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var client = new RestClient(url);
            client.Timeout = 5000;
            client.UserAgent = "RestSharp v106";

            return client;
        }

        public IRestRequest CrearRequest(string recurso)
        {
            string token = ConfigurationManager.AppSettings["APITokenOperacionesAPI"];
            log.Trace("Se inicializa RestRequest y se agrega token");
            var request = new RestRequest(recurso);
            request.AddHeader("X-Api-Key", token);

            return request;
        }
    }

}
