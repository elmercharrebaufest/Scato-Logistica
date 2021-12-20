using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using System;
using System.Data.Entity;
using System.Linq;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ObtenerSiguienteCalle : IConsultaEscalar<Calle>
    {
        private TipoCalle tipoCalle;
        private int materialid;

        public ObtenerSiguienteCalle(TipoCalle tipoCalle, int materialid)
        {
            this.tipoCalle = tipoCalle;
            this.materialid = materialid;
        }
        public Calle Ejecutar(DbContext contexto)
        {
            if (this.tipoCalle == TipoCalle.PreCalado)
            {
                var ultimoCamion = contexto.Set<CallePorRecorrido>().Where(x => x.Calle.TipoCalle == TipoCalle.Circular && (x.CargaDeCupo.Material.Id == materialid || x.Recorrido.Material.Id == materialid) && x.FechaEgreso == null).OrderBy(x => x.Id).FirstOrDefault();
                
                if(ultimoCamion != null)
                {
                    var centroInformaCircular = contexto.Set<Centro>().FirstOrDefault(x => x.Id == ultimoCamion.Calle.CentroId);
                    
                    if(centroInformaCircular != null)
                    {
                        var minutosEsperaCircular = centroInformaCircular?.MinutosEsperaCircular ?? 20;

                        if (centroInformaCircular.InformaCircular &&
                            ultimoCamion.FechaIngeso.AddMinutes(minutosEsperaCircular) <= DateTime.Now &&
                            !ultimoCamion.Calle.Bloqueada)
                        {
                            return ultimoCamion.Calle;
                        }
                    }
                }
            }

            return contexto.Set<Calle>()
                .Where(x => x.TipoCalle == tipoCalle && !x.Deshabilitada &&
                        !x.Bloqueada &&
                        contexto.Set<CallePorRecorrido>().Any(y => (y.CargaDeCupo.Material.Id == materialid || y.Recorrido.Material.Id == materialid) && y.FechaEgreso == null && y.Calle.Id == x.Id))
                .OrderBy(x => contexto.Set<CallePorRecorrido>().OrderBy(y => y.Id).FirstOrDefault(y => y.FechaEgreso == null && y.Calle.Id == x.Id).FechaIngeso)
                .FirstOrDefault();
        }
    }
}
