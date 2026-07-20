using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Interfaces;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.Servicios.Impl
{
    public class MarcaDeTiempo : IMarcaDeTiempo
    {
        private readonly IRepositorio repositorio;
        private readonly ILogger log;

        public MarcaDeTiempo(IRepositorio repositorio, ILogger log)
        {
            this.repositorio = repositorio;
            this.log = log;
        }

        public void RegistrarPorSensor(string codigoDispositivo)
        {
            var sensorPuesto = repositorio.Obtener<SensorMarcaTiempoPorPuestoDeTrabajo>(s => s.CodigoSensor == codigoDispositivo);
            if (sensorPuesto == null)
                return;

            switch (sensorPuesto.TipoSensor)
            {
                case TipoSensorMarcaTiempo.Inicio:
                    RegistrarInicioEnPuesto(sensorPuesto.PuestoDeTrabajoId);
                    break;
                case TipoSensorMarcaTiempo.Fin:
                    RegistrarFinEnPuesto(sensorPuesto.PuestoDeTrabajoId);
                    break;
            }
        }

        private void RegistrarInicioEnPuesto(int puestoDeTrabajoId)
        {
            repositorio.Agregar(new MarcaTiempoPorPuestoDeTrabajo
            {
                PuestoDeTrabajoId = puestoDeTrabajoId,
                FechaInicio = DateTime.Now,
            });
            repositorio.GuardarCambios();
        }

        private void RegistrarFinEnPuesto(int puestoDeTrabajoId)
        {
            var registro = repositorio.ObtenerMasReciente<MarcaTiempoPorPuestoDeTrabajo>(
                x => x.PuestoDeTrabajoId == puestoDeTrabajoId
                    && x.FechaInicio.HasValue,
                x => x.FechaInicio.Value);
            if (registro == null || registro.FechaFin.HasValue)
                return;

            registro.FechaFin = DateTime.Now;
            repositorio.GuardarCambios();
        }

        public void RegistrarIdentificacion(int puestoDeTrabajoId, string numeroDeTarjeta, string patente, TipoIdentificacionPorPuesto tipoIdentificacion)
        {
            var recorrido = BuscarRecorrido(numeroDeTarjeta, patente);
            if (recorrido == null)
                return;

            var registroExistente = repositorio.ObtenerMasReciente<MarcaTiempoPorPuestoDeTrabajo>(
               x => (x.PuestoDeTrabajoId == puestoDeTrabajoId || (x.PuestoDeTrabajoId == null && x.RecorridoId == recorrido.Id))
                    && x.FechaInicio.HasValue,
                x => x.FechaInicio.Value);
            if (registroExistente == null || registroExistente.FechaIdentificacion.HasValue || registroExistente.FechaFin.HasValue)
                return;

            registroExistente.RecorridoId = recorrido.Id;
            registroExistente.FechaIdentificacion = DateTime.Now;
            registroExistente.TipoIdentificacion = tipoIdentificacion;
            repositorio.GuardarCambios();
        }

        private Recorrido BuscarRecorrido(string numeroDeTarjeta = null, string patente = null, Guid? instanceId = null)
        {
            if (!string.IsNullOrEmpty(numeroDeTarjeta))
                return repositorio.ObtenerMasReciente<Recorrido>(r => r.TarjetaDeAcceso == numeroDeTarjeta, r => r.FechaInicio);

            if (!string.IsNullOrEmpty(patente))
                return repositorio.ObtenerMasReciente<Recorrido>(r => r.Patente == patente, r => r.FechaInicio);

            if (instanceId.HasValue)
                return repositorio.ObtenerMasReciente<Recorrido>(r => r.InstanciaWorkflow == instanceId.Value, r => r.FechaInicio);

            return null;
        }

        public void RegistrarFinPorInstanciaWorkflow(Guid instanceId, int? puestoDeTrabajoId = null)
        {
            var recorrido = BuscarRecorrido(instanceId: instanceId);
            log.Debug($"[RegistrarFinPorInstanciaWorkflow] Recorrido encontrado: {(recorrido != null ? recorrido.Id.ToString() : "null")}");
            if (recorrido == null)
                return;

            var registro = repositorio.ObtenerMasReciente<MarcaTiempoPorPuestoDeTrabajo>(
                x => x.RecorridoId == recorrido.Id
                    && x.FechaInicio.HasValue,
                x => x.FechaInicio.Value);
            log.Debug($"[RegistrarFinPorInstanciaWorkflow] MarcaTiempoPorPuestoDeTrabajo encontrada: {(registro != null ? registro.Id.ToString() : "null")}");
            if (registro == null || registro.FechaFin.HasValue)
                return;

            if(puestoDeTrabajoId.HasValue && !registro.PuestoDeTrabajoId.HasValue)
                registro.PuestoDeTrabajoId = puestoDeTrabajoId.Value;

            registro.FechaFin = DateTime.Now;
            repositorio.GuardarCambios();
            log.Debug($"[RegistrarFinPorInstanciaWorkflow] FechaFin registrada en Id: {registro.Id}");
        }

        public void RegistrarInicioPorInstanciaWorkflow(Guid instanceId)
        {
            var recorrido = BuscarRecorrido(instanceId: instanceId);
            if (recorrido == null)
                return;

            repositorio.Agregar(new MarcaTiempoPorPuestoDeTrabajo
            {
                FechaInicio = DateTime.Now,
                RecorridoId = recorrido.Id
            });

            repositorio.GuardarCambios();
        }

        public void RegistrarInicioPorPuestoDeTrabajo(int puestoDeTrabajoId)
        {
            repositorio.Agregar(new MarcaTiempoPorPuestoDeTrabajo
            {
                FechaInicio = DateTime.Now,
                PuestoDeTrabajoId = puestoDeTrabajoId
            });
            repositorio.GuardarCambios();
        }

        public void RegistrarFinPorPuestoDeTrabajo(int puestoDeTrabajoId)
        {
            var registro = repositorio.ObtenerMasReciente<MarcaTiempoPorPuestoDeTrabajo>(
                x => x.PuestoDeTrabajoId == puestoDeTrabajoId
                    && x.FechaInicio.HasValue
                    && !x.FechaFin.HasValue,
                x => x.FechaInicio.Value);
            if (registro == null)
                return;

            registro.FechaFin = DateTime.Now;
            repositorio.GuardarCambios();
        }
    }
}