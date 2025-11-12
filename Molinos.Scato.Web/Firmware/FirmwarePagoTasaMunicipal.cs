using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Comandos.ResultadoServicio;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Impl;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Web.ServicioHub;
using Ninject.Extensions.Logging;
using System;
using System.Linq;

namespace Molinos.Scato.Web.Firmware
{
    public class FirmwarePagoTasaMunicipal : FirmwareBase
    {
        public FirmwarePagoTasaMunicipal(ILogger log, IServicioRepositorio servicioRepositorio, IListaDeWorkflows workflows, IServicioComandos comandos, IServicioOrquestador servicioOrquestador, IServicioActividadFactory<IEjecutarService> factory, HubClientFactory hubClientFactory, IRecorridoWorkflow recorridoWorkflow) :
            base(log, servicioRepositorio, workflows, comandos, servicioOrquestador, factory, hubClientFactory, recorridoWorkflow)
        {
        }

        public override void ProcesarEvento(LecturaPuestoDeTrabajoDto lecturaPuestoDeTrabajo)
        {
            log.Info($"Procesando evento de lectura de puesto de trabajo: {lecturaPuestoDeTrabajo.PuestoDeTrabajoId} con lectura: {lecturaPuestoDeTrabajo.NumeroDeTarjeta}");
            if (!lecturaPuestoDeTrabajo.TarjetaValida)
            {
                NotificarMensajeErrorPorSignalR(lecturaPuestoDeTrabajo);
                log.Info("Fin - La tarjeta: {0} no es valida: {1}",
                         lecturaPuestoDeTrabajo.NumeroDeTarjeta,
                         lecturaPuestoDeTrabajo.MensajeError);
            }
            else
            {
                var recorrido = ObtenerRecorrido(lecturaPuestoDeTrabajo);
                if (recorrido != null)
                {
                    log.Info($"Recorrido encontrado para el puesto de trabajo: {lecturaPuestoDeTrabajo.PuestoDeTrabajoId}, InstanceId: {recorrido.InstanciaWorkflow}");
                    if (recorrido.Rechazado)
                    {
                        var configuracionIngreso = servicio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.PagoTasaMunicipal, Constantes.ConfiguracionGeneral.PagoTasaMunicipal.PermitirBloqueoDeIngreso);
                        bool.TryParse(configuracionIngreso?.Valor, out bool tieneBloqueoDeIngreso);
                        if(!tieneBloqueoDeIngreso)
                        {
                            log.Info($"El recorrido {recorrido.InstanciaWorkflow} ya se encuentra rechazado y no tiene el bloqueo de ingreso.");
                            var pagos = servicio.ObtenerPagosDigitalesPorInstanceId(recorrido.InstanciaWorkflow);
                            if (pagos.Count() > 0)
                            {
                                log.Info($"El recorrido {recorrido.InstanciaWorkflow} tiene pagos digitales asociados.");
                                foreach (var pago in pagos)
                                {
                                    comandos.Ejecutar(new ModificarComoDevolucionPagosTasaMunicipal
                                    {
                                        PagoId = pago.Id,
                                        InstanceId = recorrido.InstanciaWorkflow,
                                    });
                                }
                            }
                        }

                        lecturaPuestoDeTrabajo.MensajeError = "CAMION RECHAZADO";
                        lecturaPuestoDeTrabajo.TipoAlerta = TipoAlerta.Error;
                        lecturaPuestoDeTrabajo.Rechazado = true;
                        var resultadoEjecutar = EjecutarWorkflow(lecturaPuestoDeTrabajo, recorrido);
                        if (resultadoEjecutar.HayErrores)
                        {
                            log.Error($"Error al ejecutar el workflow: {string.Join(", ", resultadoEjecutar.Errores.Select(e => $"{e.Key}: {e.Value}"))}");
                            lecturaPuestoDeTrabajo.MensajeError = string.Join(", ", resultadoEjecutar.Errores.Select(e => $"{e.Key}: {e.Value}"));
                            NotificarMensajeErrorPorSignalR(lecturaPuestoDeTrabajo);
                            return;
                        }
                        NotificarMensajeErrorPorSignalR(lecturaPuestoDeTrabajo);
                    }
                    else
                    {
                        var resultado = comandos.Ejecutar(new VerificarPagoTasaMunicipal
                        {
                            CentroId = recorrido.CentroId,
                            InstanceId = recorrido.InstanciaWorkflow,
                            Ctg = recorrido.Ctg ?? string.Empty,
                            Patente = recorrido.Patente,
                            PatenteAcoplado = recorrido.PatenteAcoplado,
                            MaterialId = recorrido.MaterialId,
                            TipoVehiculo = recorrido.TipoVehiculo,
                            TipoOrigenDeValidacion = TipoOrigenDeValidacion.Recorrido,
                        });

                        if (resultado is ResultadoConsultarPagoTasaMunicipal resultadoPago && resultadoPago.HayErrores)
                        {
                            string mensajeError = $"Error al verificar el pago de tasa municipal: {string.Join(", ", resultadoPago.Errores.Select(e => $"{e.Key}: {e.Value}"))}";
                            log.Error(mensajeError);
                            throw new Exception(mensajeError);
                        }

                        var verificacionPago = resultado as ResultadoConsultarPagoTasaMunicipal;
                        log.Info($"Pago de tasa municipal procesado exitosamente para el puesto de trabajo: {lecturaPuestoDeTrabajo.PuestoDeTrabajoId}");
                        lecturaPuestoDeTrabajo.TipoAlerta = verificacionPago.TipoAlerta;
                        lecturaPuestoDeTrabajo.MensajeAlerta =verificacionPago.TipoAlerta == TipoAlerta.Error ? "TASA ADEUDADA" : verificacionPago.MensajeAlerta;

                        if (verificacionPago.EjecutaWorkFlow && verificacionPago.TipoAlerta == TipoAlerta.Exito)
                        {
                            var resultadoEjecutar = EjecutarWorkflow(lecturaPuestoDeTrabajo, recorrido);
                            if (resultadoEjecutar.HayErrores)
                            {
                                log.Error($"Error al ejecutar el workflow: {string.Join(", ", resultadoEjecutar.Errores.Select(e => $"{e.Key}: {e.Value}"))}");
                                lecturaPuestoDeTrabajo.MensajeError = string.Join(", ", resultadoEjecutar.Errores.Select(e => $"{e.Key}: {e.Value}"));
                                NotificarMensajeErrorPorSignalR(lecturaPuestoDeTrabajo);
                                return;
                            }
                        }
                        NotificarLecturaPorSignalR(lecturaPuestoDeTrabajo);
                    }
                }
            }
        }

        protected override void InvokeNotificarLectura(LecturaPuestoDeTrabajoDto lecturaPuestoDeTrabajo)
        {
            hubClientLectura.Invoke("NotificarLecturaPagoTasaMunicipal", lecturaPuestoDeTrabajo);
        }
    }
}