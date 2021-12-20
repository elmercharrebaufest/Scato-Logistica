using System;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
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
    [Autorizacion(PermisosScato.ActividadIngresoNumeroCot)]
    public class IngresoNumeroCotController : BaseController
    {
        private readonly IServicioActividadFactory<IIngresoNumeroCotService> factory;
        private ILogger log;

        public IngresoNumeroCotController(ILogger log, IServicioActividadFactory<IIngresoNumeroCotService> factory, IServicioRepositorio servicio)
            : base(servicio)
        {
            this.factory = factory;
            this.log = log;
        }

        [HttpGet]
        public ActionResult Index(Guid id)
        {
            var recorrido = servicio.ObtenerDatosDeInstanciaPorGuid(id);
            var control = servicio.ObtenerControlRecorrido(id, "IngresoNumeroCot");
            if (control != null)
            {
                var resultado = new Resultado();
                resultado.Errores.Add("", control.Comentario);
                ModelState.AgregarErrores(resultado);
            }
            return View(new IngresoNumeroCotModel() { InstanciaWorkflow = id, WorkflowDefinicionId = recorrido.WorkflowDefinicionId});
        }

        [HttpPost]
        public ActionResult Index(IngresoNumeroCotModel contratoModel)
        {
            return View(contratoModel);
        }

        public ActionResult Aceptar(IngresoNumeroCotModel contratoModel)
        {
            if (ModelState.IsValid)
            {
                return View("Confirmacion", contratoModel);
            }
            return View("Index",contratoModel);
        }

        [DatosUsuario]
        public ActionResult Confirmar(IngresoNumeroCotModel contratoModel, DatosUsuario datosUsuario)
        {
            var controlReorrido = new ControlRecorridoDto
                {
                    Actividad = Textos.ActIngresoNumeroCot,
                    ActividadXaml = "IngresoNumeroCot",
                    NombreUsuario = datosUsuario.NombreUsuario,
                    PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                    WorkflowInstanceId = contratoModel.InstanciaWorkflow
                };
            var contratoService = factory.CrearServicio(contratoModel.WorkflowDefinicionId);
            var resultado = contratoService.IngresoNumeroCot(contratoModel.InstanciaWorkflow, contratoModel.NumeroCot, controlReorrido);
            if (!resultado.HayErrores)
            {
                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            TempData["Alerta"] = Textos.IngresarNumeroCot_Error;
            TempData["TipoAlerta"] = TipoAlerta.Error;
            return Json("ERROR", JsonRequestBehavior.AllowGet);
        }
    }
}
