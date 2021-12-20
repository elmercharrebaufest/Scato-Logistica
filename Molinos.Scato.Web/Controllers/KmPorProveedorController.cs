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
    [Autorizacion(PermisosScato.AbmAdministracionDistancias)]
    public class KmPorProveedorController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public KmPorProveedorController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        [DatosUsuario]
        public ActionResult Index(string filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(filtro, pagina, ordenarPor, dirOrden);
            return View((object)filtro);
        }

        [AjaxOnly]
        [ActionName("Index")]
        [DatosUsuario]
        public ActionResult Listar(string filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(filtro, pagina, ordenarPor, dirOrden);
            return View("Listar", (object)filtro);
        }

        private void ListQuery(string filtro, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);

            ViewBag.Items = servicio.ListarKmPorProveedor(filtro, paginacion);
        }

        public ActionResult Crear()
        {
            CargarCentros();
            CargarProvincias(null);
            return View();
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Crear(KmPorProveedorDto model, DatosUsuario datosUsuario)
        {
            CargarProvincias(model);
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new CrearKmPorProveedor { Dto = model, Usuario = datosUsuario.NombreUsuario });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            return View(model);
        }

        public ActionResult Modificar(int id)
        {
            var kmPorProveedor = servicio.ObtenerKmPorProveedor(id);
            CargarCentros();
            CargarProvincias(kmPorProveedor);
            return View(kmPorProveedor);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Modificar(KmPorProveedorDto model, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new ModificarKmPorProveedor { Dto = model, Usuario = datosUsuario.NombreUsuario });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            return View(model);
        }

        private void CargarCentros()
        {
            ViewBag.Centros =
                servicio.ListarCentros()
                    .OrderBy(x => x.Descripcion)
                    .ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
        }
        private void CargarProvincias(KmPorProveedorDto model)
        {
            int provinciaId = 0;
            if (model != null && model.ProvinciaId != null)
            {
                provinciaId = model.ProvinciaId;
            }
           
            var provincias = servicio.ListarProvincias().OrderBy(p => p.Descripcion);
            ViewBag.Provincias = provincias.ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
            ViewBag.Localidades = servicio.ListarLocalidadesPorProvincia(provinciaId)
                            .ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
        }

        [ActionName("CargarLocalidades")]
        public JsonResult CargarLocalidades(int? provinciaId)
        {
            if (provinciaId != 0 && provinciaId != null)
            {
                var localidades =
                    servicio.ListarLocalidadesPorProvincia((int)provinciaId).OrderBy(l => l.Descripcion)
                            .ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
                return Json(localidades, JsonRequestBehavior.AllowGet);
            }
            return Json(new List<SelectList>(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Eliminar(int id, DatosUsuario datosUsuario)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarKmPorProveedor { Id = id, Usuario = datosUsuario.NombreUsuario });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }
    }
}