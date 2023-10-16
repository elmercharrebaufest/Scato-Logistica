using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarAsignacionAutomatismoNoGranoEnRecorrido : ProcesadorEliminar<EliminarAsignacionAutomatismoNoGranoEnRecorrido, AsignacionAutomatismoNoGranoEnRecorrido>
    {
        public ProcesadorEliminarAsignacionAutomatismoNoGranoEnRecorrido(IRepositorio repositorio, IConversor conversor, ILogger log)
           : base(repositorio, conversor, log)
        {
        }

        protected override int IdEntidad(EliminarAsignacionAutomatismoNoGranoEnRecorrido comando)
        {
            return Repositorio.ObtenerProyeccion<AsignacionAutomatismoNoGranoEnRecorrido, int>(x => x.RecorridoId == comando.RecorridoId, x => x.Id);
        }

        protected override void Validar(EliminarAsignacionAutomatismoNoGranoEnRecorrido comando, Resultado resultado)
        {
            if (!Repositorio.Existe<AsignacionAutomatismoNoGranoEnRecorrido>(x => x.RecorridoId == comando.RecorridoId))
            {
                resultado.Error(string.Empty, $"No existe el recorrido {comando.RecorridoId} en automatismo");
            }
        }
    }
}