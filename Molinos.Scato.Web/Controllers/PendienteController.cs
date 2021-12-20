using Molinos.Scato.Dominio.Dto;
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
        private readonly IFirmaProvider configuracion;

        public PendienteController(ILogger log, IServicioRepositorio servicio, IFirmaProvider configuracion)
            : base(servicio)
        {
            this.log = log;
            this.configuracion = configuracion;
        }

        public ActionResult Index(int id)
        {
            var cargaDeCupo = servicio.ObtenerCupoPorId(id);
            if (cargaDeCupo.CircuitoNoGranos)
            {
                return RedirectToAction("PendienteNogranos", cargaDeCupo);
            }
            var codigoSapMRP = ConfigurationManager.AppSettings["CodigoSapMRP"];
            var codigoSapMolinosAgro = configuracion.ObtenerFirmaSinLogo().CodigoSAP;

            if ((cargaDeCupo.TitularCartaPorteCodigoSap == codigoSapMRP && (cargaDeCupo.RtteComercialCodigoSap == null || cargaDeCupo.RtteComercialCodigoSap == codigoSapMRP || cargaDeCupo.RtteComercialCodigoSap == codigoSapMolinosAgro)) ||
                ((cargaDeCupo.TitularCartaPorteCodigoSap == codigoSapMolinosAgro) && (cargaDeCupo.RtteComercialCodigoSap == null || (cargaDeCupo.RtteComercialCodigoSap == codigoSapMolinosAgro))))
            {
                return RedirectToAction("Index", "IngresarCartaPorteRedespacho", new { workflow = ConfigurationManager.AppSettings["workflowRedespacho"], cargaDeCupoId = id });
            }
            else if(!string.IsNullOrEmpty(cargaDeCupo.TitularCartaPorteCodigoSap))
            {
                return RedirectToAction("Index", "CargarCartaPorte", new { workflow = ConfigurationManager.AppSettings["WorkflowIngresoPorCompra"], cargaDeCupoId = id });
            }
            ViewBag.IngresoPorCompra = ConfigurationManager.AppSettings["WorkflowIngresoPorCompra"];
            ViewBag.IngresoPorRedespacho = ConfigurationManager.AppSettings["workflowRedespacho"];
            if (!string.IsNullOrEmpty(cargaDeCupo.FotoRutaDestino))
            {
                var foto = servicio.ObtenerFotoPorPath(cargaDeCupo.FotoRutaDestino);
                if (foto.Fotos.Any())
                {
                    ViewBag.FotoMesaDigitalizacion1 = foto.Fotos.First().Foto;
                }
            }
            return View(cargaDeCupo);
        }
        [DatosUsuario]
        public ActionResult PendienteNogranos(CargaDeCupoDto cargaDeCupo, DatosUsuario datosUsuario)
        {
            ViewBag.Workflows = servicio.ListarWorkFlowsPendientesNoGrano(datosUsuario.CentroId);
            return View(cargaDeCupo);
        }
    }
}
