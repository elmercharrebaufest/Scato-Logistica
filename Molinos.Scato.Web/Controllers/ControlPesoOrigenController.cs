using System;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
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
    [Autorizacion(PermisosScato.ActividadControlPesoOrigen)]
    public class ControlPesoOrigenController : BaseController
    {
        private readonly IServicioActividadFactory<IControlPesoOrigenService> factory;
        private ILogger log;

        public ControlPesoOrigenController(ILogger log, IServicioActividadFactory<IControlPesoOrigenService> factory, IServicioRepositorio servicio)
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
                Actividad = Textos.ActControlPesoOrigen,
                ActividadXaml = "ControlPesoOrigen",
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId
            };

            ViewBag.TipoDocumentoIngreso = recorrido.TipoDocumentoIngreso;
            ViewBag.NumeroDocumentoIngreso = recorrido.NumeroDocumentoIngreso;
            ViewBag.Material = recorrido.Material.Descripcion;
            ViewBag.TipoComercial = recorrido.TipoComercial.Descripcion;
            //ViewBag.Proveedor = recorrido.Proveedor.Descripcion;
            ViewBag.PesoPlanta = recorrido.PesoBruto;
            ViewBag.PesoOrigen = recorrido.PesoBrutoOrigen;
            ViewBag.Workflow = recorrido.Workflow.Codigo;
            ViewBag.WorkflowDefinicionId = recorrido.WorkflowDefinicionId;
            CargarMotivos();

            return View(controlRecorrido);
        }

        [HttpPost]
        public ActionResult Index(ControlRecorridoDto controlRecorrido, string workflow, int workflowDefinicionId)
        {
            var controlPesoOrigenService = factory.CrearServicio(workflowDefinicionId);
            var resultado = controlPesoOrigenService.ControlPesoOrigen(controlRecorrido, controlRecorrido.WorkflowInstanceId);
            if (!resultado.HayErrores)
            {
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            ModelState.AgregarErrores(resultado);
            return RedirectToAction("Index", new { id = controlRecorrido.WorkflowInstanceId });
        }

        private void CargarMotivos()
        {
            ViewBag.Motivos = servicio.ListarMotivos().ToSelectList(x => x.Descripcion, x => x.Descripcion);
        }
    }
}
