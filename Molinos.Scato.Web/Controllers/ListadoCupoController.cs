using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.GraficoDePlanta)]
    public class ListadoCupoController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;
        private readonly IConfiguracionProvider configuracion;

        public ListadoCupoController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos, IConfiguracionProvider configuracion)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
            this.configuracion = configuracion;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario)
        {
            var centroId = datosUsuario.CentroId;
            var materiales = ActualizarCuposOtorgados(servicio, servicioComandos, configuracion, datosUsuario.CentroId);

            return View(materiales);
        }

        [DatosUsuario]
        public ActionResult Cabecera(DatosUsuario datosUsuario)
        {
            var centroId = datosUsuario.CentroId;
            var materiales = ActualizarCuposOtorgados(servicio, servicioComandos, configuracion, datosUsuario.CentroId);
            return View(materiales);
        }

        public static List<CupoMobileDto> ActualizarCuposOtorgados(IServicioRepositorio servicio, IServicioComandos servicioComandos, IConfiguracionProvider configuracion, int centroId)
        {
            return servicio.ListarEstadoCupos(centroId);
        }
    }
}