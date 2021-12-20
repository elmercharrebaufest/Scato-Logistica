using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AbmTarjetaSupervisor)]
    public class TarjetaSupervisorController : BaseController
    {
        private readonly IServicioComandos servicioComandos;
        private ILogger log;

        public TarjetaSupervisorController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }
        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, string filtro, string ordenarPor = "Numero", DirOrden dirOrden = DirOrden.Asc, int pagina = 1)
        {
            ListarConsulta(filtro, pagina, ordenarPor, dirOrden, datosUsuario.CentroId);
            ViewBag.SoloLectura = "true";
            return View();
        }

        [DatosUsuario]
        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(DatosUsuario datosUsuario, string filtro, string ordenarPor = "Numero", DirOrden dirOrden = DirOrden.Asc, int pagina = 1)
        {
            ListarConsulta(filtro, pagina, ordenarPor, dirOrden, datosUsuario.CentroId);
            return View("Listar");
        }

        private void ListarConsulta(string filtro, int pagina, string ordenarPor, DirOrden dirOrden, int centroId)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, itemsPorPagina: 10);
            ViewBag.Items = servicio.ListarPaginadoTarjetasSupervisor(filtro, paginacion, centroId);
        }

        [DatosUsuario]
        public ActionResult Crear(DatosUsuario datosUsuario)
        {
            SetearPuestosDeTrabajoDropDownList();
            var tarjeta = new TarjetaSupervisorDto { CentroId = datosUsuario.CentroId };
            return View(tarjeta);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Crear(TarjetaSupervisorDto model, string puestosdetrabajo, DatosUsuario datosUsuario)
        {
            model.Id = 0;
            if (ModelState.IsValid && puestosdetrabajo != "" )
            {
                var listaPuestos = puestosdetrabajo.FromJson<List<PuestoDeTrabajoDto>>();

                model.PuestosDeTrabajoAsociados = listaPuestos;

                var resultado = servicioComandos.Ejecutar(new CrearTarjetaSupervisor { Dto = model, Usuario = datosUsuario.NombreUsuario });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            if (puestosdetrabajo == "")
            {
                ModelState.AddModelError("PuestosDeTrabajoAsociados", string.Format(Textos.Error_Requerido, "Puesto"));
            }

            SetearPuestosDeTrabajoDropDownList();
            return View(model);
        }

        public ActionResult Modificar(int id)
        {
            var tarjetaAModificar = servicio.ObtenerTarjetaSupervisor(id);
            SetearPuestosDeTrabajoDropDownList();
            return View(tarjetaAModificar);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Modificar(TarjetaSupervisorDto model, string puestosdetrabajo, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid && puestosdetrabajo != "")
            {
                var listaPuestos = puestosdetrabajo.FromJson<List<PuestoDeTrabajoDto>>();
                
                model.PuestosDeTrabajoAsociados = listaPuestos;
                var resultado = servicioComandos.Ejecutar(new ModificarTarjetaSupervisor { Dto = model, Usuario = datosUsuario.NombreUsuario });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }

            if (puestosdetrabajo == "")
            {
                ModelState.AddModelError("PuestosAsociados", Textos.Error_Requerido);
            }


            SetearPuestosDeTrabajoDropDownList();
            return View(model);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Eliminar(int id, DatosUsuario datosUsuario)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarTarjetaSupervisor { Id = id, Usuario = datosUsuario.NombreUsuario});
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        public JsonResult ObtenerPuestosDeTrabajo(int id)
        {
            var tarjeta = servicio.ObtenerTarjetaSupervisor(id);
            var PuestosDeTrabajoAsociados = tarjeta != null ? tarjeta.PuestosDeTrabajoAsociados.ToList() : null;
            return Json(PuestosDeTrabajoAsociados, JsonRequestBehavior.AllowGet);
        }

        private void SetearPuestosDeTrabajoDropDownList()
        {
            ViewBag.PuestosDeTrabajo = servicio.ListarPuestosDeTrabajo().OrderBy(c => c.NombrePuesto).ToSelectList(x => x.Id.ToString(), x => x.NombrePuesto);
        }
    }
}
