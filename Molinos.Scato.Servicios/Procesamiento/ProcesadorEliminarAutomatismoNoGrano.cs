using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using Molinos.Scato.Dominio;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarAutomatismoNoGrano : ProcesadorEliminar<EliminarAutomatismoNoGrano, AutomatismoNoGrano>
    {
        public ProcesadorEliminarAutomatismoNoGrano(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override int IdEntidad(EliminarAutomatismoNoGrano comando)
        {
            return comando.Id;
        }

        protected override void Validar(EliminarAutomatismoNoGrano comando, Resultado resultado)
        {
        }
    }
}
