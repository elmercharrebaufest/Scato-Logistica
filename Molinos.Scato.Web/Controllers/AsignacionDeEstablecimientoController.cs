using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadAsignacionDeEstablecimiento)]
    public class AsignacionDeEstablecimientoController : BaseController
    {
        private readonly IServicioActividadFactory<IAsignacionDeEstablecimientoService> factory;
        private ILogger log;
        private IListaDeWorkflows servicioWorkflows;
        private readonly IServicioComandos comandos;
        public AsignacionDeEstablecimientoController(ILogger log, IServicioActividadFactory<IAsignacionDeEstablecimientoService> factory, IServicioRepositorio servicio, IListaDeWorkflows servicioWorkflows, IServicioComandos comandos)
            : base(servicio)
        {
            this.factory = factory;
            this.log = log;
            this.servicioWorkflows = servicioWorkflows;
            this.comandos = comandos;
        }
        public ActionResult Index(Guid id)
        {
            ViewBag.HayErrores = false;
            return View(SetearVista(id));
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Index(AsignacionDeEstablecimientoDto model, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var controlRecorrido = new ControlRecorridoDto
                {
                    Actividad = Textos.ActAsignacionDeEstablecimiento,
                    ActividadXaml = "AsignacionDeEstablecimiento",
                    WorkflowInstanceId = model.InstanceId,
                    PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                    NombreUsuario = datosUsuario.NombreUsuario
                };

                int? establecimiento;
                if (model.EstablecimientoId > 0)
                {
                    establecimiento = model.EstablecimientoId;
                }
                else
                {
                    establecimiento = null;
                }
                var accesoService = factory.CrearServicio(model.WorkflowDefinicionId);
                if (model.TipoVehiculo == TipoVehiculo.Tren)
                {
                    var vagones = servicio.ObtenerGuidVagones(model.InstanceId);

                    foreach (var vagon in vagones)
                    {
                        if (servicioWorkflows.ObtenerWorkflowProximaAccion(vagon).ProximaAccion == "AsignacionDeEstablecimiento")
                        {
                            var resultado = accesoService.AsignacionDeEstablecimiento(vagon, establecimiento, controlRecorrido, false);

                            ViewBag.HayErrores = resultado.HayErrores;
                            if (resultado.HayErrores)
                            {
                                log.Error("AsignacionDeEstablecimiento Error : {0} , id = {1}", resultado.Errores.FirstOrDefault().Value, vagon);
                                TempData["Alerta"] = resultado.Errores.FirstOrDefault().Value;
                                TempData["TipoAlerta"] = TipoAlerta.Error;
                                SetearVista(model.InstanceId);
                                return View(model);
                            }
                        }
                    }
                    return RedirectToAction("Index", "ListaDeCamiones");
                }
                else
                {
                    var resultado = accesoService.AsignacionDeEstablecimiento(model.InstanceId, establecimiento, controlRecorrido, false);
                    ViewBag.HayErrores = resultado.HayErrores;
                    if (resultado.HayErrores)
                    {
                        log.Error("AsignacionDeEstablecimiento Error : {0} , id = {1}", resultado.Errores.FirstOrDefault().Value, model.InstanceId);
                        TempData["Alerta"] = resultado.Errores.FirstOrDefault().Value;
                        TempData["TipoAlerta"] = TipoAlerta.Error;
                    }
                    else
                    {
                        return RedirectToAction("Index", "ListaDeCamiones");
                    }
                }
            }
            SetearVista(model.InstanceId);
            return View(model);
        }


        private AsignacionDeEstablecimientoDto SetearVista(Guid instanceId)
        {
            var model = servicio.ObtenerAsignacionDeEstablecimiento(instanceId);
            var establecimientos = new List<SelectListItem>();
            establecimientos.AddRange(model.Establecimientos.Select(establecimiento => new SelectListItem { Selected = false, Text = establecimiento.NombreDeEstablecimiento, Value = establecimiento.Id.ToString(CultureInfo.InvariantCulture) }));
            establecimientos = establecimientos.OrderBy(x => x.Text).ToList();
            establecimientos.Insert(0, new SelectListItem { Selected = false, Text = Textos.Default_Establecimiento, Value = "-1" });
            ViewBag.Establecimientos = establecimientos;
            ViewBag.RecorridoId = model.RecorridoId;
            ViewBag.EstablecimientosEPAId = model.Establecimientos.Where(x => x.EsSojaEPA).Select(x => x.Id).ToList();

            var foto = servicio.ObtenerFotoCPDeCartaDePortePorrecorrido(instanceId);
            if (foto.Fotos.Any())
            {
                ViewBag.FotoMesaDigitalizacion1 = foto.Fotos.First().Foto;
            }
            model.Patente = foto.Patente;
            model.NumeroDocumentoIngreso = foto.NumeroDocumentoIngreso;
            return model;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult ProcedenciaCartaPorteIgualALocalidadEstablecimiento(string instanceId, string establecimientoId)
        {
            var instance = new Guid(instanceId);
            var cartaPorte = servicio.ObtenerCartaPortePorInstanceId(instance);
            var establecimiento = establecimientoId != "" ? servicio.ObtenerEstablecimiento(Convert.ToInt32(establecimientoId)) : null;
            var establecimientoEsIgualAProcedencia = false;
            var errorMessageEstablecimiento = "";

            if (establecimiento != null)
            {
                int.TryParse(establecimiento.CodigoDeEstablecimiento, out int codigoEstablecimiento);

                if (codigoEstablecimiento > Constantes.AsignacionDeEstablecimientoRangos.Desde && codigoEstablecimiento < Constantes.AsignacionDeEstablecimientoRangos.Hasta)
                {
                    return Json(new
                    {
                        establecimientoEsIgualAProcedencia = true,
                        errorMessage = string.Empty
                    }, JsonRequestBehavior.AllowGet);
                }
            }            

            if (establecimientoId != "-1" && !String.IsNullOrEmpty(establecimientoId) && cartaPorte != null && establecimiento != null)
            {
                establecimientoEsIgualAProcedencia = establecimiento.LocalidadId == cartaPorte.ProcedenciaId;
                errorMessageEstablecimiento = Textos.ProcedenciaDistintaAEstablecimiento.Replace("{0}", establecimiento.Localidad).Replace("{1}", establecimiento.LocalidadCodigoAfip);
            }
            else
            {
                establecimientoEsIgualAProcedencia = true;
            }

            return Json(new
            {
                establecimientoEsIgualAProcedencia = establecimientoEsIgualAProcedencia,
                errorMessage = errorMessageEstablecimiento
            }, JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult Rechazar(string codigoWf, int workflowDefinicionId, Guid instanceId, DatosUsuario datosUsuario)
        {
            log.Info("{0} - AsignacionDeEstablecimiento Rechazar", instanceId);
            ViewBag.Motivos = servicio.ListarMotivos().ToSelectList(x => x.Descripcion, x => x.Descripcion);
            ViewBag.Workflow = codigoWf;
            ViewBag.WorkflowDefinicionId = workflowDefinicionId;
            var controlRecorrido = new ControlRecorridoDto
            {
                WorkflowInstanceId = instanceId,
                NombreUsuario = datosUsuario.NombreUsuario,
                Actividad = Textos.AsignacionDeEstablecimiento_Titulo + "/" + Textos.Rechazar,
                ActividadXaml = "AsignacionDeEstablecimiento",
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId
            };

            return View("_TransportistaRechazado", controlRecorrido);
        }

        [DatosUsuario]
        public ActionResult TransportistaRechazado(string workflow, int workflowDefinicionId, Guid workflowInstanceId, ControlRecorridoDto controlRecorrido)
        {
            log.Info("{0} - AsignacionDeEstablecimiento rechazado", workflowDefinicionId);
            controlRecorrido.Decision = true;
            var service = factory.CrearServicio(workflowDefinicionId);
            var resultado = service.AsignacionDeEstablecimiento(workflowInstanceId, null, controlRecorrido, false);
            if (!resultado.HayErrores)
            {
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            ModelState.AgregarErrores(resultado);
            return RedirectToAction("Index", new { id = controlRecorrido.WorkflowInstanceId });
        }
        [DatosUsuario]
        public ActionResult Demorar(int workflowDefinicionId, Guid workflowInstanceId, DatosUsuario datosUsuario)
        {
            ViewBag.WorkflowDefinicionId = workflowDefinicionId;
            ViewBag.WorkflowInstanceId = workflowInstanceId;
            return View("_VehiculoDemorado");
        }
        [DatosUsuario]
        public ActionResult Pendiente(int workflowDefinicionId, Guid instanceId, string comentario, DatosUsuario datosUsuario)
        {
            log.Info("{0} - AsignacionDeEstablecimiento Pendiente", instanceId);

            var controlRecorrido = new ControlRecorridoDto
            {
                WorkflowInstanceId = instanceId,
                NombreUsuario = datosUsuario.NombreUsuario,
                Actividad = Textos.AsignacionDeEstablecimiento_Titulo + "/" + Textos.Demorar,
                ActividadXaml = "AsignacionDeEstablecimiento",
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId
            };

            var service = factory.CrearServicio(workflowDefinicionId);
            var resultado = service.AsignacionDeEstablecimiento(instanceId, null, controlRecorrido, true);
            if (!resultado.HayErrores)
            {
                comandos.Ejecutar(new ModificarRecorridoDemora { InstanceId = instanceId, DemoraEstablecimiento = true, Motivo = comentario });
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            ModelState.AgregarErrores(resultado);
            return RedirectToAction("Index", new { id = controlRecorrido.WorkflowInstanceId });
        }
    }
}
