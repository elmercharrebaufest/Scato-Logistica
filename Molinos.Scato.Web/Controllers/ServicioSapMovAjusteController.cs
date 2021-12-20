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
    [Autorizacion(PermisosScato.ActividadMovimientoStockSap)]
    public class ServicioSapMovAjusteController : BaseController
    {
        private readonly IServicioActividadFactory<IServicioSapMovAjusteService> factory;
        private readonly ILogger log;

        public ServicioSapMovAjusteController(ILogger log, IServicioActividadFactory<IServicioSapMovAjusteService> factory, IServicioRepositorio servicio)
            : base(servicio)
        {
            this.factory = factory;
            this.log = log;
        }

        public ActionResult Index(Guid id)
        {
            var recorrido = servicio.ObtenerDatosDeInstanciaPorGuid(id);
            var model = new MovimientoStockSapDto
                {
                    WorkflowCodigo = recorrido.WorkflowCodigo,
                    WorkflowId = id
                };
            var control = servicio.ObtenerControlRecorrido(id, Textos.ActMovimientoStockSap);
            if (control != null)
            {
                var resultado = new Resultado();
                resultado.Errores.Add("WorkflowId", control.Comentario);
                ModelState.AgregarErrores(resultado);
            }
            ViewBag.WorkflowDefinicionId = recorrido.WorkflowDefinicionId;
            return View("Index", model);
        }

        [HttpPost]
        [HttpParamAction]
        [DatosUsuario]
        public ActionResult Encolar(MovimientoStockSapDto model, int workflowDefinicionId, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var controlRecorrido = new ControlRecorridoDto
                    {
                        Actividad = Textos.ActServicioSapMovAjuste,
                        ActividadXaml = "ServicioSapMovAjuste",
                        WorkflowInstanceId = model.WorkflowId,
                        PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                        NombreUsuario = datosUsuario.NombreUsuario
                    };

                var serviciowf = factory.CrearServicio(workflowDefinicionId);
                var resultado = serviciowf.ServicioSapMovAjuste(model.WorkflowId, true, controlRecorrido);
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
        public ActionResult Reintentar(MovimientoStockSapDto model, int workflowDefinicionId, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var controlRecorrido = new ControlRecorridoDto
                {
                    Actividad = Textos.ActServicioSapMovAjuste,
                    ActividadXaml = "ServicioSapMovAjuste",
                    WorkflowInstanceId = model.WorkflowId,
                    PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                    NombreUsuario = datosUsuario.NombreUsuario
                };

                var serviciowf = factory.CrearServicio(workflowDefinicionId);
                var resultado = serviciowf.ServicioSapMovAjuste(model.WorkflowId, false, controlRecorrido);
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
