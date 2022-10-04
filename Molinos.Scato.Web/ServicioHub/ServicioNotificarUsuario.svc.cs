using System;
using System.Configuration;
using Microsoft.AspNet.SignalR.Client;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.ServicioHub
{
    public class ServicioNotificarUsuario : IServicioNotificarUsuario
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;
        private readonly HubClientNotificar hubClientNotificar;
        private readonly HubClientNotificar hubClientNotificarLectura;

        public ServicioNotificarUsuario(ILogger log, IServicioComandos servicioComandos, HubClientFactory hubClientFactory)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
            this.hubClientNotificar = hubClientFactory.GetClientNotificar("notificarUsuario");
            this.hubClientNotificarLectura = hubClientFactory.GetClientNotificar("notificarLectura");
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
    }
}
