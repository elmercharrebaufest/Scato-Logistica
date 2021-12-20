using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarMicroMuestrasPorCasillero : ProcesadorEliminar<EliminarMicroMuestrasPorCasillero, MicroMuestrasPorCasillero>
    {
        public ProcesadorEliminarMicroMuestrasPorCasillero(IRepositorio repositorio, IConversor conversor, ILogger log)
         : base(repositorio, conversor, log)
        {
        }

        protected override int IdEntidad(EliminarMicroMuestrasPorCasillero comando)
        {
            return comando.Id;
        }

        protected override void Validar(EliminarMicroMuestrasPorCasillero comando, Resultado resultado)
        {
        }
    }
}
