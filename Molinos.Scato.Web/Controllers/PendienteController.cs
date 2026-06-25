using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;
using System;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.CamionesPendientesMesa)]
    public class PendienteController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;
        private readonly IFirmaProvider configuracion;

        public PendienteController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos, IFirmaProvider configuracion)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
            this.configuracion = configuracion;
        }

        public ActionResult Index(int id)
        {
            ViewBag.FotoMesaDigitalizacionSustentable = null;
            var cargaDeCupo = servicio.ObtenerCupoPorId(id);
            if (cargaDeCupo == null)
            {
                log.Warn("Pendiente.Index: CargaDeCupo no encontrada para Id={0}", id);
                return RedirectToAction("Index", "ListaDeCamiones");
            }

            if (cargaDeCupo.CircuitoNoGranos)
            {
                return RedirectToAction("PendienteNogranos", cargaDeCupo);
            }

            var workflowCompraGranos = ConfigurationManager.AppSettings["WorkflowIngresoPorCompra"];
            var workflowRedespachoGranos = ConfigurationManager.AppSettings["workflowRedespacho"];
            var workflowImportacionGranos = ConfigurationManager.AppSettings["workflowIngresoPorImpoGranos"];

            var codigoSapMRP = ConfigurationManager.AppSettings["CodigoSapMRP"];
            var codigoSapMolinosAgro = configuracion.ObtenerFirmaSinLogo().CodigoSAP;
            var datosCpe = ObtenerDatosCpe(cargaDeCupo);
            var escenario = WorkflowCartaPorteHelper.ObtenerEscenario(
                cargaDeCupo.TitularCartaPorteCodigoSap,
                cargaDeCupo.RtteComercialCodigoSap,
                datosCpe.CodigoSapDestino,
                datosCpe.CodigoSapDestinatario,
                codigoSapMolinosAgro,
                codigoSapMRP,
                cargaDeCupo.CodEstab,
                cargaDeCupo.RtteComercialVentaSecundariaCuit);

            if (escenario == EscenarioWorkflowCartaPorte.Compra)
            {
                return RedirectToAction("Index", "CargarCartaPorte", new { workflow = workflowCompraGranos, cargaDeCupoId = id });
            }

            if (escenario == EscenarioWorkflowCartaPorte.Redespacho)
            {
                return RedirectToAction("Index", "IngresarCartaPorteRedespacho", new { workflow = workflowRedespachoGranos, cargaDeCupoId = id });
            }

            if (escenario == EscenarioWorkflowCartaPorte.Importacion)
            {
                return RedirectToAction("Index", "IngresarCartaPorteRedespachoImportaciones", new { workflow = workflowImportacionGranos, cargaDeCupoId = id });
            }

            ViewBag.IngresoPorCompra = workflowCompraGranos;
            ViewBag.IngresoPorRedespacho = workflowRedespachoGranos;
            ViewBag.IngresoPorImpoGranos = workflowImportacionGranos;
            if (!string.IsNullOrEmpty(cargaDeCupo.FotoRutaDestino))
            {
                var foto = servicio.ObtenerFotoPorPath(cargaDeCupo.FotoRutaDestino);
                if (foto.Fotos.Any())
                {
                    ViewBag.FotoMesaDigitalizacion1 = foto.Fotos.First().Foto;
                }
            }
            if (!string.IsNullOrEmpty(cargaDeCupo.FotoRutaSustentable))
            {
                var foto = servicio.ObtenerFotoPorPath(cargaDeCupo.FotoRutaSustentable);
                if (foto.Fotos.Any())
                {
                    ViewBag.FotoMesaDigitalizacionSustentable = foto.Fotos.First().Foto;
                }
            }
            return View(cargaDeCupo);
        }

        private DatosCpePendiente ObtenerDatosCpe(CargaDeCupoDto cargaDeCupo)
        {
            if (cargaDeCupo == null || string.IsNullOrWhiteSpace(cargaDeCupo.CTG) || cargaDeCupo.CentroId <= 0)
                return new DatosCpePendiente();

            long nroCtg;
            if (!long.TryParse(cargaDeCupo.CTG, out nroCtg))
            {
                log.Warn("Pendiente.Index: CTG inválido '{0}' para CargaDeCupo Id={1}", cargaDeCupo.CTG, cargaDeCupo.Id);
                return new DatosCpePendiente();
            }

            var usuario = System.Security.Claims.ClaimsPrincipal.Current?.FindFirst(System.IdentityModel.Claims.ClaimTypes.NameIdentifier)?.Value;
            var resultado = servicioComandos.Ejecutar(new ConsultarCPDigital
            {
                CentroId = cargaDeCupo.CentroId,
                NroCtg = nroCtg,
                Usuario = usuario
            }) as ResultadoCartaPorteElectronica;

            return new DatosCpePendiente
            {
                CodigoSapDestinatario = resultado?.Cpe?.DestinatarioCodigoSap,
                CodigoSapDestino = resultado?.Cpe?.DestinoCodigoSap
            };
        }

        private class DatosCpePendiente
        {
            public string CodigoSapDestinatario { get; set; }
            public string CodigoSapDestino { get; set; }
        }

        [DatosUsuario]
        public ActionResult PendienteNogranos(CargaDeCupoDto cargaDeCupo, DatosUsuario datosUsuario)
        {
            ViewBag.Workflows = servicio.ListarWorkFlowsPendientesNoGrano(datosUsuario.CentroId);
            return View(cargaDeCupo);
        }
    }
}
