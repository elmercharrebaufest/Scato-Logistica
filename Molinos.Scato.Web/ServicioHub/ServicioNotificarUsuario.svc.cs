using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.ServicioHub.Client;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.Web.ServicioHub
{
    public class ServicioNotificarUsuario : IServicioNotificarUsuario
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;
        private readonly IHubClient hubClientNotificar;
        private readonly IHubClient hubClientNotificarLectura;

        public ServicioNotificarUsuario(
            ILogger log,
            IServicioComandos servicioComandos,
            HubClients hubClients)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
            this.hubClientNotificar = hubClients.Notificar;
            this.hubClientNotificarLectura = hubClients.Lectura;
        }

        public void Notificar(NotificacionDto notificacion)
        {
            try
            {
                log.Debug("Iniciando- NotificarMensaje usuario: {0}, mensaje: {1}", notificacion.Grupo, notificacion.Mensaje);
                notificacion.Hora = DateTime.Now;
                if(notificacion.TipoAlerta != Dominio.Enums.TipoAlerta.NotificacionEstadoWeb 
                    && notificacion.TipoAlerta != Dominio.Enums.TipoAlerta.CambioEstadoBalanzas
                    && notificacion.TipoAlerta != Dominio.Enums.TipoAlerta.CambioEstadoBarrera
                    && notificacion.TipoAlerta != Dominio.Enums.TipoAlerta.CambioEstadoBarreraHidraulica)
                {
                    var resultado = servicioComandos.Ejecutar(new CrearNotificacion { Dto = notificacion }) as ResultadoCrear;
                    if (resultado != null)
                    {
                        notificacion.Id = resultado.Id;
                    }
                }

                hubClientNotificar.Invoke("Notificar", notificacion);

                log.Debug("Fin- Mensaje enviado a usuario: {0} exitosamente", notificacion.Grupo);
            }
            catch (Exception e)
            {
                log.Error(e, "Error al enviar notificación: {0}", notificacion.Mensaje);
            }
        }

        public void NotificarLectura(LecturaCpeDto notificacion)
        {
            try
            {
                log.Debug("Iniciando- NotificarMensaje puesto: {0}, para cpe: {1}", notificacion.PuestoId, notificacion.NroCtg);

                hubClientNotificarLectura.Invoke("Notificar", notificacion);

                log.Debug("Fin- Mensaje enviado a usuario: {0} exitosamente", notificacion.NroCtg);
            }
            catch (Exception e)
            {
                log.Error(e, "Error al enviar notificación: {0}", notificacion.NroCtg);
            }
        }

        public void NotificarEstadoServicioExterno(NotificacionDto notificacion)
        {
            try
            {
                log.Debug("Iniciando- NotificarMensaje usuario: {0}, mensaje: {1}", notificacion.Grupo, notificacion.Mensaje);
                notificacion.Hora = DateTime.Now;
                hubClientNotificar.Invoke("NotificarEstadoServicioExterno", notificacion);

                log.Debug("Fin- Mensaje enviado a usuario: {0} exitosamente", notificacion.Grupo);
            }
            catch (Exception e)
            {
                log.Error(e, "Error al enviar notificación: {0}", notificacion.Mensaje);
            }
        }
    }
}
