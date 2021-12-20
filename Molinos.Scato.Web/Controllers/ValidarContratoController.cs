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
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadValidarContrato)]
    public class ValidarContratoController : BaseController
    {
        private readonly IServicioActividadFactory<IValidarContratoService> factory;
        private ILogger log;

        public ValidarContratoController(ILogger log, IServicioActividadFactory<IValidarContratoService> factory, IServicioRepositorio servicio)
            : base(servicio)
        {
            this.factory = factory;
            this.log = log;
        }

        [HttpGet]
        public ActionResult Index(Guid id)
        {
            var recorrido = servicio.ObtenerDatosDeInstanciaPorGuid(id);
            var control = servicio.ObtenerControlRecorrido(id, "ValidarContrato");
            string contrato = null;
            if (control != null)
            {
                var resultado = new Resultado();
                resultado.Errores.Add("", control.Comentario);
                ModelState.AgregarErrores(resultado);
                var carta = servicio.ObtenerCartaPortePorInstanceId(id);
                contrato = carta.AcuerdoMarco;
            }
            return View(new ValidarContratoModel { InstanciaWorkflow = id, WorkflowDefinicionId = recorrido.WorkflowDefinicionId, Contrato = contrato});
        }

        [HttpPost]
        public ActionResult Index(ValidarContratoModel contratoModel)
        {
            return View(contratoModel);
        }


        [DatosUsuario]
        public ActionResult Aceptar(ValidarContratoModel contratoModel, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var controlRecorrido = new ControlRecorridoDto
                        {
                            Actividad = Textos.ActValidarContrato,
                            ActividadXaml = "ValidarContrato",
                            WorkflowInstanceId = contratoModel.InstanciaWorkflow,
                            PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                            NombreUsuario = datosUsuario.NombreUsuario
                        };

                    var contratoService = factory.CrearServicio(contratoModel.WorkflowDefinicionId);
                    var resultado = contratoService.ValidarContrato(contratoModel.InstanciaWorkflow, contratoModel.Contrato, false, controlRecorrido);
                    if (resultado.HayErrores)
                    {
                        contratoModel.Mensaje = resultado.Errores.FirstOrDefault().Value;
                        return View("Confirmacion", contratoModel);
                    }
                    return Json("OK", JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    contratoModel.Mensaje = Textos.IngresarLote_ErrorSap;
                    return View("Confirmacion", contratoModel);
                }
            }
            TempData["Alerta"] = Textos.ValidarContrato_ContratoRequerido;
            TempData["TipoAlerta"] = TipoAlerta.Error;
            return Json("ERROR", JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult Confirmar(ValidarContratoModel contratoModel, DatosUsuario datosUsuario)
        {
            var controlRecorrido = new ControlRecorridoDto
        {
                Actividad = Textos.ActValidarContrato,
                ActividadXaml = "ValidarContrato",
                WorkflowInstanceId = contratoModel.InstanciaWorkflow,
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                NombreUsuario = datosUsuario.NombreUsuario
            };

            var contratoService = factory.CrearServicio(contratoModel.WorkflowDefinicionId);
            contratoService.ValidarContrato(contratoModel.InstanciaWorkflow, null, true, controlRecorrido);
            return Json("OK", JsonRequestBehavior.AllowGet);
        }
    }
}
