using Molinos.Scato.Dominio.Comandos;
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
                Log.Debug($"Ejecutando comando RegistrarMarcaDeTiempo: Tipo={comando.Tipo}, CodigoDispositivo={comando.CodigoDispositivo}, PuestoDeTrabajoId={comando.PuestoDeTrabajoId}, NumeroDeTarjeta={comando.NumeroDeTarjeta}, Patente={comando.Patente}, Trigger={comando.Trigger}, InstanceId={comando.InstanceId}");
                if (!Validar(comando, resultado))
                    return resultado;

                switch (comando.Tipo)
                {
                    case TipoRegistroMarcaDeTiempo.Inicio:
                        if (comando.InstanceId.HasValue)
                            marcaDeTiempo.RegistrarInicioPorInstanciaWorkflow(comando.InstanceId.Value);
                        else
                            marcaDeTiempo.RegistrarInicioPorSensor(comando.CodigoDispositivo);
                        break;

                    case TipoRegistroMarcaDeTiempo.Identificacion:
                        marcaDeTiempo.RegistrarIdentificacion(comando.PuestoDeTrabajoId.Value, comando.NumeroDeTarjeta, comando.Patente, comando.Trigger.Value);
                        break;

                    case TipoRegistroMarcaDeTiempo.Fin:
                        if (comando.InstanceId.HasValue)
                            marcaDeTiempo.RegistrarFinPorInstanciaWorkflow(comando.InstanceId.Value, comando.PuestoDeTrabajoId);
                        else
                            marcaDeTiempo.RegistrarFinPorSensor(comando.CodigoDispositivo);
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
            if (comando.Tipo == TipoRegistroMarcaDeTiempo.Inicio && string.IsNullOrEmpty(comando.CodigoDispositivo) && !comando.InstanceId.HasValue)
                resultado.Error(nameof(comando.CodigoDispositivo), "El codigo dispositivo o instanceId es requerido para fecha inicio");

            if (comando.Tipo == TipoRegistroMarcaDeTiempo.Identificacion && !comando.PuestoDeTrabajoId.HasValue)
                resultado.Error(nameof(comando.PuestoDeTrabajoId), "El puesto de trabajo es requerido para fecha de identificación");

            if (comando.Tipo == TipoRegistroMarcaDeTiempo.Identificacion && string.IsNullOrEmpty(comando.NumeroDeTarjeta) && string.IsNullOrEmpty(comando.Patente))
                resultado.Error(nameof(comando.NumeroDeTarjeta), "La patente o número de tarjeta es requerido para fecha de identificación");

            if (comando.Tipo == TipoRegistroMarcaDeTiempo.Identificacion && !comando.Trigger.HasValue)
                resultado.Error(nameof(comando.Trigger), "El trigger es requerido para fecha de identificación");

            if (comando.Tipo == TipoRegistroMarcaDeTiempo.Fin && !comando.InstanceId.HasValue && string.IsNullOrEmpty(comando.CodigoDispositivo))
                resultado.Error(nameof(comando.InstanceId), "El instanceId o codigoDispositivo es requerido para fecha de fin");

            return !resultado.HayErrores;
        }
    }
}
