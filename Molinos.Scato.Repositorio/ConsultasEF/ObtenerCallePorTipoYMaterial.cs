using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using System.Data.Entity;
using System.Linq;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ObtenerCallePorTipoYMaterial : IConsultaEscalar<Calle>
    {
        private TipoCalle tipoCalle;
        private int materialId;

        public ObtenerCallePorTipoYMaterial(TipoCalle tipoCalle, Material material)
        {
            this.tipoCalle = tipoCalle;
            this.materialId = material != null ? material.Id : 0;
        }

        public Calle Ejecutar(DbContext contexto)
        {
            Calle calle = ObtenerCalle(contexto);
            return calle;
        }

        private Calle ObtenerCalle(DbContext contexto)
        {
            return contexto.Set<Calle>().Where(x => x.TipoCalle == tipoCalle 
                                               && !x.Deshabilitada
                                               && (x.Material.Id == materialId || x.Material == null))
                                              .FirstOrDefault();
        }
    }
}
