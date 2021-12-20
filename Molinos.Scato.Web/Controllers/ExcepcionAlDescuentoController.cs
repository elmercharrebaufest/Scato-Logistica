using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AbmExcepcionAlDescuento)]
    public class ExcepcionAlDescuentoController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public ExcepcionAlDescuentoController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, string filtro, int pagina = 1, string ordenarPor = "Id",
                                  DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(filtro, pagina, ordenarPor, dirOrden, datosUsuario.CentroId);
            return View((object) filtro);
        }

        [AjaxOnly]
        [ActionName("Index")]
        [DatosUsuario]
        public ActionResult Listar(DatosUsuario datosUsuario, string filtro, int pagina = 1, string ordenarPor = "Id",
                                                      DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(filtro, pagina, ordenarPor, dirOrden, datosUsuario.CentroId);
            return View("Listar", (object) filtro);
        }

        private void ListQuery(string filtro, int pagina, string ordenarPor, DirOrden dirOrden, int centroId)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);

            ViewBag.Items = servicio.ListarExcepcionesAlDescuento(filtro, paginacion, centroId);
        }

        public ActionResult Crear()
        {
            SetearVista();
            return View();
        }

        [DatosUsuario]
        [HttpPost]
        public ActionResult Crear(DatosUsuario datosUsuario, ExcepcionAlDescuentoDto model)
        {
            if (ModelState.IsValid)
            {
                model.Usuario = datosUsuario.NombreUsuario;
                model.CentroId = datosUsuario.CentroId;
                var resultado = servicioComandos.Ejecutar(new CrearExcepcionAlDescuento { Dto = model, Usuario = datosUsuario.NombreUsuario });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            SetearVista();
            return View(model);
        }

        public ActionResult Modificar(int id)
        {
            SetearVista();
            var excepcionAlDescuentoDto = servicio.ObtenerExcepcionAlDescuento(id);
            return View(excepcionAlDescuentoDto);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Modificar(DatosUsuario datosUsuario, ExcepcionAlDescuentoDto model)
        {
            if (ModelState.IsValid)
            {
                model.Usuario = datosUsuario.NombreUsuario;
                model.CentroId = datosUsuario.CentroId;
                var resultado = servicioComandos.Ejecutar(new ModificarExcepcionAlDescuento { Dto = model, Usuario = datosUsuario.NombreUsuario });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            SetearVista();
            return View(model);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Eliminar(DatosUsuario datosUsuario,int id)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarExcepcionAlDescuento { Id = id, Usuario = datosUsuario.NombreUsuario });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        [DatosUsuario]
        public JsonResult SetearCaracteristicasCalidadDropDownList(int materialId, DatosUsuario datosUsuario)
        {
            var caracteristicas = servicio.ListarCaracteristicasDeCalidadPorMaterial(materialId, datosUsuario.CentroId).OrderBy(x => x.Descripcion).Select(x => new { x.Id, x.Descripcion });

            return Json(caracteristicas, JsonRequestBehavior.AllowGet);
        }


        private void SetearVista()
        {          
            ViewBag.Camaras = servicio.ListarCamaras().OrderBy(x => x.Descripcion).ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
        }
    }
}