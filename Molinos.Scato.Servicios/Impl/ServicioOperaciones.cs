using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Dto.OperacionesAPI;
using Newtonsoft.Json;
using Ninject.Extensions.Logging;
using RestSharp;

namespace Molinos.Scato.Servicios.Impl
{
    public class ServicioOperaciones : IServicioOperaciones
    {
        #region -- Fields --

        private readonly ILogger log;
        private readonly IExternalServiceException externalServiceException;
        private readonly IRestClientFactory clientFactory;

        #endregion

        #region -- Constructors --

        public ServicioOperaciones(ILogger log, IExternalServiceException externalServiceException, IRestClientFactory clientFactory)
        {
            this.log = log;
            this.externalServiceException = externalServiceException;
            this.clientFactory = clientFactory;
        }

        #endregion

        #region -- Methods --

        #region -- FASON --

        /// <summary>
        /// Obtiene las órdenes de carga de un vehículo de FASON por patente, siendo su origen MOA Operaciones.
        /// </summary>
        /// <param name="patente">La patente.</param>
        /// <returns>La lista de órdenes de FASON.</returns>
        public IEnumerable<OrdenDeCargaDto> ObtenerOrdenesDeCarga(string patente)
        {
            if (string.IsNullOrWhiteSpace(patente))
                throw externalServiceException.ThrowException("La patente no puede estar vacía.");

            IEnumerable<OrdenDeCargaDto> ordenes = null;
            const string RECURSO = "ObtenerOrdenesDeCarga";
            const bool FASON = true;
            const bool FAS = false;
            
            IRestResponse<IEnumerable<OrdenDeCargaDto>> restResponse;
            
            try
            {
                var request = this.CrearRequest(RECURSO);
                request.AddParameter("patenteChasis", patente);
                request.AddParameter("fason", FASON);
                request.AddParameter("fas", FAS);

                var client = clientFactory.CrearClientOperaciones();
                restResponse = client.Get<IEnumerable<OrdenDeCargaDto>>(request);

                ordenes = restResponse.Data;
            }
            catch (Exception ex)
            {
                throw externalServiceException.ThrowException("Error general al consumir el servicio externo MOAOperaciones.", ex.Message, ex);
            }

            if (!restResponse.IsSuccessful)
                RespuestaError(restResponse);

            return ordenes;
        }

        public void InformarViajeOrdenesDeCargaFason(IngresosEgresosFasonesDto ingresosEgresosFasonesDto)
        {
            const string RECURSO = "InformarViajeOrdenesDeCargaFason";

            if (ingresosEgresosFasonesDto == null)
                throw externalServiceException.ThrowException("El objeto de datos no puede ser nulo.");

            var request = this.CrearRequest(RECURSO);
            IRestResponse restResponse;
            var json = JsonConvert.SerializeObject(ingresosEgresosFasonesDto);

            log.Debug("Se ejecuta la consulta a la Api");
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            try
            {
                var client = clientFactory.CrearClientOperaciones();
                restResponse = client.Post(request);
            }
            catch (Exception ex)
            {
                throw externalServiceException.ThrowException("Error general al consumir el servicio externo.", ex.Message, ex);
            }

            if (restResponse.IsSuccessful)
            {
                log.Debug("Viaje informado con éxito.");
            }
            else
            {
                RespuestaError(restResponse);
            }
        }

        #endregion

        #region -- Residuos / Insumos --

        /// <summary>
        /// Obtiene las órdenes de carga de un vehículo de Residuos/Insumos por patente, siendo su origen MOA Operaciones.
        /// </summary>
        /// <param name="patente">La patente.</param>
        /// <returns>La lista de órdenes de Residuos/Insumos.</returns>
        public IEnumerable<OrdenResiduosDto> ObtenerOrdenesResiduos(string patente)
        {
            if (string.IsNullOrWhiteSpace(patente))
                throw externalServiceException.ThrowException("La patente no puede estar vacía.");

            IEnumerable<OrdenResiduosDto> ordenes = null;
            const string RECURSO = "OrdenesResiduos";
                        
            IRestResponse<IEnumerable<OrdenResiduosDto>> restResponse;

            try
            {
                var request = this.CrearRequest(RECURSO);
                request.AddParameter("patenteChasis", patente);

                var client = clientFactory.CrearClientOperaciones();
                restResponse = client.Get<IEnumerable<OrdenResiduosDto>>(request);

                ordenes = restResponse.Data;
            }
            catch (Exception ex)
            {
                throw externalServiceException.ThrowException("Error general al consumir el servicio externo MOAOperaciones.", ex.Message, ex);
            }            

            if (!restResponse.IsSuccessful)
                this.RespuestaError(restResponse);

            return ordenes;
        }
      
