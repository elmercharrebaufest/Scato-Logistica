using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class TransmisionASapConsulta : IConsultaPaginada<TransmisionASapDto>
    {
        private readonly FiltroPanelDeTransaccionesSapDto filtro;
        private readonly Paginacion paginacion;


        public TransmisionASapConsulta(FiltroPanelDeTransaccionesSapDto filtro, Paginacion paginacion)
        {
            this.filtro = filtro;
            this.paginacion = paginacion;
        }

        public ListaPaginada<TransmisionASapDto> Ejecutar(DbContext contexto)
        {
            var monsanto = new List<FuncionSAP>
                {
                    FuncionSAP.MuestreoPesajeTransporteAutomotorRegistro,
                    FuncionSAP.MuestreoPesajeVagonFerroviarioRegistro,
                    FuncionSAP.CartaPorteTransporteAutomotorRegistro,
                    FuncionSAP.CartaPorteVagonFerroviarioRegistro
                };

            IQueryable<TransmisionASapDto> resultados = from tra in contexto.Set<TransmisionASap>()
                                                        join rec in contexto.Set<Recorrido>() on tra.InstanciaWorkflow equals rec.InstanciaWorkflow
                                                        where filtro.CentroId == rec.Centro.Id && (!filtro.EstadoTransmisionASap.HasValue || filtro.EstadoTransmisionASap == tra.Estado)
                                                        && (tra.Fecha >= filtro.FechaDesde && tra.Fecha <= filtro.FechaHasta) && rec.NumeroDocumentoIngreso.Contains(filtro.NumeroDocumentoIngreso)
                                                        && rec.Patente.Contains(filtro.Patente) && (!filtro.TipoDocumentoIngreso.HasValue || filtro.TipoDocumentoIngreso == rec.TipoDocumentoIngreso)
                                                        && ((filtro.TipoDeServicio == TipoDeServicio.Sap && !monsanto.Contains(tra.FuncionSap) && tra.FuncionSap != FuncionSAP.InformarCupo ) || (filtro.TipoDeServicio == TipoDeServicio.Monsanto && monsanto.Contains(tra.FuncionSap))
                                                        || (filtro.TipoDeServicio == TipoDeServicio.Cupo && tra.FuncionSap == FuncionSAP.InformarCupo))
                            select
                                new TransmisionASapDto
                                    {
                                        Id = tra.Id,
                                        Estado = tra.Estado,
                                        Fecha = tra.Fecha,
                                        FuncionSap = tra.FuncionSap,
                                        InstanciaWorkflow = tra.InstanciaWorkflow,
                                        MensajeError = tra.MensajeError,
                                        NumeroDocumento = rec.NumeroDocumentoIngreso,
                                        Patente = rec.Patente,
                                        TipoDocumentoIngreso = rec.TipoDocumentoIngreso
                                    };
            if (paginacion.OrdenarPor != null)
            {
                var selectorOrden = Expresiones.Propiedad<TransmisionASapDto>(paginacion.OrdenarPor);
                resultados = paginacion.DireccionOrden == DirOrden.Asc
                                 ? resultados.OrderBy(selectorOrden)
                                 : resultados.OrderByDescending(selectorOrden);
            }
            var itemsTotales = resultados.Count();

            resultados = resultados.Skip((paginacion.Pagina - 1) * paginacion.ItemsPorPagina).Take(paginacion.ItemsPorPagina);

            return new ListaPaginada<TransmisionASapDto>(resultados.ToList(), paginacion.Pagina, paginacion.ItemsPorPagina, itemsTotales);
        }
    }
}
