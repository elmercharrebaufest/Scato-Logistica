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
using System;
using System.Globalization;
using System.Web.Mvc;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadAltaCTGDG)]
    public class AltaCTGDGController : BaseController
    {
        private readonly IServicioActividadFactory<IAltaCTGDGService> factory;
        private readonly ILogger log;

        public AltaCTGDGController(ILogger log, IServicioActividadFactory<IAltaCTGDGService> factory, IServicioRepositorio servicio)
            : base(servicio)
        {
            this.factory = factory;
            this.log = log;
        }

        public ActionResult Index(Guid id)
        {
            var recorrido = servicio.ObtenerDatosDeInstanciaAltaCTGPorGuid(id);
            var dto = new AltaCTGDto
                {
                    WorkflowId = id,
                    WorkflowCodigo = recorrido.WorkflowCodigo
            };

            SetearVista(id, recorrido.WorkflowDefinicionId, recorrido.SolicitaConfirmarCTG, false);
            return View(dto);
        }

        [HttpPost]
        [HttpParamAction]
        [DatosUsuario]
        public ActionResult CargarAltaCtg(AltaCTGDto model, int workflowDefinicionId, bool verReintentar, DatosUsuario datosUsuario)
        {
            if (string.IsNullOrEmpty(model.Sucursal))
                ModelState.AddModelError("Sucursal", string.Format(Textos.Error_Requerido, "Sucursal CPE"));
            if (string.IsNullOrEmpty(model.NroOrden))
                ModelState.AddModelError("NroOrden", string.Format(Textos.Error_Requerido, "NroOrden CPE"));
            if (!string.IsNullOrEmpty(model.Sucursal) && model.Sucursal.Length > 5)
                ModelState.AddModelError("Sucursal", "Sucursal CPE no debe exceder de 5 dígitos");
            if (!string.IsNullOrEmpty(model.NroOrden) && model.NroOrden.Length > 8)
                ModelState.AddModelError("NroOrden", "NroOrden CPE no debe exceder de 8 dígitos");

            if (ModelState.IsValid)
            {
                var controlRecorrido = new ControlRecorridoDto
                {
                    Actividad = Textos.ActAltaCTGDG,
                    ActividadXaml = "AltaCTGDG",
                    WorkflowInstanceId = model.WorkflowId,
                    PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                    NombreUsuario = datosUsuario.NombreUsuario
                };

                var serviciowf = factory.CrearServicio(workflowDefinicionId);
                var resultado = serviciowf.AltaCTGDG(model.WorkflowId, DecisionCtg.DarDeAltaManual, model.CodigoCTG, controlRecorrido, model.Sucursal, model.NroOrden);
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
                Actividad = Textos.ActAltaCTGDG,
                ActividadXaml = "AltaCTGDG",
                WorkflowInstanceId = model.WorkflowId,
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                NombreUsuario = datosUsuario.NombreUsuario
            };

            var serviciowf = factory.CrearServicio(workflowDefinicionId);
            var resultado = serviciowf.AltaCTGDG(model.WorkflowId, DecisionCtg.DarDeAltaAutomaticamente, "", controlRecorrido, model.Sucursal, model.NroOrden);
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
        [DatosUsuario]
        public ActionResult Rechazar(AltaCTGDto model, int workflowDefinicionId, bool verReintentar, DatosUsuario datosUsuario)
        {
            var controlRecorrido = new ControlRecorridoDto
            {
                Actividad = Textos.ActAltaCTGDG,
                ActividadXaml = "AltaCTGDG",
                WorkflowInstanceId = model.WorkflowId,
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                NombreUsuario = datosUsuario.NombreUsuario
            };
            var serviciowf = factory.CrearServicio(workflowDefinicionId);
            var resultado = serviciowf.AltaCTGDG(model.WorkflowId, DecisionCtg.Rechazar, "", controlRecorrido, model.Sucursal, model.NroOrden);
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

        [HttpGet]
        public JsonResult ValidarCTGyCPE(string ctg, string cpe, string sucursal)
        {
            var respuesta = RespuestaEstandarDto.Crear<dynamic>();

            if (!string.IsNullOrEmpty(ctg))
            {
                var ctgEnUso = servicio.ValidarAltaCTGRepetida(ctg);
                if (ctgEnUso)
                {
                    var mensaje = string.Format(CultureInfo.InvariantCulture, Textos.AltaCTG_ValidarCTGRepetido, ctg);
                    respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = mensaje, TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                }
            }
            var sucursalInt = 0;
            if (!string.IsNullOrEmpty(cpe) && int.TryParse(sucursal, out sucursalInt))
            {
                var cpeEnUso = servicio.ValidarAltaCPERepetida(cpe, sucursalInt);
                if (cpeEnUso)
                {
                    var mensaje = string.Format(CultureInfo.InvariantCulture, Textos.AltaCTG_ValidarCPERepetido, sucursal, cpe);
                    respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = mensaje, TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                }
            }

            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }

        private void SetearVista(Guid id, int workflowDefinicionId, bool solicitaConfirmarCTG, bool postDeManual)
        {
            var control = servicio.ObtenerControlRecorrido(id, Textos.ActAltaCTGDG);
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