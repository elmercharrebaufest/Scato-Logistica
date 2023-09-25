using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarPasoDirectoAutomatismoGrano : ProcesadorModificar<ModificarPasoDirectoAutomatismoGrano>
    {
        public ProcesadorModificarPasoDirectoAutomatismoGrano(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarPasoDirectoAutomatismoGrano comando)
        {
            var automatismo = Repositorio.Obtener<AutomatismoGrano>(comando.Id);
            automatismo.EsPasoDirecto = comando.EsPasoDirecto;
            if (!automatismo.EsPasoDirecto)
            {
                automatismo.Llamado1a1 = false;
            }
        }

        protected override void Validar(ModificarPasoDirectoAutomatismoGrano comando, Resultado resultado)
        {
        }
    }
}