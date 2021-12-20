using System;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadAutorizarRecepcionUvas)]
    public class AutorizarRecepcionUvasController : BaseController
    {
        private readonly IServicioActividadFactory<IAutorizarRecepcionUvasService> factory;
        private ILogger log;

        public AutorizarRecepcionUvasController(ILogger log, IServicioActividadFactory<IAutorizarRecepcionUvasService> factory, IServicioRepositorio servicio)
            : base(servicio)
        {
            this.factory = factory;
            this.log = log;
        }

        [DatosUsuario]
        public ActionResult Index(Guid id, DatosUsuario datosUsuario)
        {
            var recorrido = servicio.ObtenerRecorridoPorGuid(id);
            var documentoDeIngreso = servicio.ObtenerRemitoBodegaUvaPorGuid(id);

            ViewBag.NumeroDocumentoIngreso = recorrido.NumeroDocumentoIngreso;
            ViewBag.Patente = recorrido.Patente;
            ViewBag.Proveedor = documentoDeIngreso.Proveedor;
            ViewBag.TipoComercial = recorrido.TipoComercial.Descripcion;
            ViewBag.Material = recorrido.Material.Descripcion;
            ViewBag.Vinedo = documentoDeIngreso.Vinedo;
            ViewBag.TipoDocumentoIngreso = recorrido.TipoDocumentoIngreso;
            ViewBag.Workflow = recorrido.Workflow.Codigo;
            ViewBag.WorkflowDefinicionId = recorrido.WorkflowDefinicionId;

            ViewBag.PesoNetoObtenido = recorrido.PesoBruto.HasValue && recorrido.PesoTaraBodega.HasValue ? recorrido.PesoBruto - recorrido.PesoTaraBodega : null;

            var resultado = servicio.ObtenerKilosARecibirPorVinedo(documentoDeIngreso.VinedoId, documentoDeIngreso.VariedadId, documentoDeIngreso.Cosecha);
            if (resultado.Error)
            {
                ViewBag.KgARecibirError = resultado.MensajeError;
            }
            else
            {
                ViewBag.KgARecibir = resultado.KilosARecibir + (ViewBag.PesoNetoObtenido ?? 0);
            }
            
            var controlRecorrido = new ControlRecorridoDto
            {
                WorkflowInstanceId = id,
                NombreUsuario = datosUsuario.NombreUsuario,
                Actividad = Textos.ActAutorizarRecepcionUvas,
                ActividadXaml = "AutorizarRecepcionUvas",
                Mensaje = String.Format(Textos.Error_AutorizarRecepcionUvas, recorrido.Patente, ViewBag.PesoNetoObtenido, ViewBag.Finca, recorrido.Material.VariedadDesc, ViewBag.KgARecibir),
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId
            };
            return View(controlRecorrido);
        }

        [HttpPost]
        public ActionResult Index(ControlRecorridoDto controlRecorrido, string workflow, int workflowDefinicionId)
        {
            var autorizarRecepcionUvasService = factory.CrearServicio(workflowDefinicionId);
            autorizarRecepcionUvasService.AutorizarRecepcionUvas(controlRecorrido.WorkflowInstanceId, controlRecorrido);
            return RedirectToAction("Index", "ListaDeCamiones");
        }
    }
}
