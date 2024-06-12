using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Dto.OperacionesAPI;
using Molinos.Scato.Servicios.Properties;
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
        private readonly IRestClientFactory clientFactory;

        public ServicioOperaciones(ILogger log, IExternalServiceException externalServiceException, IRestClientFactory clientFactory)
        {
            this.log = log;
            this.externalServiceException = externalServiceException;
            this.clientFactory = clientFactory;
        }

        public IEnumerable<OrdenDeCargaDto> ObtenerOrdenesDeCarga(string patente)
        {
            const string RECURSO = "ObtenerOrdenesDeCarga";
            const bool FASON = true;
            const bool FAS = false;

            if (string.IsNullOrWhiteSpace(patente))
                throw externalServiceException.ThrowException("La patente no puede ser nula o estar vacía.");

            var request = CrearRequest(RECURSO);
            IRestResponse<IEnumerable<OrdenDeCargaDto>> restResponse;

            request.AddParameter("patenteChasis", patente);
            request.AddParameter("fason", FASON);
            request.AddParameter("fas", FAS);

            log.Trace("Se ejecuta la consulta a la Api");

            try
            {
               var Client = clientFactory.CrearClientOperaciones();
               restResponse = Client.Get<IEnumerable<OrdenDeCargaDto>>(request);
            }
            catch (Exception ex)
            {
                throw externalServiceException.ThrowException("Error general al consumir el servicio externo MOAOperaciones.", ex.Message, ex);
            }

            if (restResponse.IsSuccessful)
            {
                return restResponse.Data;

            } else {
                RespuestaError(restResponse);
                return restResponse.Data;
            }
        }

        public void InformarViajeOrdenesDeCargaFason(IngresosEgresosFasonesDto ingresosEgresosFasonesDto)
        {
            const string RECURSO = "InformarViajeOrdenesDeCargaFason";

            if (ingresosEgresosFasonesDto == null)
                throw externalServiceException.ThrowException("El objeto de datos no puede ser nulo.");


            var request = CrearRequest(RECURSO);
            IRestResponse restResponse;

            var json = JsonConvert.SerializeObject(ingresosEgresosFasonesDto);
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            log.Trace("Se ejecuta la consulta a la Api");

            try
            {
                var Client = clientFactory.CrearClientOperaciones();
                restResponse = Client.Post(request);
            }
            catch (Exception ex)
            {
                throw externalServiceException.ThrowException("Error general al consumir el servicio externo.", ex.Message, ex);
            }

            if (restResponse.IsSuccessful)
            {
                log.Trace("Viaje informado con éxito.");
            }
            else
            {
                RespuestaError(restResponse);
            }
        }

        private void RespuestaError(IRestResponse restResponse)
        {
            string RestMessage = "";
            try
            {
                var errorContent = JsonConvert.DeserializeObject<ErrorResponse>(restResponse.Content);
               // var errorContent = JsonConvert.DeserializeObject<Dictionary<string, string>>(restResponse.Content);

                if (errorContent != null && errorContent.ExceptionMessage != null && errorContent.ExceptionMessage.Contains("Message"))
                {
                    RestMessage += errorContent.ExceptionMessage;
                }
            }
            catch (JsonException jsonEx)
            {
                if (jsonEx != null)
                {
                    throw externalServiceException.ThrowException("Error al deserializar la respuesta del servicio externo.", jsonEx.Message, jsonEx);
                } else
                {
                    throw externalServiceException.ThrowException("Error al deserializar la respuesta del servicio MoaOperaciones.");
                }
            }

            log.Trace(" Código de estado: " + (int)restResponse.StatusCode + ". Causa: " + RestMessage);

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

                case HttpStatusCode.InternalServerError:
                    throw externalServiceException.ThrowException("El Servicio de MoaOperaciones ha respondido con un mensaje de error interno 'InternalServerError'.");

                default:
                    throw restResponse.ErrorException;
            }
        }

        public IRestRequest CrearRequest(string recurso)
        {
            string token = ConfigurationManager.AppSettings["APITokenOperacionesAPI"];
            log.Trace("Se inicializa RestRequest y se agrega token");

            var request = new RestRequest(recurso);
            request.AddHeader("X-Api-Key", token);

            if (string.IsNullOrEmpty(token))
            {
                throw new System.NullReferenceException("La propiedad APITokenOperacionesAPI no está configurada.");
            }
            
            return request;

        }


    }
}
