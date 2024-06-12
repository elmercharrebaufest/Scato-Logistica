using RestSharp;

namespace Molinos.Scato.Servicios
{
    public interface IRestClientFactory
    {
        IRestClient CrearClientOperaciones();
    }
}
