using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarAsignacionNoGranoEnRecorrido : ProcesadorEliminar<EliminarAsignacionNoGranoEnRecorrido, AsignacionNoGranoEnRecorrido>
    {
        public ProcesadorEliminarAsignacionNoGranoEnRecorrido(IRepositorio repositorio, IConversor conversor, ILogger log)
           : base(repositorio, conversor, log)
        {
        }

        protected override int IdEntidad(EliminarAsignacionNoGranoEnRecorrido comando)
        {
            return Repositorio.ObtenerProyeccion<AsignacionNoGranoEnRecorrido, int>(x => x.RecorridoId == comando.RecorridoId, x => x.Id);
        }

        protected override void Validar(EliminarAsignacionNoGranoEnRecorrido comando, Resultado resultado)
        {
            if (!Repositorio.Existe<AsignacionNoGranoEnRecorrido>(x => x.RecorridoId == comando.RecorridoId))
            {
                resultado.Error(string.Empty, $"No existe el recorrido {comando.RecorridoId} en automatismo");
            }
        }
    }
}