using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;
using System;
using System.Web.Mvc;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.NotificacionAplicacion)]
    public class NotificacionAplicacionController : BaseController
    {
        private ILogger log;
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioNotificarUsuario notificador;

        public NotificacionAplicacionController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos, IConfiguracionProvider config, IServicioNotificarUsuario notificador)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
            this.notificador = notificador;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario)
        {
            var model = this.CargarModel(datosUsuario.NombreUsuario);
            return View(model);
        }

        [DatosUsuario]
        public ActionResult Listar(DatosUsuario datosUsuario)
        {
            var model = this.CargarModel(datosUsuario.NombreUsuario);

            return View("_Listar", model);
        }

        [DatosUsuario]
        public ActionResult ActivarMensaje(DatosUsuario datosUsuario, int idMensaje, string tipoMensaje, string titulo, string detalle)
        {
            var model = new NotificacionAplicacionDto();
            var resultado = (ResultadoCrear)servicioComandos.Ejecutar(new NotificacionAplicacionComando
            {
                Dto = new NotificacionAplicacionDto()
                {
                    FechaAccion = DateTime.Now,
                    TipoAccion = idMensaje > 0 && tipoMensaje == "Alta" ? "Modificacion" : tipoMensaje,
                    Titulo = titulo,
                    Detalle = detalle,
                    Usuario = datosUsuario.NombreUsuario
                }
            });

            model.Usuario = datosUsuario.NombreUsuario;
            model.FechaAccion = DateTime.Now;
            model.Titulo = titulo;
            model.Detalle = detalle;
            model.Usuario = datosUsuario.NombreUsuario;
            model.Id = resultado.Id;

            notificador.Notificar(new NotificacionDto
            {
                Grupo = Constantes.NotificacionGrupos.NotificacionAplicacion,
                TipoAlerta = TipoAlerta.AlertaBrowser
            });

            return View("_NotificacionActiva", model);
        }

        private NotificacionAplicacionDto CargarModel(string nombreUsuario)
        {
            try
            {
                var resultado = servicio.ObtenerNotificacionAplicacion(nombreUsuario);
                return resultado;
            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }
            return null;
        }
    }
}