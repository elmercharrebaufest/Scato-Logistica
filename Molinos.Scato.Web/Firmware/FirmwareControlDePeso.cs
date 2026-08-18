using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Web.ServicioHub.Client;
using Ninject.Extensions.Logging;
using System;
using System.Linq;

namespace Molinos.Scato.Web.Firmware
{
    public abstract class FirmwareControlDePeso : FirmwareBase
    {
        private readonly IServicioActividadFactory<IControlDePesoEsperadoService> factoryControlDePeso;
        public bool repesar;

        public FirmwareControlDePeso(
            ILogger log, 
            IServicioRepositorio servicioRepositorio, 
            IListaDeWorkflows workflows,
            IServicioComandos comandos,
            IServicioOrquestador servicioOrquestador,
            IServicioActividadFactory<IEjecutarService> factory,
            IServicioActividadFactory<IControlDePesoEsperadoService> factoryControlDePeso,
            IRecorridoWorkflow recorridoWorkflow,
            HubClients hubClients) 
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
            this.factoryControlDePeso = factoryControlDePeso;
        }

        public override string ProcesarEvento(LecturaPuestoDeTrabajoDto lecturaPuestoDeTrabajo)
        {
            var resultadoEjecucion = string.Empty;
            if (!lecturaPuestoDeTrabajo.TarjetaValida)
            {
                NotificarMensajeErrorPorSignalR(lecturaPuestoDeTrabajo);
                resultadoEjecucion = lecturaPuestoDeTrabajo.MensajeError;
            }
            else
            {
                var recorrido = ObtenerRecorrido(lecturaPuestoDeTrabajo);
                resultadoEjecucion = EjecutarControlDePeso(lecturaPuestoDeTrabajo, recorrido, repesar);
            }

            NotificarLecturaPorSignalR(lecturaPuestoDeTrabajo);
            return resultadoEjecucion;
        }

        protected string EjecutarControlDePeso(LecturaPuestoDeTrabajoDto lecturaPuestoDeTrabajo, DatosRecorridoDto recorrido, bool repesar)
        {
            try
            {
                log.Debug("Validando puesto sin patente. Tarjeta: {0} Puesto: {1}", lecturaPuestoDeTrabajo.NumeroDeTarjeta, lecturaPuestoDeTrabajo.PuestoDeTrabajoId);

                var resultado = ValidarProximaActividad(lecturaPuestoDeTrabajo, recorrido);
                if ((!recorrido.SinRecorrido && !string.IsNullOrEmpty(resultado.NumeroDocumentoIngreso) && !string.IsNullOrEmpty(resultado.Patente)) || recorrido.SinRecorrido)
                    EjecutarDispositivos(lecturaPuestoDeTrabajo, recorrido);

                if (!resultado.Valida)
                {
                    log.Warn("No se puede ejecutar el WF relacionado con la tarjeta {0}. Mensaje: {1}", lecturaPuestoDeTrabajo.NumeroDeTarjeta, resultado.MensajeError);
                    lecturaPuestoDeTrabajo.TarjetaValida = false;
                    lecturaPuestoDeTrabajo.MensajeError = resultado.MensajeError;
                    return lecturaPuestoDeTrabajo.MensajeError;
                }
                
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

                var huboErrorEnWorkflow = resultadoActividad != null && resultadoActividad.HayErrores;
                if (huboErrorEnWorkflow) log.Error("La ejecución de la actividad {0} terminó con errores: {1}", resultado.ProximaActividad, resultadoActividad.Errores.First().Value);
                return huboErrorEnWorkflow ? resultadoActividad.Errores.First().Value : Constantes.ResultadoProcesoIdentificacionVehicular.EjecucionExitosa;
            }
            catch (Exception e)
            {
                log.Error(e, "Fallo la ejecucion del workflow relacionado con la tarjeta: {0}", lecturaPuestoDeTrabajo.NumeroDeTarjeta);
                return e.Message;
            }
        }
    }
}


