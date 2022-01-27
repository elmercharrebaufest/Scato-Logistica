using System;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio;
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
    [Autorizacion(PermisosScato.ActividadVisteo)]
    public class VisteoController : BaseController
    {
        private readonly IServicioActividadFactory<IVisteoService> factory;
        private ILogger log;

        public VisteoController(ILogger log, IServicioActividadFactory<IVisteoService> factory, IServicioRepositorio servicio)
            : base(servicio)
        {
            this.factory = factory;
            this.log = log;
        }

        [DatosUsuario]
        public ActionResult Index(Guid id, DatosUsuario datosUsuario)
        {
            var recorrido = servicio.ObtenerRecorridoPorGuid(id);
            var cliente = servicio.ObtenerClientePorInstanceId(recorrido.InstanciaWorkflow, recorrido.TipoDocumentoIngreso);
            
            var controlRecorrido = new ControlRecorridoDto
            {
                WorkflowInstanceId = id,
                NombreUsuario = datosUsuario.NombreUsuario,
                Actividad = Textos.ActVisteo,
                ActividadXaml = "Visteo",
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId
            };

            ViewBag.Material = recorrido.Material.Descripcion;
            ViewBag.EsGrano = recorrido.Material.EsGrano;
            ViewBag.TipoComercial = recorrido.TipoComercial.Descripcion;
            ViewBag.Workflow = recorrido.Workflow.Codigo;
            ViewBag.Patente = recorrido.Patente;
            ViewBag.WorkflowDefinicionId = recorrido.WorkflowDefinicionId;
            ViewBag.RecorridoId = recorrido.Id;
            if (cliente != null)
            {
                ViewBag.Cliente = cliente.Descripcion;
                ViewBag.ClienteCodigoSap = cliente.CodigoSap;
            }
            else
            {
                ViewBag.Cliente = null;
                ViewBag.ClienteCodigoSap = null;
            }
            var ultimoEstado = servicio.ObtenerEstadoUltimoRecorrido(recorrido.InstanciaWorkflow);

            if (ultimoEstado != null)
            {
                ViewBag.Estado = String.Format(@Textos.MotivoRechazoCamion, ultimoEstado.fecha.ToString("dd-MM-yyyy"), ultimoEstado.Descripcion);
            }
            return View(controlRecorrido);
        }

        [HttpPost]
        public ActionResult Index(ControlRecorridoDto controlRecorrido, string workflow, int workflowDefinicionId)
        {
            var service = factory.CrearServicio(workflowDefinicionId);

            var resultado = service.Visteo(controlRecorrido, controlRecorrido.WorkflowInstanceId);
            if (!resultado.HayErrores)
            {
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            ModelState.AgregarErrores(resultado);
            return RedirectToAction("Index", new { id = controlRecorrido.WorkflowInstanceId });
        }

        [DatosUsuario]
        public ActionResult Rechazar(string codigoWf, int workflowDefinicionId, Guid instanceId, DatosUsuario datosUsuario)
        {
            log.Info("{0} - Visteo Rechazar", instanceId);
            ViewBag.Motivos = servicio.ListarMotivos().ToSelectList(x => x.Descripcion, x => x.Descripcion);
            var actividad = servicio.ObtenerEntidadActividadPorCodigos(Constantes.Entidad.Visteo, Constantes.TipoDeActividad.Rechazar);
            ViewBag.Workflow = codigoWf;
            ViewBag.WorkflowDefinicionId = workflowDefinicionId;
            var controlRecorrido = new ControlRecorridoDto
            {
                WorkflowInstanceId = instanceId,
                NombreUsuario = datosUsuario.NombreUsuario,
                Actividad = actividad.Entidad.Descripcion + "/" + actividad.TipoDeActividad.Descripcion,
                ActividadXaml = actividad.Entidad.Descripcion,
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId
            };
            return View("_VehiculoRechazado", controlRecorrido);
        }

    }
}