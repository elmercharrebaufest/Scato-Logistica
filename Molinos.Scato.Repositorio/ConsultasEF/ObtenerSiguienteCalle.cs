using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
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
            var calle = ObtenerCalleCircular(contexto);
            LoggerHelper.WriteLine("Calle Id: "+(calle != null ? calle.Id : 0));
            if (calle == null) {
                LoggerHelper.WriteLine("No es circular");
                calle = ObtenerCalle(contexto);
                LoggerHelper.WriteLine("Calle Id: " + (calle != null ? calle.Id : 0));
            }

            return calle;
        }

        private Calle ObtenerCalleCircular(DbContext contexto)
        {
            if (this.tipoCalle == TipoCalle.PreCalado)
            {
                var ultimoCamion = ObtenerUltimoCamion(contexto);

                if (ultimoCamion != null)
                {
                    var centroInformaCircular = contexto.Set<Centro>().FirstOrDefault(x => x.Id == ultimoCamion.Calle.CentroId);

                    if (centroInformaCircular != null)
                    {
                        var camionesLlamadosPorCalado = ObtenerCantidadCamionesLlamadosPorCalado(contexto);

                        if (camionesLlamadosPorCalado < (centroInformaCircular?.LimiteCamionesCalado ?? 8))
                        {
                            var minutosEsperaCircular = centroInformaCircular?.MinutosEsperaCircular ?? 20;

                            if (centroInformaCircular.InformaCircular
                                && !ultimoCamion.Calle.Bloqueada
                                && ultimoCamion.FechaIngeso.AddMinutes(minutosEsperaCircular) <= DateTime.Now)
                            {
                                return ultimoCamion.Calle;
                            }
                        }
                    }
                }
            }
            return null;
        }

        private Calle ObtenerCalle(DbContext contexto)
        {
            var camionesLlamadosPorCalado = ObtenerCantidadCamionesLlamadosPorCalado(contexto);
            var ultimoCamion = ObtenerUltimoCamion(contexto);
            if (ultimoCamion != null)
            {
                var centro = contexto.Set<Centro>().FirstOrDefault(x => x.Id == ultimoCamion.Calle.CentroId);
                if (camionesLlamadosPorCalado < (centro?.LimiteCamionesCalado ?? 8))
                {
                    //return contexto.Set<Calle>()
                    //    .Where(x => x.TipoCalle == tipoCalle && !x.Deshabilitada &&
                    //            !x.Bloqueada &&
                    //            contexto.Set<CallePorRecorrido>().Any(y => (y.CargaDeCupo.Material.Id == materialid || y.Recorrido.Material.Id == materialid) && y.FechaEgreso == null && y.Calle.Id == x.Id))
                    //    .OrderBy(x => contexto.Set<CallePorRecorrido>().OrderBy(y => y.Id).FirstOrDefault(y => y.FechaEgreso == null && y.Calle.Id == x.Id).FechaIngeso)
                    //    .FirstOrDefault();
                    var fechaLimite = DateTime.Now.AddMinutes(-(centro?.MinutosEsperaPrecalado ?? 30));

                    return contexto.Set<CallePorRecorrido>()
                              .Where(x => x.FechaEgreso == null
                               && x.Calle.Deshabilitada == false
                               && x.Calle.Bloqueada == false
                               && x.Calle.TipoCalle == tipoCalle
                               && x.FechaIngeso < fechaLimite
                               && (x.CargaDeCupo.Material.Id == materialid || x.Recorrido.Material.Id == materialid))
                              .OrderBy(x => x.FechaIngeso)
                              .Select(q => q.Calle)
                              .FirstOrDefault();
                }
            }
            return null;
        }

        private int ObtenerCantidadCamionesLlamadosPorCalado(DbContext contexto)
        {
            return contexto.Set<CallePorRecorrido>()
                .Count(x => x.FechaEgreso == null
                && x.Calle.TipoCalle == tipoCalle
                && x.Calle.FechaLLamada.HasValue &&
                (x.Recorrido.Material.Id == materialid || x.CargaDeCupo.Material.Id == materialid));
        }

        private CallePorRecorrido ObtenerUltimoCamion(DbContext contexto)
        {
            return contexto.Set<CallePorRecorrido>()
                            .Where(x => x.FechaEgreso == null
                             && x.Calle.TipoCalle == tipoCalle
                            && (x.CargaDeCupo.Material.Id == materialid || x.Recorrido.Material.Id == materialid))
                            .OrderBy(x => x.Id)
                            .FirstOrDefault();
        }
    }
}