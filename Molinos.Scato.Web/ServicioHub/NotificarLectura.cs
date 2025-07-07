using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Web.ServicioHub
{
    [HubName("notificaLectura")]
    public class NotificaLectura : Hub
    {
        public void NotificarLectura(LecturaPuestoDeTrabajoDto notificacion)
        {
            if (Clients != null)
            {
                Clients.Group(notificacion.CentroId + "|" + notificacion.PuestoDeTrabajoId).informarLectura(new { notificacion.NumeroDeTarjeta, notificacion.PrimerNumeroDeTarjeta, notificacion.PuestoDeTrabajoId, notificacion.TarjetaValida, notificacion.MensajeError, notificacion.EsTarjetaSupervisor, notificacion.PatenteLeida, notificacion.Patente, notificacion.OcrActivo, notificacion.ReconocimientoExitoso});
            }
        }

        public void NotificarLecturaCpe(LecturaCpeDto notificacion)
        {
            if (Clients != null)
            {
                Clients.Group(notificacion.CentroId + "|" + notificacion.PuestoId).informarLecturaCpe(new { notificacion.NroCtg });
            }
        }

        public void NotificarEstadoConexion(EstadoConexionDto notificacion)
        {
            if (Clients != null)
            {
                Clients.Group(notificacion.CentroId + "|" + notificacion.PuestoDeTrabajoId).informarEstadoConexion(new { notificacion.PuestoDeTrabajoId, notificacion.Estado, notificacion.Mensaje });
            }
        }

        public void EscucharPuestosDeTrabajo(string centroId, string puestoId)
        {
            Groups.Add(Context.ConnectionId, centroId + "|" + puestoId.ToLower());
        }

        public void UnirseAGrupo(string codigoGrupo)
        {
            Groups.Add(Context.ConnectionId, codigoGrupo);
        }

        public void NotificarCambioEstadoIntercomunicador(EstadoIntercomunicadorDto estado)
        {
            if (Clients != null)
            {
                Clients.Group(Constantes.NotificacionGrupos.Intercomunicador).actualizarEstadoIntercomunicador(estado);
            }
        }

        public void NotificarLecturaPagoTasaMunicipal(LecturaPuestoDeTrabajoDto notificacion)
        {
            if (Clients != null)
            {
                Clients.Group(notificacion.CentroId + "|" + notificacion.PuestoDeTrabajoId).informarLecturaPagoTasaMunicipal(new { notificacion.NumeroDeTarjeta, notificacion.Patente, notificacion.PuestoDeTrabajoId, notificacion.TarjetaValida, notificacion.MensajeError, notificacion.TipoAlerta, notificacion.MensajeAlerta });
            }
        }
    }
}