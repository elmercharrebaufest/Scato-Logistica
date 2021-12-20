using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarCoordinadorPuerto : ProcesadorEliminar<EliminarCoordinadorPuerto, CoordinadorPuerto>
    {
        public ProcesadorEliminarCoordinadorPuerto(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override int IdEntidad(EliminarCoordinadorPuerto comando)
        {
            return comando.Id;
        }

        protected override void Validar(EliminarCoordinadorPuerto comando, Resultado resultado)
        {
        }
    }
}