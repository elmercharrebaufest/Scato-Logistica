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
    [Autorizacion(PermisosScato.ActividadVerificacionCamionRechazado)]
    public class VerificacionCamionRechazadoController : BaseController
    {
        private readonly IServicioActividadFactory<IVerificacionCamionRechazadoService> factory;
        private ILogger log;

        public VerificacionCamionRechazadoController(ILogger log, IServicioActividadFactory<IVerificacionCamionRechazadoService> factory, IServicioRepositorio servicio)
            : base(servicio)
        {
            this.factory = factory;
            this.log = log;
        }

        [DatosUsuario]
        public ActionResult Index(Guid id, DatosUsuario datosUsuario)
        {
            var recorrido = servicio.ObtenerRecorridoPorGuid(id);

            var controlRecorrido = new ControlRecorridoDto
            {
                WorkflowInstanceId = id,
                NombreUsuario = datosUsuario.NombreUsuario,
                Actividad = Textos.ActVerificacionCamionRechazado,
                ActividadXaml = "VerificacionCamionRechazado",
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId
            };

            ViewBag.Peso = recorrido.PesoBruto;
            ViewBag.PesoMaximo = servicio.ObtenerPesoMaximo(recorrido.TipoVehiculo, datosUsuario.CentroId);
            ViewBag.Workflow = recorrido.Workflow.Codigo;
            ViewBag.WorkflowDefinicionId = recorrido.WorkflowDefinicionId;
            CargarMotivos();

            return View(controlRecorrido);
        }

        [HttpPost]
        public ActionResult Index(ControlRecorridoDto controlRecorrido, string workflow, int workflowDefinicionId)
        {
            var camionRechazadoService = factory.CrearServicio(workflowDefinicionId);
            var resultado = camionRechazadoService.VerificacionCamionRechazado(controlRecorrido, controlRecorrido.WorkflowInstanceId);
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
