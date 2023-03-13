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
            Calle calleDisponible = null;

            var ultimaAsignacion = contexto.Set<CallePorRecorrido>()
                .Where(x => x.Calle.TipoCalle == TipoCalle.PreBalanzaGranos && x.FechaEgreso == null)
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();

            var callesDisponibles = contexto.Set<Calle>().Where(x => x.TipoCalle == TipoCalle.PreBalanzaGranos 
                                            && x.Material.Id == materialId
                                            && !x.Bloqueada
                                            && !x.Deshabilitada
                                            && (contexto.Set<CallePorRecorrido>()
                                              .Count(y => y.FechaEgreso == null && y.Calle.Id == x.Id)) < x.CantidadDeCamiones).
                                              OrderBy(x => x.Id)
                                              .ToList();

            //busca en calle actual
            if (ultimaAsignacion != null)
            {
                var idCalle = ultimaAsignacion.Calle.Id;
                calleDisponible = callesDisponibles.FirstOrDefault(x => x.Id == idCalle) ?? callesDisponibles.FirstOrDefault(x => x.Id > idCalle);
            }

            //busca en todas las calles 
            if(calleDisponible == null)
            {
                calleDisponible = callesDisponibles.FirstOrDefault();
            }

            return calleDisponible;
        }
    }
}
