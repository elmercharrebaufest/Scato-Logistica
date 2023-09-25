using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarPasoDirectoUnoAUnoAutomatismoGrano : ProcesadorModificar<ModificarPasoDirectoUnoAUnoAutomatismoGrano>
    {
        public ProcesadorModificarPasoDirectoUnoAUnoAutomatismoGrano(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarPasoDirectoUnoAUnoAutomatismoGrano comando)
        {
            var automatismo = Repositorio.Obtener<AutomatismoGrano>(comando.Id);
            automatismo.EsPasoDirecto = comando.EsPasoDirecto;
            automatismo.Llamado1a1 = comando.LlamadoUnoAUno;
        }

        protected override void Validar(ModificarPasoDirectoUnoAUnoAutomatismoGrano comando, Resultado resultado)
        {
        }
    }
}