using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AbmVinedoPropio)]
    public class VinedoPropioController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;
        private readonly IFirmaProvider configuracion;

        public VinedoPropioController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos, IFirmaProvider configuracion)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
            this.configuracion = configuracion;
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
        public ActionResult Crear(VinedoPropioDto model)
        {
            if (ModelState.IsValid)
            {
                model.Cuarteles = model.CuartelesJson.FromJson<CuartelDto[]>().ToList();
                var resultado = servicioComandos.Ejecutar(new CrearVinedoPropio { Dto = model, CodigoSapProveedor = configuracion.ObtenerFirmaSinLogo().CodigoSAP });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            CargarZonas(model);
            ViewBag.CuartelesJsonPostBack = model.CuartelesJson;
            return View(model);
        }
        public ActionResult Modificar(int id)
        {
            var aModificar = servicio.ObtenerVinedoPropio(id);
            ViewBag.CuartelesJsonPostBack = aModificar.Cuarteles.ToJson();
            CargarZonas(aModificar);
            return View(aModificar);
        }

        [HttpPost]
        public ActionResult Modificar(VinedoPropioDto model)
        {
            if (ModelState.IsValid)
            {
                model.Cuarteles = model.CuartelesJson.FromJson<CuartelDto[]>().ToList();
                var resultado = servicioComandos.Ejecutar(new ModificarVinedoPropio { Dto = model, CodigoSapProveedor = configuracion.ObtenerFirmaSinLogo().CodigoSAP });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            CargarZonas(model);
            ViewBag.CuartelesJsonPostBack = model.CuartelesJson;
            return View(model);
        }

        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarVinedoPropio { Id = id });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        private void ListQuery(string filtro, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);
            ViewBag.Items = servicio.ListarVinedoPropio(filtro, paginacion);
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

        private void CargarZonas(VinedoPropioDto model = null)
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
