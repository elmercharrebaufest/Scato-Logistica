using System.Web.Mvc;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ReporteControlBalanza)]
    public class ReporteControlDeBalanzaController : BaseController
    {
        private readonly ILogger log;

        public ReporteControlDeBalanzaController(ILogger log, IServicioRepositorio servicio)
            : base(servicio)
        {
            this.log = log;
        }

        public ActionResult Index()
        {
            return View();
        }

        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(FechaModel fechaModel)
        {
            if (fechaModel.FechaHasta < fechaModel.FechaDesde)
            {
                ModelState.AddModelError("", Textos.TarjetaRango_ErrorFechas);
            }
            if (ModelState.IsValid)
            {
                var items = servicio.ListarControlDeBalanza(fechaModel.FechaDesde, fechaModel.FechaHasta.AddDays(1).AddTicks(-1));
                if (items.Count == 0)
                {
                    ModelState.AddModelError("", Textos.Error_NoResultados);
                }
                ViewBag.Items = items;
            }
            return View("Listar", (object)fechaModel);
        }
    }
}