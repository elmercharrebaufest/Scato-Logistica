using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using System.Web.UI;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.WebMobile.Atributos;
using Molinos.Scato.WebMobile.Helpers;
using Ninject.Extensions.Logging;
namespace Molinos.Scato.WebMobile.Controllers
{
    [Autorizacion(PermisosScato.WebMobile)]
    public class IndexController : Controller
    {
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioRepositorio servicio;
        private readonly ILogger log;

        public IndexController(
            ILogger log,
            IServicioRepositorio servicio,
            IServicioComandos servicioComandos
            )
        {
            this.log = log;
            this.servicio = servicio;
            this.servicioComandos = servicioComandos;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Menu()
        {
            return PartialView("_Menu");
        }

        //[OutputCache(Duration = 3600, Location = OutputCacheLocation.Client)]
        //public FileContentResult Logo()
        //{
        //    return File(firmaProvider.ObtenerLogo(), "image/png");
        //}

        [AllowAnonymous]
        public string Favicon()
        {
            return "";
        }

        //public ActionResult GenerarGraficoEficienciaHidraulicas(GraficoEficienciaHidraulicasDto model)
        //{
        //    var centroId = ClaimsPrincipal.Current.GetUserClaim("CentroId");
        //    log.Debug("Obteniendo Datos Gráfico Eficiencia Hidráulicas para centro Id : {0}", centroId.Value);
        //    return Json(servicio.ObtenerGraficoDeEficienciaHidraulicas(model, Int32.Parse(centroId.Value), configuracion.AppSettings["CodigoHidraulicaVagon"]), JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult GenerarGraficoDePlanta()
        //{
        //    var centroId = ClaimsPrincipal.Current.GetUserClaim("CentroId");
        //    log.Debug("Obteniendo Datos gráfico de planta, centro: {0}", centroId);
        //    return Json(listaDeWorkflows.ListarGraficoDePlanta(Int32.Parse(centroId.Value)), JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult Modificar(string actividad)
        //{
        //    var centroId = ClaimsPrincipal.Current.GetUserClaim("CentroId");
        //    var actividadNew = servicio.ObtenerGraficoDePlanta(actividad, Int32.Parse(centroId.Value));
        //    ViewBag.CentroId = Int32.Parse(centroId.Value);
        //    ViewBag.NombreActividad = actividad;
        //    return View("Modificar", actividadNew);
        //}

        //[HttpPost]
        //public ActionResult Modificar(GraficoDePlantaDto model)
        //{
        //    var centroId = ClaimsPrincipal.Current.GetUserClaim("CentroId");
        //    log.Debug("Se va a Modificar Datos gráfico de planta, centro: {0}", Int32.Parse(centroId.Value));
        //    if (ModelState.ContainsKey("Id"))
        //    {
        //        ModelState["Id"].Errors.Clear();
        //    }
        //    if (ModelState.IsValid)
        //    {
        //        model.CentroId = Int32.Parse(centroId.Value);
        //        var resultado = servicioComandos.Ejecutar(new ModificarGraficoDePlanta { Dto = model });

        //        if (!resultado.HayErrores)
        //        {
        //            log.Debug("Datos gráfico de planta modificados correctamente");
        //            return new AjaxEditSuccessResult();
        //        }
        //        log.Error("Hubo un problema al modificar datos grafico de planta: {0}", resultado.Errores.FirstOrDefault().Value);
        //        ModelState.AgregarErrores(resultado);
        //    }
        //    log.Error("Model inválido");
        //    ViewBag.CentroId = model.CentroId;
        //    ViewBag.NombreActividad = model.NombreActividad;
        //    return View(model);
        //}
        //public ActionResult GenerarGraficoCamionesPorHora(GraficoCamionesHoraDto model)
        //{
        //    var centroId = ClaimsPrincipal.Current.GetUserClaim("CentroId");
        //    if (model.FechaVieja == DateTime.MinValue || model.FechaVieja == DateTime.MaxValue)
        //    {
        //        model = new GraficoCamionesHoraDto { FechaVieja = DateTime.Now.AddDays(-365) };
        //    }
        //    log.Debug("Obteniendo Datos Gráfico Camiones Por hora para centro Id : {0}", centroId.Value);
        //    return Json(servicio.ObtenerGraficoDeCamionesPorHora(model, Int32.Parse(centroId.Value)), JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult GenerarGraficoCamionesPorDia(GraficoCamionesDiaDto model)
        //{
        //    var centroId = ClaimsPrincipal.Current.GetUserClaim("CentroId");
        //    log.Debug("Obteniendo Datos Gráfico Camiones Por Día para centro Id : {0}", centroId.Value);
        //    return Json(servicio.ObtenerGraficoDeCamionesPorDia(model, Int32.Parse(centroId.Value)), JsonRequestBehavior.AllowGet);
        //}
        //public ActionResult GenerarGraficoToneladasPorRangoDeDia(GraficoToneladasRangoDeDiasDto model)
        //{
        //    var centroId = ClaimsPrincipal.Current.GetUserClaim("CentroId");
        //    log.Debug("Obteniendo Datos Grafico Toneladas por Rango de Dias para el centroId: {0}", centroId);
        //    return Json(servicio.ObtenerGraficoToneladasPorRangoDeDias(model, Int32.Parse(centroId.Value)), JsonRequestBehavior.AllowGet);
        //}
    }
}
