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
    [Autorizacion(PermisosScato.ActividadControlDePesoEsperado)]
    public class ControlDePesoEsperadoController : BaseController
    {
        private readonly IServicioActividadFactory<IControlDePesoEsperadoService> factory;
        private ILogger log;

        public ControlDePesoEsperadoController(ILogger log, IServicioActividadFactory<IControlDePesoEsperadoService> factory, IServicioRepositorio servicio)
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
                Actividad = Textos.ActControlDePesoEsperado,
                ActividadXaml = "ControlDePesoEsperado",
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId
            };

            ViewBag.Material = recorrido.Material.Descripcion;
            ViewBag.Transportista = recorrido.Transportista.RazonSocial;
            ViewBag.TipoComercial = recorrido.TipoComercial.Descripcion;
            ViewBag.PesoEsperado = recorrido.TipoComercial.PesoEsperado;
            ViewBag.PesoBalanza = recorrido.PesoBruto.HasValue && recorrido.PesoTara.HasValue ? 
                recorrido.PesoBruto - recorrido.PesoTara: null;
            ViewBag.Workflow = recorrido.Workflow.Codigo;
            ViewBag.WorkflowDesc = recorrido.Workflow.Descripcion;
            ViewBag.TipoDocumentoIngreso = recorrido.TipoDocumentoIngreso;
            ViewBag.NumeroDocumentoIngreso = recorrido.NumeroDocumentoIngreso;
            ViewBag.Patente = recorrido.Patente;
            ViewBag.WorkflowDefinicionId = recorrido.WorkflowDefinicionId;
            return View(controlRecorrido);
        }

        [HttpPost]
        public ActionResult Index(ControlRecorridoDto controlRecorrido, string workflow, int workflowDefinicionId)
        {
            var controlDePesoEsperadoService = factory.CrearServicio(workflowDefinicionId);
            var resultado = controlDePesoEsperadoService.ControlDePesoEsperado(controlRecorrido, controlRecorrido.WorkflowInstanceId);
            if (!resultado.HayErrores)
            {
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            ModelState.AgregarErrores(resultado);
            return RedirectToAction("Index", new { id = controlRecorrido.WorkflowInstanceId });
        }
    }
}
