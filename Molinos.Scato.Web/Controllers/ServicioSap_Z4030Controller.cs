using System;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Filtros;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadFletesDobleTramo)]
    public class ServicioSap_Z4030Controller : BaseController
    {
        private readonly IServicioActividadFactory<IServicioSap_Z4030Service> factory;
        private readonly ILogger log;

        public ServicioSap_Z4030Controller(ILogger log, IServicioActividadFactory<IServicioSap_Z4030Service> factory, IServicioRepositorio servicio)
            : base(servicio)
        {
            this.factory = factory;
            this.log = log;
        }

        public ActionResult Index(Guid id)
        {
            var recorrido = servicio.ObtenerDatosDeInstanciaPorGuid(id);
            var model = new FletesDobleTramoTransmisionASapDto
            {
                WorkflowCodigo = recorrido.WorkflowCodigo,
                WorkflowId = id
            };

            var control = servicio.ObtenerControlRecorrido(id, Textos.ActFletesDobleTramo);
            if (control != null)
            {
                var resultado = new Resultado();
                resultado.Errores.Add("InstanciaWorkflow", control.Comentario);
                ModelState.AgregarErrores(resultado);
            }
            ViewBag.WorkflowDefinicionId = recorrido.WorkflowDefinicionId;
            return View("Index", model);
        }

        [HttpPost]
        [HttpParamAction]
        [DatosUsuario]
        public ActionResult Encolar(FletesDobleTramoTransmisionASapDto model, int workflowDefinicionId, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var controlRecorrido = new ControlRecorridoDto
                    {
                        Actividad = Textos.ActFletesDobleTramo,
                        ActividadXaml = "ServicioSap_Z4030",
                        WorkflowInstanceId = model.InstanciaWorkflow,
                        PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                        NombreUsuario = datosUsuario.NombreUsuario
                    };

                var serviciowf = factory.CrearServicio(workflowDefinicionId);
                var resultado = serviciowf.ServicioSap_Z4030(model.WorkflowId, true, controlRecorrido);
                if (!resultado.HayErrores)
                {
                    return RedirectToAction("Index", "ListaDeCamiones");
                }
                ModelState.AgregarErrores(resultado);
            }
            ViewBag.WorkflowDefinicionId = workflowDefinicionId;
            return View("Index", model);
        }

        [HttpPost]
        [HttpParamAction]
        [DatosUsuario]
        public ActionResult Reintentar(FletesDobleTramoTransmisionASapDto model, int workflowDefinicionId, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var controlRecorrido = new ControlRecorridoDto
                {
                    Actividad = Textos.ActFletesDobleTramo,
                    ActividadXaml = "ServicioSap_Z4030",
                    WorkflowInstanceId = model.InstanciaWorkflow,
                    PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                    NombreUsuario = datosUsuario.NombreUsuario
                };
                var serviciowf = factory.CrearServicio(workflowDefinicionId);
                var resultado = serviciowf.ServicioSap_Z4030(model.WorkflowId, false, controlRecorrido);
                if (!resultado.HayErrores)
                {
                    return RedirectToAction("Index", "ListaDeCamiones");
                }
                ModelState.AgregarErrores(resultado);
            }
            ViewBag.WorkflowDefinicionId = workflowDefinicionId;
            return View("Index", model);
        }

        [HttpPost]
        [HttpParamAction]
        public ActionResult Cancelar()
        {
            return RedirectToAction("Index", "ListaDeCamiones");
        }

    }
}
