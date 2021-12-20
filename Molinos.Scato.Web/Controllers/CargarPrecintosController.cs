using System;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadCargarPrecintos)]
    public class CargarPrecintosController : BaseController
    {
        private readonly IServicioActividadFactory<ICargarPrecintosService> factory;
        private ILogger log;

        public CargarPrecintosController(ILogger log, IServicioActividadFactory<ICargarPrecintosService> factory, IServicioRepositorio servicio)
            : base(servicio)
        {
            this.factory = factory;
            this.log = log;
        }

        public ActionResult Index(Guid id)
        {
            var recorrido = servicio.ObtenerRecorridoPorGuid(id);

            ViewBag.TipoDocumentoIngreso = recorrido.TipoDocumentoIngreso;
            ViewBag.NumeroDocumentoIngreso = recorrido.NumeroDocumentoIngreso;
            ViewBag.Patente = recorrido.Patente;
            ViewBag.Workflow = recorrido.Workflow.Codigo;
            ViewBag.Guid = id;
            ViewBag.WorkflowDefinicionId = recorrido.WorkflowDefinicionId;
            return View();
        }

        [DatosUsuario]
        public ActionResult CargarPrecintos(string precintos, string workflow, int workflowDefinicionId, Guid guid, DatosUsuario datosUsuario)
        {
            var listaPrecintos = precintos.FromJson<PrecintoDto[]>();

            var controlRecorrido = new ControlRecorridoDto
                {
                    Actividad = Textos.ActCargarPrecintos,
                    ActividadXaml = "CargarPrecintos",
                    WorkflowInstanceId = guid,
                    PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                    NombreUsuario = datosUsuario.NombreUsuario
                };
            
            var precintosService = factory.CrearServicio(workflowDefinicionId);
            Resultado resultado = precintosService.CargarPrecintos(listaPrecintos, guid, controlRecorrido);
            if (!resultado.HayErrores)
            {
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            ModelState.AgregarErrores(resultado);
            return RedirectToAction("Index", new { id = guid });
        }

        public JsonResult ObtenerPrecintos(Guid instanceId)
        {
            var precintos = servicio.ListarPrecintos(instanceId).ToList();
            return Json(precintos, JsonRequestBehavior.AllowGet);
        }
    }
}
