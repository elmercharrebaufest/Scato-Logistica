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
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AbmBocaDestino)]
    public class BocaDestinoController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public BocaDestinoController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
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

            ViewBag.Items = servicio.ListarPaginadoBocasDestino(filtro, paginacion);
        }

        public ActionResult Crear()
        {
            CargarPaises();
            return View();
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Crear(BocaDestinoDto model, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new CrearBocaDestino { Dto = model, Usuario = datosUsuario.NombreUsuario});
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            CargarPaises(model);
            return View(model);
        }

        public ActionResult Modificar(int id)
        {
            var bocaDestinoAModificar = servicio.ObtenerBocaDestino(id);
            CargarPaises(bocaDestinoAModificar);
            return View(bocaDestinoAModificar);

        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Modificar(BocaDestinoDto model, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                model.Proveedor = null;
                var resultado = servicioComandos.Ejecutar(new ModificarBocaDestino { Dto = model, Usuario = datosUsuario.NombreUsuario});
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            CargarPaises(model);
            return View(model);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Eliminar(int id, DatosUsuario datosUsuario)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarBocaDestino { Id = id, Usuario = datosUsuario.NombreUsuario});
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        private void CargarPaises(BocaDestinoDto model = null)
        {
            var paises = servicio.ListarPaises();

            var paisId = paises.FirstOrDefault() != null ? paises.FirstOrDefault()?.Id : 0;
            var provinciaId = 0;
            if (model != null)
            {
                provinciaId = model.ProvinciaId;
                paisId = model.PaisId;
            }

            ViewBag.Paises = paises.ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
            ViewBag.Provincias = servicio.ListarProvinciasPorPais((int)paisId).ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
            ViewBag.Localidades = servicio.ListarLocalidadesPorProvincia(provinciaId)
                            .ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
        }

        public JsonResult CargarProvincias(int? paisId)
        {
            if (paisId != 0 && paisId != null)
            {
                var provincias =
                    servicio.ListarProvinciasPorPais(paisId.Value)
                            .ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
                return Json(new {provincias = provincias, localidades = new List<SelectList>()}, JsonRequestBehavior.AllowGet);
            }
            return Json(new { provincias = new List<SelectList>(), localidades = new List<SelectList>() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CargarLocalidades(int? provinciaId)
        {
            if (provinciaId != 0 && provinciaId != null)
            {
                var localidades =
                    servicio.ListarLocalidadesPorProvincia(provinciaId.Value)
                            .ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
                return Json(localidades, JsonRequestBehavior.AllowGet);
            }
            return Json(new List<SelectList>(), JsonRequestBehavior.AllowGet);
        }
    }
}
