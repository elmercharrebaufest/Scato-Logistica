using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
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
    [Autorizacion(PermisosScato.IngresoDeDatosDeExportacion)]
    public class IngresoDeDatosDeExportacionController : BaseController
    {
        private readonly IServicioActividadFactory<IIngresoDeDatosDeExportacionService> factory;
        private ILogger log;

        public IngresoDeDatosDeExportacionController(ILogger log, IServicioActividadFactory<IIngresoDeDatosDeExportacionService> factory, IServicioRepositorio servicio)
            : base(servicio)
        {
            this.factory = factory;
            this.log = log;
        }
        //
        // GET: /IngresoDeDatosDeExportacion/

        public ActionResult Index(Guid id)
        {
            var recorrido = servicio.ObtenerRecorridoPorGuid(id);
            ViewBag.TipoDocumentoIngreso = recorrido.TipoDocumentoIngreso.ToString();
            ViewBag.NumeroDocumentoIngreso = recorrido.NumeroDocumentoIngreso;
            ViewBag.Patente = recorrido.Patente;
            ViewBag.Material = recorrido.Material.Descripcion;
            ViewBag.Firmas = servicio.ListarFirmas().ToSelectList(dto => dto.Id.ToString(CultureInfo.InvariantCulture), dto => dto.RazonSocial);
            ViewBag.Nacionalidades = servicio.ListarPaises().ToSelectList(dto => dto.Id.ToString(CultureInfo.InvariantCulture), dto => dto.Descripcion).OrderBy(s => s.Text);
            return View(new IngresoDeDatosDeExportacionDto {InstanciaWorkflow = id, WorkflowDefinicionId = recorrido.WorkflowDefinicionId});
        }

        [DatosUsuario]
        [HttpPost]
        public ActionResult Index(IngresoDeDatosDeExportacionDto model, DatosUsuario datosUsuario)
        {
            if (ModelState.ContainsKey("Id"))
            {
                ModelState.Remove("Id"); //Para descartar que id como guid
            }
            if (ModelState.IsValid)
            {
                var controlRecorrido = new ControlRecorridoDto
                {
                    Actividad = Textos.ActIngresoDeDatosDeExportacion,
                    ActividadXaml = "IngresoDeDatosDeExportacion",
                    WorkflowInstanceId = model.InstanciaWorkflow,
                    PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                    NombreUsuario = datosUsuario.NombreUsuario
                };

                var ingresoDatosExportacionService = factory.CrearServicio(model.WorkflowDefinicionId);
                
                var resultado = ingresoDatosExportacionService.IngresoDeDatosDeExportacion(model.InstanciaWorkflow, controlRecorrido, model);
                if (!resultado.HayErrores)
                {
                    return RedirectToAction("Index", "ListaDeCamiones");
                }
                return RedirectToAction("Index", new {id = controlRecorrido.WorkflowInstanceId});
            }

            var recorrido = servicio.ObtenerRecorridoPorGuid(model.InstanciaWorkflow);
            ViewBag.TipoDocumentoIngreso = recorrido.TipoDocumentoIngreso.ToString();
            ViewBag.NumeroDocumentoIngreso = recorrido.NumeroDocumentoIngreso;
            ViewBag.Patente = recorrido.Patente;
            ViewBag.Material = recorrido.Material.Descripcion;
            ViewBag.Firmas = servicio.ListarFirmas().ToSelectList(dto => dto.Id.ToString(CultureInfo.InvariantCulture), dto => dto.RazonSocial);
            ViewBag.Nacionalidades = servicio.ListarPaises().ToSelectList(dto => dto.Id.ToString(CultureInfo.InvariantCulture), dto => dto.Descripcion).OrderBy(s => s.Text);
            return View(model);
        }
    }
}