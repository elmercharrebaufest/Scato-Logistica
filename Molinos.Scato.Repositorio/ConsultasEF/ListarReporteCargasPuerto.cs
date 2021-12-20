using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ListarReporteCargasPuerto : IConsultaPaginada<ReportePesadaDto>
    {
        private readonly DateTime inicio;
        private readonly DateTime fin;
        private Paginacion paginacion;
        private string tipo;

        public ListarReporteCargasPuerto(DateTime inicio, DateTime fin, string tipo, Paginacion paginacion = null)
        {
            this.inicio = inicio;
            this.fin = fin;
            this.paginacion = paginacion;
            this.tipo = tipo;
        }

        public ListaPaginada<ReportePesadaDto> Ejecutar(DbContext contexto)
        {

            ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var reporteCargas = (from balanzadas in contexto.Set<Balanzada>()
                                 join carga in contexto.Set<Carga>() on
                                            new { key1 = balanzadas.CargaInicial_Id, key2 = balanzadas.CargaInicial_NumeroBalanza } equals new { key1 = carga.Id, key2 = carga.NumeroBalanza }

                                 where balanzadas.CargaInicial.Fecha >= inicio && balanzadas.CargaInicial.Fecha <= fin
                                 group new { balanzadas } by carga into g
                                 select new ReportePesadaDto()
                                 {
                                     Fecha = g.Key.Fecha,
                                     Bodega = g.Key.Bodega.Nombre,
                                     Commodity = g.Key.Material.Descripcion,
                                     Destino = g.Key.Destino.Nombre,
                                     Exportador = g.Key.Exportador.Nombre,
                                     IdCarga = g.Key.Id,
                                     NumeroBalanza = g.Key.NumeroBalanza,
                                     PesoProgramado = g.Key.PesoProgramado,
                                     TotalEmbarcado = (tipo == "online") ? g.Sum(x => x.balanzadas.PesoNeto) : g.Key.CargaOpuesta.ToneladasAW,
                                     Vapor = g.Key.Vapor.Nombre
                                 }
                                 );


            if (paginacion.OrdenarPor != null)
            {
                var selectorOrden = Expresiones.Propiedad<ReportePesadaDto>(paginacion.OrdenarPor);
                reporteCargas = paginacion.DireccionOrden == DirOrden.Asc
                                 ? reporteCargas.AsQueryable().OrderBy(selectorOrden)
                                 : reporteCargas.AsQueryable().OrderByDescending(selectorOrden);
            }
            var itemsTotales = reporteCargas.Count();

            reporteCargas = reporteCargas.Skip((paginacion.Pagina - 1) * paginacion.ItemsPorPagina).Take(paginacion.ItemsPorPagina);

            return new ListaPaginada<ReportePesadaDto>(reporteCargas.ToList(), paginacion.Pagina, paginacion.ItemsPorPagina, itemsTotales);
        }
    }
}
