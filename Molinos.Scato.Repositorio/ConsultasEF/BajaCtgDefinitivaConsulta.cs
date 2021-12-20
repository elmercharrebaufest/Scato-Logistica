using System;
using System.Data.Entity;
using System.Data.Objects;
using System.Linq;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class BajaCtgDefinitivaConsulta : IConsultaPaginada<TransmisionBajaCtgDefinitivaDto>
    {
        private readonly FiltroPanelDeBajaCtgDefinitivaDto filtro;
        private readonly Paginacion paginacion;


        public BajaCtgDefinitivaConsulta(FiltroPanelDeBajaCtgDefinitivaDto filtro, Paginacion paginacion)
        {
            this.filtro = filtro;
            this.paginacion = paginacion;
        }

        public ListaPaginada<TransmisionBajaCtgDefinitivaDto> Ejecutar(DbContext contexto)
        {
            var previos = from tra in contexto.Set<BajaCTG>()
                          join rec in contexto.Set<Recorrido>() on tra.WorkflowId equals rec.InstanciaWorkflow
                          where filtro.CentroId == rec.Centro.Id &&
                          (!filtro.EstadoTransmisionCTG.HasValue ||
                          (filtro.EstadoTransmisionCTG == EstadoTransmisionCTG.Correcto
                                && tra.CodigoDeBaja != null && tra.CodigoDeBajaDefinitivo != null)
                                ||
                                (filtro.EstadoTransmisionCTG == EstadoTransmisionCTG.ErrorBaja && tra.CodigoDeBaja == null)
                                ||
                                (filtro.EstadoTransmisionCTG == EstadoTransmisionCTG.ErrorBajaDefinitiva
                                && (tra.CodigoDeBajaDefinitivo == null && rec.Terminado)))
                            && !rec.Rechazado
                            && ((DateTime)EntityFunctions.AddHours(tra.Fecha, -3) >= filtro.FechaDesde && (DateTime)EntityFunctions.AddHours(tra.Fecha, -3) <= filtro.FechaHasta) 
                            && (filtro.NumeroDocumentoIngreso == "" || rec.NumeroDocumentoIngreso == filtro.NumeroDocumentoIngreso)
                            && (filtro.Patente == "" || rec.Patente == filtro.Patente) && (!filtro.TipoDocumentoIngreso.HasValue || filtro.TipoDocumentoIngreso == rec.TipoDocumentoIngreso)
                            group new { tra, rec } by tra.WorkflowId into g
                            select new 
                            {
                                bajas = g.FirstOrDefault(x => x.tra.Id == g.Max( y => y.tra.Id)),
                                                                         
                            };
            IQueryable<TransmisionBajaCtgDefinitivaDto> resultados = previos.Select( x => new TransmisionBajaCtgDefinitivaDto
            {
                Id = x.bajas.tra.Id,
                EstadoCtg = (x.bajas.tra.CodigoDeBaja == null) ? EstadoTransmisionASap.Error : EstadoTransmisionASap.Correcto,
                EstadoCtgDefinitivo = (x.bajas.tra.CodigoDeBajaDefinitivo == null) ? (x.bajas.rec.Terminado ? EstadoTransmisionASap.Error : EstadoTransmisionASap.Pendiente) : EstadoTransmisionASap.Correcto,
                Fecha = (DateTime)EntityFunctions.AddHours(x.bajas.tra.Fecha, -3),
                InstanciaWorkflow = x.bajas.tra.WorkflowId,
                MensajeError = (x.bajas.tra.CodigoDeBaja == null || x.bajas.tra.CodigoDeBajaDefinitivo == null) ? contexto.Set<ControlRecorrido>().Where(y => y.WorkflowInstanceId == x.bajas.tra.WorkflowId && (y.Actividad == "Baja CTG Definitivo")).OrderByDescending(y => y.Id).FirstOrDefault().Comentario : "",
                NumeroDocumento = x.bajas.rec.NumeroDocumentoIngreso,
                Patente = x.bajas.rec.Patente,
                TipoDocumentoIngreso = x.bajas.rec.TipoDocumentoIngreso,
            });

            //resultados = (IQueryable<TransmisionBajaCtgDefinitivaDto>)resultados.GroupBy(x => x.InstanciaWorkflow).Select(x => x.Key);

            if (paginacion.OrdenarPor != null)
            {
                var selectorOrden = Expresiones.Propiedad<TransmisionBajaCtgDefinitivaDto>(paginacion.OrdenarPor);
                resultados = paginacion.DireccionOrden == DirOrden.Asc
                                 ? resultados.OrderBy(selectorOrden)
                                 : resultados.OrderByDescending(selectorOrden);
            }
            var itemsTotales = resultados.Count();

            resultados = resultados.Skip((paginacion.Pagina - 1) * paginacion.ItemsPorPagina).Take(paginacion.ItemsPorPagina);

            return new ListaPaginada<TransmisionBajaCtgDefinitivaDto>(resultados.ToList(), paginacion.Pagina, paginacion.ItemsPorPagina, itemsTotales);
        }
    }
}
