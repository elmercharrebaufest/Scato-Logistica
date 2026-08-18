using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Comandos.ResultadoServicio;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Web.ServicioHub.Client;
using Ninject.Extensions.Logging;
using System;
using System.Linq;
namespace Molinos.Scato.Web.Firmware
{
    public class FirmwarePagoTasaMunicipal : FirmwareBase
    {
        public FirmwarePagoTasaMunicipal(
            ILogger log, 
            IServicioRepositorio servicioRepositorio, 
            IListaDeWorkflows workflows, 
            IServicioComandos comandos, 
            IServicioOrquestador servicioOrquestador, 
            IServicioActividadFactory<IEjecutarService> factory, 
            HubClients hubClients, 
            IRecorridoWorkflow recorridoWorkflow) 
            : base(
                  log, 
                  servicioRepositorio, 
                  workflows, 
                  comandos, 
                  servicioOrquestador, 
                  factory, 
                  hubClients, 
                  recorridoWorkflow)
        {
        }

        public override string ProcesarEvento(LecturaPuestoDeTrabajoDto lecturaPuestoDeTrabajo)
        {
            log.Info($"Procesando evento de lectura de puesto de trabajo: {lecturaPuestoDeTrabajo.PuestoDeTrabajoId} con lectura: {lecturaPuestoDeTrabajo.NumeroDeTarjeta}");
            if (!lecturaPuestoDeTrabajo.TarjetaValida)
            {
                NotificarMensajeErrorPorSignalR(lecturaPuestoDeTrabajo);
                return lecturaPuestoDeTrabajo.MensajeError;
            }

            var recorrido = ObtenerRecorrido(lecturaPuestoDeTrabajo);
            log.Info($"Recorrido encontrado para el puesto de trabajo: {lecturaPuestoDeTrabajo.PuestoDeTrabajoId}, InstanceId: {recorrido.InstanciaWorkflow}");
            if (recorrido.Rechazado)
            {
                var configuracionIngreso = servicio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.PagoTasaMunicipal, Constantes.ConfiguracionGeneral.PagoTasaMunicipal.PermitirBloqueoDeIngreso);
                bool.TryParse(configuracionIngreso?.Valor, out bool tieneBloqueoDeIngreso);
                var pagos = servicio.ObtenerPagosDigitalesPorInstanceId(recorrido.InstanciaWorkflow);
                if (!tieneBloqueoDeIngreso)
                {
                    log.Info($"El recorrido {recorrido.InstanciaWorkflow} ya se encuentra rechazado y no tiene el bloqueo de ingreso.");                   
                    if (pagos.Count() > 0)
                    {
                        log.Info($"El recorrido {recorrido.InstanciaWorkflow} tiene pagos digitales asociados.");
                        foreach (var pago in pagos)
                        {
                            comandos.Ejecutar(new LiberarPagoTasaMunicipal
                            {
                                PagoId = pago.Id,
                                InstanceId = recorrido.InstanciaWorkflow,
                            });
                        }
                    }
                }
                else if (pagos.Any())
                {
                    foreach (var pago in pagos)
                    {
                        comandos.Ejecutar(new MOAPayInformarPagoComoConsumido
                        {
                            Id = pago.IdMOAPay,
                            Disponible = "N",
                            IdIntance = recorrido.InstanciaWorkflow
                        });
                    }
                }
                lecturaPuestoDeTrabajo.MensajeError = "CAMION RECHAZADO";
                lecturaPuestoDeTrabajo.TipoAlerta = TipoAlerta.Error;
                lecturaPuestoDeTrabajo.Rechazado = true;
                var resultadoEjecutar = EjecutarWorkflow(lecturaPuestoDeTrabajo, recorrido);
                if (resultadoEjecutar.HayErrores)
                    lecturaPuestoDeTrabajo.MensajeError = string.Join(", ", resultadoEjecutar.Errores.Select(e => $"{e.Key}: {e.Value}"));

                NotificarMensajeErrorPorSignalR(lecturaPuestoDeTrabajo);
                return resultadoEjecutar.HayErrores ? lecturaPuestoDeTrabajo.MensajeError : Constantes.ResultadoProcesoIdentificacionVehicular.EjecucionExitosa;
            }
            else
            {
                var resultadoPago = comandos.Ejecutar(new VerificarPagoTasaMunicipal
                {
                    CentroId = recorrido.CentroId,
                    InstanceId = recorrido.InstanciaWorkflow,
                    Ctg = recorrido.Ctg ?? string.Empty,
                    Patente = recorrido.Patente,
                    PatenteAcoplado = recorrido.PatenteAcoplado,
                    MaterialId = recorrido.MaterialId,
                    TipoVehiculo = recorrido.TipoVehiculo,
                    TipoOrigenDeValidacion = TipoOrigenDeValidacion.Recorrido,
                }) as ResultadoConsultarPagoTasaMunicipal;

                lecturaPuestoDeTrabajo.TipoAlerta = resultadoPago.TipoAlerta;
                lecturaPuestoDeTrabajo.MensajeAlerta = resultadoPago.MensajeAlerta;

                if (resultadoPago.HayErrores)
                {
                    string mensajeError = $"Error al verificar el pago de tasa municipal: {string.Join(", ", resultadoPago.Errores.Select(e => $"{e.Key}: {e.Value}"))}";
                    log.Error(mensajeError);
                    lecturaPuestoDeTrabajo.MensajeError = "Error al verificar el pago de tasa municipal";
                    NotificarMensajeErrorPorSignalR(lecturaPuestoDeTrabajo);
                    return lecturaPuestoDeTrabajo.MensajeError;
                }

                log.Info($"Pago de tasa municipal procesado exitosamente para el puesto de trabajo: {lecturaPuestoDeTrabajo.PuestoDeTrabajoId}");

                       
                comandos.Ejecutar(new CrearModificarRecorridoTasaMunicipal
                    {
                        Id = recorrido.Id,
                        MotivoExcepcion = resultadoPago.MotivoExcepcion,
                        TieneExcepcion = resultadoPago.TieneExcepcion
                    });

                if (resultadoPago.EjecutaWorkFlow && resultadoPago.TipoAlerta == TipoAlerta.Exito)
                {
                    var resultadoEjecutar = EjecutarWorkflow(lecturaPuestoDeTrabajo, recorrido);
                    if (resultadoEjecutar.HayErrores)
                    {
                        log.Error($"Error al ejecutar el workflow: {string.Join(", ", resultadoEjecutar.Errores.Select(e => $"{e.Key}: {e.Value}"))}");
                        lecturaPuestoDeTrabajo.MensajeError = string.Join(", ", resultadoEjecutar.Errores.Select(e => $"{e.Key}: {e.Value}"));
                        NotificarMensajeErrorPorSignalR(lecturaPuestoDeTrabajo);
                        return lecturaPuestoDeTrabajo.MensajeError;
                    }
                }
                NotificarLecturaPorSignalR(lecturaPuestoDeTrabajo);
                return Constantes.ResultadoProcesoIdentificacionVehicular.EjecucionExitosa;
            }
        }


