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
    [Autorizacion(PermisosScato.ActividadIngresosBodega)]
    public class ServicioSapIngresosBodegaController : BaseController
    {
        private readonly IServicioActividadFactory<IServicioSapIngresosBodegaService> factory;
        private readonly ILogger log;

        public ServicioSapIngresosBodegaController(ILogger log, IServicioActividadFactory<IServicioSapIngresosBodegaService> factory, IServicioRepositorio servicio)
            : base(servicio)
        {
            this.factory = factory;
            this.log = log;
        }

        public ActionResult Index(Guid id)
        {
            var recorrido = servicio.ObtenerDatosDeInstanciaPorGuid(id);
            var model = new ServicioASapDto
                {
                    WorkflowCodigo = recorrido.WorkflowCodigo,
                    WorkflowId = id
                };
            var control = servicio.ObtenerControlRecorrido(id, Textos.ActServicioSapIngresosBodega);
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
        public ActionResult Encolar(ServicioASapDto model, int workflowDefinicionId, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var controlRecorrido = new ControlRecorridoDto
                    {
                        Actividad = Textos.ActServicioSapIngresosBodega,
                        ActividadXaml = "ServicioSapIngresosBodega",
                        WorkflowInstanceId = model.WorkflowId,
                        PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                        NombreUsuario = datosUsuario.NombreUsuario
                    };

                var serviciowf = factory.CrearServicio(workflowDefinicionId);
                var resultado = serviciowf.ServicioSapIngresosBodega(model.WorkflowId, true, controlRecorrido);
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
        public ActionResult Reintentar(ServicioASapDto model, int workflowDefinicionId, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var controlRecorrido = new ControlRecorridoDto
                {
                    Actividad = Textos.ActServicioSapIngresosBodega,
                    ActividadXaml = "ServicioSapIngresosBodega",
                    WorkflowInstanceId = model.WorkflowId,
                    PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                    NombreUsuario = datosUsuario.NombreUsuario
                };

                var serviciowf = factory.CrearServicio(workflowDefinicionId);
                var resultado = serviciowf.ServicioSapIngresosBodega(model.WorkflowId, false, controlRecorrido);
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
