using System.Collections.Generic;
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
    [Autorizacion(PermisosScato.AbmVinedoTerceros)]
    public class VinedoTercerosController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public VinedoTercerosController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }
        public ActionResult Index(string filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(filtro, pagina, ordenarPor, dirOrden);
            return View((object)filtro);
        }
        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(string filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(filtro, pagina, ordenarPor, dirOrden);
            return View("Listar", (object)filtro);
        }
        public ActionResult Crear()
        {
            CargarZonas();
            return View();
        }
        [HttpPost]
        public ActionResult Crear(VinedoTercerosDto model)
        {
            var proveedor = servicio.ObtenerProveedor(model.ProveedorId);
            if (proveedor != null)
            {
                model.esProveedorPR = proveedor.PR;
            }
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new CrearVinedoTerceros { Dto = model });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            CargarZonas(model);
            return View(model);
        }
        public ActionResult Modificar(int id)
        {
            var aModificar = servicio.ObtenerVinedoTerceros(id);
            CargarZonas(aModificar);
            return View(aModificar);
        }

        [HttpPost]
        public ActionResult Modificar(VinedoTercerosDto model)
        {
            var proveedor = servicio.ObtenerProveedor(model.ProveedorId);
            if (proveedor != null)
            {
                model.esProveedorPR = proveedor.PR;
            }
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new ModificarVinedoTerceros { Dto = model });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            CargarZonas(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarVinedoTerceros { Id = id });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        private void ListQuery(string filtro, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);
            ViewBag.Items = servicio.ListarVinedoTerceros(filtro, paginacion);
        }
        [ActionName("CargarSubZonas")]
        public JsonResult CargarSubZonas(int? zonaId)
        {
            if (zonaId != 0 && zonaId != null)
            {
                var subZonas =
                    servicio.ListarSubZonasPorZona((int)zonaId)
                            .ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
                return Json(subZonas, JsonRequestBehavior.AllowGet);
            }
            return Json(new List<SelectList>(), JsonRequestBehavior.AllowGet);
        }

        private void CargarZonas(VinedoTercerosDto model = null)
        {
            var zonas = servicio.ListarZonas();

            int zonaId = 0;
            if (model != null && model.ZonaId != null)
            {
                zonaId = model.ZonaId.Value;
            }

            ViewBag.Zonas = zonas.ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
            ViewBag.SubZonas = servicio.ListarSubZonasPorZona(zonaId)
                            .ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
        }
    }
}
