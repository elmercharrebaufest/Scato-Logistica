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
            var calle = ObtenerCalleCircular(contexto);
            if (calle == null && ValidarCallesLlamadas(contexto)) {
                calle = ObtenerCalle(contexto);
            }

            return calle;
        }

        private Calle ObtenerCalleCircular(DbContext contexto)
        {
            if (this.tipoCalle == TipoCalle.PreCalado)
            {
                var ultimoCamion = ObtenerUltimoCamion(contexto, true);

                if (ultimoCamion != null)
                {
                    var centroInformaCircular = contexto.Set<Centro>().FirstOrDefault(x => x.Id == ultimoCamion.Calle.CentroId);

                    if (centroInformaCircular != null)
                    {
                        var camionesLlamadosPorCalado = ObtenerCantidadCamionesLlamadosPorCalado(contexto, true);

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
            var camionesLlamadosPorCalado = ObtenerCantidadCamionesLlamadosPorCalado(contexto, false);
            var ultimoCamion = ObtenerUltimoCamion(contexto, false);
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
                               && x.FechaIngeso < fechaLimite)
                              .OrderBy(x => x.FechaIngeso)
                              .Select(q => q.Calle)
                              .FirstOrDefault();
                }
            }
            return null;
        }

        private int ObtenerCantidadCamionesLlamadosPorCalado(DbContext contexto, bool esCircular)
        {
            var query = contexto.Set<CallePorRecorrido>()
                .Where(x => x.FechaEgreso == null
                && x.Calle.TipoCalle == tipoCalle
                && x.Calle.FechaLLamada.HasValue);
            if(esCircular)
            {
                query = query.Where(x => x.Recorrido.Material.Id == materialid || x.CargaDeCupo.Material.Id == materialid);
            }
            return query.Count();
        }

        private CallePorRecorrido ObtenerUltimoCamion(DbContext contexto, bool esCircular)
        {
            var query = contexto.Set<CallePorRecorrido>()
                .Where(x => x.FechaEgreso == null
                             && x.Calle.TipoCalle == tipoCalle);
            if (esCircular)
            {
                query = query.Where(x => x.CargaDeCupo.Material.Id == materialid || x.Recorrido.Material.Id == materialid);
            }
            return query.OrderBy(x => x.Id).FirstOrDefault();
        }

        private bool ValidarCallesLlamadas(DbContext contexto)
        {
            var limiteCallesLlamadas = contexto.Set<ConfiguracionGeneral>()
                                               .Where(x => x.Pantalla.Equals("EstadoDeCallePreCalado") && x.Nombre.Equals("LimiteFilasLlamadas"))
                                               .Select(x => x.Valor)
                                               .FirstOrDefault();

            var callesLlamadas = contexto.Set<CallePorRecorrido>()
                                        .Where(x => x.FechaEgreso == null && 
                                                    x.Calle.TipoCalle == tipoCalle &&
                                                    x.Calle.FechaLLamada != null)
                                        .Select(x => x.Calle.Id)
                                        .Distinct()
                                        .Count();

            return callesLlamadas < (!string.IsNullOrEmpty(limiteCallesLlamadas) ? int.Parse(limiteCallesLlamadas) : 2);
        }
    }
}
