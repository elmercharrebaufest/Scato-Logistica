using System;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
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
    [Autorizacion(PermisosScato.ActividadVerificarImpresion)]
    public class VerificarImpresionController : BaseController
    {
        private readonly IServicioActividadFactory<IVerificarImpresionService> factory;
        private ILogger log;

        public VerificarImpresionController(ILogger log, IServicioActividadFactory<IVerificarImpresionService> factory, IServicioRepositorio servicio)
            : base(servicio)
        {
            this.factory = factory;
            this.log = log;
        }

        [DatosUsuario]
        public ActionResult Index(Guid id, DatosUsuario datosUsuario)
        {
            var recorrido = servicio.ObtenerRecorridoPorGuid(id);

            var controlRecorrido = new ControlRecorridoDto
            {
                WorkflowInstanceId = id,
                NombreUsuario = datosUsuario.NombreUsuario,
                Actividad = Textos.ActVerificarImpresion,
                ActividadXaml = "VerificarImpresion",
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId
            };
            ViewBag.Workflow = recorrido.Workflow.Codigo;
            ViewBag.WorkflowDefinicionId = recorrido.WorkflowDefinicionId;
            return View(controlRecorrido);
        }

        [HttpPost]
        public ActionResult Index(ControlRecorridoDto controlRecorrido, string workflow, int workflowDefinicionId)
        {
            log.Debug("Index Verificar Impresión CIU , Wf:{0}",workflow);
            var service = factory.CrearServicio(workflowDefinicionId);
            var resultado = service.VerificarImpresion(controlRecorrido, controlRecorrido.WorkflowInstanceId);
            if (!resultado.HayErrores)
            {
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            TempData["Alerta"] = Textos.ErrorEliminarImpresion;
            TempData["TipoAlerta"] = TipoAlerta.Advertencia;
            log.Error("Error al eliminar la impresion: {0}", resultado.Errores.FirstOrDefault(x => x.Key == "EliminarImpresion").Value);
            return RedirectToAction("Index", "ListaDeCamiones");
        }
    }
}
