using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarAsignacionAutomatismoGranoEnRecorrido : ProcesadorEliminar<EliminarAsignacionAutomatismoGranoEnRecorrido, AsignacionAutomatismoGranoEnRecorrido>
    {
        public ProcesadorEliminarAsignacionAutomatismoGranoEnRecorrido(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override int IdEntidad(EliminarAsignacionAutomatismoGranoEnRecorrido comando)
        {
            return Repositorio.ObtenerProyeccion<AsignacionAutomatismoGranoEnRecorrido, int>(x => x.RecorridoId == comando.RecorridoId, x => x.Id);
        }

        protected override void Validar(EliminarAsignacionAutomatismoGranoEnRecorrido comando, Resultado resultado)
        {
            if (!Repositorio.Existe<AsignacionAutomatismoGranoEnRecorrido>(x => x.RecorridoId == comando.RecorridoId))
            {
                resultado.Error(string.Empty, $"No existe el recorrido {comando.RecorridoId} en automatismo");
            }
        }
    }
}
