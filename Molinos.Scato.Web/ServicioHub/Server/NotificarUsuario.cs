using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Web.ServicioHub.Server
{
    [HubName("notificarUsuario")]
    public class NotificarUsuario : Hub
    {
        public void UnirseAGrupo(string groupNames)
        {
            foreach (var groupName in groupNames.Split(','))
            {
                Groups.Add(Context.ConnectionId, groupName.ToLower());
            }
        }

        public void Notificar(NotificacionDto notificacion)
        {
            if (Clients != null) Clients.OthersInGroup(notificacion.Grupo.ToLower()).actualizarNotificaciones(notificacion);
        }


        public void NotificarEstadoServicioExterno(NotificacionDto notificacion)
        {
            if (Clients != null) NotificarUsuarioBroadcaster.NotificarEstadoServicioExterno(Clients, notificacion);
        }
    }
}
