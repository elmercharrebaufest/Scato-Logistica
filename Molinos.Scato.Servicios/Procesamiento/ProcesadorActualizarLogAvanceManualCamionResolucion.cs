using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorActualizarLogAvanceManualCamionResolucion : ProcesadorComando<ActualizarLogAvanceManualCamionResolucion>
    {
        public ProcesadorActualizarLogAvanceManualCamionResolucion(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ActualizarLogAvanceManualCamionResolucion comando)
        {
            var resultado = new Resultado();
            try
            {
                var log = Repositorio.Obtener<LogAvanceManualCamion>(comando.LogId);
                if (log == null)
                {
                    resultado.Error("LogNoEncontrado", $"No se encontró el registro con Id {comando.LogId}.");
                    return resultado;
                }
                log.PatenteIngresada                    = comando.PatenteIngresada;
                log.Atendido                            = 2;
                log.FechaAtencion                       = DateTime.Now;
                log.NombreUsuario                       = comando.Usuario;
                log.LogIdentificacionVehicularDestinoId = comando.LogIdentificacionVehicularDestinoId;
                Repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al actualizar la resolución del LogAvanceManualCamion {0}", comando.LogId);
                resultado.Error(nameof(Exception), e.Message);
            }
            return resultado;
        }
    }
}
