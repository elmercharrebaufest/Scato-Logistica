using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ListarImpresiones : IConsultaPaginada<ImpresionDto>
    {
        private readonly Paginacion paginacion;
        private readonly TipoDocumentoIngreso? tipodoc;
        private readonly string numerodoc;
        private readonly string patente;
        private readonly TipoImpresion? tipoImpresion;

        public ListarImpresiones(TipoDocumentoIngreso? tipodoc, string numerodoc, string patente, TipoImpresion? tipoImpresion, Paginacion paginacion)
        {
            this.paginacion = paginacion;
            this.numerodoc = numerodoc;
            this.tipodoc = tipodoc;
            this.patente = patente;
            this.tipoImpresion = tipoImpresion;
        }

        public ListaPaginada<ImpresionDto> Ejecutar(DbContext contexto)
        {
            try
            {
                ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

                var resultQuery = from impresion in contexto.Set<Impresion>()
                            join rec in contexto.Set<Recorrido>() on impresion.WorkflowId equals rec.InstanciaWorkflow
                            where !impresion.Eliminada
                            && (string.IsNullOrEmpty(numerodoc) || rec.NumeroDocumentoIngreso == numerodoc)
                            && (string.IsNullOrEmpty(patente) || rec.Patente == patente)
                            && (tipodoc == null || rec.TipoDocumentoIngreso == tipodoc)
                            && (tipoImpresion == null || impresion.TipoImpresion == tipoImpresion)


                            select new ImpresionDto
                            {
                                Id = impresion.Id,
                                FechaImpresion = impresion.FechaImpresion,
                                TipoImpresion = impresion.TipoImpresion,
                                Patente = impresion.Patente,
                                Eliminada = impresion.Eliminada,
                                Ctg = null,
                                CtgDG = null,
                            };

                var impresionCPE = ObtenerImpresionCartaPorteElectronica(contexto);
                if (impresionCPE != null && impresionCPE.Any())
                {
                    resultQuery = resultQuery.Union(impresionCPE);
                }

                var impresionCPEDG = ObtenerImpresionCartaPorteElectronicaDerivadoGranario(contexto);
                if (impresionCPEDG != null && impresionCPEDG.Any())
                {
                    resultQuery = resultQuery.Union(impresionCPEDG);
                }

                var itemsTotales = resultQuery.Count();

                if (paginacion.OrdenarPor != null)
                {
                    var selectorOrden = Expresiones.Propiedad<ImpresionDto>(paginacion.OrdenarPor);
                    resultQuery = paginacion.DireccionOrden == DirOrden.Asc
                                     ? resultQuery.OrderBy(selectorOrden)
                                     : resultQuery.OrderByDescending(selectorOrden);
                }

                resultQuery = resultQuery.Skip((paginacion.Pagina - 1) * paginacion.ItemsPorPagina).Take(paginacion.ItemsPorPagina);
                return new ListaPaginada<ImpresionDto>(resultQuery.ToList(), paginacion.Pagina, paginacion.ItemsPorPagina, itemsTotales);
            }
            catch (Exception ex)
            {
                return new ListaPaginada<ImpresionDto>(new List<ImpresionDto>(), paginacion.Pagina, paginacion.ItemsPorPagina, 0);
            }
        }

        private IQueryable<ImpresionDto> ObtenerImpresionCartaPorteElectronica(DbContext contexto)
        {
            if ((!string.IsNullOrEmpty(numerodoc) && (tipodoc is null || TipoDocumentoIngreso.CartaPorte == tipodoc) && (tipoImpresion is null || TipoImpresion.CartaDePorteElectronica == tipoImpresion))
                || (!string.IsNullOrEmpty(patente) && (tipodoc is null || TipoDocumentoIngreso.CartaPorte == tipodoc) && (tipoImpresion is null || TipoImpresion.CartaDePorteElectronica == tipoImpresion)))
            {
                IQueryable<CartaPorteElectronica> consulta;

                if (!string.IsNullOrEmpty(numerodoc))
                {
                    var nroCTG = long.Parse(numerodoc);
                    consulta = contexto.Set<CartaPorteElectronica>()
                        .Where(q => q.NroCTG == nroCTG && q.Pdf != null);
                }
                else // Si patente tiene valor, buscar por Dominio
                {
                    consulta = contexto.Set<CartaPorteElectronica>()
                        .Where(q => (q.Dominio.StartsWith(patente + ",") || q.Dominio == patente) && q.Pdf != null);
                }

                var impresionCPE = consulta.Select(q => new ImpresionDto()
                {
                    Id = 0,
                    FechaImpresion = q.FechaEmision ?? DateTime.Now,
                    TipoImpresion = TipoImpresion.CartaDePorteElectronica,
                    Patente = q.Dominio.Contains(",") ? q.Dominio.Substring(0, q.Dominio.IndexOf(",")) : q.Dominio,
                    Eliminada = false,
                    Ctg = q.NroCTG,
                    CtgDG = null,
                });
                return impresionCPE;
            }
            return null;
        }

        private IQueryable<ImpresionDto> ObtenerImpresionCartaPorteElectronicaDerivadoGranario(DbContext contexto)
        {
            if (!string.IsNullOrEmpty(numerodoc) && (tipodoc is null || TipoDocumentoIngreso.OrdenCargaFas == tipodoc || TipoDocumentoIngreso.OrdenCargaInterna == tipodoc || TipoDocumentoIngreso.OrdenCargaInternaFason == tipodoc) && (tipoImpresion is null || TipoImpresion.CartaPorteElectronicaDerivadoGranario == tipoImpresion))
            {
                var impresionCPE = contexto.Set<CartaPorteDerivadoGranario>()
                                        .Where(q => q.Recorrido.NumeroDocumentoIngreso == numerodoc && !string.IsNullOrEmpty(q.RutaFotoCPEDG))
                                        .Select(q => new ImpresionDto()
                {
                    Id = 0,
                    FechaImpresion = q.FechaEmision ?? DateTime.Now,
                    TipoImpresion = TipoImpresion.CartaPorteElectronicaDerivadoGranario,
                    Patente = q.Recorrido.Patente,
                    Eliminada = false,
                    Ctg = null,
                    CtgDG = q.NroCTG,
                });
                return impresionCPE;
            }

            if (!string.IsNullOrEmpty(patente) && (tipodoc is null || TipoDocumentoIngreso.OrdenCargaFas == tipodoc || TipoDocumentoIngreso.OrdenCargaInterna == tipodoc || TipoDocumentoIngreso.OrdenCargaInternaFason == tipodoc) && (tipoImpresion is null || TipoImpresion.CartaPorteElectronicaDerivadoGranario == tipoImpresion))
            {
                var impresionCPE = contexto.Set<CartaPorteDerivadoGranario>()
                                        .Where(q => q.Recorrido.Patente == patente && !string.IsNullOrEmpty(q.RutaFotoCPEDG))
                                        .Select(q => new ImpresionDto()
                                        {
                                            Id = 0,
                                            FechaImpresion = q.FechaEmision ?? DateTime.Now,
                                            TipoImpresion = TipoImpresion.CartaPorteElectronicaDerivadoGranario,
                                            Patente = q.Recorrido.Patente,
                                            Eliminada = false,
                                            Ctg = null,
                                            CtgDG = q.NroCTG,
                                        });
                return impresionCPE;
            }
            return null;
        }
    }
}
