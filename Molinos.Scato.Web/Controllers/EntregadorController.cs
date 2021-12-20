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
    [Autorizacion(PermisosScato.AbmEntregador)]
    public class EntregadorController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public EntregadorController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
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

        private void ListQuery(string filtro, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);
            ViewBag.Items = servicio.ListarEntregadores(filtro,paginacion);
        }

        public ActionResult Crear()
        {
            SetearCombos(0,0);
            return View(new EntregadorDto{Activo = true});
        }

        [HttpPost]
        public ActionResult Crear(EntregadorDto model)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new CrearEntregador() { Dto = model });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            SetearCombos(model.PaisId, model.ProvinciaId);
            return View(model);
        }

        private void SetearCombos(int? paisId, int? provinciaId)
        {
            var paises = servicio.ListarPaises().ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
            paises.Insert(0, new SelectListItem { Text = "", Value = "0" });
            ViewBag.Paises = paises;

            var provincias = servicio.ListarProvinciasPorPais(paisId ?? 0).ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
            provincias.Insert(0, new SelectListItem { Text = "", Value = "0" });
            ViewBag.Provincias = provincias;

            var localidades = servicio.ListarLocalidadesPorProvincia(provinciaId ?? 0).ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
            localidades.Insert(0, new SelectListItem { Text = "", Value = "0" });
            ViewBag.Localidades = localidades;
        }

        public ActionResult Modificar(int id)
        {
            var aModificar = servicio.ObtenerEntregador(id);
            SetearCombos(aModificar.PaisId, aModificar.ProvinciaId);
            return View(aModificar);
        }

        [HttpPost]
        public ActionResult Modificar(EntregadorDto model)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new ModificarEntregador() { Dto = model });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            SetearCombos(model.PaisId, model.ProvinciaId);
            return View(model);
        }


        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarEntregador() { Id = id });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        [ActionName("CargarLocalidades")]
        public JsonResult CargarLocalidades(int? provinciaId)
        {
            if (provinciaId != 0 && provinciaId != null)
            {
                var localidades =
                    servicio.ListarLocalidadesPorProvincia((int)provinciaId)
                            .ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
                return Json(localidades, JsonRequestBehavior.AllowGet);
            }
            return Json(new List<SelectList>(), JsonRequestBehavior.AllowGet);
        }

        [ActionName("CargarProvincias")]
        public JsonResult CargarProvincias(int? paisId)
        {
            if (paisId != 0 && paisId != null)
            {
                var localidades =
                    servicio.ListarProvinciasPorPais((int)paisId)
                            .ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
                return Json(localidades, JsonRequestBehavior.AllowGet);
            }
            return Json(new List<SelectList>(), JsonRequestBehavior.AllowGet);
        }



    }
}