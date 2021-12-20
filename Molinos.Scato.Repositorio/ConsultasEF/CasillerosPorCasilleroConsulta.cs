using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class CasillerosPorCasilleroConsulta: IConsultaPaginada<ConsultaCasilleroDto>
    {
        private readonly ConsultaCasilleroDto filtro;
        private readonly Paginacion paginacion;

        public CasillerosPorCasilleroConsulta(ConsultaCasilleroDto filtro, Paginacion paginacion)
        {
            this.filtro = filtro;
            this.paginacion = paginacion;
        }

        public ListaPaginada<ConsultaCasilleroDto> Ejecutar(DbContext contexto)
        {
            IEnumerable<ConsultaCasilleroDto> resultados = from x in contexto.Set<MicroMuestrasPorCasillero>().ToList()
                                                           where x.Casillero.Centro.Id == filtro.CentroId
            && (Convert.ToInt64(x.Casillero.Numero.Replace("-", "").Trim()) >= Convert.ToInt64(filtro.Desde.Replace("-", "").Trim())
                && Convert.ToInt64(x.Casillero.Numero.Replace("-", "").Trim()) <= Convert.ToInt64(filtro.Hasta.Replace("-", "").Trim()))
                                                           group x by new { CasilleroId = x.Casillero.Id, MuestraId = x.Muestra.Id, x.Casillero.Numero, x.Muestra.NroDocumento, x.Muestra.Patente, x.Muestra.TipoDocumento } into g
                                                           let cantMuestra = g.Select(r => r.Muestra.Id).Count().ToString()
                                                           select
                                                               new ConsultaCasilleroDto
                                                               {
                                                                   NDeCasillero = g.Key.Numero,
                                                                   CantMuestra = cantMuestra,
                                                                   TipoDocumento = g.Key.TipoDocumento.GetAttributeValue<DisplayAttribute, string>(d => d.GetName()),
                                                                   NumeroDocumento = g.Key.NroDocumento,
                                                                   Patente = g.Key.Patente
                                                               };

            if (paginacion.OrdenarPor != null)
            {
                var selectorOrden = Expresiones.Propiedad<ConsultaCasilleroDto>(paginacion.OrdenarPor);
                resultados = paginacion.DireccionOrden == DirOrden.Asc
                                 ? resultados.AsQueryable().OrderBy(selectorOrden)
                                 : resultados.AsQueryable().OrderByDescending(selectorOrden);
            }
            var itemsTotales = resultados.Count();

            resultados = resultados.Skip((paginacion.Pagina - 1) * paginacion.ItemsPorPagina).Take(paginacion.ItemsPorPagina);

            return new ListaPaginada<ConsultaCasilleroDto>(resultados.ToList(), paginacion.Pagina, paginacion.ItemsPorPagina, itemsTotales);
        }
    }
}
