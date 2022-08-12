
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.WebMobile.Atributos;
using Molinos.Scato.WebMobile.Helpers;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;

namespace Molinos.Scato.WebMobile.Controllers
{
    [Autorizacion(PermisosScato.EstadoDeCallePlayaInterna)]
    public class EstadoPlayaInternaController : ConsultasController
    {
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioRepositorio servicio;
        private readonly ILogger log;
        IConfiguracionProvider configuracion;

        public EstadoPlayaInternaController(
            ILogger log,
            IServicioRepositorio servicio,
            IConfiguracionProvider configuracion,
            IServicioComandos servicioComandos

            ) : base(log, servicio, configuracion)
        {
            this.log = log;
            this.servicio = servicio;
            this.configuracion = configuracion;
            this.servicioComandos = servicioComandos;
        }

        public ActionResult Index()
        {
            var centro = ClaimsPrincipal.Current.GetUserClaim("CentroId");
            var centroId = int.Parse(centro.Value);
            return View(servicio.ObtenerCallesPorCentro(centroId).Where(x => (x.TipoCalle == TipoCalle.PlayaInterna || x.TipoCalle == TipoCalle.PlantaNoGranos || x.TipoCalle == TipoCalle.EnTransito) && !x.Deshabilitada).OrderBy(x=>x.Posicion).ToList());
        }

        public JsonResult EstadoDeCalle()
        {
            var centro = ClaimsPrincipal.Current.GetUserClaim("CentroId");
            var centroId = int.Parse(centro.Value);
            var camiones = servicio.ObtenerEstadoDeCalle();
            var calles = servicio.ObtenerCallesPorCentro(centroId).Where(x => x.TipoCalle == TipoCalle.PlayaInterna || x.TipoCalle == TipoCalle.PlantaNoGranos || x.TipoCalle == TipoCalle.EnTransito);
            var materiales = camiones.Where(x => x.TipoCalle != TipoCalle.NoGranos).Select(x => new { x.MaterialId, x.MaterialDesc })
                .GroupBy(x => x).Select(x => x.Key).Where(x => x.MaterialId != 0);

            return Json(new { estado = camiones, materiales, calles }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MostrarDetalleCamion(string patente, int calleId)
        {
            var model = servicio.ObtenerInfoPatente(patente, calleId);
            return PartialView("_DetalleCamion", model);
        }

    }
}
