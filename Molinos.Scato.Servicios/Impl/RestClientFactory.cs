using Ninject.Extensions.Logging;
using RestSharp;
using System;
using System.Configuration;
using System.Security.Policy;

namespace Molinos.Scato.Servicios.Impl
{
    public class RestClientFactory : IRestClientFactory
    {
        private readonly string url;
        private readonly ILogger log;

        public RestClientFactory(ILogger log)
        {
            url = ConfigurationManager.AppSettings["URLOperacionesAPI"];
            this.log = log;
        }

        public IRestClient CrearClientOperaciones()
        {
            log.Trace("Se inicializa RestClient y parámetros de configuración");

            if (string.IsNullOrEmpty(url))
            {
                throw new NullReferenceException("La propiedad URLOperacionesAPI no está configurada.");
            }

            try
            {
                var client = new RestClient(url);
                client.Timeout = 5000;
                client.UserAgent = "RestSharp v106";
                return client;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear el cliente REST.", ex);
            }
        }
    }
}
