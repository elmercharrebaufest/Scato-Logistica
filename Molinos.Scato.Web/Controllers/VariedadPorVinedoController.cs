using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AbmVariedadPorVinedo)]
    public class VariedadPorVinedoController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public VariedadPorVinedoController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        public ActionResult Index(int? vinedoId, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(vinedoId, pagina, ordenarPor, dirOrden);
            return View();
        }

        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(int? vinedoId, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(vinedoId, pagina, ordenarPor, dirOrden);
            return View("Listar");
        }

        private void ListQuery(int? vinedoId, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);

            ViewBag.Items = servicio.ListarPaginadoVariedadPorVinedo(vinedoId ?? 0, paginacion);
        }

        public ActionResult Crear(int? vinedoId)
        {
            ViewBag.VinedoId = vinedoId ?? 0;
            ListarVariedades();
            return View();
        }
        [HttpPost]
        public ActionResult Crear(VariedadPorVinedoDto model)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new CrearVariedadPorVinedo { Dto = model });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            ListarVariedades();
            ViewBag.VinedoId = model.VinedoId;
            return View(model);
        }

        public ActionResult Modificar(int id)
        {
            var aModificar = servicio.ObtenerVariedadPorVinedo(id);
            ListarVariedades();
            ViewBag.VinedoId = aModificar.VinedoId;
            return View(aModificar);
        }

        [HttpPost]
        public ActionResult Modificar(VariedadPorVinedoDto model)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new ModificarVariedadPorVinedo { Dto = model });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            ViewBag.VinedoId = model.VinedoId;
            ListarVariedades();
            return View(model);
        }

        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarVariedadPorVinedo { Id = id });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        public JsonResult ObtenerRecibidos(int vinedoId, int variedadId, string cosecha)
        {
            var recibidos = servicio.ObtenerKilosRecibidosPorVinedo(vinedoId, variedadId, cosecha);

            return Json(recibidos, JsonRequestBehavior.AllowGet);
        }

        public void ListarVariedades()
        {
            ViewBag.Variedades = servicio.ListarVariedades().ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
        }
    }
}
