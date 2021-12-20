using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Web.ServicioHub;
using Ninject.Extensions.Logging;
using System;
using System.Linq;

namespace Molinos.Scato.Web.Firmware
{
    public abstract class FirmwareControlDePeso : FirmwareBase
    {
        private readonly IServicioActividadFactory<IControlDePesoEsperadoService> factoryControlDePeso;
        public bool repesar;
        public FirmwareControlDePeso(ILogger log, 
            IServicioRepositorio servicioRepositorio, 
            IListaDeWorkflows workflows,
            IServicioComandos comandos,
            IServicioOrquestador servicioOrquestador,
            IServicioActividadFactory<IEjecutarService> factory,
            IServicioActividadFactory<IControlDePesoEsperadoService> factoryControlDePeso,

            HubClientFactory hubClientFactory) : base(
                log, servicioRepositorio, workflows, comandos, servicioOrquestador, factory, hubClientFactory)
        {
            this.factoryControlDePeso = factoryControlDePeso;
        }

        public override void ProcesarEvento(LecturaPuestoDeTrabajoDto lecturaPuestoDeTrabajo)
        {
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
                EjecutarControlDePeso(lecturaPuestoDeTrabajo, recorrido, repesar);
            }
            NotificarLecturaPorSignalR(lecturaPuestoDeTrabajo);
        }

        protected void EjecutarControlDePeso(LecturaPuestoDeTrabajoDto lecturaPuestoDeTrabajo, DatosRecorridoDto recorrido, bool repesar)
        {
            try
            {
                log.Debug("Validando puesto sin patente. Tarjeta: {0} Puesto: {1}",
                    lecturaPuestoDeTrabajo.NumeroDeTarjeta, lecturaPuestoDeTrabajo.PuestoDeTrabajoId);

                var resultado = ValidarProximaActividad(lecturaPuestoDeTrabajo, recorrido);

                if ((!recorrido.SinRecorrido && !string.IsNullOrEmpty(resultado.NumeroDocumentoIngreso) && !string.IsNullOrEmpty(resultado.Patente)) || recorrido.SinRecorrido)
                {
                    EjecutarDispositivos(lecturaPuestoDeTrabajo, recorrido);
                }

                if (resultado.Valida)
                {
                    var serviciowf = factoryControlDePeso.CrearServicio(resultado.WorkflowDefinicionId);
                    var resultadoActividad = serviciowf.ControlDePesoEsperado(new ControlRecorridoDto
                    {
                        WorkflowInstanceId = resultado.InstanceId,
                        NombreUsuario = String.Empty,
                        Actividad = Textos.ResourceManager.GetString("Act" + resultado.ProximaActividad) ?? resultado.ProximaActividad,
                        ActividadXaml = resultado.ProximaActividad,
                        Decision = repesar,
                        PuestoDeTrabajoId = resultado.PuestoDeTrabajoId
                    }, resultado.InstanceId);

                    if (resultadoActividad != null && resultadoActividad.HayErrores)
                    {
                        log.Error("La ejecución de la actividad {0} terminó con errores: {1}",
                                resultado.ProximaActividad, resultadoActividad.Errores.First().Value);
                    }
                }
                else
                {
                    log.Warn("No se puede ejecutar el WF relacionado con la tarjeta {0}. Mensaje: {1}",
                            lecturaPuestoDeTrabajo.NumeroDeTarjeta, resultado.MensajeError);
                    lecturaPuestoDeTrabajo.TarjetaValida = false;
                    lecturaPuestoDeTrabajo.MensajeError = resultado.MensajeError;
                }
            }
            catch (Exception e)
            {
                log.Error(e, "Fallo la ejecucion del workflow relacionado con la tarjeta: {0}", lecturaPuestoDeTrabajo.NumeroDeTarjeta);
            }
        }
    }
}


