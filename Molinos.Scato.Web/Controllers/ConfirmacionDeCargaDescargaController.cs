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
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadConfirmacionDeCargaDescarga)]
    public class ConfirmacionDeCargaDescargaController : BaseController
    {
        private readonly IServicioActividadFactory<IEjecutarService> factory;
        private ILogger log;

        public ConfirmacionDeCargaDescargaController(ILogger log, IServicioActividadFactory<IEjecutarService> factory, IServicioRepositorio servicio, IServicioComandos comandos)
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
                Actividad = Textos.ActConfirmacionDeCargaDescarga,
                ActividadXaml = "ConfirmacionDeCargaDescarga",
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId
            };

            var esCarga = recorrido.Workflow.TipoDeWorkflow == TipoDeWorkflow.Egreso;
            ViewBag.EsCarga = esCarga;

            ViewBag.Material = recorrido.Material.Descripcion;
            ViewBag.Almacen = recorrido.Almacen.Descripcion;
            ViewBag.TipoComercial = recorrido.TipoComercial.Descripcion;
            ViewBag.Workflow = recorrido.Workflow.Codigo;
            ViewBag.TipoDocumentoIngreso = recorrido.TipoDocumentoIngreso;
            ViewBag.NumeroDocumentoIngreso = recorrido.NumeroDocumentoIngreso;
            ViewBag.Patente = recorrido.Patente;
            ViewBag.Peso = esCarga ? 0 : recorrido.PesoNetoOrigen;
            ViewBag.Calidad = "";
            ViewBag.WorkflowDefinicionId = recorrido.WorkflowDefinicionId;
            return View(controlRecorrido);
        }

        [HttpPost]
        public ActionResult Index(ControlRecorridoDto controlRecorrido, string workflow, int workflowDefinicionId)
        {
            controlRecorrido.Decision = true;

            var serviciowf = factory.CrearServicio(workflowDefinicionId);

            var resultado = serviciowf.Ejecutar(controlRecorrido.WorkflowInstanceId, controlRecorrido);
            if (!resultado.HayErrores)
            {
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            TempData["Alerta"] = resultado.Errores.Values.FirstOrDefault() ?? Textos.Error_ActualizarGenerico;
            TempData["TipoAlerta"] = TipoAlerta.Advertencia;
            return RedirectToAction("Index", "ListaDeCamiones");
        }
    }
}
