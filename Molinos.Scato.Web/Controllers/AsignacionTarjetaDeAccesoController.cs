using System;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
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
    [Autorizacion(PermisosScato.ActividadAsignacionTarjetaDeAcceso)]
    public class AsignacionTarjetaDeAccesoController : BaseController
    {
        private readonly IServicioActividadFactory<IAsignacionTarjetaDeAccesoService> factory;
        private ILogger log;
        private readonly IListaDeWorkflows workflows;

        public AsignacionTarjetaDeAccesoController(ILogger log, IServicioActividadFactory<IAsignacionTarjetaDeAccesoService> factory, IServicioRepositorio servicio, IListaDeWorkflows workflows)
            : base(servicio)
        {
            this.factory = factory;
            this.log = log;
            this.workflows = workflows;
        }

        [DatosUsuario]
        public ActionResult Index(Guid id, DatosUsuario datosUsuario)
        {
            var recorrido = servicio.ObtenerDatosDeInstanciaPorGuid(id);
            log.Debug("GET Index asignación tarjeta de acceso recorrido {0}", id);
            SetearVista(datosUsuario, id);
            return View(new AsignacionTarjetaDeAccessoModel { InstanceId = id, WorkflowDefinicionId = recorrido.WorkflowDefinicionId, Workflow = recorrido.WorkflowCodigo });
        }
        private void SetearVista(DatosUsuario datosUsuario, Guid id)
        {
            var puestosDeTrabajo = servicio.ListarPuestosDeTrabajoPorNombrePc(datosUsuario.NombrePc, datosUsuario.CentroId).Where(x => x.PidePatente).ToArray();
            ViewBag.PuestosDeTrabajo = puestosDeTrabajo.ToJson();
            ViewBag.CentroId = datosUsuario.CentroId;

            var foto = servicio.ObtenerFotoCPDeCartaDePortePorrecorrido(id);
            if (foto.Fotos.Any())
            {
                ViewBag.FotoMesaDigitalizacion1 = foto.Fotos.First().Foto;
            }
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Index(AsignacionTarjetaDeAccessoModel model, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    log.Debug("Iniciando asignación tarjeta de acceso recorrido {0}", model.InstanceId);
                    if (servicio.EsTarjetaBloqueada(model.Numero, datosUsuario.CentroId))
                    {
                        ModelState.AddModelError("Numero", Textos.AsignacionTarjetaDeAcceso_TarjetaBloqueada);
                        SetearVista(datosUsuario, model.InstanceId);
                        return View(model);
                    }
                    if (!servicio.EsTarjetaEnRangoValido(model.Numero, datosUsuario.CentroId))
                    {
                        ModelState.AddModelError("Numero", Textos.AsignacionTarjetaDeAcceso_TarjetaSinRango);
                        SetearVista(datosUsuario, model.InstanceId);
                        return View(model);
                    }
                    var instanciaWorkflow = servicio.ObtenerRecorridoInstanceIdPorTarjetaDeAcceso(model.Numero, datosUsuario.CentroId);
                    if (workflows.VerificarExistenciaDeWorkflowPorGuid(instanciaWorkflow))
                    {
                        ModelState.AddModelError("Numero", Textos.ImpresionTarjetaDeAcceso_EnUso);
                        SetearVista(datosUsuario, model.InstanceId);
                        return View(model);
                    }
                    var controlRecorrido = new ControlRecorridoDto
                        {
                            Actividad = Textos.ActAsignacionTarjetaDeAcceso,
                            ActividadXaml = "AsignacionTarjetaDeAcceso",
                            WorkflowInstanceId = model.InstanceId,
                            PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                            NombreUsuario = datosUsuario.NombreUsuario
                        };
                    log.Debug("Creando servicio de actividad asignación tarjeta de acceso recorrido {0}", model.InstanceId);
                    var accesoService = factory.CrearServicio(model.WorkflowDefinicionId);
                    log.Debug("Ejecutando servicio de actividad asignación tarjeta de acceso recorrido {0}", model.InstanceId);
                    var resultado = accesoService.AsignacionTarjetaDeAcceso(model.InstanceId, model.Numero, controlRecorrido);
                    if (resultado.HayErrores)
                    {
                        log.Error("Ejecutado con errores servicio de actividad asignación tarjeta de acceso recorrido {0}", model.InstanceId);
                        ModelState.AgregarErrores(resultado);
                    }
                    else
                    {
                        log.Debug("Ejecutado sin errores servicio de actividad asignación tarjeta de acceso recorrido {0}", model.InstanceId);
                        return RedirectToAction("Index", "ListaDeCamiones");
                    }
                }
                catch (Exception e)
                {
                    log.Fatal("Hubo una excepción",e);
                    ModelState.AddModelError("", Textos.AsignacionTarjetaDeAcceso_Error);
                }
            }
            SetearVista(datosUsuario, model.InstanceId);
            return View(model);
        }
    }
}
