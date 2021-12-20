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
    [Autorizacion(PermisosScato.ActividadIngresoCIU)]
    public class IngresoCIUController : BaseController
    {
        private readonly IServicioActividadFactory<IIngresoCIUService> factory;
        private ILogger log;
        private readonly IListaDeWorkflows workflows;

        public IngresoCIUController(ILogger log, IServicioActividadFactory<IIngresoCIUService> factory, IServicioRepositorio servicio, IListaDeWorkflows workflows)
            : base(servicio)
        {
            this.factory = factory;
            this.log = log;
            this.workflows = workflows;
        }
        public ActionResult Index(Guid id)
        {
            var recorrido = servicio.ObtenerDatosDeInstanciaPorGuid(id);
            return View(new IngresoCIUModel { InstanceId = id, WorkflowDefinicionId = recorrido.WorkflowDefinicionId, Workflow = recorrido.WorkflowCodigo });
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Index(IngresoCIUModel model, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (servicio.EsCiuAnulado(model.Numero))
                    {
                        ModelState.AddModelError("Numero", Textos.IngresoCiu_CiuAnulado);
                        return View(model);
                    }

                    var instanceId = servicio.ObtenerRecorridoGuidPorNumeroCiu(model.Numero);
                    if (instanceId != Guid.Empty && model.InstanceId != instanceId )
                    {
                        ModelState.AddModelError("Numero", Textos.IngresoCiu_CiuEnUso);
                        return View(model);
                    }
                    var controlRecorrido = new ControlRecorridoDto
                    {
                        Actividad = Textos.ActIngresoCIU,
                        ActividadXaml = "IngresoCIU",
                        WorkflowInstanceId = model.InstanceId,
                        PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                        NombreUsuario = datosUsuario.NombreUsuario
                    };

                    var accesoService = factory.CrearServicio(model.WorkflowDefinicionId);
                    var resultado = accesoService.IngresoCIU(model.InstanceId, model.Numero, controlRecorrido);
                    if (resultado.HayErrores)
                    {
                        ModelState.AgregarErrores(resultado);
                    }
                    else
                    {
                        return RedirectToAction("Index", "ListaDeCamiones");
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", Textos.IngresoCiu_Error);
                }
            }
            return View(model);
        }
    }
}
