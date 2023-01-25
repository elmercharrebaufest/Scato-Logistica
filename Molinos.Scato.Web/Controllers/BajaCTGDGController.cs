using System;
using System.Linq;
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
    [Autorizacion(PermisosScato.ActividadBajaCTG)]
    public class BajaCTGDGController : BaseController
    {
        private readonly IServicioActividadFactory<IBajaCTGService> factory;
        private readonly IServicioComandos comandos;
        private readonly ILogger log;

        public BajaCTGDGController(ILogger log, IServicioActividadFactory<IBajaCTGService> factory, IServicioRepositorio servicio, IServicioComandos comandos)
            : base(servicio)
        {
            this.factory = factory;
            this.comandos = comandos;
            this.log = log;
        }

        public ActionResult Index(Guid id)
        {
            var recorrido = servicio.ObtenerDatosDeInstanciaAltaCTGPorGuid(id);
            var cargaCupo = servicio.ObtenerCupoPorRecorrido(recorrido.Id);
            bool esCPE = cargaCupo is null ? servicio.ObtenerCartaDePortePorrecorrido(recorrido.Id)?.Cpe ?? false : cargaCupo?.CPE ?? false;
            var dto = new BajaCTGDto
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
        public ActionResult CargarBajaCtg(BajaCTGDto model, int workflowDefinicionId, bool verReintentar, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var controlRecorrido = new ControlRecorridoDto
                    {
                        Actividad = Textos.ActBajaCTG,
                        ActividadXaml = "BajaCTG",
                        WorkflowInstanceId = model.WorkflowId,
                        PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                        NombreUsuario = datosUsuario.NombreUsuario
                    };

                var serviciowf = factory.CrearServicio(workflowDefinicionId);
                var resultado = serviciowf.BajaCTG(model.WorkflowId, DecisionCtg.DarDeBajaManual, model.CodigoDeBaja, controlRecorrido);
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
        public ActionResult ReintentarCtg(BajaCTGDto model, int workflowDefinicionId, bool verReintentar, DatosUsuario datosUsuario)
        {
            var controlRecorrido = new ControlRecorridoDto
            {
                Actividad = Textos.ActBajaCTG,
                ActividadXaml = "BajaCTG",
                WorkflowInstanceId = model.WorkflowId,
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                NombreUsuario = datosUsuario.NombreUsuario
            };

            var serviciowf = factory.CrearServicio(workflowDefinicionId);
            var resultado = serviciowf.BajaCTG(model.WorkflowId, DecisionCtg.DarDeBajaAutomaticamente, "", controlRecorrido);
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

        [HttpPost]
        [HttpParamAction]
        [DatosUsuario]
        public ActionResult Rechazar(BajaCTGDto model, int workflowDefinicionId, bool verReintentar, DatosUsuario datosUsuario)
        {

            var controlRecorrido = new ControlRecorridoDto
            {
                Actividad = Textos.ActBajaCTG,
                ActividadXaml = "BajaCTG",
                WorkflowInstanceId = model.WorkflowId,
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                NombreUsuario = datosUsuario.NombreUsuario
            };

            var serviciowf = factory.CrearServicio(workflowDefinicionId);
            var resultado = serviciowf.BajaCTG(model.WorkflowId, DecisionCtg.Rechazar, model.CodigoDeBaja, controlRecorrido);
            if (!resultado.HayErrores)
            {
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            ModelState.AgregarErrores(resultado);
            SetearVista(model.WorkflowId, workflowDefinicionId, verReintentar, true);
            return View("index", model);
        }

        [HttpPost]
        [HttpParamAction]
        [DatosUsuario]
        public ActionResult Demorar(BajaCTGDto model, int workflowDefinicionId, bool verReintentar, DatosUsuario datosUsuario,string MotivoDemora)
        {
            var controlRecorrido = new ControlRecorridoDto
            {
                Actividad = Textos.ActBajaCTG,
                ActividadXaml = "BajaCTG",
                WorkflowInstanceId = model.WorkflowId,
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                NombreUsuario = datosUsuario.NombreUsuario
            };
            comandos.Ejecutar(new ModificarRecorridoDemora { InstanceId = model.WorkflowId, Motivo = MotivoDemora, DemoraVehiculo = true });
            var serviciowf = factory.CrearServicio(workflowDefinicionId);
            var resultado = serviciowf.BajaCTG(model.WorkflowId, DecisionCtg.Demorar, model.CodigoDeBaja, controlRecorrido);
            if (!resultado.HayErrores)
            {
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            ModelState.AgregarErrores(resultado);
            SetearVista(model.WorkflowId, workflowDefinicionId, verReintentar, true);
            return View("index", model);
        }
        private void SetearVista(Guid id, int workflowDefinicionId, bool solicitaConfirmarCTG, bool postDeManual)
        {
            var control = servicio.ObtenerControlRecorrido(id, Textos.ActBajaCTG);
            if (control != null)
            {
                var resultado = new Resultado();
                resultado.Errores.Add("Errores", control.Comentario);
                ModelState.AgregarErrores(resultado);
            }
            ViewBag.WorkflowDefinicionId = workflowDefinicionId;
            ViewBag.VerReintentar = !solicitaConfirmarCTG;
            ViewBag.PostDeManual = postDeManual;

            var foto = servicio.ObtenerFotoCPDeCartaDePortePorrecorrido(id);
            if (foto.Fotos.Any())
            {
                ViewBag.FotoMesaDigitalizacion1 = foto.Fotos.First().Foto;
            }
        }
    }
}
