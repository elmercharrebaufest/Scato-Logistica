using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using System.Data.Entity;
using System.Linq;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ObtenerDisponibilidad : IConsultaEscalar<int>
    {
        private TipoCalle tipoCalle;
        private int materialId;

        public ObtenerDisponibilidad(TipoCalle tipoCalle, int materialId)
        {
            this.tipoCalle = tipoCalle;
            this.materialId = materialId;
        }
        public int Ejecutar(DbContext contexto)
        {
            var disp = contexto.Set<Calle>().Where(x => x.TipoCalle == tipoCalle).ToList();
            var dispBruta = disp.Where(x => tipoCalle == TipoCalle.NoGranos ? x.Material.Id == materialId : true).Sum(x => x.CantidadDeCamiones);
            return dispBruta - contexto.Set<CallePorRecorrido>().Count(y => y.FechaEgreso == null && (tipoCalle == TipoCalle.NoGranos ? y.Calle.TipoCalle == tipoCalle && y.Calle.Material.Id == materialId : y.Calle.TipoCalle == tipoCalle));
        }
    }
}
