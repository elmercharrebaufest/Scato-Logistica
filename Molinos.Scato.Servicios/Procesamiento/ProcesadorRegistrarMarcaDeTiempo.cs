using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Interfaces;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorRegistrarMarcaDeTiempo : ProcesadorComando<RegistrarMarcaDeTiempo>
    {
        private readonly IMarcaDeTiempo marcaDeTiempo;

        public ProcesadorRegistrarMarcaDeTiempo(
            IRepositorio repositorio,
            IConversor conversor,
            ILogger log,
            IMarcaDeTiempo marcaDeTiempo)
            : base(repositorio, conversor, log)
        {
            this.marcaDeTiempo = marcaDeTiempo;
        }

        public override Resultado Ejecutar(RegistrarMarcaDeTiempo comando)
        {
            var resultado = new Resultado();

            try
            {
                Log.Debug($"[RegistrarMarcaDeTiempo]: Tipo={comando.Tipo}, CodigoDispositivo={comando.CodigoDispositivo}, PuestoDeTrabajoId={comando.PuestoDeTrabajoId}, NumeroDeTarjeta={comando.NumeroDeTarjeta}, Patente={comando.Patente}, Trigger={comando.Trigger}, InstanceId={comando.InstanceId}");
                if (!Validar(comando, resultado))
                    return resultado;

                switch (comando.Tipo)
                {
                    case TipoSensorMarcaTiempo.Inicio:
                        marcaDeTiempo.RegistrarInicioPorInstanciaWorkflow(comando.InstanceId.Value);
                        break;

                    case TipoSensorMarcaTiempo.Identificacion:
                        marcaDeTiempo.RegistrarIdentificacion(comando.PuestoDeTrabajoId.Value, comando.NumeroDeTarjeta, comando.Patente, comando.Trigger.Value);
                        break;

                    case TipoSensorMarcaTiempo.InicioConIdentificacion:
                        marcaDeTiempo.RegistrarInicioConIdentificacion(comando.PuestoDeTrabajoId.Value, comando.NumeroDeTarjeta, comando.Patente, comando.Trigger.Value);
                        break;

                    case TipoSensorMarcaTiempo.Fin:
                        marcaDeTiempo.RegistrarFinPorInstanciaWorkflow(comando.InstanceId.Value, comando.PuestoDeTrabajoId);
                        break;

                    case TipoSensorMarcaTiempo.InicioOFinPorSensor:
                        marcaDeTiempo.RegistrarPorSensor(comando.CodigoDispositivo);
                        break;

                    case TipoSensorMarcaTiempo.InicioPorGaritaIngreso:
                        marcaDeTiempo.RegistrarInicioPorGaritaIngreso(comando.PuestoDeTrabajoId.Value);
                        break;

                    case TipoSensorMarcaTiempo.FinPorGaritaIngreso:
                        var tipoIngreso = string.IsNullOrEmpty(comando.NumeroDeTarjeta) ? TipoIdentificacionPorPuesto.IngresoPorPatente : TipoIdentificacionPorPuesto.IngresoPorLectura;
                        marcaDeTiempo.RegistrarFinPorGaritaIngreso(comando.PuestoDeTrabajoId.Value, tipoIngreso);
                        break;

                    default:
                        resultado.Error(nameof(comando.Tipo), $"Tipo de registro desconocido: {comando.Tipo}");
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error al procesar comando RegistrarMarcaDeTiempo: {ex.Message}");
                resultado.Error("Excepcion", "Ocurrió un error al registrar la marca de tiempo");
            }

            return resultado;
        }

        private bool Validar(RegistrarMarcaDeTiempo comando, Resultado resultado)
        {
            if (comando.Tipo == TipoSensorMarcaTiempo.Inicio && !comando.InstanceId.HasValue)
                resultado.Error(nameof(comando.CodigoDispositivo), "El instanceId o puesto de trabajo es requerido para fecha inicio");

            if (comando.Tipo == TipoSensorMarcaTiempo.Identificacion && !comando.PuestoDeTrabajoId.HasValue)
                resultado.Error(nameof(comando.PuestoDeTrabajoId), "El puesto de trabajo es requerido para fecha de identificación");

            if (comando.Tipo == TipoSensorMarcaTiempo.Identificacion && string.IsNullOrEmpty(comando.NumeroDeTarjeta) && string.IsNullOrEmpty(comando.Patente))
                resultado.Error(nameof(comando.NumeroDeTarjeta), "La patente o número de tarjeta es requerido para fecha de identificación");

            if (comando.Tipo == TipoSensorMarcaTiempo.Identificacion && !comando.Trigger.HasValue)
                resultado.Error(nameof(comando.Trigger), "El trigger es requerido para fecha de identificación");

            if (comando.Tipo == TipoSensorMarcaTiempo.InicioConIdentificacion && !comando.PuestoDeTrabajoId.HasValue)
                resultado.Error(nameof(comando.PuestoDeTrabajoId), "El puesto de trabajo es requerido para inicio con identificación");

            if (comando.Tipo == TipoSensorMarcaTiempo.InicioConIdentificacion && string.IsNullOrEmpty(comando.NumeroDeTarjeta) && string.IsNullOrEmpty(comando.Patente))
                resultado.Error(nameof(comando.NumeroDeTarjeta), "La patente o número de tarjeta es requerido para inicio con identificación");

            if (comando.Tipo == TipoSensorMarcaTiempo.InicioConIdentificacion && !comando.Trigger.HasValue)
                resultado.Error(nameof(comando.Trigger), "El trigger es requerido para inicio con identificación");

            if (comando.Tipo == TipoSensorMarcaTiempo.Fin && !comando.InstanceId.HasValue)
                resultado.Error(nameof(comando.InstanceId), "El instanceId o puesto de trabajo es requerido para fecha de fin");
            
            if (comando.Tipo == TipoSensorMarcaTiempo.InicioOFinPorSensor && string.IsNullOrEmpty(comando.CodigoDispositivo))
                resultado.Error(nameof(comando.CodigoDispositivo), "El codigo dispositivo es requerido para inicio/fin por sensor");

            if (comando.Tipo == TipoSensorMarcaTiempo.InicioPorGaritaIngreso && !comando.PuestoDeTrabajoId.HasValue)
                resultado.Error(nameof(comando.PuestoDeTrabajoId), "El puesto de trabajo es requerido para fecha de inicio");

            if (comando.Tipo == TipoSensorMarcaTiempo.FinPorGaritaIngreso && !comando.PuestoDeTrabajoId.HasValue)
                resultado.Error(nameof(comando.PuestoDeTrabajoId), "El puesto de trabajo es requerido para fecha de fin");

            return !resultado.HayErrores;
        }
    }
}