        public void InformarViajeOrdenesResiduos(IngresosEgresosResiduosDto ingresosEgresosResiduosDto)
        {
            const string RECURSO = "InformarViajeOrdenesResiduos";

            if (ingresosEgresosResiduosDto == null)
                throw externalServiceException.ThrowException("El objeto de datos no puede ser nulo.");

            var request = this.CrearRequest(RECURSO);
            var json = JsonConvert.SerializeObject(ingresosEgresosResiduosDto);

            log.Debug("Se ejecuta la consulta a la Api");
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            IRestResponse restResponse;

            try
            {
                var client = clientFactory.CrearClientOperaciones();
                restResponse = client.Patch(request);
            }
            catch (Exception ex)
            {
                throw externalServiceException.ThrowException("Error general al consumir el servicio externo.", ex.Message, ex);
            }

            if (restResponse.IsSuccessful)
            {
                log.Debug("Viaje informado con éxito.");
            }
            else
            {
                RespuestaError(restResponse);
            }
        }

        #endregion

        #region -- FAS --

        /// <summary>
        /// Obtiene las órdenes de carga de un vehículo de FAS por patente, siendo su origen MOA Operaciones.
        /// </summary>
        /// <param name="patente">La patente.</param>
        /// <returns>La lista de órdenes de FAS.</returns>
        public IEnumerable<OrdenDeCargaDto> ObtenerOrdenesDeCargaFas(string patente)
        {
            if (string.IsNullOrWhiteSpace(patente))
                throw externalServiceException.ThrowException("La patente no puede estar vacía.");

            IEnumerable<OrdenDeCargaDto> ordenes = null;
            const string RECURSO = "ObtenerOrdenesDeCarga";
            const bool FASON = false;
            const bool FAS = true;

            IRestResponse<IEnumerable<OrdenDeCargaDto>> restResponse;

            try
            {
                var request = this.CrearRequest(RECURSO);
                request.AddParameter("patenteChasis", patente);
                request.AddParameter("fason", FASON);
                request.AddParameter("fas", FAS);

                var client = clientFactory.CrearClientOperaciones();
                restResponse = client.Get<IEnumerable<OrdenDeCargaDto>>(request);

                ordenes = restResponse.Data;
            }
            catch (Exception ex)
            {
                throw externalServiceException.ThrowException("Error general al consumir el servicio externo MOAOperaciones.", ex.Message, ex);
            }

            if (!restResponse.IsSuccessful)
                RespuestaError(restResponse);

            return ordenes;
        }

        #endregion

        private void RespuestaError(IRestResponse restResponse)
        {
            ErrorResponse errorContent;

            try
            {
                errorContent = JsonConvert.DeserializeObject<ErrorResponse>(restResponse.Content);
            }
            catch (JsonException ex)
            {
                if (ex != null)
                {
                    throw externalServiceException.ThrowException("Error al deserializar la respuesta del servicio externo.", ex.Message, ex);
                }
                else
                {
                    throw externalServiceException.ThrowException("Error al deserializar la respuesta del servicio MoaOperaciones.");
                }
            }

            log.Debug("Código de estado: " + (int)restResponse.StatusCode + ". Causa: " + (errorContent?.Message ?? restResponse.ErrorMessage));

            switch (restResponse.StatusCode)
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
            
            if (string.IsNullOrEmpty(token))
                throw new NullReferenceException("La propiedad APITokenOperacionesAPI no está configurada.");

            var request = new RestRequest(recurso);
            request.AddHeader("X-Api-Key", token);

            return request;
        }

        #endregion
    }
}