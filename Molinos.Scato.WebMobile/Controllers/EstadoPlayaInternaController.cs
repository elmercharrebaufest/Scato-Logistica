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
        private IConfiguracionProvider configuracion;

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
            var tipoCallePlantaLista = ObtenerTiposDeCallesPlanta();
            return View(tipoCallePlantaLista);
        }

        public JsonResult EstadoDeCalle()
        {
            var centro = ClaimsPrincipal.Current.GetUserClaim("CentroId");
            var centroId = int.Parse(centro.Value);
            var camiones = servicio.ObtenerEstadoDeCalle();
            var calles = servicio.ObtenerCallesPorCentro(centroId).Where(x => x.TipoCalle == TipoCalle.PlayaInterna || x.TipoCalle == TipoCalle.PlantaNoGranos || x.TipoCalle == TipoCalle.EnTransito || x.TipoCalle == TipoCalle.SalidaNoGranos || x.TipoCalle == TipoCalle.EsperaAduanaNoGranos);
            var materiales = camiones.Where(x => x.TipoCalle == TipoCalle.PlayaInterna || x.TipoCalle == TipoCalle.PlantaNoGranos || x.TipoCalle == TipoCalle.EnTransito || x.TipoCalle == TipoCalle.SalidaNoGranos || x.TipoCalle == TipoCalle.EsperaAduanaNoGranos)
                .Select(x => new { x.MaterialId, x.MaterialDesc })
                .GroupBy(x => x).Select(x => x.Key).Where(x => x.MaterialId != 0);

            return Json(new { estado = camiones, materiales, calles }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerCalles(string tiposCalleStr)
        {
            if(string.IsNullOrEmpty(tiposCalleStr))
                return Json(new List<TipoCallePlantaDto>(), JsonRequestBehavior.AllowGet);

            var tiposCalle = new List<TipoCalle>();
            var arrTipoCallesStr = tiposCalleStr.Split(',');
            foreach (var tipoCalleStr in arrTipoCallesStr)
            {
                var tipoCalleInt = int.Parse(tipoCalleStr);
                tiposCalle.Add((TipoCalle)tipoCalleInt);
            }
            var tipoCallePlantaLista = ObtenerTiposDeCallesPlanta();
            return Json(tipoCallePlantaLista.Where(x => tiposCalle.Contains(x.TipoCalle)).ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult MostrarDetalleCamion(string patente, int calleId)
        {
            var model = servicio.ObtenerInfoPatente(patente, calleId);
            return PartialView("_DetalleCamion", model);
        }

        private List<TipoCallePlantaDto> ObtenerTiposDeCallesPlanta()
        {
            var centro = ClaimsPrincipal.Current.GetUserClaim("CentroId");
            var centroId = int.Parse(centro.Value);
            var camiones = servicio.ObtenerEstadoDeCalle();
            var calles = servicio.ObtenerCallesPorCentro(centroId).Where(x => (x.TipoCalle == TipoCalle.PlayaInterna || x.TipoCalle == TipoCalle.PlantaNoGranos || x.TipoCalle == TipoCalle.EnTransito || x.TipoCalle == TipoCalle.SalidaNoGranos || x.TipoCalle == TipoCalle.EsperaAduanaNoGranos) && !x.Deshabilitada).OrderBy(x => x.Posicion).ToList();
            var tipoCallePlantaLista = new List<TipoCallePlantaDto>();

            foreach (var calle in calles)
            {
                var tipoCallePlanta = tipoCallePlantaLista.FirstOrDefault(q => q.TipoCalle == calle.TipoCalle);
                if (tipoCallePlanta == null)
                {
                    tipoCallePlanta = new TipoCallePlantaDto
                    {
                        TipoCalle = calle.TipoCalle
                    };
                    tipoCallePlantaLista.Add(tipoCallePlanta);
                }
                var callePlanta = new CallePlantaDto
                {
                    CalleId = calle.Id,
                    CalleDesc = calle.Nombre,
                    ColorFondo = calle.ColorFondo ?? "#000",
                    ColorTexto = calle.ColorTexto ?? "#fff",
                    LimiteDeCamiones = calle.CantidadDeCamiones,
                    Bloqueada = calle.Bloqueada
                };
                tipoCallePlanta.Calles.Add(callePlanta);

                foreach (var camion in camiones.Where(q => q.CalleId == calle.Id))
                {
                    var camionPlanta = new CamionPlantaDto
                    {
                        CamionId = camion.Id,
                        Patente = camion.Patente,
                        Escalable = camion.Escalable,
                        UltimoDeLaFila = camion.UltimoDeLaFila,
                        ColorFondo = camion.ColorFondo ?? "#000",
                        ColorTexto = camion.ColorTexto ?? "#fff",
                        CalleId = camion.CalleId,
                        Rechazado = camion.Rechazado,
                        FechaIngreso = camion.FechaIngeso
                    };
                    callePlanta.Camiones.Add(camionPlanta);
                }
            }
            return tipoCallePlantaLista;
        }
    }
}