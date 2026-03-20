using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Comandos.ResultadoServicio;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Firmware;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadEnPlayaExterna)]
    public class EnPlayaExternaController : BaseController
    {
        private readonly IServicioActividadFactory<IEjecutarService> factory;
        private readonly ILogger log;
        private readonly IRecorridoWorkflow recorridoWorflow;
        private readonly IServicioOrquestador servicioOrquestador;
        private readonly IServicioComandos comandos;
        public EnPlayaExternaController(ILogger log, 
                                        IServicioActividadFactory<IEjecutarService> factory,
                                        IServicioRepositorio servicio,
                                        IRecorridoWorkflow recorridoWorkflow,
                                        IServicioOrquestador servicioOrquestador,
                                        IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.factory = factory;
            this.log = log;
            this.recorridoWorflow = recorridoWorkflow;
            this.servicioOrquestador = servicioOrquestador;
            this.comandos = servicioComandos;
        }
        public ActionResult Index(Guid id)
        {
            var recorrido = servicio.ObtenerRecorridoPorGuid(id);

            ViewBag.TipoDocumentoIngreso = recorrido.TipoDocumentoIngreso;
            ViewBag.NumeroDocumentoIngreso = recorrido.NumeroDocumentoIngreso;
            ViewBag.Patente = recorrido.Patente;
            ViewBag.Workflow = recorrido.Workflow.Codigo;
            ViewBag.WorkflowDefinicionId = recorrido.WorkflowDefinicionId;
            var observacion = new ObservacionRDto { WorkflowInstanceId = id };

            return View(observacion);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Index(ObservacionRDto observacion, string workflow, int workflowDefinicionId, DatosUsuario datosUsuario)
        {
            log.Info("Iniciando proceso de En Playa Externa. WorkflowInstanceId: {0}, Workflow: {1}, WorkflowDefinicionId: {2}, Usuario: {3}, CentroId: {4}, PuestoDeTrabajoId: {5}",
                observacion.WorkflowInstanceId, workflow, workflowDefinicionId, datosUsuario.NombreUsuario, datosUsuario.CentroId, datosUsuario.PuestoDeTrabajoId);

            try
            {
                var controlRecorrido = new ControlRecorridoDto
                {
                    Actividad = Textos.ActEnPlayaExterna,
                    ActividadXaml = "EnPlayaExterna",
                    WorkflowInstanceId = observacion.WorkflowInstanceId,
                    PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                    NombreUsuario = datosUsuario.NombreUsuario,
                    Decision = true,
                    Comentario = observacion.Observaciones
                };

                var recorrido = servicio.ObtenerRecorridoPorGuid(observacion.WorkflowInstanceId);
                log.Debug("Procesando En Playa Externa para el recorrido: {0}", recorrido.Id);
                var recorridoActivo = servicio.ObtenerDatosRecorridoActivo(null, new List<string> { recorrido.TarjetaDeAcceso });
                if (recorridoActivo != null && !recorridoActivo.Rechazado)
                {
                    var resultadoPagoTasaMunicipal = comandos.Ejecutar(new VerificarPagoTasaMunicipal
                    {
                        CentroId = recorridoActivo.CentroId,
                        InstanceId = recorridoActivo.InstanciaWorkflow,
                        Ctg = recorridoActivo.NumeroDocumentoIngreso ?? string.Empty,
                        Patente = recorridoActivo.Patente,
                        PatenteAcoplado = recorridoActivo.PatenteAcoplado,
                        MaterialId = recorridoActivo.MaterialId,
                        TipoVehiculo = recorridoActivo.TipoVehiculo,
                        TipoOrigenDeValidacion = TipoOrigenDeValidacion.Recorrido,
                    }) as ResultadoConsultarPagoTasaMunicipal;
                    if (resultadoPagoTasaMunicipal != null && resultadoPagoTasaMunicipal.HayErrores)
                    {
                        string mensajeError = $"Error al verificar el pago de tasa municipal: {string.Join(", ", resultadoPagoTasaMunicipal.Errores.Select(e => $"{e.Key}: {e.Value}"))}";
                        log.Error(mensajeError);
                        return RedirectToAction("Index", "ListaDeCamiones");
                    }

                    if (resultadoPagoTasaMunicipal != null && resultadoPagoTasaMunicipal.EjecutaWorkFlow && resultadoPagoTasaMunicipal.TipoAlerta == TipoAlerta.Exito)
                        EjecutarWorkflow(recorridoActivo, controlRecorrido);
                
                    InformarPagoTasaMunicipal(recorrido.InstanciaWorkflow);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, $"Error al procesar En Playa Externa para el WorkflowInstanceId: {observacion.WorkflowInstanceId}");
            }

            return RedirectToAction("Index", "ListaDeCamiones");
        }

        private void EjecutarWorkflow(DatosRecorridoDto datosRecorrido, ControlRecorridoDto controlRecorrido)
        {
                var serviciowf = factory.CrearServicio(datosRecorrido.WorkflowDefinicionId);
                var resultadoActividad = serviciowf.Ejecutar(controlRecorrido.WorkflowInstanceId, controlRecorrido);
                if (resultadoActividad != null && resultadoActividad.HayErrores)
                {
                    string mensajeError = $"Error al ejecutar workflow Playa Externa: {string.Join(", ", resultadoActividad.Errores.Select(e => $"{e.Key}: {e.Value}"))}";
                    log.Error(mensajeError);
            }
        }


        private void InformarPagoTasaMunicipal(Guid instanciaWorkflow)
        {
            var pagos = servicio.ObtenerPagosDigitalesPorInstanceId(instanciaWorkflow);
            if (pagos.Any())
            {
                log.Debug($"El recorrido {instanciaWorkflow} tiene pagos digitales asociados.");
                foreach (var pago in pagos)
                {
                    var resultado = comandos.Ejecutar(new MOAPayInformarPagoComoConsumido
                    {
                        Id = pago.IdMOAPay,
                        Disponible = "N",
                        IdIntance = instanciaWorkflow
                    });
                    if(resultado.HayErrores)
                    {
                        log.Error($"Error al informar el pago MOAPay como consumido para el pago {pago.IdMOAPay}: {string.Join(", ", resultado.Errores.Select(e => $"{e.Key}: {e.Value}"))}");
                    }
                    else
                    {
                        log.Debug($"Pago MOAPay informado como consumido exitosamente para el pago {pago.IdMOAPay}");
                    }
                }
            }
        }
    }
}
