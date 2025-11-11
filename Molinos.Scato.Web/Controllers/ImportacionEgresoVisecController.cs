using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadImportacionEgresoVisec)]
    public class ImportacionEgresoVisecController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioActividadFactory<IImportacionEgresoVisecService> factory;

        public ImportacionEgresoVisecController(IServicioRepositorio servicio, ILogger log, IServicioComandos servicioComandos, IServicioActividadFactory<IImportacionEgresoVisecService> factory)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
            this.factory = factory;
        }

        public ActionResult Index(Guid id)
        {
            var recorrido = servicio.ObtenerRecorridoPorGuid(id);
            var viewModel = new ImportacionEgresoVisecViewModel
            {
                CartaPorte = recorrido.NumeroDocumentoIngreso,
                Patente = recorrido.Patente,
                Material = recorrido.Material.Descripcion,
                PesoVisecRequerido = recorrido.PesoNeto.GetValueOrDefault(),
                InstanciaWorkflow = recorrido.InstanciaWorkflow,
                WorkflowDefinicionId = recorrido.WorkflowDefinicionId,
            };
            return View(viewModel);
        }

        [DatosUsuario]
        [HttpPost]
        public ActionResult Reintentar(ImportacionEgresoVisecViewModel model, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                RegistrarControlRecorrido(datosUsuario, model.InstanciaWorkflow, "Reintento de importación de egreso VISEC");
                var service = factory.CrearServicio(model.WorkflowDefinicionId);
                var resultado = service.ImportacionEgresoVisec(model.InstanciaWorkflow);
                if (!resultado.HayErrores)
                    return RedirectToAction("Index", "ListaDeCamiones");
                
                TempData["Alerta"] = resultado.Errores.First().Value;
                TempData["TipoAlerta"] = TipoAlerta.Error;
                return View("Index", model);
            }
            return View("Index", model);
        }

        private void RegistrarControlRecorrido(DatosUsuario datosUsuario, Guid instanciaWorkflow, string mensaje)
        {
            servicioComandos.Ejecutar(new CrearControlRecorrido
            {
                Dto = new ControlRecorridoDto
                {
                    WorkflowInstanceId = instanciaWorkflow,
                    NombreUsuario = datosUsuario.NombreUsuario,
                    Actividad = Textos.ActImportacionEgresoVisec,
                    Mensaje = mensaje,
                    Fecha = DateTime.Now,
                    PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId
                }
            });
        }
    }
}