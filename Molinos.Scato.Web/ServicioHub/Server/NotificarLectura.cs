using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Web.ServicioHub.Server
{
    [HubName("notificaLectura")]
    public class NotificaLectura : Hub
    {
        public void EscucharPuestosDeTrabajo(string centroId, string puestoId)
        {
            Groups.Add(Context.ConnectionId, centroId + "|" + puestoId.ToLower());
        }

        public void UnirseAGrupo(string codigoGrupo)
        {
            Groups.Add(Context.ConnectionId, codigoGrupo);
        }

        public void NotificarLectura(LecturaPuestoDeTrabajoDto notificacion)
        {
            if (Clients != null) NotificaLecturaBroadcaster.NotificarLectura(Clients, notificacion);
        }

        public void NotificarLecturaCpe(LecturaCpeDto notificacion)
        {
            if (Clients != null) NotificaLecturaBroadcaster.NotificarLecturaCpe(Clients, notificacion);
        }

        public void NotificarEstadoConexion(EstadoConexionDto notificacion)
        {
            if (Clients != null) NotificaLecturaBroadcaster.NotificarEstadoConexion(Clients, notificacion);
        }

        public void NotificarCambioEstadoIntercomunicador(EstadoIntercomunicadorDto estado)
        {
            if (Clients != null) NotificaLecturaBroadcaster.NotificarCambioEstadoIntercomunicador(Clients, estado);
        }

        public void NotificarLecturaPagoTasaMunicipal(LecturaPuestoDeTrabajoDto notificacion)
        {
            if (Clients != null) NotificaLecturaBroadcaster.NotificarLecturaPagoTasaMunicipal(Clients, notificacion);
        }

        public void NotificarEncolamientoCalado(ColaIdentificacionVehicularDto notificacion)
        {
            if (Clients != null) NotificaLecturaBroadcaster.NotificarEncolamientoCalado(Clients, notificacion);
        }
    }
}