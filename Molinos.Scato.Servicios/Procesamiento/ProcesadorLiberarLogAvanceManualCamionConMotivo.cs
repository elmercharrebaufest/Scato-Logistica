using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorLiberarLogAvanceManualCamionConMotivo : ProcesadorComando<LiberarLogAvanceManualCamionConMotivo>
    {
        public ProcesadorLiberarLogAvanceManualCamionConMotivo(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(LiberarLogAvanceManualCamionConMotivo comando)
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
                log.Atendido      = 4;
                log.MotivoLiberar = comando.Motivo;
                log.NombreUsuario = comando.Usuario;
                log.FechaAtencion = DateTime.Now;
                Repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al liberar con motivo el LogAvanceManualCamion {0}", comando.LogId);
                resultado.Error(nameof(Exception), e.Message);
            }
            return resultado;
        }
    }
}
