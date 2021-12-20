using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.WebMobile.Helpers;
using Ninject.Extensions.Logging;
namespace Molinos.Scato.WebMobile.Controllers
{
    public class MenuController : ConsultasController
    {
        private readonly ILogger log;
        private readonly IServicioRepositorio servicio;
        private readonly IServicioComandos servicioComandos;
        private readonly IConfiguracionProvider configuracion;

        public MenuController(
            ILogger log,
            IServicioRepositorio servicio,
            IConfiguracionProvider configuracion,
            IServicioComandos servicioComandos
            ) :base(log, servicio, configuracion)
        {
            this.log = log;
            this.servicio = servicio;
            this.configuracion = configuracion;
            this.servicioComandos = servicioComandos;
        }

        public List<EstadoMaterialDto> GetPanelInfo(bool mostrarIngresos, bool esGrano)
        {
            var codigoSapSoja = configuracion.AppSettings["CodigoSapSemillaSoja"];
            var centro = ClaimsPrincipal.Current.GetUserClaim("CentroId");
            var centroId = Int32.Parse(centro.Value);
            var material = servicio.ObtenerMaterialIdYDescripcionPorCodigoSap(codigoSapSoja);

            if (material != null)
            {
                ViewBag.MaterialIdMenu = material.MaterialId;
                ViewBag.MaterialDescripcionMenu = material.Descripcion;
            }

            var materiales = servicio.ListarEstadoPlanta(centroId, mostrarIngresos, esGrano);

            var cupos = servicio.ListarEstadoCupos(centroId);
            var cupeados = cupos.Sum(x => x.Otorgados);
            var arribados = cupos.Sum(x => x.ArribadosDia);
            ViewBag.CupeadosMenu = cupeados;
            ViewBag.ArribadosMenu = arribados;
            ViewBag.DescargadosMenu = cupos.Sum(x => x.Descargados);
            ViewBag.CumplimientoMenu = cupeados == 0 ? 0 : arribados * 100 / cupeados;

            ViewBag.MostrarIngresos = mostrarIngresos;
            return materiales;
        }

        public ActionResult Menu()
        {

            var usuario = ClaimsPrincipal.Current.GetUserClaim(ClaimTypes.NameIdentifier);
            ViewBag.Centros = servicio.ListarCentrosPorUsuario(usuario.Value).OrderBy(x => x.Descripcion).ToList();
            var centro = ClaimsPrincipal.Current.GetUserClaim("CentroDescripcion");
            ViewBag.Centro = centro.Value;

            var materialesMenu = GetPanelInfo(true, true);
            return PartialView("_Menu",materialesMenu);
        }
        public ActionResult GraficoDePlantaHead(bool mostrarIngresos = true, bool esGrano = true)
        {
            
            var materialesMenu = GetPanelInfo(mostrarIngresos, esGrano);
            return PartialView("_GraficoDePlantaHead", materialesMenu);
        }
        public ActionResult SeleccionarCentro(int? centroId)
        {
            if (centroId.HasValue && centroId > 0)
            {
                var centro = servicio.ObtenerCentro(centroId.Value);
                log.Debug("Cambio de centro a: {0}", centro.Descripcion);
                ClaimsPrincipal.Current.AddUpdateUserClaim("CentroId", centroId.ToString());
                ClaimsPrincipal.Current.AddUpdateUserClaim("CentroDescripcion", centro.Descripcion);
            }
            return Redirect(Request.UrlReferrer.ToString());
        }
    }
}
