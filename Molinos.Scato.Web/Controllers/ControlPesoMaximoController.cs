using System;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadControlPesoMaximo)]
    public class ControlPesoMaximoController : BaseController
    {
        private readonly IServicioActividadFactory<IControlPesoMaximoService> factory;
        private ILogger log;

        public ControlPesoMaximoController(ILogger log, IServicioActividadFactory<IControlPesoMaximoService> factory, IServicioRepositorio servicio)
            : base(servicio)
        {
            this.factory = factory;
            this.log = log;
        }

        [DatosUsuario]
        public ActionResult Index(Guid id, DatosUsuario datosUsuario)
        {
            var recorrido = servicio.ObtenerRecorridoPorGuid(id);

            ViewBag.ControlPesoMaximoNotifica = recorrido.DatosProximaActividad ==  "SoloNotifica";

            var controlRecorrido = new ControlRecorridoDto
            {
                WorkflowInstanceId = id,
                NombreUsuario = datosUsuario.NombreUsuario,
                Actividad = Textos.ActControlPesoMaximo,
                ActividadXaml = "ControlPesoMaximo",
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId
            };

            // En este punto ya tiene que haber un peso bruto

            ViewBag.Peso = recorrido.PesoBruto;
            ViewBag.PesoMaximo = servicio.LeerPesoMaximo(id, TipoDeWorkflow.Egreso);
            ViewBag.PesoMaximoReal = servicio.LeerPesoMaximo(id, recorrido.Workflow.TipoDeWorkflow);
            ViewBag.Workflow = recorrido.Workflow.Codigo;
            ViewBag.WorkflowDefinicionId = recorrido.WorkflowDefinicionId;
            ViewBag.TipoDeWorkflow = recorrido.Workflow.TipoDeWorkflow;
            CargarMotivos();

            return View(controlRecorrido);
        }

        [HttpPost]
        public ActionResult Index(ControlRecorridoDto controlRecorrido, string workflow, int workflowDefinicionId)
        {
            var controlPesoMaximoService = factory.CrearServicio(workflowDefinicionId);
            var resultado = controlPesoMaximoService.ControlPesoMaximo(controlRecorrido, controlRecorrido.WorkflowInstanceId);
            if (!resultado.HayErrores)
            {
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            ModelState.AgregarErrores(resultado);
            return RedirectToAction("Index", new { id = controlRecorrido.WorkflowInstanceId });
        }

        private void CargarMotivos()
        {
            ViewBag.Motivos = servicio.ListarMotivos().ToSelectList(x => x.Descripcion, x => x.Descripcion);
        }
    }
}
