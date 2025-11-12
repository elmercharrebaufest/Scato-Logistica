using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using System;
using System.Data.Entity;
using System.Linq;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class PanelPagoMunicipalConsulta : IConsultaPaginada<PanelPagoMunicipalDto>
    {
        private readonly FiltroPanelPagoMunicipalDto filtro;
        private readonly Paginacion paginacion;


        public PanelPagoMunicipalConsulta(FiltroPanelPagoMunicipalDto filtro, Paginacion paginacion)
        {
            this.filtro = filtro;
            this.paginacion = paginacion;
        }

        public ListaPaginada<PanelPagoMunicipalDto> Ejecutar(DbContext contexto)
        {

            var query = from rec in contexto.Set<Recorrido>()
                        join pago in contexto.Set<PagosTasaMunicipal>()
                            on rec.InstanciaWorkflow equals pago.IdInstance into pagosGroup
                        from pago in pagosGroup.DefaultIfEmpty() // Left join
                        where

                            // Filtra por rango de fechas de ingreso
                            filtro.IngresoDesde <= rec.FechaInicio && rec.FechaInicio <= filtro.IngresoHasta
                            // Filtra por número de documento si se especifica
                            && (string.IsNullOrEmpty(filtro.NroDocumento) || rec.NumeroDocumentoIngreso == filtro.NroDocumento)
                            // Filtra por patente si se especifica
                            && (string.IsNullOrEmpty(filtro.Patente) || rec.Patente == filtro.Patente)
                            // Filtra por pago consumido si se especifica
                            && (!filtro.PagoConsumido.HasValue || (rec.PagoTasaMunicipalInformado == filtro.PagoConsumido))
                            // Filtra por workflow si se especifica
                            && (string.IsNullOrEmpty(filtro.WorkflowCodigo) || rec.Workflow.Codigo == filtro.WorkflowCodigo)
                            // Excluye recorridos rechazados
                            && rec.IngresoContingenciaPagoMunicipal
                            && !rec.Rechazado
                        select new PanelPagoMunicipalDto
                        {
                            Id = rec.Id,
                            NombreUsuario = rec.Usuario,
                            NumeroDocumento = rec.NumeroDocumentoIngreso,
                            WorkflowCodigo = rec.Workflow.Codigo,
                            WorkflowDescripcion = rec.Workflow.Descripcion,
                            CuitInterviniente = pago.CuitInterviniente,
                            Patente = rec.Patente,
                            TipoVehiculo = rec.TipoVehiculo,
                            Importe = pago != null ? (decimal?)pago.Importe : null,
                            FechaEmision = pago != null ? pago.FechaEmision : default(DateTime),
                            FechaIngreso = rec.FechaInicio,
                            NumeroRecibo = pago != null ? pago.NroRecibo : null,
                            PagoConsumido = rec.PagoTasaMunicipalInformado
                        };

            // Ordenación y paginación
            if (paginacion.OrdenarPor != null)
            {
                var selectorOrden = Expresiones.Propiedad<PanelPagoMunicipalDto>(paginacion.OrdenarPor);
                query = paginacion.DireccionOrden == DirOrden.Asc
                    ? query.OrderBy(selectorOrden)
                    : query.OrderByDescending(selectorOrden);
            }
            var itemsTotales = query.Count();
            query = query.Skip((paginacion.Pagina - 1) * paginacion.ItemsPorPagina).Take(paginacion.ItemsPorPagina);

            return new ListaPaginada<PanelPagoMunicipalDto>(query.ToList(), paginacion.Pagina, paginacion.ItemsPorPagina, itemsTotales);
        }
    }
}
