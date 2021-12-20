using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ObtenerDisponibilidadPreCalado : IConsultaEscalar<int>
    {
        private int materialId;

        public ObtenerDisponibilidadPreCalado(int materialId)
        {
            this.materialId = materialId;
        }
        public int Ejecutar(DbContext contexto)
        {
            var calles = contexto.Set<Calle>().Where(x => x.TipoCalle == TipoCalle.PreCalado && !x.Deshabilitada && !x.Bloqueada && x.Material.Id == materialId);
            var callesPorRecorrido = contexto.Set<CallePorRecorrido>().Where(x=> x.FechaEgreso == null && calles.Contains(x.Calle));
            return calles.Count() > 0 ? calles.Sum(x => x.CantidadDeCamiones) - callesPorRecorrido.Count() : 0;
        }
    }
}
