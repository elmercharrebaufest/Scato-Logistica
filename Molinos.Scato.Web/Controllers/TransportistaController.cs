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
    [Autorizacion(PermisosScato.AbmTransportista)]
    public class TransportistaController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public TransportistaController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
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

            ViewBag.Items = servicio.ListarPaginadoTransportistas(filtro, paginacion);
        }

        public ActionResult Crear()
        {
            CargarProvincias();
            return View();
        }
        [HttpPost]
        [DatosUsuario]
        public ActionResult Crear(TransportistaDto model, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new CrearTransportista { Dto = model, Usuario = datosUsuario.NombreUsuario});
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            CargarProvincias(model);
            return View(model);
        }

        public ActionResult Modificar(int id)
        {
            var transportistaAModificar = servicio.ObtenerTransportista(id);

            CargarProvincias(transportistaAModificar);
            return View(transportistaAModificar);

        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Modificar(TransportistaDto model, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new ModificarTransportista { Dto = model, Usuario = datosUsuario.NombreUsuario});
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            CargarProvincias(model);
            return View(model);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Eliminar(int id, DatosUsuario datosUsuario)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarTransportista { Id = id, Usuario = datosUsuario.NombreUsuario });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        [ActionName("CargarLocalidades")]
        public JsonResult CargarLocalidades(int? provinciaId)
        {
            if (provinciaId != 0 && provinciaId != null)
            {
                var localidades =
                    servicio.ListarLocalidadesPorProvincia((int) provinciaId)
                            .ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
                return Json(localidades, JsonRequestBehavior.AllowGet);
            }
            return Json(new List<SelectList>(), JsonRequestBehavior.AllowGet);
        }

        private void CargarProvincias(TransportistaDto model = null)
        {
            var provincias = servicio.ListarProvincias();

            int provinciaId = 0;
            if (model != null && model.ProvinciaId != null)
            {
                provinciaId = model.ProvinciaId.Value;
            }
            
            ViewBag.Provincias = provincias.ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
            ViewBag.Localidades = servicio.ListarLocalidadesPorProvincia(provinciaId)
                            .ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
        }
    }
}
