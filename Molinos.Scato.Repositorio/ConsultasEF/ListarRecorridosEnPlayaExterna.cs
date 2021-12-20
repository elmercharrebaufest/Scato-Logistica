using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ListarRecorridosEnPlayaExterna : IConsultaPaginada<InstanciaWorkflowDto>
    {
        private readonly FiltroListaDeWorkflowsDto filtro;
        private readonly Paginacion paginacion;


        public ListarRecorridosEnPlayaExterna(FiltroListaDeWorkflowsDto filtro, Paginacion paginacion)
        {
            this.filtro = filtro;
            this.paginacion = paginacion;
        }

        public virtual ListaPaginada<InstanciaWorkflowDto> Ejecutar(DbContext contexto)
        {
            ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var resultado = from r in contexto.Set<Recorrido>()
                            join materialPorCentro in contexto.Set<MaterialPorCentro>().DefaultIfEmpty() on new { Key1 = r.Material.Id, Key2 = r.Centro.Id } equals new { Key1 = materialPorCentro.Material.Id, Key2 = materialPorCentro.Centro.Id } into materialPorCentroJoined
                            from materialPorCentro in materialPorCentroJoined.DefaultIfEmpty()
                            where contexto.Set<LogActividad>().Where(y => y.WorkflowInstanceId == r.InstanciaWorkflow).OrderByDescending(y => y.Id).FirstOrDefault().ActividadXaml == "EnPlayaExterna"
                                                      && !r.Terminado && !r.Rechazado && filtro.CentroId == r.Centro.Id 
                                                      &&(filtro.MaterialId == r.Material.Id || filtro.MaterialId == null)
                                                      &&(filtro.TipoComercialId == r.TipoComercial.Id || filtro.TipoComercialId == null)
                                                      &&(filtro.Patente == r.Patente || filtro.Patente == null)
                                                      &&(filtro.NumeroDocumentoDeIngreso == r.NumeroDocumentoIngreso || filtro.NumeroDocumentoDeIngreso == null)
                                                      &&(filtro.TipoDocumentoDeIngreso == r.TipoDocumentoIngreso || filtro.TipoDocumentoDeIngreso == null)
                                                      &&(filtro.Workflow == r.Workflow.Descripcion || filtro.Workflow == null)

                                                  select
                                                      new InstanciaWorkflowDto
                                                          {
                                                              Rechazado = r.Rechazado,
                                                              Id = r.InstanciaWorkflow,
                                                              NumeroDocumentoDeIngreso = r.NumeroDocumentoIngreso,
                                                              Patente = r.Patente,
                                                              CentroId = r.Centro.Id,
                                                              Centro = r.Centro.Descripcion,
                                                              CentroCodigoSap = r.Centro.CodigoSAP,
                                                              Codigo = r.Workflow.Codigo,
                                                              Workflow = r.Workflow.Descripcion,
                                                              MaterialId = r.Material.Id,
                                                              Material = r.Material.Descripcion,
                                                              MaterialCodigoSap = r.Material.CodigoSAP,
                                                              TransportistaId = r.Transportista.Id,
                                                              Transportista = r.Transportista.RazonSocial,
                                                              EsSustentable = r.Establecimiento != null,
                                                              FechaCreacion = r.FechaInicio,
                                                              TipoDocumentoDeIngreso = r.TipoDocumentoIngreso,
                                                              TipoComercial = r.TipoComercial.Descripcion,
                                                              TipoComercialId = r.TipoComercial.Id,
                                                              PagaTicketMunicipal = r.PagaTicketMunicipal ?? (materialPorCentro != null ? materialPorCentro.ImprimeReciboMunicipal : true),
                                                              Modificado = r.PagaTicketMunicipal != null
                                                          };

            if (paginacion.OrdenarPor != null)
            {
                var selectorOrden = Expresiones.Propiedad<InstanciaWorkflowDto>(paginacion.OrdenarPor);
                resultado = paginacion.DireccionOrden == DirOrden.Asc
                                 ? resultado.OrderBy(selectorOrden)
                                 : resultado.OrderByDescending(selectorOrden);
            }
            var itemsTotales = resultado.Count();

            resultado = resultado.Skip((paginacion.Pagina - 1) * paginacion.ItemsPorPagina).Take(paginacion.ItemsPorPagina);

            return new ListaPaginada<InstanciaWorkflowDto>(resultado.ToList(), paginacion.Pagina, paginacion.ItemsPorPagina, itemsTotales);
        }
    }
}
