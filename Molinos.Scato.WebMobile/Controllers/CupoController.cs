using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.WebMobile.Helpers;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.WebMobile.Controllers
{
    public class CupoController : Controller
    {
        private readonly IServicioRepositorio servicio;
        private readonly IServicioComandos servicioComandos;
        private readonly IConfiguracionProvider configuracion;
        private readonly ILogger log;

        public CupoController(
            ILogger log,
            IServicioRepositorio servicio,
            IServicioComandos servicioComandos,
            IConfiguracionProvider configuracion
            )
        {
            this.log = log;
            this.servicio = servicio;
            this.servicioComandos = servicioComandos;
            this.configuracion = configuracion;
        }

        public ActionResult Index()
        {
            var centroId = int.Parse(ClaimsPrincipal.Current.GetUserClaim("CentroId").Value);      
            ViewBag.EstadoMateriales = servicio.ListarEstadoPlanta(centroId, true, true);
            ViewBag.MostrarIngresos = true;

            return View(servicio.ListarEstadoCupos(centroId));
        }
    }
}
