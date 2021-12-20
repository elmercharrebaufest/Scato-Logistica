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
    [Autorizacion(PermisosScato.ActividadIngresosPorCompraDeGranos)]
    public class ServicioSapFill_Z1000Controller : BaseController
    {
        private readonly IServicioActividadFactory<IServicioSapFill_Z1000Service> factory;
        private readonly ILogger log;

        public ServicioSapFill_Z1000Controller(ILogger log, IServicioActividadFactory<IServicioSapFill_Z1000Service> factory, IServicioRepositorio servicio)
            : base(servicio)
        {
            this.factory = factory;
            this.log = log;
        }

        public ActionResult Index(Guid id)
        {
            var recorrido = servicio.ObtenerDatosDeInstanciaPorGuid(id);
            var model = new IngresosPorCompraDeGranosTransmisionASapDto
            {
                WorkflowCodigo = recorrido.WorkflowCodigo,
                InstanciaWorkflow = id
            };

            var control = servicio.ObtenerControlRecorrido(id, Textos.ActIngresosPorCompraDeGranos);
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
        public ActionResult Encolar(IngresosPorCompraDeGranosTransmisionASapDto model, int workflowDefinicionId, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var controlRecorrido = new ControlRecorridoDto
                    {
                        Actividad = Textos.ActServicioSapFill_Z1000,
                        ActividadXaml = "ServicioSapFill_Z1000",
                        WorkflowInstanceId = model.InstanciaWorkflow,
                        PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                        NombreUsuario = datosUsuario.NombreUsuario
                    };

                var serviciowf = factory.CrearServicio(workflowDefinicionId);
                var resultado = serviciowf.ServicioSapFill_Z1000(model.InstanciaWorkflow, true, controlRecorrido);
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
        public ActionResult Reintentar(IngresosPorCompraDeGranosTransmisionASapDto model, int workflowDefinicionId, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var controlRecorrido = new ControlRecorridoDto
                {
                    Actividad = Textos.ActServicioSapFill_Z1000,
                    ActividadXaml = "ServicioSapFill_Z1000",
                    WorkflowInstanceId = model.InstanciaWorkflow,
                    PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                    NombreUsuario = datosUsuario.NombreUsuario
                };

                var serviciowf = factory.CrearServicio(workflowDefinicionId);
                var resultado = serviciowf.ServicioSapFill_Z1000(model.InstanciaWorkflow, false, controlRecorrido);
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
