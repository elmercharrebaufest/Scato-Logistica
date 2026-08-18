using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Newtonsoft.Json;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Molinos.Scato.Web.Firmware
{
    public class RecorridoWorkflow : IRecorridoWorkflow
    {
        protected readonly ILogger log;
        protected readonly IServicioRepositorio servicio;
        protected readonly IListaDeWorkflows workflows;

        public RecorridoWorkflow(ILogger log, IServicioRepositorio servicio, IListaDeWorkflows workflows)
        {
            this.log = log;
            this.servicio = servicio;
            this.workflows = workflows;
        }

        private DatosRecorridoDto ObtenerRecorrido(string numeroDeTarjeta, int puestoDeTrabajoId)
        {
            log.Info($"Obteniendo recorrido activo para la tarjeta: {numeroDeTarjeta}, Puesto de Trabajo: {puestoDeTrabajoId}");
            var recorrido = servicio.ObtenerDatosRecorridoActivo(null, numeroDeTarjeta);
            if (recorrido == null)
            {
                var datosDeRecorrido = new DatosRecorridoDto
                {
                    CentroCodigoSap = numeroDeTarjeta,
                    SinRecorrido = true
                };
                return datosDeRecorrido;
            }

            log.Info("Recorrido activo encontrado. InstanciaWorkflow: {0}, Patente: {1}, ProximaAccion: {2}", recorrido.InstanciaWorkflow, recorrido.Patente, recorrido.ProximaAccion);
            var proximaActividad = workflows.ObtenerWorkflowProximaAccion(recorrido.InstanciaWorkflow);
            if (proximaActividad == null || string.IsNullOrEmpty(proximaActividad.ProximaAccion))
                throw new InvalidOperationException($"No se ha encontrado una proxima accion para el recorrido: {recorrido.InstanciaWorkflow}");
            
            log.Info(JsonConvert.SerializeObject(proximaActividad));
            recorrido.ProximaAccion = proximaActividad.ProximaAccion;
            recorrido.ProximaAccionMensaje = proximaActividad.Mensaje;

            return recorrido;
        }

        public ValidarProximaAccionDto ObtenerWorkflowProximaAccion(string numeroDeTarjeta, int puestoDeTrabajoId)
        {
          
            try
            {
                var recorrido = ObtenerRecorrido(numeroDeTarjeta, puestoDeTrabajoId);
                return ValidarProximaActividad(numeroDeTarjeta, puestoDeTrabajoId, recorrido);
            }
            catch (Exception)
            {
                throw;
            }

        }

        public ValidarProximaAccionDto ObtenerWorkflowProximaAccionConRecorrido(string numeroDeTarjeta, int puestoDeTrabajoId, DatosRecorridoDto recorrido)
        {
            try
            {
                return ValidarProximaActividad(numeroDeTarjeta, puestoDeTrabajoId, recorrido);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private ValidarProximaAccionDto ValidarProximaActividad(string numeroDeTarjeta, int puestoDeTrabajoId, DatosRecorridoDto recorrido)
        {
            List<PuestoDeTrabajoDto> puestos = new List<PuestoDeTrabajoDto> { new PuestoDeTrabajoDto { Lectura = numeroDeTarjeta, Id = puestoDeTrabajoId } };
            log.Info("Validando proxima actividad por puesto sin patente: {0}", JsonConvert.SerializeObject(puestos));
            if (recorrido.SinRecorrido)
            {
                throw new InvalidOperationException($"No se ha encontrado un recorrido activo para la tarjeta: {numeroDeTarjeta}");
            }
            var resultadoValidarProximaActividad = servicio.ValidarProximaActividadPorPuestoSinPatente(recorrido, recorrido.ProximaAccion, puestos);

            if (!resultadoValidarProximaActividad.Valida)
            {
                throw new InvalidOperationException(resultadoValidarProximaActividad.MensajeError);
            }
            if (resultadoValidarProximaActividad.WorkflowDefinicionId <= 0)
                throw new ArgumentException(nameof(ValidarProximaAccionDto), "El id del workflow no puede ser menor o igual a 0");

            if (resultadoValidarProximaActividad.InstanceId == Guid.Empty)
                throw new ArgumentException(nameof(ValidarProximaAccionDto), "El id de la instancia del workflow no puede ser nulo o vacio");

            if (resultadoValidarProximaActividad.PuestoDeTrabajoId <= 0)
                throw new ArgumentException(nameof(ValidarProximaAccionDto), "El id del puesto de trabajo no puede ser menor o igual a 0");

            if (string.IsNullOrEmpty(resultadoValidarProximaActividad.ProximaActividad))
                throw new ArgumentException(nameof(ValidarProximaAccionDto), "La proxima actividad no puede ser nula o vacia");

            if (string.IsNullOrEmpty(resultadoValidarProximaActividad.NumeroDocumentoIngreso))
                throw new ArgumentException(nameof(ValidarProximaAccionDto), "El numero de documento de ingreso no puede ser nulo o vacio");

            if (string.IsNullOrEmpty(resultadoValidarProximaActividad.Patente))
                throw new ArgumentException(nameof(ValidarProximaAccionDto), "La patente no puede ser nula o vacia");

            if (string.IsNullOrEmpty(resultadoValidarProximaActividad.CodigoSapCentro))
                throw new ArgumentException(nameof(ValidarProximaAccionDto), "El codigo SAP del centro no puede ser nulo o vacio");

            return resultadoValidarProximaActividad;
        }
    }
}