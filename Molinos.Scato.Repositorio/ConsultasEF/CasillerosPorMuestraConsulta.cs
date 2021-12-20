using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class CasillerosPorMuestraConsulta : IConsultaPaginada<ConsultaCasilleroMuestraDto>
    {
        private readonly ConsultaCasilleroMuestraDto filtro;
        private readonly Paginacion paginacion;

        public CasillerosPorMuestraConsulta(ConsultaCasilleroMuestraDto filtro, Paginacion paginacion)
        {
            this.filtro = filtro;
            this.paginacion = paginacion;
        }

        public ListaPaginada<ConsultaCasilleroMuestraDto> Ejecutar(DbContext contexto)
        {
            IEnumerable<ConsultaCasilleroMuestraDto> resultados = from x in contexto.Set<MicroMuestrasPorCasillero>().ToList()
                                                           where x.Casillero.Centro.Id == filtro.CentroId
                                                           && x.Muestra.NroDocumento == filtro.NumeroDocumento.Replace("-", "") && x.Muestra.Patente == filtro.Patente
                                                           && x.Muestra.TipoDocumento == filtro.TipoDocumento
                                                           group x by new { CasilleroId = x.Casillero.Id, MuestraId = x.Muestra.Id, x.Casillero.Numero, x.Muestra.NroDocumento, x.Muestra.Patente } into g
                                                           let cantMuestra = g.Select(r => r.Muestra.Id).Count().ToString()
                                                           select
                                                            new ConsultaCasilleroMuestraDto
                                                            {
                                                                NumeroDocumento = g.Key.NroDocumento,
                                                                Patente = g.Key.Patente,
                                                                NDeCasillero = g.Key.Numero,
                                                                CantMuestra = cantMuestra
                                                            };

            if (paginacion.OrdenarPor != null)
            {
                var selectorOrden = Expresiones.Propiedad<ConsultaCasilleroMuestraDto>(paginacion.OrdenarPor);
                resultados = paginacion.DireccionOrden == DirOrden.Asc
                                 ? resultados.AsQueryable().OrderBy(selectorOrden)
                                 : resultados.AsQueryable().OrderByDescending(selectorOrden);
            }
            var itemsTotales = resultados.Count();

            resultados = resultados.Skip((paginacion.Pagina - 1) * paginacion.ItemsPorPagina).Take(paginacion.ItemsPorPagina);

            return new ListaPaginada<ConsultaCasilleroMuestraDto>(resultados.ToList(), paginacion.Pagina, paginacion.ItemsPorPagina, itemsTotales);
        }
    }
}
