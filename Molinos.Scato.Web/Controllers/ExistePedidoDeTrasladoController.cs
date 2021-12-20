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
    [Autorizacion(PermisosScato.ActividadExistePedidoDeTraslado)]
    public class ExistePedidoDeTrasladoController : BaseController
    {
        private readonly IServicioActividadFactory<IExistePedidoDeTrasladoService> factory;
        private readonly ILogger log;

        public ExistePedidoDeTrasladoController(ILogger log, IServicioActividadFactory<IExistePedidoDeTrasladoService> factory, IServicioRepositorio servicio)
            : base(servicio)
        {
            this.factory = factory;
            this.log = log;
        }

        public ActionResult Index(Guid id)
        {
            var recorrido = servicio.ObtenerDatosDeInstanciaPorGuid(id);
            var dto = new ExistePedidoDeTrasladoDto
            {
                WorkflowId = id,
                WorkflowCodigo = recorrido.WorkflowCodigo
            };
            var control = servicio.ObtenerControlRecorrido(id, "ExistePedidoDeTraslado");
            if (control != null)
            {
                var resultado = new Resultado();
                resultado.Errores.Add("WorkflowId", control.Comentario);
                ModelState.AgregarErrores(resultado);
            }
            ViewBag.WorkflowDefinicionId = recorrido.WorkflowDefinicionId;
            return View(dto);
        }

        [HttpPost]
        [HttpParamAction]
        public ActionResult Reintentar(ExistePedidoDeTrasladoDto model, int workflowDefinicionId, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var serviciowf = factory.CrearServicio(workflowDefinicionId);

                var controlRecorrido = new ControlRecorridoDto
                {
                    WorkflowInstanceId = model.WorkflowId,
                    NombreUsuario = datosUsuario.NombreUsuario,
                    Actividad = Textos.ActExistePedidoDeTraslado,
                    ActividadXaml = "ExistePedidoDeTraslado",
                    Decision = true,
                    PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId
                };

                var resultado = serviciowf.ExistePedidoDeTraslado(model.WorkflowId, controlRecorrido);
                if (!resultado.HayErrores)
                {
                    return RedirectToAction("Index", "ListaDeCamiones");
                }
                ModelState.AgregarErrores(resultado);
            }
            ViewBag.WorkflowDefinicionId = workflowDefinicionId;
            return View("index", model);
        }

        [HttpPost]
        [HttpParamAction]
        public ActionResult Rechazar(ExistePedidoDeTrasladoDto model, int workflowDefinicionId, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var serviciowf = factory.CrearServicio(workflowDefinicionId);

                var controlRecorrido = new ControlRecorridoDto
                {
                    WorkflowInstanceId = model.WorkflowId,
                    NombreUsuario = datosUsuario.NombreUsuario,
                    Actividad = Textos.ActExistePedidoDeTraslado,
                    ActividadXaml = "ExistePedidoDeTraslado",
                    Decision = false,
                    PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId
                };

                var resultado = serviciowf.ExistePedidoDeTraslado(model.WorkflowId, controlRecorrido);
                if (!resultado.HayErrores)
                {
                    return RedirectToAction("Index", "ListaDeCamiones");
                }
                ModelState.AgregarErrores(resultado);
            }
            ViewBag.WorkflowDefinicionId = workflowDefinicionId;
            return View("index", model);
        }

        [HttpPost]
        [HttpParamAction]
        public ActionResult Cancelar()
        {
            return RedirectToAction("Index", "ListaDeCamiones");
        }
    }
}
