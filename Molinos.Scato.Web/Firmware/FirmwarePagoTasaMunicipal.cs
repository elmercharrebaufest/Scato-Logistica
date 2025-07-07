using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Servicios;
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
            base(log, servicioRepositorio, workflows, comandos, servicioOrquestador, factory, hubClientFactory , recorridoWorkflow)
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
                    var resultado= comandos.Ejecutar(new VerificarPagoTasaMunicipal
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
                    lecturaPuestoDeTrabajo.MensajeAlerta = verificacionPago.MensajeAlerta;

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
                log.Warn($"El puesto de trabajo {lecturaPuestoDeTrabajo.PuestoDeTrabajoId} no está configurado para automatización o la tarjeta no es válida.");
            }
        }
     
        protected override void InvokeNotificarLectura(LecturaPuestoDeTrabajoDto lecturaPuestoDeTrabajo)
        {
            hubClientLectura.Invoke("NotificarLecturaPagoTasaMunicipal", lecturaPuestoDeTrabajo);
        }
    }
}