using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ObtenerCalle : IConsultaEscalar<Calle>
    {
        private TipoCalle tipoCalle;
        private int materialId;
        private bool incluirBloqueadas;

        public ObtenerCalle(TipoCalle tipoCalle, Material material, bool incluirBloqueadas = false)
        {
            this.tipoCalle = tipoCalle;
            this.materialId = material != null ? material.Id : 0;
            this.incluirBloqueadas = incluirBloqueadas;
        }
        public Calle Ejecutar(DbContext contexto)
        {
            var ultimoCamionAsignado = UltimoCamionAsignado(contexto);
            Calle calleDisponible = null;
            if (ultimoCamionAsignado != null)
            {
                calleDisponible = ObtenerCalleIncompletaDelUltimoCamionAsignado(contexto, ultimoCamionAsignado.Calle.Id);
            }

            if (calleDisponible == null)
            {
                calleDisponible = ObtenerSiguienteCalleVacia(contexto, ultimoCamionAsignado);
            }

            if (calleDisponible == null)
            {
                calleDisponible = ObtenerSiguienteCalleIncompletaLiberada(contexto);
            }

            if (calleDisponible == null)
            {
                calleDisponible = ObtenerSiguienteCalleIncompleta(contexto);
            }

            return calleDisponible;
        }

        private CallePorRecorrido UltimoCamionAsignado(DbContext contexto)
        {
            return contexto.Set<CallePorRecorrido>().Where(x => x.Calle.TipoCalle == tipoCalle && 
            x.FechaEgreso == null && (x.CargaDeCupo.Material.Id == materialId || x.Recorrido.Material.Id == materialId))
                                              .OrderByDescending(x => x.Id)
                                              .FirstOrDefault();
        }

        private Calle ObtenerSiguienteCalleIncompletaLiberada(DbContext contexto)
        {
            var calle = contexto.Set<Calle>()
                .Where(x => x.TipoCalle == tipoCalle && !x.Deshabilitada &&
                        (incluirBloqueadas || !x.Bloqueada) &&
                        contexto.Set<CallePorRecorrido>().Any(y => (y.CargaDeCupo.Material.Id == materialId || y.Recorrido.Material.Id == materialId) && y.FechaEgreso == null && y.Calle.Id == x.Id && y.UltimoDeLaFila) &&
                        contexto.Set<CallePorRecorrido>().Count(y => y.FechaEgreso == null && y.Calle.Id == x.Id) < x.CantidadDeCamiones)
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();

            if(calle == null)
            {
                calle = contexto.Set<Calle>()
                .Where(x => x.TipoCalle == tipoCalle && !x.Deshabilitada &&
                        (incluirBloqueadas || !x.Bloqueada) && x.Material == null &&
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
                        (incluirBloqueadas || !x.Bloqueada) &&
                        contexto.Set<CallePorRecorrido>().Any(y => (y.CargaDeCupo.Material.Id == materialId || y.Recorrido.Material.Id == materialId) && y.FechaEgreso == null && y.Calle.Id == x.Id) &&
                        contexto.Set<CallePorRecorrido>().Count(y => y.FechaEgreso == null && y.Calle.Id == x.Id) < x.CantidadDeCamiones)
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();

            if(calle == null)
            {
                calle = contexto.Set<Calle>()
                .Where(x => x.TipoCalle == tipoCalle && !x.Deshabilitada &&
                        (incluirBloqueadas || !x.Bloqueada) && x.Material == null &&
                        contexto.Set<CallePorRecorrido>().Any(y => y.FechaEgreso == null && y.Calle.Id == x.Id) &&
                        contexto.Set<CallePorRecorrido>().Count(y => y.FechaEgreso == null && y.Calle.Id == x.Id) < x.CantidadDeCamiones)
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();
            }

            return calle;
        }

        private Calle ObtenerCalleIncompletaDelUltimoCamionAsignado(DbContext contexto, int ultimoCamionAsignadoCalleId)
        {
            return contexto.Set<Calle>()
                .Where(x => x.TipoCalle == tipoCalle && !x.Deshabilitada &&
                        (incluirBloqueadas || !x.Bloqueada) && x.Id == ultimoCamionAsignadoCalleId &&
                        //redundante ya que controlamos el material al obtener el último camion asignado
                        //contexto.Set<CallePorRecorrido>().Any(y => (y.CargaDeCupo.Material.Id == materialId || y.Recorrido.Material.Id == materialId) && y.FechaEgreso == null && y.Calle.Id == x.Id) &&
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
                                    .Count(y => y.FechaEgreso == null && y.Calle.Id == x.Id) == 0)
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
    }
}
