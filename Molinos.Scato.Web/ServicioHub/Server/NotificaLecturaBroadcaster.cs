using Microsoft.AspNet.SignalR.Hubs;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Web.ServicioHub.Server
{
    public static class NotificaLecturaBroadcaster
    {
        public static void NotificarLectura(IHubConnectionContext<dynamic> clients, LecturaPuestoDeTrabajoDto notificacion)
        {
            var grupo = notificacion.CentroId + "|" + notificacion.PuestoDeTrabajoId;
            clients.Group(grupo).informarLectura(new
            {
                notificacion.NumeroDeTarjeta,
                notificacion.PrimerNumeroDeTarjeta,
                notificacion.PuestoDeTrabajoId,
                notificacion.TarjetaValida,
                notificacion.MensajeError,
                notificacion.EsTarjetaSupervisor,
                notificacion.PatenteLeida,
                notificacion.Patente,
                notificacion.OcrActivo,
                notificacion.ReconocimientoExitoso
            });
        }

        public static void NotificarLecturaCpe(IHubConnectionContext<dynamic> clients, LecturaCpeDto notificacion)
        {
            var grupo = notificacion.CentroId + "|" + notificacion.PuestoId;
            clients.Group(grupo).informarLecturaCpe(new
            {
                notificacion.NroCtg
            });
        }

        public static void NotificarEstadoConexion(IHubConnectionContext<dynamic> clients, EstadoConexionDto notificacion)
        {
            var grupo = notificacion.CentroId + "|" + notificacion.PuestoDeTrabajoId;
            clients.Group(grupo).informarEstadoConexion(new
            {
                notificacion.PuestoDeTrabajoId,
                notificacion.Estado,
                notificacion.Mensaje
            });
        }

        public static void NotificarCambioEstadoIntercomunicador(IHubConnectionContext<dynamic> clients, EstadoIntercomunicadorDto estado)
        {
            clients.Group(Constantes.NotificacionGrupos.Intercomunicador).actualizarEstadoIntercomunicador(estado);
        }

        public static void NotificarLecturaPagoTasaMunicipal(IHubConnectionContext<dynamic> clients, LecturaPuestoDeTrabajoDto notificacion)
        {
            var grupo = notificacion.CentroId + "|" + notificacion.PuestoDeTrabajoId;
            clients.Group(grupo).informarLecturaPagoTasaMunicipal(new
            {
                notificacion.NumeroDeTarjeta,
                notificacion.Patente,
                notificacion.PuestoDeTrabajoId,
                notificacion.TarjetaValida,
                notificacion.MensajeError,
                notificacion.TipoAlerta,
                notificacion.MensajeAlerta,
                notificacion.Rechazado
            });
        }

        public static void NotificarEncolamientoCalado(IHubConnectionContext<dynamic> clients, ColaIdentificacionVehicularDto notificacion)
        {
            var grupo = Constantes.NotificacionGrupos.Calado + notificacion.PuestoDeTrabajoId.ToString();
            clients.Group(grupo).informarEncolamientoCalado(notificacion);
        }
    }
}
