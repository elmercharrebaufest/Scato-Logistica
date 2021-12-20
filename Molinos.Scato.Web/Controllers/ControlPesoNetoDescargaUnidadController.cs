using System;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
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
    [Autorizacion(PermisosScato.ActividadControlPesoNetoDescargaUnidad)]
    public class ControlPesoNetoDescargaUnidadController : BaseController
    {
        private readonly IServicioActividadFactory<IControlPesoNetoService> factory;
        private ILogger log;

        public ControlPesoNetoDescargaUnidadController(ILogger log, IServicioActividadFactory<IControlPesoNetoService> factory, IServicioRepositorio servicio)
            : base(servicio)
        {
            this.factory = factory;
            this.log = log;
        }

        [DatosUsuario]
        public ActionResult Index(Guid id, DatosUsuario datosUsuario)
        {
            var recorrido = servicio.ObtenerRecorridoPorGuid(id);
            var pesoNetoDescargaUnidad = servicio.ObtenerPesoNetoDescargaUnidad(id);
            var controlRecorrido = new ControlRecorridoDto
            {
                WorkflowInstanceId = id,
                NombreUsuario = datosUsuario.NombreUsuario,
                Actividad = Textos.ActControlPesoNetoDescargaUnidad,
                ActividadXaml = "ControlPesoNetoDescargaUnidad",
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
            };

            ViewBag.PesoNetoBalanza = recorrido.PesoNeto.HasValue ? recorrido.PesoNeto : 0;
            ViewBag.TipoDocumentoIngreso = recorrido.TipoDocumentoIngreso;
            ViewBag.NumeroDocumentoIngreso = recorrido.NumeroDocumentoIngreso;
            ViewBag.Material = recorrido.Material != null ? recorrido.Material.Descripcion : "";
            ViewBag.TipoComercial = recorrido.TipoComercial.Descripcion;
            ViewBag.Patente = recorrido.Patente;
            ViewBag.PesoNetoCentro = recorrido.PesoNeto;
            //ViewBag.NumeroRomaneo = "??";////////////////Todo: setear correctamente romaneo////////////////////
            ViewBag.pesoNetoDescargaUnidad = pesoNetoDescargaUnidad;
            ViewBag.NumeroDeOrden = recorrido.Id;
            ViewBag.Workflow = recorrido.Workflow.Codigo;
            ViewBag.Motivos = servicio.ListarMotivos().ToSelectList(x => x.Descripcion, x => x.Descripcion);
            ViewBag.WorkflowDefinicionId = recorrido.WorkflowDefinicionId;

            return View(controlRecorrido);
        }

        [HttpPost]
        public ActionResult Index(ControlRecorridoDto controlRecorrido, string workflow, int workflowDefinicionId)
        {
            var controlPesoNetoDescargaUnidadService = factory.CrearServicio(workflowDefinicionId);
            var resultado = controlPesoNetoDescargaUnidadService.ControlPesoNeto(controlRecorrido, controlRecorrido.WorkflowInstanceId);
            if (!resultado.HayErrores)
            {
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            ModelState.AgregarErrores(resultado);
            return RedirectToAction("Index", new { id = controlRecorrido.WorkflowInstanceId });
        }
    }
}
