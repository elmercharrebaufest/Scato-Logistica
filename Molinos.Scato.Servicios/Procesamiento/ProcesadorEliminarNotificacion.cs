using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarNotificacion : ProcesadorEliminar<EliminarNotificacion, Notificacion>
    {
        public ProcesadorEliminarNotificacion(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override int IdEntidad(EliminarNotificacion comando)
        {
            return comando.Id;
        }

        protected override void Validar(EliminarNotificacion comando, Resultado resultado)
        {
        }
    }
}
