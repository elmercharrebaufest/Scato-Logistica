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
    public class LiberacionDeCasillerosConsulta : IConsultaPaginada<LiberacionDeCasillerosDto>
    {
        private readonly LiberacionDeCasillerosDto filtro;
        private readonly Paginacion paginacion;

        public LiberacionDeCasillerosConsulta(LiberacionDeCasillerosDto filtro, Paginacion paginacion)
        {
            this.filtro = filtro;
            this.paginacion = paginacion;
        }

        public ListaPaginada<LiberacionDeCasillerosDto> Ejecutar(DbContext contexto)
        {
            IEnumerable<LiberacionDeCasillerosDto> resultados = from x in contexto.Set<MicroMuestrasPorCasillero>().ToList()
                                                                where (filtro.CentroId == 0 || x.Casillero.Centro.Id == filtro.CentroId)
                 && (Convert.ToInt64(x.Casillero.Numero.Replace("-", "").Trim()) >= Convert.ToInt64(filtro.Desde.Replace("-", "").Trim())
                     && Convert.ToInt64(x.Casillero.Numero.Replace("-", "").Trim()) <= Convert.ToInt64(filtro.Hasta.Replace("-", "").Trim()))
                  && (string.IsNullOrEmpty(filtro.DiasDeAntiguedad) || x.Fecha <= (DateTime.Now.AddDays(-Convert.ToInt32(filtro.DiasDeAntiguedad))))
                  && (string.IsNullOrEmpty(filtro.Patente) || x.Muestra.Patente == filtro.Patente)
                  && (string.IsNullOrEmpty(filtro.NumeroDocumento) || x.Muestra.NroDocumento == filtro.NumeroDocumento.Replace("-", "").Trim())
                  && (!filtro.TipoDocumento.HasValue || x.Muestra.TipoDocumento == filtro.TipoDocumento.Value)

                            select
                                new LiberacionDeCasillerosDto
                                    {
                                        Id = x.Id,
                                        NDeCasillero = x.Casillero.Numero,
                                        TipoDocumentoIngreso = x.Muestra.TipoDocumento.GetAttributeValue<DisplayAttribute, string>(d => d.GetName()),
                                        NumeroDocumento = x.Muestra.NroDocumento,
                                        Patente = x.Muestra.Patente
                                    };

            if (paginacion.OrdenarPor != null)
            {
                var selectorOrden = Expresiones.Propiedad<LiberacionDeCasillerosDto>(paginacion.OrdenarPor);
                resultados = paginacion.DireccionOrden == DirOrden.Asc
                                 ? resultados.AsQueryable().OrderBy(selectorOrden)
                                 : resultados.AsQueryable().OrderByDescending(selectorOrden);
            }
            var itemsTotales = resultados.Count();

            resultados = resultados.Skip((paginacion.Pagina - 1) * paginacion.ItemsPorPagina).Take(paginacion.ItemsPorPagina);

            return new ListaPaginada<LiberacionDeCasillerosDto>(resultados.ToList(), paginacion.Pagina, paginacion.ItemsPorPagina, itemsTotales);
        }
    }
}
