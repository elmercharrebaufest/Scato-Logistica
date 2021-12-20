using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ControlDeCalado)]
    public class ControlDeCaladoController : BaseController
    {
        private ILogger log;
        private readonly IServicioComandos servicioComandos;

        public ControlDeCaladoController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos, IConfiguracionProvider config) 
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario)
        {
            var model = this.cargarModel(datosUsuario.CentroId, DateTime.Now.AddHours(-8), DateTime.Now);
            return View(model);
        }

        [DatosUsuario]
        public ActionResult Listar(DatosUsuario datosUsuario, ControlDeCaladoModel modelRecivido)
        {
            var model = this.cargarModel(datosUsuario.CentroId, modelRecivido.FechaDesde, modelRecivido.FechaHasta);

            return View("_Listar", model);
        }

        [DatosUsuario]
        public ActionResult CambiarPinchazos(DatosUsuario datosUsuario, int tipoPinchazo, string motivo)
        {
            var model = new ControlDeCaladoModel();

            var resultado = servicioComandos.Ejecutar(new CambiarPinchazosComando { Dto = new PinchazosPorCaladaDto()
            {
                FechaModificacion = DateTime.Now,
                Motivo = motivo,
                TipoPinchazo = (TipoPinchazo)tipoPinchazo,
                Usuario = new UsuarioDto()
                {
                    NombreUsuario = datosUsuario.NombreUsuario
                }
            }, CentroId = datosUsuario.CentroId
            });

            model.UltimoCambioPinchazo = servicio.UltimoCambioPinchazo(datosUsuario.CentroId);
            string usuario = $"{model.UltimoCambioPinchazo.Usuario.Nombre} {model.UltimoCambioPinchazo.Usuario.Apellido} ({model.UltimoCambioPinchazo.Usuario.NombreUsuario})";
            EnvioMailCambioPinchazo(model.UltimoCambioPinchazo.TipoPinchazo, usuario, model.UltimoCambioPinchazo.FechaModificacion, model.UltimoCambioPinchazo.Motivo);

            return View("_PinchazosPorCalada", model);
        }

        private ControlDeCaladoModel cargarModel(int centroId, DateTime desde, DateTime hasta)
        {
            var model = new ControlDeCaladoModel();
            model.FechaDesde = desde;
            model.FechaHasta = hasta;
            model.CaladosPorHora = servicio.CaladosPorHora(desde, hasta);
            model.UltimoCamion = servicio.UltimoCalado(desde, hasta, centroId);
            model.EstadisticasCalado = servicio.ObtenerEstadisticasCalado(desde, hasta, centroId);
            model.UltimoCambioPinchazo = servicio.UltimoCambioPinchazo(centroId);
            model.CambiarPinchazos = servicio.ObtenerCentro(centroId).ModificaPinchazosPorCalada;

            return model;
        }

        private void EnvioMailCambioPinchazo(TipoPinchazo tipoPinchazo, string usuario, DateTime fecha, string motivo)
        {
            try
            {
                var usuarios = servicio.ObtenerUsuariosCambioPinchazosPorCalada();
                var tipoPinchazoDesc = tipoPinchazo.GetAttributeValue<DescriptionAttribute, string>(x => x.Description);
                if (usuarios.Count > 0)
                {
                    servicioComandos.Ejecutar(new EnvioMail
                    {
                        Destinatarios = usuarios,
                        Titulo = $"Cantidad pinchazos calados a {tipoPinchazoDesc}",
                        Cuerpo = string.Format(Textos.MailCambioPinchazosPorCalada, usuario, tipoPinchazoDesc, fecha.ToString("dd/MM/yyyy HH:mm"), motivo)
                    });
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }
        }
    }
}
