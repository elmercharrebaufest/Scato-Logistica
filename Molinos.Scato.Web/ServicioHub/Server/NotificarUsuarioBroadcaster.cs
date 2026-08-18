using Microsoft.AspNet.SignalR.Hubs;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Web.ServicioHub.Server
{
    public static class NotificarUsuarioBroadcaster
    {
        public static void Notificar(IHubConnectionContext<dynamic> clients, NotificacionDto notificacion)
        {
            var grupo = notificacion.Grupo.ToLower();
            clients.Group(grupo).actualizarNotificaciones(notificacion);
        }

        public static void NotificarEstadoServicioExterno(IHubConnectionContext<dynamic> clients, NotificacionDto notificacion)
        {
            clients.Group(Constantes.NotificacionGrupos.EstadoServicioExterno.ToLower()).actualizarEstadoServicioExterno(notificacion);
        }
    }
}
