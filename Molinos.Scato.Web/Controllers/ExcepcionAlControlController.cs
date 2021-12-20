using System;
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
    [Autorizacion(PermisosScato.AbmExcepcionAlControl)]
    public class ExcepcionAlControlController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public ExcepcionAlControlController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        public ActionResult Index(string filtro, int pagina = 1, string ordenarPor = "Id",
                                  DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(filtro, pagina, ordenarPor, dirOrden);
            return View((object) filtro);
        }

        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(string filtro, int pagina = 1, string ordenarPor = "Id",
                                                      DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(filtro, pagina, ordenarPor, dirOrden);
            return View("Listar", (object) filtro);
        }

        private void ListQuery(string filtro, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);

            ViewBag.Items = servicio.ListarExcepcionesAlControl(filtro, paginacion);
        }

        public ActionResult Crear()
        {
            SetearCentrosDropDownList();
            return View();
        }

        [DatosUsuario]
        [HttpPost]
        public ActionResult Crear(DatosUsuario datosUsuario, ExcepcionAlControlDto model)
        {
            if (ModelState.IsValid)
            {
                model.FechaDeCarga = DateTime.Now;
                model.Usuario = datosUsuario.NombreUsuario;
                var resultado = servicioComandos.Ejecutar(new CrearExcepcionAlControl { Dto = model });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            
            SetearCentrosDropDownList();
            return View(model);
        }

        public ActionResult Modificar(int id)
        {
            var excepcionAlControlDto = servicio.ObtenerExcepcionAlControl(id);
            SetearCentrosDropDownList();
            return View(excepcionAlControlDto);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Modificar(DatosUsuario datosUsuario,ExcepcionAlControlDto model)
        {
            if (ModelState.IsValid)
            {
                model.FechaDeCarga = DateTime.Now;
                model.Usuario = datosUsuario.NombreUsuario;
                var resultado = servicioComandos.Ejecutar(new ModificarExcepcionAlControl { Dto = model });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            SetearCentrosDropDownList();
            return View(model);
        }

        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarExcepcionAlControl { Id = id });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        private void SetearCentrosDropDownList()
        {
            ViewBag.Centros = servicio.ListarCentros().OrderBy(x => x.Descripcion).ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
        }
    }
}