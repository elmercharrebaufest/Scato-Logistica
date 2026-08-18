using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorActualizarLogIdentificacionVehicularResultado : ProcesadorComando<ActualizarLogIdentificacionVehicularResultado>
    {
        public ProcesadorActualizarLogIdentificacionVehicularResultado(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ActualizarLogIdentificacionVehicularResultado comando)
        {
            var resultado = new Resultado();

            try
            {
                Validar(comando, resultado);
                if (resultado.HayErrores)
                    return resultado;

                var log = Repositorio.ObtenerPrimero<LogIdentificacionVehicular>(x => x.Id == comando.LogId);
                if (log == null)
                    return resultado;

                log.ResultadoWorkflow = comando.ResultadoWorkflow;

                if (comando.RecorridoId.HasValue)
                    log.Recorrido = Repositorio.Obtener<Recorrido>(comando.RecorridoId.Value);

                if (comando.PuestoDeTrabajoId.HasValue)
                    log.PuestoDeTrabajo = Repositorio.Obtener<PuestoDeTrabajo>(comando.PuestoDeTrabajoId.Value);

                Repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al actualizar el resultado del log de identificación vehicular");
                resultado.Error(nameof(Exception), ex.Message);
            }

            return resultado;
        }

        private void Validar(ActualizarLogIdentificacionVehicularResultado comando, Resultado resultado)
        {
            if (comando.LogId <= 0)
            {
                resultado.Error("LogId", "El ID del log es requerido");
            }

            if (string.IsNullOrWhiteSpace(comando.ResultadoWorkflow))
            {
                resultado.Error("ResultadoWorkflow", "El resultado del workflow es requerido");
            }
        }
    }
}
