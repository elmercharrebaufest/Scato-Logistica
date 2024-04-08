using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Servicios
{
    public interface IExternalServiceException
    {
        string ResponseContent { get; }
        string Message { get; }
        Exception ThrowException(string message, string statusCode, string responseContent = null, Exception exception = null);
        Exception ThrowException(string message, string statusCode, Exception exception);
        Exception ThrowException(string message);
    }
}
