using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarBodega : ProcesadorEliminar<EliminarBodega, Bodega>
    {
        public ProcesadorEliminarBodega(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override int IdEntidad(EliminarBodega comando)
        {
            return comando.Id;
        }

        protected override void Validar(EliminarBodega comando, Resultado resultado)
        {
        }
    }
}
