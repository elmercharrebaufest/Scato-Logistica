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
    [Autorizacion(PermisosScato.ActividadAltaCTG)]
    public class AltaCTGController : BaseController
    {
        private readonly IServicioActividadFactory<IAltaCTGService> factory;
        private readonly ILogger log;

        public AltaCTGController(ILogger log, IServicioActividadFactory<IAltaCTGService> factory, IServicioRepositorio servicio)
            : base(servicio)
        {
            this.factory = factory;
            this.log = log;
        }

        public ActionResult Index(Guid id)
        {
            var recorrido = servicio.ObtenerDatosDeInstanciaAltaCTGPorGuid(id);
            var cargaCupo = servicio.ObtenerCupoRecorridoId(recorrido.Id);
            bool esCPE = cargaCupo is null ? servicio.ObtenerCartaDePortePorrecorrido(recorrido.Id)?.Cpe ?? false : cargaCupo?.CPE ?? false;
            var dto = new AltaCTGDto
                {
                    WorkflowId = id,
                    WorkflowCodigo = recorrido.WorkflowCodigo,
                    Cpe = esCPE
            };

            SetearVista(id, recorrido.WorkflowDefinicionId, recorrido.SolicitaConfirmarCTG, false);
            return View(dto);
        }

        [HttpPost]
        [HttpParamAction]
        [DatosUsuario]
        public ActionResult CargarAltaCtg(AltaCTGDto model, int workflowDefinicionId, bool verReintentar, DatosUsuario datosUsuario)
        {
            if (model.Cpe)
            {
                if(string.IsNullOrEmpty(model.Sucursal))
                    ModelState.AddModelError("Sucursal", string.Format(Textos.Error_Requerido, "Sucursal CPE"));
                if(string.IsNullOrEmpty(model.NroOrden))
                    ModelState.AddModelError("NroOrden", string.Format(Textos.Error_Requerido, "NroOrden CPE"));
            }
            if (ModelState.IsValid)
            {
                var controlRecorrido = new ControlRecorridoDto
                    {
                        Actividad = Textos.ActAltaCTG,
                        ActividadXaml = "AltaCTG",
                        WorkflowInstanceId = model.WorkflowId,
                        PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                        NombreUsuario = datosUsuario.NombreUsuario
                    };

                var serviciowf = factory.CrearServicio(workflowDefinicionId);
                var resultado = serviciowf.AltaCTG(model.WorkflowId, DecisionCtg.DarDeAltaManual, model.CodigoCTG,model.TarifaReferencia.Value, controlRecorrido, model.Sucursal, model.NroOrden);
                if (!resultado.HayErrores)
                {
                    return RedirectToAction("Index", "ListaDeCamiones");
                }
                ModelState.AgregarErrores(resultado);
            }
            SetearVista(model.WorkflowId, workflowDefinicionId, verReintentar, true);
            return View("index", model);
        }

        [HttpPost]
        [HttpParamAction]
        [DatosUsuario]
        public ActionResult ReintentarCtg(AltaCTGDto model, int workflowDefinicionId, bool verReintentar, DatosUsuario datosUsuario)
        {
            var controlRecorrido = new ControlRecorridoDto
            {
                Actividad = Textos.ActAltaCTG,
                ActividadXaml = "AltaCTG",
                WorkflowInstanceId = model.WorkflowId,
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                NombreUsuario = datosUsuario.NombreUsuario
            };

            var serviciowf = factory.CrearServicio(workflowDefinicionId);
            var resultado = serviciowf.AltaCTG(model.WorkflowId, DecisionCtg.DarDeAltaAutomaticamente, "", 0, controlRecorrido, model.Sucursal, model.NroOrden);
            if (!resultado.HayErrores)
            {
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            SetearVista(model.WorkflowId, workflowDefinicionId, verReintentar, false);
            ModelState.AgregarErrores(resultado);
            return View("index", model);
        }

        [HttpPost]
        [HttpParamAction]
        public ActionResult Cancelar()
        {
            return RedirectToAction("Index", "ListaDeCamiones");
        }

        private void SetearVista(Guid id, int workflowDefinicionId, bool solicitaConfirmarCTG, bool postDeManual)
        {
            var control = servicio.ObtenerControlRecorrido(id, Textos.ActAltaCTG);
            if (control != null)
            {
                var resultado = new Resultado();
                resultado.Errores.Add("Errores", control.Comentario);
                ModelState.AgregarErrores(resultado);
            }
            ViewBag.WorkflowDefinicionId = workflowDefinicionId;
            ViewBag.VerReintentar = !solicitaConfirmarCTG;
            ViewBag.PostDeManual = postDeManual;
        }
    }
}
