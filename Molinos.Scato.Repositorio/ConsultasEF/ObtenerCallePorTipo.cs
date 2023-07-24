using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using System.Data.Entity;
using System.Linq;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ObtenerCallePorTipo : IConsultaEscalar<Calle>
    {
        private TipoCalle tipoCalle;

        public ObtenerCallePorTipo(TipoCalle tipoCalle)
        {
            this.tipoCalle = tipoCalle;
            
        }

        public Calle Ejecutar(DbContext contexto)
        {
            Calle calle = ObtenerCallePasoDirecto(contexto);
            return calle;
        }

        private Calle ObtenerCallePasoDirecto(DbContext contexto)
        {
            return contexto.Set<Calle>().Where(x => x.TipoCalle == tipoCalle 
                                               && !x.Deshabilitada
                                               && x.EsPasoDirecto)
                                              .FirstOrDefault();
        }
    }
}
