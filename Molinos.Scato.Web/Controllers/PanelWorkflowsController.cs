using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.ApplicationServer.StoreManagement.Query;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.PanelWorkflows)]
    public class PanelWorkflowsController : BaseController
    {
        private readonly IListaDeWorkflows listaDeWorkflows;
        private ILogger log;
        public PanelWorkflowsController(IServicioRepositorio servicio, IListaDeWorkflows listaDeWorkflows, ILogger log)
            : base(servicio)
        {
            this.log = log;
            this.listaDeWorkflows = listaDeWorkflows;
        }

        [DatosUsuario]
        public ActionResult Index(FiltroListaDeWorkflowsDto filtro, DatosUsuario datosUsuario)
        {
            var paginacion = new Paginacion("FechaUltimaModificacion", DirOrden.Desc, 1, 25);
            filtro.CentroId = datosUsuario.CentroId;
            SetearVista(paginacion,filtro,datosUsuario);
            
            return View(filtro);
        }

        [DatosUsuario]
        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(DatosUsuario datosUsuario, FiltroListaDeWorkflowsDto filtro, int pagina = 1, string ordenarPor = "FechaUltimaModificacion", DirOrden dirOrden = DirOrden.Asc)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 25);
            filtro.CentroId = datosUsuario.CentroId;
            if (filtro.Patente != null)
            {
                filtro.Patente = filtro.Patente.ToUpper();
            }
            SetearVista(paginacion, filtro, datosUsuario);
            return View("Listar", filtro);
        }


        private void SetearVista(Paginacion paginacion, FiltroListaDeWorkflowsDto filtro, DatosUsuario datosUsuario)
        {
            var datosWorkflow = listaDeWorkflows.ListarTotalWorkFlows(paginacion, filtro);
            ViewBag.Items = datosWorkflow != null ? datosWorkflow.InstanciasWorkflowDto : null;
        }

        [DatosUsuario]
        public JsonResult ResumirWorkflow(Guid id, DatosUsuario usuario)
        {
            log.Info("El usuario {0} intenta resumir el workflow {1}", usuario.NombreUsuario, id);
            var workflow = listaDeWorkflows.ObtenerWorkflow(id);
            var estado = workflow.Estado;
            if (estado == InstanceStatus.Suspended)
            {
                var resultado = listaDeWorkflows.ResumirInstanciaWorkflow(id);
                if (!resultado.HayErrores)
                {
                    log.Info("El usuario {0} resumió el workflow {1}", usuario.NombreUsuario, id);
                    return Json("El comando para resumir el workflow fue encolado", JsonRequestBehavior.AllowGet);
                }
                return Json(resultado.Errores.FirstOrDefault().Value, JsonRequestBehavior.AllowGet);
            }
            return Json("El estado actual del workflow no permite que sea resumido", JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        [HttpPost]
        [Autorizacion(PermisosScato.EliminarWorkflows)]
        public ActionResult EliminarWorkflow(Guid id, DatosUsuario usuario)
        {
            log.Info("El usuario {0} intenta eliminar el workflow {1}", usuario.NombreUsuario, id);

            var resultado = listaDeWorkflows.EliminarInstanciaWorkflow(id);
            if (!resultado.HayErrores)
            {
                log.Info("El usuario {0} eliminó el workflow {1}", usuario.NombreUsuario, id);
                return Content("true");
            }
            return Content(resultado.Errores.FirstOrDefault().Value);
        }

        public ActionResult ListarSeguimientoWorkflow(Guid id)
        {
            ViewBag.LogItems = servicio.ConsultaControlRecorridoLogActividad(id);
            return PartialView("ListarLogActividadControlRecorrido");
        }
    }
}
