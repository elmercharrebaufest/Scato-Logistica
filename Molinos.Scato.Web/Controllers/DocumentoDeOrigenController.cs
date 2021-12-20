using System.Web.Mvc;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    public class DocumentoDeOrigenController : BaseController
    {
        private readonly ILogger log;

        public DocumentoDeOrigenController(ILogger log, IServicioRepositorio servicio)
            : base(servicio)
        {
            this.log = log;
        }

        public ActionResult Index(DocumentoDeOrigenDto documentoDeOrigenDto, string nroDocumento, string patente, bool esActividad)
        {
            ViewBag.NroDocumento = nroDocumento;
            ViewBag.Patente = patente;
            ViewBag.EsActividad = esActividad;

            return View(documentoDeOrigenDto);
        }

    }
}
