using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Configuration;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public static class WebAPIRestClientFactory
    {
        public static RestClient GenerarClienteWebAPI()
        {
            var url = ConfigurationManager.AppSettings["UrlBaseWebAPI"];
            var username = ConfigurationManager.AppSettings["UserCredentialWebAPI"];
            var password = ConfigurationManager.AppSettings["PassCredentialWebAPI"];

            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                throw new Exception("Faltan configuraciones para generar el cliente RestSharp.");
            }

            var client = new RestClient(url)
            {
                Authenticator = new HttpBasicAuthenticator(username, Encriptador.Decrypt(password))
            };

            return client;
        }
    }
}