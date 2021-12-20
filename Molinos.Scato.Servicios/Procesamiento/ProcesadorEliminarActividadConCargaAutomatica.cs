using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarActividadConCargaAutomatica : ProcesadorEliminar<EliminarActividadConCargaAutomatica, ActividadConCargaAutomatica>
    {
        public ProcesadorEliminarActividadConCargaAutomatica(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override int IdEntidad(EliminarActividadConCargaAutomatica comando)
        {
            return comando.Id;
        }

        protected override void Validar(EliminarActividadConCargaAutomatica comando, Resultado resultado)
        {
        }
    }
}