using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ObtenerDisponibilidadCalleInterna : IConsultaEscalar<int>
    {
        private int? calleId;

        public ObtenerDisponibilidadCalleInterna(int? calleId)
        {
            this.calleId = calleId;
        }
        public int Ejecutar(DbContext contexto)
        {
            var callesPorRecorrido = contexto.Set<CallePorRecorrido>().Where(x => x.Calle.Id == calleId && x.FechaEgreso == null);
            var calles = contexto.Set<Calle>().Where(x => x.Id == calleId);
            return calles.Count() > 0 ? calles.Sum(x => x.CantidadDeCamiones) - callesPorRecorrido.Count():0;
        }
    }
}
