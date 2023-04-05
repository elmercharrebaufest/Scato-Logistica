using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.WebMobile.Helpers;
using Molinos.Scato.WebMobile.Helpers.Molinos.Scato.Dominio.Helpers;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.WebMobile.Controllers
{
    public class MenuController : ConsultasController
    {
        private readonly ILogger log;
        private readonly IServicioRepositorio servicio;
        private readonly IServicioComandos servicioComandos;
        private readonly IConfiguracionProvider configuracion;
        private readonly IServicioOrquestador servicioOrquestador;


        public MenuController(
            ILogger log,
            IServicioRepositorio servicio,
            IConfiguracionProvider configuracion,
            IServicioComandos servicioComandos,
            IServicioOrquestador servicioOrquestador
            ) :base(log, servicio, configuracion)
        {
            this.log = log;
            this.servicio = servicio;
            this.configuracion = configuracion;
            this.servicioComandos = servicioComandos;
            this.servicioOrquestador = servicioOrquestador;
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

        public ActionResult MenuPesadas()
        {
            var usuario = ClaimsPrincipal.Current.GetUserClaim(ClaimTypes.NameIdentifier);
            ViewBag.Centros = servicio.ListarCentrosPorUsuario(usuario.Value).OrderBy(x => x.Descripcion).ToList();
            var centro = ClaimsPrincipal.Current.GetUserClaim("CentroDescripcion");
            ViewBag.Centro = centro.Value;

            BuscarCamarasAsignadas();
            BuscarPesadaOnline();
            BuscarInformacionMeteorologica();
            return PartialView("_MenuPesadas");
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

        public void BuscarCamarasAsignadas()
        {
            ViewBag.Camaras = servicio.ListarVideoCamarasPuerto().ToSelectList(x => x.Codigo, x => x.Codigo);
        }

        public void BuscarPesadaOnline()
        {
            var balanzas = servicio.ListarBalanzasPuertoReales();
            //int balanzaNumero = 1;
            var datos = new List<ReportePesadaDto>();
            foreach (string balanza in balanzas)
            {
                var dato = servicio.ListarPesadasOnline(balanza);
                datos.Add(dato ?? new ReportePesadaDto { NumeroBalanza = balanza });
            }
            ViewBag.Datos = datos;
        }

        public void BuscarInformacionMeteorologica()
        {
            try
            {
                var estacion = ClaimsPrincipal.Current.GetUserClaim("EstacionMeteorologica");

                var informacionMeteorologica = (ResultadoMeteorologica)servicioOrquestador.Ejecutar(new EjecutarEstacionMeteorologica { CodigoDispositivo = estacion.Value });

                foreach (var dato in informacionMeteorologica.Imagenes)
                {
                    if (dato.Descripcion == "TEMPERATURA") { ViewBag.Temperatura = dato.Detalle[1]; }
                    else if (dato.Descripcion == "HUMEDAD") { ViewBag.Humedad = dato.Detalle[1]; }
                    else if (dato.Descripcion == "VIENTO") { ViewBag.Viento = dato.Detalle[1]; }
                    else if (dato.Descripcion == "LLUVIA") { ViewBag.Lluvia = dato.Detalle[1]; }
                }
            }
            catch (Exception e)
            {
                log.Error(e, "Error al obtener información meterologica");
            }

        }

    }
}
