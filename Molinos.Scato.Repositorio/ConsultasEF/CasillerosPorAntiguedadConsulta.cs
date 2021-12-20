using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class CasillerosPorAntiguedadConsulta : IConsultaPaginada<ConsultaCasilleroAntiguedadDto>
    {
        private readonly ConsultaCasilleroAntiguedadDto filtro;
        private readonly Paginacion paginacion;

        public CasillerosPorAntiguedadConsulta(ConsultaCasilleroAntiguedadDto filtro, Paginacion paginacion)
        {
            this.filtro = filtro;
            this.paginacion = paginacion;
        }

        public ListaPaginada<ConsultaCasilleroAntiguedadDto> Ejecutar(DbContext contexto)
        {
            var casillerosAgrupados = from x in contexto.Set<MicroMuestrasPorCasillero>().ToList()
                                      group x by new { DescCentro = x.Casillero.Centro.Descripcion, CasilleroId = x.Casillero.Id } into g
                              let maxFecha = g.Max(r => r.Fecha)
                              select new
                              {
                                  g.Key.DescCentro,
                                  g.Key.CasilleroId,
                                  maxFecha
                              };

            var casilleros = from x in contexto.Set<Casillero>().ToList()
                             select x;

            IEnumerable<ConsultaCasilleroAntiguedadDto> resultados = from desc in casillerosAgrupados.Select(x => x.DescCentro).Distinct()
                                                           select
                                                               new ConsultaCasilleroAntiguedadDto
                                                               {
                                                                   Centro = desc,
                                                                   OcupadosMas = String.Concat(casillerosAgrupados.Count(c => c.DescCentro == desc && c.maxFecha <= DateTime.Now.AddDays(-Convert.ToInt32(filtro.DiasDeAntiguedad))).ToString(),
                                                                   " (", 
                                                                   (casillerosAgrupados.Count(c => c.DescCentro == desc && c.maxFecha <= DateTime.Now.AddDays(-Convert.ToInt32(filtro.DiasDeAntiguedad))) * 100 / (casilleros.Count(c => c.Centro.Descripcion == desc))).ToString(),
                                                                   "%)"),
                                                                   OcupadosMenos = String.Concat(casillerosAgrupados.Count(c => c.DescCentro == desc && c.maxFecha >= DateTime.Now.AddDays(-Convert.ToInt32(filtro.DiasDeAntiguedad))).ToString(),
                                                                   " (", 
                                                                   (casillerosAgrupados.Count(c => c.DescCentro == desc && c.maxFecha >= DateTime.Now.AddDays(-Convert.ToInt32(filtro.DiasDeAntiguedad))) * 100 / (casilleros.Count(c => c.Centro.Descripcion == desc))).ToString(),
                                                                   "%)"),
                                                                   Libres = (casilleros.Count(c => c.Centro.Descripcion == desc) - casillerosAgrupados.Count(c => c.DescCentro == desc)).ToString()
                                                               };


            if (paginacion.OrdenarPor != null)
            {
                var selectorOrden = Expresiones.Propiedad<ConsultaCasilleroAntiguedadDto>(paginacion.OrdenarPor);
                resultados = paginacion.DireccionOrden == DirOrden.Asc
                                 ? resultados.AsQueryable().OrderBy(selectorOrden)
                                 : resultados.AsQueryable().OrderByDescending(selectorOrden);
            }
            var itemsTotales = resultados.Count();

            resultados = resultados.Skip((paginacion.Pagina - 1) * paginacion.ItemsPorPagina).Take(paginacion.ItemsPorPagina);

            return new ListaPaginada<ConsultaCasilleroAntiguedadDto>(resultados.ToList(), paginacion.Pagina, paginacion.ItemsPorPagina, itemsTotales);
        }
    }
}
