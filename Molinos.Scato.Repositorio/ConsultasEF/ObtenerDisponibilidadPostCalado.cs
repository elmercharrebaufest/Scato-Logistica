using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using System.Data.Entity;
using System.Linq;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ObtenerDisponibilidadPostCalado : IConsultaEscalar<int>
    {
        private int materialId;
        private TipoCalidad calidad;

        public ObtenerDisponibilidadPostCalado(int materialId, TipoCalidad calidad)
        {
            this.materialId = materialId;
            this.calidad = calidad;
        }
        public int Ejecutar(DbContext contexto)
        {
            var callesVacias = ListarCallesVacias(contexto, contexto.Set<Calle>());
            int disponibilidad = callesVacias.Count() > 0 ? callesVacias.Sum(x => x.CantidadDeCamiones) : 0;
            var callesIncompletas = ListarCallesIncompletas(contexto, contexto.Set<Calle>());
            var callePorRecorrido = contexto.Set<CallePorRecorrido>().Where(x => x.FechaEgreso == null && callesIncompletas.Contains(x.Calle));
            disponibilidad += callesIncompletas.Count() > 0 ? callesIncompletas.Sum(
                x => x.CantidadDeCamiones) - callePorRecorrido.Count() : 0;


            return disponibilidad;
        }

        private IQueryable<Calle> ListarCallesVacias(DbContext contexto, IQueryable<Calle> calleDisponibleqry)
        {
            return calleDisponibleqry.Where(x => x.TipoCalle == TipoCalle.PostCalado
            && !x.Deshabilitada && contexto.Set<CallePorRecorrido>().Count(y => y.FechaEgreso == null && y.Calle.Id == x.Id) == 0);
        }


        private IQueryable<Calle> ListarCallesIncompletas(DbContext contexto, IQueryable<Calle> calleDisponibleqry)
        {
            return calleDisponibleqry
                .Where(x => x.TipoCalle == TipoCalle.PostCalado && !x.Deshabilitada &&
                        contexto.Set<CallePorRecorrido>().Any(y => (y.Recorrido.CaracteristicasAnalizadasList.FirstOrDefault().Calidad == calidad) &&
                        y.Recorrido.Material.Id == materialId && y.FechaEgreso == null && y.Calle.Id == x.Id) &&
                        contexto.Set<CallePorRecorrido>().Count(y => y.FechaEgreso == null && y.Calle.Id == x.Id) < x.CantidadDeCamiones);
        }
    }
}
