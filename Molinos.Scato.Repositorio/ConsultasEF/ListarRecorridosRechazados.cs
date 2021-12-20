using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ListarRecorridosRechazados : IConsultaPaginada<InstanciaWorkflowDto>
    {
        private readonly FiltroListaDeWorkflowsDto filtro;
        private readonly Paginacion paginacion;


        public ListarRecorridosRechazados(FiltroListaDeWorkflowsDto filtro, Paginacion paginacion )
        {
            this.filtro = filtro;
            this.paginacion = paginacion;
        }

        public virtual ListaPaginada<InstanciaWorkflowDto> Ejecutar(DbContext contexto)
        {
            var date = DateTime.Now.AddDays(-3); //se requiere que se muestren los camiones rechazados de las ultimas 72 horas
            ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var resultado = from r in contexto.Set<Recorrido>()      
                            where
                            (!r.Terminado || (r.FechaEgreso.HasValue && r.FechaEgreso > date)) && r.Rechazado && filtro.CentroId == r.Centro.Id 
                            &&(filtro.MaterialId == r.Material.Id || filtro.MaterialId == null)
                            &&(filtro.Patente == r.Patente || filtro.Patente == null)
                            &&(filtro.NumeroDocumentoDeIngreso == r.NumeroDocumentoIngreso || filtro.NumeroDocumentoDeIngreso == null)
                                select
                                    new InstanciaWorkflowDto
                                        {
                                            Rechazado = r.Rechazado,
                                            Id = r.InstanciaWorkflow,
                                            NumeroDocumentoDeIngreso = r.NumeroDocumentoIngreso,
                                            Patente = r.Patente,
                                            RecorridoId = r.Id,
                                            Material = r.Material.Descripcion,
                                            Transportista = r.Transportista.RazonSocial,
                                            FechaCreacion = r.FechaInicio,
                                            TipoDocumentoDeIngreso = r.TipoDocumentoIngreso,
                                            NumeroDeTarjeta = r.TarjetaDeAcceso,
                                            Proveedor = r.Vehiculo == null ? "" : (
                                                (r.Vehiculo.CartaPorte.Destinatario != null && r.Vehiculo.CartaPorte.Destinatario.CodigoSap != filtro.CodigoSapMolinos) ? r.Vehiculo.CartaPorte.Destinatario.Descripcion :
                                                (r.Vehiculo.CartaPorte.Corredor != null ? r.Vehiculo.CartaPorte.Corredor.Descripcion :
                                                (r.Vehiculo.CartaPorte.CorredorVendedor != null ? r.Vehiculo.CartaPorte.CorredorVendedor.Descripcion :
                                                (r.Vehiculo.CartaPorte.RtteComercial != null ? r.Vehiculo.CartaPorte.RtteComercial.Descripcion :
                                                 r.Vehiculo.CartaPorte.TitularCartaPorte != null ? r.Vehiculo.CartaPorte.TitularCartaPorte.Descripcion : "")))),
                                            FechaCalado = r.Calado != null && r.Calado.FechaCreacion.HasValue ? r.Calado.FechaCreacion.Value : DateTime.MinValue,
                                            TieneEntregador = r.Vehiculo != null && r.Vehiculo.CartaPorte.Entregador != null? r.Vehiculo.CartaPorte.Entregador.RazonSocial : "No",
                                            Terminado = r.Terminado
                                    };
            
            if (paginacion.OrdenarPor != null)
            {
                var selectorOrden = Expresiones.Propiedad<InstanciaWorkflowDto>(paginacion.OrdenarPor);
                resultado = paginacion.DireccionOrden == DirOrden.Asc
                                 ? resultado.OrderBy(selectorOrden)
                                 : resultado.OrderByDescending(selectorOrden);
            }
            var itemsTotales = resultado.Count();

            var resultadoMaterializado = resultado.Skip((paginacion.Pagina - 1) * paginacion.ItemsPorPagina).Take(paginacion.ItemsPorPagina).ToList();
            var instancias = resultadoMaterializado.Select(x => x.Id).ToList();
            
            var motivos = from cr in contexto.Set<ControlRecorrido>() 
                                join motivo in contexto.Set<Motivo>() on cr.Mensaje equals motivo.Descripcion
                                where instancias.Contains(cr.WorkflowInstanceId)
                          select new { cr.Mensaje, cr.Comentario, cr.WorkflowInstanceId, cr.ActividadXaml, cr.Actividad };

            var proximasAcciones = (from y in contexto.Set<LogActividad>()
                where instancias.Contains(y.WorkflowInstanceId)
                select y).GroupBy(x => x.WorkflowInstanceId)
                .Select(g => g.OrderByDescending(c => c.Id).FirstOrDefault()).Select(x => new { x.WorkflowInstanceId, x.ActividadXaml});
            
            foreach (var recorrido in resultadoMaterializado)
            {
                var motivo = motivos.FirstOrDefault(x => x.WorkflowInstanceId == recorrido.Id);
                if (motivo != null)
                {
                    recorrido.MotivoDeRechazo = motivo.Mensaje + ": " + motivo.Comentario;
                    recorrido.RechazoAccion = string.IsNullOrEmpty(motivo.ActividadXaml) ? motivo.Actividad : motivo.ActividadXaml;
                }

                var accion = proximasAcciones.FirstOrDefault(x => x.WorkflowInstanceId == recorrido.Id);
                if (accion != null)
                {
                    recorrido.ProximaAccion = accion.ActividadXaml;
                }
            }
            
            return new ListaPaginada<InstanciaWorkflowDto>(resultadoMaterializado, paginacion.Pagina, paginacion.ItemsPorPagina, itemsTotales);
        }
    }
}
