using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using System.Data.Entity;
using System.Linq;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class TransmisionAVisecConsulta : IConsultaPaginada<VisecTransmisionDto>
    {
        private readonly FiltroPanelDeTransaccionesVisecDto filtro;
        private readonly Paginacion paginacion;

        public TransmisionAVisecConsulta(FiltroPanelDeTransaccionesVisecDto filtro, Paginacion paginacion)
        {
            this.filtro = filtro;
            this.paginacion = paginacion;
        }

        public ListaPaginada<VisecTransmisionDto> Ejecutar(DbContext contexto)
        {
            var query = contexto.Set<VisecTransmision>().AsQueryable().Include(x => x.VisecTransmisionMovimientos);

            if (filtro.EstadoTransmisionAVisec.HasValue)
            {
                query = query.Where(q => q.Estado == (int)filtro.EstadoTransmisionAVisec.Value);
            }

            if (filtro.FechaDesde.HasValue)
            {
                query = query.Where(q => q.FechaTransaccion >= filtro.FechaDesde.Value);
            }

            if (filtro.FechaHasta.HasValue)
            {
                var hasta = filtro.FechaHasta.Value.AddDays(1);
                query = query.Where(q => q.FechaTransaccion < hasta);
            }

            if (!string.IsNullOrEmpty(filtro.NumeroDocumento))
            {
                query = query.Where(q => q.NumeroCTG.Contains(filtro.NumeroDocumento));
            }

            if (!string.IsNullOrEmpty(paginacion.OrdenarPor))
            {
                var selectorOrden = Expresiones.Propiedad<VisecTransmision>(paginacion.OrdenarPor);
                query = paginacion.DireccionOrden == DirOrden.Asc
                    ? query.OrderBy(selectorOrden)
                    : query.OrderByDescending(selectorOrden);
            }
            else
            {
                query = query.OrderBy(q => q.Id);
            }

            var itemsTotales = query.Count();
            var paginados = query
                .Skip((paginacion.Pagina - 1) * paginacion.ItemsPorPagina)
                .Take(paginacion.ItemsPorPagina)
                .ToList();

            var resultados = paginados.Select(q => new VisecTransmisionDto
            {
                Id = q.Id,
                CUITEmpresa = q.CUITEmpresa,
                NumeroProceso = q.NumeroProceso,
                Estado = (EstadoTransmisionAVisec)q.Estado,
                FechaTransaccion = q.FechaTransaccion,
                DetalleTransaccion = q.DetalleTransaccion,
                HistorialProcesos = q.HistorialProcesos,
                FechaHoraMovimiento = q.FechaHoraMovimiento,
                FechaCPE = q.FechaCPE,
                NumeroCPE = q.NumeroCPE,
                NumeroCTG = q.NumeroCTG,
                CUITTitular = q.CUITTitular,
                NumeroRUCAOrigen = q.NumeroRUCAOrigen,
                CUITDestinatario = q.CUITDestinatario,
                CUITDestino = q.CUITDestino,
                NumeroRUCADestino = q.NumeroRUCADestino,
                Producto = q.Producto,
                Campania = q.Campania,
                PesoNetoCargaKg = q.PesoNetoCargaKg,
                DocumentosAsociados = q.VisecTransmisionMovimientos != null
                        ? string.Join(",", q.VisecTransmisionMovimientos
                            .Where(m => !string.IsNullOrEmpty(m.NumeroCTGAsignado))
                            .Select(m => m.NumeroCTGAsignado))
                        : string.Empty
            }).ToList();

            return new ListaPaginada<VisecTransmisionDto>(resultados, paginacion.Pagina, paginacion.ItemsPorPagina, itemsTotales);
        }
    }
}