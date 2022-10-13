using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using System.Data.Entity;
using System.Linq;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ObtenerUltimaCallePorTipoYMaterial : IConsultaEscalar<Calle>
    {
        private TipoCalle tipoCalle;
        private int materialId;

        public ObtenerUltimaCallePorTipoYMaterial(TipoCalle tipoCalle, Material material)
        {
            this.tipoCalle = tipoCalle;
            this.materialId = material != null ? material.Id : 0;
        }

        public Calle Ejecutar(DbContext contexto)
        {
            Calle calleDisponible = ObtenerCalleDisponible(contexto);
            if (calleDisponible == null)
            {
                calleDisponible = ObtenerCalleDeUltimoCamion(contexto);
            }
            return calleDisponible;
        }

        private Calle ObtenerCalleDeUltimoCamion(DbContext contexto)
        {
            return contexto.Set<CallePorRecorrido>().Where(x => x.Calle.TipoCalle == tipoCalle 
                                                                && !x.Calle.Bloqueada
                                                                && x.Calle.FechaLLamada == null
                                                                && !x.Calle.Deshabilitada
                                                                && x.FechaEgreso == null 
                                                                && (x.CargaDeCupo.Material.Id == materialId || x.Recorrido.Material.Id == materialId))
                                                    .OrderByDescending(x => x.FechaIngeso)
                                                    .Select(x => x.Calle)
                                                    .FirstOrDefault();
        }

        private Calle ObtenerCalleDisponible(DbContext contexto)
        {
            return contexto.Set<Calle>().Where(x => x.TipoCalle == tipoCalle 
                                            && x.Material.Id == materialId
                                            && !x.Bloqueada
                                            && !x.Deshabilitada
                                            && x.FechaLLamada == null
                                            && (x.CantidadDeCamiones > contexto.Set<CallePorRecorrido>().Count(y => y.FechaEgreso == null && y.Calle.Id == x.Id)))
                                              .FirstOrDefault();
        }
    }
}
