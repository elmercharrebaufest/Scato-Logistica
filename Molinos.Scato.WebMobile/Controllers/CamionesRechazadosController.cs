using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
//using Molinos.Scato.Web.Helpers;
using Molinos.Scato.WebMobile.Helpers;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.WebMobile.Controllers
{
    public class CamionesRechazadosController : Controller
    {
        private readonly IServicioRepositorio servicio;
        private readonly ILogger log;

        public CamionesRechazadosController(
            ILogger log,
            IServicioRepositorio servicio
            )
        {
            this.log = log;
            this.servicio = servicio;
        }

        public ActionResult Index()
        {
            ListQuery();
            return View();
        }

        private void ListQuery()
        {
            log.Debug("Obteniendo listado de camiones rechazados");
            var paginacion = new Paginacion("FechaCreacion", DirOrden.Desc, 1, 1000);
            var centroId = ClaimsPrincipal.Current.GetUserClaim("CentroId");
            var filtro = new FiltroListaDeWorkflowsDto
            {
                CentroId = int.Parse(centroId.Value)
            };
            ViewBag.CantidadCamiones = servicio.ObtenerCantidadCamionesRechazados(filtro.CentroId ?? 0);
            ViewBag.Items = servicio.ListarRecorridosRechazados(filtro, paginacion).Items;
        }

        public ActionResult FotosCamiones(Guid id)
        {
            var fotos = servicio.ListarFotosCamion(id, "");

            return View(fotos);
        }

        public ActionResult DocumentoOrigen(Guid id)
        {
            var cartaPorte = servicio.ObtenerCartaPortePorInstanceId(id);

            return new Rotativa.ViewAsPdf("CartaDePorte", cartaPorte) { FileName = "CartaDePorte.pdf" };
        }
    }
}



