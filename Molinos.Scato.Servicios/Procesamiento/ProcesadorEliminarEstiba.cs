using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarEstiba : ProcesadorEliminar<EliminarEstiba, Estiba>
    {
        public ProcesadorEliminarEstiba(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override int IdEntidad(EliminarEstiba comando)
        {
            return comando.Id;
        }

        protected override void Validar(EliminarEstiba comando, Resultado resultado)
        {
        }
    }
}