using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using System.Data.Entity;
using System.Linq;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ObtenerCalleNoGranos : IConsultaEscalar<Calle>
    {
        private TipoCalle tipoCalle;
        private int materialId;

        public ObtenerCalleNoGranos(TipoCalle tipoCalle, Material material)
        {
            this.tipoCalle = tipoCalle;
            this.materialId = material != null ? material.Id : 0;
        }

        public Calle Ejecutar(DbContext contexto)
        {
            Calle calleDisponible = ObtenerCalleLibre(contexto);

            if (calleDisponible == null)
            {
                calleDisponible = ObtenerCalleSobreasignacion(contexto);
            }
          
            return calleDisponible;
        }

        private CallePorRecorrido UltimoCamionAsignado(DbContext contexto)
        {
            return contexto.Set<CallePorRecorrido>().Where(x => x.Calle.TipoCalle == tipoCalle &&
            x.FechaEgreso == null && (x.CargaDeCupo.Material.Id == materialId || x.Recorrido.Material.Id == materialId) && (x.CargaDeCupo.Material.EsAsignableCalle || x.Recorrido.Material.EsAsignableCalle))
                                              .OrderByDescending(x => x.Id)
                                              .FirstOrDefault();
        }

        private Calle ObtenerCalleIncompletaDelUltimoCamionAsignado(DbContext contexto, int ultimoCamionAsignadoCalleId)
        {
            return contexto.Set<Calle>()
                .Where(x => x.TipoCalle == tipoCalle && !x.Deshabilitada &&
                        !x.Bloqueada && x.Id == ultimoCamionAsignadoCalleId &&
                        contexto.Set<CallePorRecorrido>().Count(y => y.FechaEgreso == null && y.Calle.Id == x.Id) < x.CantidadDeCamiones)
                .FirstOrDefault();
        }

        private Calle ObtenerSiguienteCalleVacia(DbContext contexto, CallePorRecorrido ultimoCamionAsignado)
        {
            IQueryable<Calle> calleDisponibleqry = contexto.Set<Calle>();
            if (ultimoCamionAsignado != null)
            {
                var idCalle = ultimoCamionAsignado.Calle.Id;
                calleDisponibleqry = calleDisponibleqry.Where(x => x.Id >= idCalle);
            }
            return FiltrarCalleVacia(contexto, calleDisponibleqry) ?? FiltrarCalleVacia(contexto, contexto.Set<Calle>());
        }

        private Calle FiltrarCalleVacia(DbContext contexto, IQueryable<Calle> calleDisponibleqry)
        {
            var calleDisponible = calleDisponibleqry
                                    .Where(x => x.TipoCalle == tipoCalle && x.Material.Id == materialId && !x.Deshabilitada && contexto.Set<CallePorRecorrido>()
                                    .Count(y => y.FechaEgreso == null && y.Calle.Id == x.Id) == 0 && x.Material.EsAsignableCalle)
                                    .OrderBy(x => x.Id)
                                    .FirstOrDefault();

            if(calleDisponible == null)
            {
                //para las calles que admiten cualquier material
                calleDisponible = calleDisponibleqry
                                    .Where(x => x.TipoCalle == tipoCalle && x.Material == null && !x.Deshabilitada && contexto.Set<CallePorRecorrido>()
                                    .Count(y => y.FechaEgreso == null && y.Calle.Id == x.Id) == 0)
                                    .OrderBy(x => x.Id)
                                    .FirstOrDefault();
            }

            return calleDisponible;
        }

        private Calle ObtenerSiguienteCalleIncompletaLiberada(DbContext contexto)
        {
            var calle = contexto.Set<Calle>()
                .Where(x => x.TipoCalle == tipoCalle && !x.Deshabilitada &&
                        !x.Bloqueada &&
                        contexto.Set<CallePorRecorrido>().Any(y => (y.CargaDeCupo.Material.Id == materialId || y.Recorrido.Material.Id == materialId) && y.FechaEgreso == null && y.Calle.Id == x.Id && y.UltimoDeLaFila) &&
                        contexto.Set<CallePorRecorrido>().Count(y => y.FechaEgreso == null && y.Calle.Id == x.Id) < x.CantidadDeCamiones && x.Material.EsAsignableCalle)
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();

            if (calle == null)
            {
                calle = contexto.Set<Calle>()
                .Where(x => x.TipoCalle == tipoCalle && !x.Deshabilitada &&
                        !x.Bloqueada && x.Material == null &&
                        contexto.Set<CallePorRecorrido>().Any(y => y.FechaEgreso == null && y.Calle.Id == x.Id && y.UltimoDeLaFila) &&
                        contexto.Set<CallePorRecorrido>().Count(y => y.FechaEgreso == null && y.Calle.Id == x.Id) < x.CantidadDeCamiones)
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();
            }

            return calle;
        }

        private Calle ObtenerSiguienteCalleIncompleta(DbContext contexto)
        {
            var calle = contexto.Set<Calle>()
                .Where(x => x.TipoCalle == tipoCalle && !x.Deshabilitada &&
                        !x.Bloqueada &&
                        contexto.Set<CallePorRecorrido>().Any(y => (y.CargaDeCupo.Material.Id == materialId || y.Recorrido.Material.Id == materialId) && y.FechaEgreso == null && y.Calle.Id == x.Id) &&
                        contexto.Set<CallePorRecorrido>().Count(y => y.FechaEgreso == null && y.Calle.Id == x.Id) < x.CantidadDeCamiones && x.Material.EsAsignableCalle)
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();

            if (calle == null)
            {
                calle = contexto.Set<Calle>()
                .Where(x => x.TipoCalle == tipoCalle && !x.Deshabilitada &&
                        !x.Bloqueada && x.Material == null &&
                        contexto.Set<CallePorRecorrido>().Any(y => y.FechaEgreso == null && y.Calle.Id == x.Id) &&
                        contexto.Set<CallePorRecorrido>().Count(y => y.FechaEgreso == null && y.Calle.Id == x.Id) < x.CantidadDeCamiones)
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();
            }

            return calle;
        }

        private Calle ObtenerCalleLibre(DbContext contexto)
        {
            return contexto.Set<Calle>().Where(x => x.TipoCalle == tipoCalle 
                                               && !x.Deshabilitada 
                                               && (x.Material.Id == materialId || x.Material == null) 
                                               && contexto.Set<CallePorRecorrido>().Count(y => y.FechaEgreso == null && y.Calle.Id == x.Id) < x.CantidadDeCamiones)
                                              .FirstOrDefault();
        }

        private Calle ObtenerCalleSobreasignacion(DbContext contexto)
        {
            return contexto.Set<CallePorRecorrido>().Where(y => y.Calle.TipoCalle == tipoCalle
                                                          && y.Calle.Material.Id == materialId
                                                          && y.FechaEgreso == null 
                                                          && (y.CargaDeCupo.Material.Id == materialId || y.Recorrido.Material.Id == materialId)
                                                          && !y.Calle.Deshabilitada)
                                                          .OrderByDescending(x => x.FechaIngeso)
                                                          .Select(x => x.Calle)
                                                          .FirstOrDefault();
        }
    }
}
