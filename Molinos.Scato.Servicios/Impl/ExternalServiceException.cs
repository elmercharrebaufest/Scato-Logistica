using NPOI.OpenXmlFormats.Shared;
using System;
using System.Net;

namespace Molinos.Scato.Servicios.Impl
{
    public class ExternalServiceException : Exception, IExternalServiceException
    {
        public string ResponseContent { get; }

        public ExternalServiceException(string message, string responseContent, Exception innerException = null)
            : base(message, innerException)
        {
            ResponseContent = responseContent;
        }

        public ExternalServiceException(string message) : base(message)
        {
        }

        public ExternalServiceException()
        {
            
        }

        public Exception ThrowException(string message, string statusCode, string responseContent = null, Exception exception = null)
        {
            throw new ExternalServiceException(message + " Código de estado: " + statusCode + ". Causa: ServicioOperaciones Responde:" + (responseContent ?? string.Empty), responseContent);
        }

        public Exception ThrowException(string message, string statusCode, Exception exception)
        {
            throw new ExternalServiceException(message + " Código de estado: " + statusCode + ". Causa: ServicioOperaciones Responde:" + (ResponseContent ?? string.Empty), ResponseContent, exception);
        }

        public Exception ThrowException(string message)
        {
            throw new ExternalServiceException(message);
        }

    }

}
