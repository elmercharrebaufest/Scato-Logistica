using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using System.Data.Entity;
using System.Linq;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ObtenerCallePlantaNoGranos : IConsultaEscalar<Calle>
    {
        private TipoCalle tipoCalle;
        private int materialId;

        public ObtenerCallePlantaNoGranos(TipoCalle tipoCalle, Material material)
        {
            this.tipoCalle = tipoCalle;
            this.materialId = material != null ? material.Id : 0;
        }

        public Calle Ejecutar(DbContext contexto)
        {
            Calle calle = ObtenerCalle(contexto, materialId);
            if(calle == null)
            {
                calle = ObtenerCalle(contexto);
            }
            return calle;
        }

        private Calle ObtenerCalle(DbContext contexto, int? material = null)
        {
            return contexto.Set<Calle>().Where(x => x.TipoCalle == tipoCalle && (x.Material.Id == material || material == null))
                                              .OrderByDescending(x => x.Id)
                                              .FirstOrDefault();
        }
    }
}
