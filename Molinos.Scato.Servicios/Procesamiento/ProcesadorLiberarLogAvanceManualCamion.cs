using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorLiberarLogAvanceManualCamion : ProcesadorComando<LiberarLogAvanceManualCamion>
    {
        public ProcesadorLiberarLogAvanceManualCamion(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(LiberarLogAvanceManualCamion comando)
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
                if (log.Atendido == 1)
                {
                    log.Atendido      = 0;
                    log.NombreUsuario = null;
                    Repositorio.GuardarCambios();
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al liberar el LogAvanceManualCamion {0}", comando.LogId);
                resultado.Error(nameof(Exception), e.Message);
            }
            return resultado;
        }
    }
}
