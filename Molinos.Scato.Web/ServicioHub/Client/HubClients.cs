using Ninject;

namespace Molinos.Scato.Web.ServicioHub.Client
{
    public class HubClients
    {
        public readonly IHubClient Lectura;
        public readonly IHubClient Notificar;

        public HubClients(
            [Named("notificaLectura")] IHubClient lectura,
            [Named("notificarUsuario")] IHubClient notificar)
        {
            Lectura  = lectura;
            Notificar = notificar;
        }
    }
}
