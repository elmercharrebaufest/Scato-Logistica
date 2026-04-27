using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using static Molinos.Scato.Dominio.Constantes.MOAPay;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ListarImpresiones : IConsultaPaginada<ImpresionDto>
    {
        private readonly Paginacion paginacion;
        private readonly TipoDocumentoIngreso? tipodoc;
        private readonly string numerodoc;
        private readonly string patente;
        private readonly TipoImpresion? tipoImpresion;
        private readonly int centroId;

        public ListarImpresiones(TipoDocumentoIngreso? tipodoc, string numerodoc, string patente, TipoImpresion? tipoImpresion, Paginacion paginacion, int centroId)
        {
            this.paginacion = paginacion;
            this.numerodoc = numerodoc;
            this.tipodoc = tipodoc;
            this.patente = patente;
            this.tipoImpresion = tipoImpresion;
            this.centroId = centroId;
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
                            && (centroId == null || rec.Centro.Id == centroId)


                                  select new ImpresionDto
                            {
                                Id = impresion.Id,
                                FechaImpresion = impresion.FechaImpresion,
                                TipoImpresion = impresion.TipoImpresion,
                                Patente = impresion.Patente,
                                Eliminada = impresion.Eliminada,
                                Ctg = null,
                                CtgDG = null,
                                CtgDocumentoCpe = rec.NumeroDocumentoIngreso
                             };

                var impresionCPE = ObtenerImpresionCartaPorteElectronica(contexto);
                
                if(impresionCPE != null)
                resultQuery = resultQuery.Union(impresionCPE);
                
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

                var pagina = resultQuery
                            .Skip((paginacion.Pagina - 1) * paginacion.ItemsPorPagina)
                            .Take(paginacion.ItemsPorPagina)
                            .ToList();

                foreach (var x in pagina)
                {
                    if (x.TipoImpresion == TipoImpresion.CartaDePorteElectronica)
                    {
                        x.Ctg = long.TryParse(x.CtgDocumentoCpe, out var ctg)
                            ? ctg
                            : (long?)null;
                    }
                }

                return new ListaPaginada<ImpresionDto>(pagina, paginacion.Pagina, paginacion.ItemsPorPagina, itemsTotales);
            }
            catch (Exception ex)
            {
                return new ListaPaginada<ImpresionDto>(new List<ImpresionDto>(), paginacion.Pagina, paginacion.ItemsPorPagina, 0);
            }
        }

        private IQueryable<ImpresionDto> ObtenerImpresionCartaPorteElectronica(DbContext contexto)
        { 
            bool filtroValido =
                (tipodoc is null || tipodoc == TipoDocumentoIngreso.CartaPorte) &&
                (tipoImpresion is null || tipoImpresion == TipoImpresion.CartaDePorteElectronica) &&
                (!string.IsNullOrEmpty(numerodoc) || !string.IsNullOrEmpty(patente));

            if (!filtroValido)
                return null;

            var consulta = contexto.Set<DocumentoPorRecorrido>()
                .Where(d => d.Tipo == TipoImpresion.CartaDePorteElectronica)
                .Join(
                    contexto.Set<Recorrido>(),
                    d => d.RecorridoId,
                    r => r.Id,
                    (d, r) => new { d, r }
                );

            if (!string.IsNullOrEmpty(numerodoc))
            {
                consulta = consulta.Where(x => x.r.NumeroDocumentoIngreso == numerodoc);
            }
            else
            {
                consulta = consulta.Where(x => x.r.Patente == patente);
            }

            return consulta.Select(x => new ImpresionDto
            {
                Id = 0,
                FechaImpresion = x.d.FechaDeGuardado,
                TipoImpresion = TipoImpresion.CartaDePorteElectronica,
                Patente = x.r.Patente,
                Eliminada = false,
                Ctg = 0,
                CtgDG = null,
                CtgDocumentoCpe = x.r.NumeroDocumentoIngreso
            });
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
                    CtgDocumentoCpe = null,
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
                                            CtgDocumentoCpe = null,
                                        });
                return impresionCPE;
            }
            return null;
        }
    }
}
