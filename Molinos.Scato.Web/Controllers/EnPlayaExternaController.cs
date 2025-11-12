using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Comandos.ResultadoServicio;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Firmware;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;
using System;
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
            log.Debug($"Nombre Usuario: {controlRecorrido.NombreUsuario} CentroId: {datosUsuario.CentroId} PuestoId: {controlRecorrido.PuestoDeTrabajoId}");
            if (controlRecorrido.PuestoDeTrabajoId == 0) 
            {
                string mensajeError = "El puesto de trabajo no está asignado al usuario.";
                log.Error(mensajeError);
                throw new Exception(mensajeError);
            }
            var puestoDeTrabajo = servicio.ObtenerPuestoDeTrabajo(controlRecorrido.PuestoDeTrabajoId);
           // var puestoDeTrabajo = servicio.ObtenerPuestoDeTrabajoPorNombrePc(datosUsuario.NombrePc, datosUsuario.CentroId);
            log.Debug("Puesto de trabajo obtenido: {0}", puestoDeTrabajo?.Id ?? 0);
            var datosRecorrido = recorridoWorflow.ObtenerRecorrido(recorrido.TarjetaDeAcceso, puestoDeTrabajo.Id);
            log.Debug("Datos del recorrido obtenidos para la tarjeta: {0}", recorrido.TarjetaDeAcceso);
            var numeroDeTarjeta = recorrido.TarjetaDeAcceso;
            
            if (recorrido != null)
            {
                log.Info("Iniciando procesamiento de En Playa Externa para el recorrido: {0}", recorrido.Id);
                if (recorrido.Rechazado)
                {
                    var resultadoEjecutar = EjecutarWorkflow(puestoDeTrabajo, datosRecorrido);
                    if (resultadoEjecutar.HayErrores)
                        log.Error($"Error al ejecutar el workflow: {string.Join(", ", resultadoEjecutar.Errores.Select(e => $"{e.Key}: {e.Value}"))}");
                }
                else
                {
                    log.Info("Verificando pago de tasa municipal para el recorrido: {0}", recorrido.Id);
                    var resultado = comandos.Ejecutar(new VerificarPagoTasaMunicipal
                    {
                        CentroId = datosRecorrido.CentroId,
                        InstanceId = recorrido.InstanciaWorkflow,
                        Ctg = datosRecorrido.Ctg ?? string.Empty,
                        Patente = recorrido.Patente,
                        PatenteAcoplado = datosRecorrido.PatenteAcoplado,
                        MaterialId = datosRecorrido.MaterialId,
                        TipoVehiculo = recorrido.TipoVehiculo,
                        TipoOrigenDeValidacion = TipoOrigenDeValidacion.Recorrido,
                    });
                    log.Info("Resultado de la verificación de pago de tasa municipal para el recorrido: {0}", recorrido.Id);
                    if (resultado is ResultadoConsultarPagoTasaMunicipal resultadoPago && resultadoPago.HayErrores)
                    {
                        string mensajeError = $"Error al verificar el pago de tasa municipal: {string.Join(", ", resultadoPago.Errores.Select(e => $"{e.Key}: {e.Value}"))}";
                        log.Error(mensajeError);
                        throw new Exception(mensajeError);
                    }

                    var verificacionPago = resultado as ResultadoConsultarPagoTasaMunicipal;
                    log.Info($"Pago de tasa municipal procesado exitosamente para el puesto de trabajo: {puestoDeTrabajo.Id}");
                   
                    if (verificacionPago.EjecutaWorkFlow && verificacionPago.TipoAlerta == TipoAlerta.Exito)
                    {
                        var resultadoEjecutar = EjecutarWorkflow(puestoDeTrabajo, datosRecorrido);
                        if (resultadoEjecutar.HayErrores)
                        {
                            log.Error($"Error al ejecutar el workflow: {string.Join(", ", resultadoEjecutar.Errores.Select(e => $"{e.Key}: {e.Value}"))}");
                        }
                    } 
                }
            }
            return RedirectToAction("Index", "ListaDeCamiones");
        }

        protected Resultado EjecutarWorkflow(PuestoDeTrabajoDto puestoDeTrabajo, DatosRecorridoDto recorrido)
        {
            Resultado resultado = new Resultado();
            try
            {
                var proximaAccion = recorridoWorflow.ObtenerWorkflowProximaAccionConRecorrido(recorrido.TarjetaDeAcceso, puestoDeTrabajo.Id, recorrido);

                if (recorrido != null && proximaAccion != null)
                    EjecutarDispositivos(puestoDeTrabajo, recorrido);

                var workflowId = proximaAccion.WorkflowDefinicionId;
                var instanceId = proximaAccion.InstanceId;
                var proximaActividad = proximaAccion.ProximaActividad;
                var puestoDeTrabajoId = proximaAccion.PuestoDeTrabajoId;

                log.Info("Ejecutando workflow. Tarjeta: {0} Puesto: {1} WorkflowId: {2} InstanceId: {3} ProximaActividad: {4} PuestoDeTrabajoId: {5}",
                    recorrido.TarjetaDeAcceso, puestoDeTrabajo.Id, workflowId, instanceId, proximaActividad, puestoDeTrabajoId);

                var serviciowf = factory.CrearServicio(workflowId);
                var resultadoActividad = serviciowf.Ejecutar(instanceId, new ControlRecorridoDto
                {
                    WorkflowInstanceId = instanceId,
                    NombreUsuario = String.Empty,
                    Actividad = Textos.ResourceManager.GetString("Act" + proximaActividad) ?? proximaActividad,
                    ActividadXaml = proximaActividad,
                    Decision = true,
                    PuestoDeTrabajoId = puestoDeTrabajoId
                });

                if (resultadoActividad != null && resultadoActividad.HayErrores)
                {
                    string mensajeError = $"Error al ejecutar la actividad {proximaActividad} " +
                                          $"en el workflow: {workflowId}. " +
                                          $"Errores: {string.Join(", ", resultadoActividad.Errores.Select(e => $"{e.Key}: {e.Value}"))}";

                    resultado.Errores.Add(nameof(Resultado), mensajeError);
                }
                log.Info("Workflow ejecutado exitosamente. Tarjeta: {0} Puesto: {1} WorkflowId: {2} InstanceId: {3} ProximaActividad: {4} PuestoDeTrabajoId: {5}",
                        recorrido.TarjetaDeAcceso, puestoDeTrabajo.Id, workflowId, instanceId, proximaActividad, puestoDeTrabajoId);

            }
            catch (Exception e)
            {
                resultado.Errores.Add(nameof(Resultado), $"Error al ejecutar el workflow: {e.Message}");
            }

            return resultado;
        }

        protected void EjecutarDispositivos(PuestoDeTrabajoDto puestoDeTrabajo, DatosRecorridoDto recorrido)
        {
            try
            {
                log.Debug($"Validando puesto : {puestoDeTrabajo.Id}");
                var dispositivosEntrada = puestoDeTrabajo.Entrada.Split(',');
                if( dispositivosEntrada.Length > 0 || !string.IsNullOrEmpty(dispositivosEntrada[0]))
                {
                    foreach (var dispositivo in dispositivosEntrada)
                    {
                        log.Debug("Ejecutando Barrera de entrada {1} para el puesto: {0}", puestoDeTrabajo.Id, dispositivo);
                        var resultado = servicioOrquestador.Ejecutar(new EjecutarAperturaBarrera
                        {
                            CodigoDispositivo = dispositivo
                        });

                        if (resultado.Mensaje.Codigo != 0)
                        {
                            log.Error("Fallo la Apertura del dispositivo: {0}", resultado.Mensaje.Descripcion);
                        }
                    }
                }

                var dispositivosSalida = puestoDeTrabajo.CierreEntrada.Split(',');
                foreach (var dispositivo in dispositivosSalida)
                {
                    log.Debug("Ejecutando cierre de Barrera {1} para el puesto: {0}", puestoDeTrabajo.Id, dispositivo);
                    var resultado = servicioOrquestador.Ejecutar(new EjecutarCierreBarrera
                    {
                        CodigoDispositivo = dispositivo
                    });

                    if (resultado.Mensaje.Codigo != 0)
                    {
                        log.Error("Fallo el Cierre del dispositivo: {0}", resultado.Mensaje.Descripcion);
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e, $"Fallo la ejecucion de los dispositivos relacionados al workflow:  {recorrido.InstanciaWorkflow}");
            }
        }
    }
}
