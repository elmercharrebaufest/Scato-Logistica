using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ListarPaginadoEmbarques : IConsultaPaginada<CargaDto>
    {
        private readonly CargaFiltroDto filtro;
        private readonly Paginacion paginacion;

        public ListarPaginadoEmbarques(CargaFiltroDto filtro, Paginacion paginacion)
        {
            this.filtro = filtro;
            this.paginacion = paginacion;
        }

        public ListaPaginada<CargaDto> Ejecutar(DbContext contexto)
        {
            ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            Expression<Func<Carga, bool>> expresionFiltro = null;

            if (filtro != null)
            {
                filtro.IdMaterial = (filtro.IdMaterial.HasValue) ? ((filtro.IdMaterial == 0) ? null : filtro.IdMaterial) : null;
                filtro.IdVapor = (filtro.IdVapor.HasValue) ? ((filtro.IdVapor == 0) ? null : filtro.IdVapor) : null;
                filtro.IdExportador = (filtro.IdExportador.HasValue) ? ((filtro.IdExportador == 0) ? null : filtro.IdExportador) : null;
                filtro.IdDestino = (filtro.IdDestino.HasValue) ? ((filtro.IdDestino == 0) ? null : filtro.IdDestino) : null;
                expresionFiltro = x => (!filtro.Id.HasValue || filtro.Id == x.Id) &&
                                      (string.IsNullOrEmpty(filtro.NumeroBalanza) || filtro.NumeroBalanza == x.NumeroBalanza) &&
                                      (!filtro.IdMaterial.HasValue || filtro.IdMaterial == x.Material.Id) &&
                                      (!filtro.IdVapor.HasValue || filtro.IdVapor == x.Vapor.Id) &&
                                      (!filtro.IdExportador.HasValue || filtro.IdExportador == x.Exportador.Id) &&
                                      (!filtro.IdDestino.HasValue || filtro.IdDestino == x.Destino.Id) &&
                                      (!filtro.IdBodega.HasValue || filtro.IdBodega == x.Bodega.Id) &&
                                      (x.FechaInicio != null);
            }

            var resultado = contexto.Set<Carga>().Where(expresionFiltro).GroupBy(
                x => new { 
                    VaporId = x.Vapor.Id, 
                    Vapor = x.Vapor.Nombre,
                    MaterialId = x.Material.Id,
                    Material = x.Material.Descripcion,
                    ExportadorId = x.Exportador.Id, 
                    Exportador = x.Exportador.Nombre,
                    DestinoId= x.Destino.Id,  
                    Destino = x.Destino.Nombre
                }).Select(x => new CargaDto() 
                { 
                    Id = x.FirstOrDefault().Id,
                    Fecha = x.FirstOrDefault().Fecha,
                    VaporId = x.Key.VaporId, 
                    Vapor = x.Key.Vapor,
                    MaterialId = x.Key.MaterialId,
                    Material = x.Key.Material,
                    ExportadorId = x.Key.ExportadorId,
                    Exportador = x.Key.Exportador,
                    DestinoId = x.Key.DestinoId,
                    Destino = x.Key.Destino,
                    PesoProgramado = x.Sum( y => y.PesoProgramado),
                    ToneladasAW = x.Sum(y => y.ToneladasAW)
                });

            if (paginacion.OrdenarPor != null)
            {
                var selectorOrden = Expresiones.Propiedad<CargaDto>(paginacion.OrdenarPor);
                resultado = paginacion.DireccionOrden == DirOrden.Asc
                                 ? resultado.OrderBy(selectorOrden)
                                 : resultado.OrderByDescending(selectorOrden);
            }

            var itemsTotales = resultado.Count();

            var resultadoPagina = resultado.Skip((paginacion.Pagina - 1) * paginacion.ItemsPorPagina).Take(paginacion.ItemsPorPagina);

            return new ListaPaginada<CargaDto>(resultadoPagina.ToList(), paginacion.Pagina, paginacion.ItemsPorPagina, itemsTotales);
        }
    }
}
