using System;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Filtros;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadBajaCTGDefinitivo)]
    public class BajaCTGDGDefinitivoController : BaseController
    {
        private readonly IServicioActividadFactory<IBajaCTGDefinitivoService> factory;
        private readonly ILogger log;

        public BajaCTGDGDefinitivoController(ILogger log, IServicioActividadFactory<IBajaCTGDefinitivoService> factory, IServicioRepositorio servicio)
            : base(servicio)
        {
            this.factory = factory;
            this.log = log;
        }

        public ActionResult Index(Guid id)
        {
            var recorrido = servicio.ObtenerDatosDeInstanciaAltaCTGPorGuid(id);
            var dto = new BajaCTGDto
                {
                    WorkflowId = id,
                    WorkflowCodigo = recorrido.WorkflowCodigo
                };
            SetearVista(id, recorrido.WorkflowDefinicionId, recorrido.SolicitaConfirmarCTG);
            return View(dto);
        }

        [HttpPost]
        [HttpParamAction]
        [DatosUsuario]
        public ActionResult CargarBajaCtg(BajaCTGDto model, int workflowDefinicionId, bool verReintentar, DatosUsuario datosUsuario)
        {
            var controlRecorrido = new ControlRecorridoDto
                {
                    Actividad = Textos.ActBajaCTGDGDefinitivo,
                    ActividadXaml = "BajaCTGDGDefinitivo",
                    WorkflowInstanceId = model.WorkflowId,
                    PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                    NombreUsuario = datosUsuario.NombreUsuario
                };

            var serviciowf = factory.CrearServicio(workflowDefinicionId);
            var resultado = serviciowf.BajaCTGDefinitivo(model.WorkflowId, DecisionCtg.DarDeBajaManual, controlRecorrido);
            if (!resultado.HayErrores)
            {
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            ModelState.AgregarErrores(resultado);
            SetearVista(model.WorkflowId, workflowDefinicionId, verReintentar);
            return View("index", model);
        }

        [HttpPost]
        [HttpParamAction]
        [DatosUsuario]
        public ActionResult ReintentarCtg(BajaCTGDto model, int workflowDefinicionId, bool verReintentar, DatosUsuario datosUsuario)
        {
            var controlRecorrido = new ControlRecorridoDto
            {
                Actividad = Textos.ActBajaCTGDGDefinitivo,
                ActividadXaml = "BajaCTGDGDefinitivo",
                WorkflowInstanceId = model.WorkflowId,
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                NombreUsuario = datosUsuario.NombreUsuario
            };

            var serviciowf = factory.CrearServicio(workflowDefinicionId);
            var resultado = serviciowf.BajaCTGDefinitivo(model.WorkflowId, DecisionCtg.DarDeBajaAutomaticamente, controlRecorrido);
            if (!resultado.HayErrores)
            {
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            SetearVista(model.WorkflowId, workflowDefinicionId, verReintentar);
            ModelState.AgregarErrores(resultado);
            return View("index", model);
        }

        [HttpPost]
        [HttpParamAction]
        public ActionResult Cancelar()
        {
            return RedirectToAction("Index", "ListaDeCamiones");
        }

        private void SetearVista(Guid id, int workflowDefinicionId, bool solicitaConfirmarCTG)
        {
            var control = servicio.ObtenerControlRecorrido(id, Textos.ActBajaCTGDefinitivo);
            if (control != null)
            {
                var resultado = new Resultado();
                resultado.Errores.Add("Errores", control.Comentario);
                ModelState.AgregarErrores(resultado);
            }
            ViewBag.WorkflowDefinicionId = workflowDefinicionId;
            ViewBag.VerReintentar = !solicitaConfirmarCTG;
        }
    }
}
