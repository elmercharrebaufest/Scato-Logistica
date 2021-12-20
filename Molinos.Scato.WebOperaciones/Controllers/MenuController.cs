using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.WebOperaciones.Atributos;
using Molinos.Scato.WebOperaciones.Helpers;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
namespace Molinos.Scato.WebOperaciones.Controllers
{
    [Autorizacion(PermisosScato.ScatoPuerto)]
    public class MenuController : Controller
    {
        private readonly ILogger log;
        private readonly IServicioRepositorio servicio;
        private readonly IServicioOrquestador servicioOrquestador;

        public MenuController(ILogger log, IServicioRepositorio servicio, IServicioOrquestador servicioOrquestador)
        {
            this.log = log;
            this.servicio = servicio;
            this.servicioOrquestador = servicioOrquestador;
        }
        public ActionResult Menu()
        {
            var usuario = ClaimsPrincipal.Current.GetUserClaim(ClaimTypes.NameIdentifier);
            ViewBag.Centros = servicio.ListarCentrosPorUsuario(usuario.Value).OrderBy(x => x.Descripcion).ToList();
            var centro = ClaimsPrincipal.Current.GetUserClaim("CentroDescripcion");
            ViewBag.Centro = centro.Value;

            BuscarCamarasAsignadas();
            BuscarPesadaOnline();
            BuscarInformacionMeteorologica();
            return PartialView("_Menu");
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

        public void BuscarPesadaOnline()
        {
            var balanzas = servicio.ListarBalanzasPuertoReales();
            //int balanzaNumero = 1;
            var datos = new List<ReportePesadaDto>();
            foreach (string balanza in balanzas)
            {
                var dato = servicio.ListarPesadasOnline(balanza);
                datos.Add(dato ?? new ReportePesadaDto { NumeroBalanza = balanza});
            }
            ViewBag.Datos = datos;
        }

        public void BuscarCamarasAsignadas()
        {
            ViewBag.Camaras = servicio.ListarVideoCamarasPuerto().ToSelectList(x => x.Codigo, x => x.Codigo);
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
