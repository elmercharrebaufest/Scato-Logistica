using System;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Consultas;
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
    [Autorizacion(PermisosScato.ActividadAutorizarDescuentosEntregador)]
    public class AutorizarDescuentosEntregadorController : BaseController
    {
        private readonly IServicioActividadFactory<IAutorizarDescuentosEntregadorService> factory;
        private readonly ILogger log;
        private readonly IListaDeWorkflows workflows;

        public AutorizarDescuentosEntregadorController(ILogger log, IServicioRepositorio servicio, IServicioActividadFactory<IAutorizarDescuentosEntregadorService> factory, IListaDeWorkflows workflows)
            : base(servicio)
        {
            this.log = log;
            this.factory = factory;
            this.workflows = workflows;
        }

        public ActionResult Index(Guid id, int pagina = 1, string ordenarPor = "AnalisisDeCalidad", DirOrden dirOrden = DirOrden.Asc)
        {
            var recorrido = servicio.ObtenerRecorridoPorGuid(id);
            Listar(recorrido, pagina, ordenarPor, dirOrden);
            return View(recorrido);
        }

        private void Listar(RecorridoDto recorrido, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);
            ViewBag.Caracteristicas = servicio.ListarPaginadoAnalisisYCaladoPorCaracteristica(recorrido.InstanciaWorkflow, paginacion);
        }

        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(Guid id, int pagina = 1, string ordenarPor = "AnalisisDeCalidad", DirOrden dirOrden = DirOrden.Asc)
        {
            var recorrido = servicio.ObtenerRecorridoPorGuid(id);
            Listar(recorrido, pagina, ordenarPor, dirOrden);
            return View("Listar", recorrido);
        }

        [DatosUsuario]
        public ActionResult Rechazar(string codigoWf, int workflowDefinicionId, Guid instanceId, DatosUsuario datosUsuario)
        {
            CargarMotivos();
            ViewBag.Workflow = codigoWf;
            ViewBag.WorkflowDefinicionId = workflowDefinicionId;
            var controlRecorrido = new ControlRecorridoDto
            {
                WorkflowInstanceId = instanceId,
                NombreUsuario = datosUsuario.NombreUsuario,
                Actividad = Textos.ActAutorizarDescuentosEntregador,
                ActividadXaml = "AutorizarDescuentosEntregador",
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId
            };

            return View("_TransportistaRechazado", controlRecorrido);
        }

        [HttpPost]
        public ActionResult TransportistaRechazado(ControlRecorridoDto controlRecorrido, string workflow, int workflowDefinicionId)
        {
            var serviciowf = factory.CrearServicio(workflowDefinicionId);
            controlRecorrido.Decision = false;
            var resultado = serviciowf.AutorizarDescuentosEntregador(controlRecorrido.WorkflowInstanceId, controlRecorrido);
            if (!resultado.HayErrores)
            {
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            ModelState.AgregarErrores(resultado);
            return RedirectToAction("Index", new { id = controlRecorrido.WorkflowInstanceId });
        }

        [DatosUsuario]
        [HttpPost]
        public ActionResult Aceptar(string codigoWf, int workflowDefinicionId, Guid instanceId, DatosUsuario datosUsuario)
        {
            var instaceWorflow = workflows.ObtenerWorkflowPorGuid(instanceId); //workflows.ListarWorflows();

            if(instaceWorflow.ProximaAccion == "AutorizarDescuentosEntregador")
            {
                var serviciowf = factory.CrearServicio(workflowDefinicionId);

                var recorrido = CrearControlRecorrido(codigoWf, workflowDefinicionId, instanceId, datosUsuario, true);
                var resultado = serviciowf.AutorizarDescuentosEntregador(instanceId, recorrido);
                if (!resultado.HayErrores)
                {
                    return RedirectToAction("Index", "ListaDeCamiones");
                }
                ModelState.AgregarErrores(resultado);
                return RedirectToAction("Index", new { id = recorrido.WorkflowInstanceId });
            } else
            {
                TempData["FlagMostrarValidacionEtapaAutomatica"] = true;
                TempData["messageEtapaAutomatica"] = $"La tarea seleccionada se encuentra en otro etapa : {instaceWorflow.ProximaAccion}";
                return RedirectToAction("Index", "ListaDeCamiones");
            }   
        }

        private ControlRecorridoDto CrearControlRecorrido(string codigoWf, int workflowDefinicionId, Guid instanceId, DatosUsuario datosUsuario, bool decision)
        {
            ViewBag.Workflow = codigoWf;
            ViewBag.WorkflowDefinicionId = workflowDefinicionId;
            return new ControlRecorridoDto
            {
                WorkflowInstanceId = instanceId,
                NombreUsuario = datosUsuario.NombreUsuario,
                Actividad = Textos.ActAutorizarDescuentosEntregador,
                ActividadXaml = "AutorizarDescuentosEntregador",
                Decision = decision,
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId
            };
        }

        private void CargarMotivos()
        {
            ViewBag.Motivos = servicio.ListarMotivos().ToSelectList(x => x.Descripcion, x => x.Descripcion);
        }
    }
}
