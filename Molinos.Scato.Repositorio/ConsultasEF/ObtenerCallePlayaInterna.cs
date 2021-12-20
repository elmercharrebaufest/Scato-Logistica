using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ObtenerCallePlayaInterna : IConsultaEscalar<Calle>
    {
        private int calleId;

        public ObtenerCallePlayaInterna(int calleId)
        {
            this.calleId = calleId;
        }
        public Calle Ejecutar(DbContext contexto)
        {
            var calleDisponible = ObtenerCalleAsignada(contexto, calleId);

            return calleDisponible;
        }

        private Calle ObtenerCalleAsignada(DbContext contexto, int calleId)
        {
            return contexto.Set<Calle>()
                .Where(x => x.Id == calleId &&
                        //redundante ya que controlamos el material al obtener el último camion asignado
                        //contexto.Set<CallePorRecorrido>().Any(y => (y.CargaDeCupo.Material.Id == materialId || y.Recorrido.Material.Id == materialId) && y.FechaEgreso == null && y.Calle.Id == x.Id) &&
                        contexto.Set<CallePorRecorrido>().Count(y => y.FechaEgreso == null && y.Calle.Id == x.Id) < x.CantidadDeCamiones)
                .FirstOrDefault();
        }
    }
}