        protected override void InvokeNotificarLectura(LecturaPuestoDeTrabajoDto lecturaPuestoDeTrabajo)
        {
            hubClientLectura.Invoke("NotificarLecturaPagoTasaMunicipal", lecturaPuestoDeTrabajo);
        }

        private Resultado EjecutarWorkflow(LecturaPuestoDeTrabajoDto lecturaPuestoDeTrabajo, DatosRecorridoDto recorrido)
        {
            Resultado resultado = new Resultado();
            try
            {
                log.Debug("EjecutarWorkflow. Tarjeta: {0} Puesto: {1}", recorrido.TarjetaDeAcceso, lecturaPuestoDeTrabajo.PuestoDeTrabajoId);
                var proximaAccion = recorridoWorkflow.ObtenerWorkflowProximaAccionConRecorrido(recorrido.TarjetaDeAcceso, lecturaPuestoDeTrabajo.PuestoDeTrabajoId, recorrido);
                var workflowId = proximaAccion.WorkflowDefinicionId;
                var instanceId = proximaAccion.InstanceId;
                var proximaActividad = proximaAccion.ProximaActividad;
                var puestoDeTrabajoId = proximaAccion.PuestoDeTrabajoId;

                log.Info("Ejecutando workflow. Tarjeta: {0} Puesto: {1} WorkflowId: {2} InstanceId: {3} ProximaActividad: {4} PuestoDeTrabajoId: {5}",
                    recorrido.TarjetaDeAcceso, lecturaPuestoDeTrabajo.PuestoDeTrabajoId, workflowId, instanceId, proximaActividad, puestoDeTrabajoId);

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
                    lecturaPuestoDeTrabajo.TarjetaValida = false;
                    lecturaPuestoDeTrabajo.MensajeError = proximaAccion.MensajeError;
                }
                
                log.Info("Workflow ejecutado exitosamente. Tarjeta: {0} Puesto: {1} WorkflowId: {2} InstanceId: {3} ProximaActividad: {4} PuestoDeTrabajoId: {5}",
                        lecturaPuestoDeTrabajo.NumeroDeTarjeta, lecturaPuestoDeTrabajo.PuestoDeTrabajoId, workflowId, instanceId, proximaActividad, puestoDeTrabajoId);

            }
            catch (Exception e)
            {
                resultado.Errores.Add(nameof(Resultado), $"Error al ejecutar el workflow: {e.Message}");
            }

            return resultado;
        }
    }
}