using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarControlDeTiempo : ProcesadorEliminar<EliminarControlDeTiempo, ControlDeTiempo>
    {
        public ProcesadorEliminarControlDeTiempo(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override int IdEntidad(EliminarControlDeTiempo comando)
        {
            return comando.Id;
        }

        protected override void Validar(EliminarControlDeTiempo comando, Resultado resultado)
        {
        }
    }
}