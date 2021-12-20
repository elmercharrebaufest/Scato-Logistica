using System.Data.Entity;
using System.Linq;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class TipoComercialPorWfConsulta : IConsultaPaginada<TipoComercialPorWfDto>
    {
        private string filtro;
        private readonly int centroId;
        private readonly Paginacion paginacion;

        public TipoComercialPorWfConsulta(string filtro, int centroId, Paginacion paginacion)
        {
            this.filtro = filtro;
            this.centroId = centroId;
            this.paginacion = paginacion;
        }

        public ListaPaginada<TipoComercialPorWfDto> Ejecutar(DbContext contexto)
        {
            IQueryable<TipoComercialPorWfDto> resultados = 
                         from workflow in contexto.Set<Workflow>()
                         from tipoComercial in workflow.TiposComercialesAsociados
                         where workflow.Activo && workflow.Centro.Id == centroId
                         select
                             new TipoComercialPorWfDto
                             {
                                 WorkflowId = workflow.Id,
                                 WorkflowDescripcion = workflow.Descripcion,
                                 TipoComercialId = tipoComercial.Id,
                                 TipoComercialDescripcion = tipoComercial.Descripcion
                             };

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                filtro = filtro.Trim();
                resultados = resultados.Where(x => x.WorkflowDescripcion.Contains(filtro) || x.TipoComercialDescripcion.Contains(filtro));            
            }

            var itemsTotales = resultados.Count();

            if (paginacion.OrdenarPor != null)
            {
                var selectorOrden = Expresiones.Propiedad<TipoComercialPorWfDto>(paginacion.OrdenarPor);
                resultados = paginacion.DireccionOrden == DirOrden.Asc
                                 ? resultados.OrderBy(selectorOrden)
                                 : resultados.OrderByDescending(selectorOrden);
            }

            resultados = resultados.Skip((paginacion.Pagina - 1) * paginacion.ItemsPorPagina).Take(paginacion.ItemsPorPagina);

            return new ListaPaginada<TipoComercialPorWfDto>(resultados.ToList(), paginacion.Pagina, paginacion.ItemsPorPagina, itemsTotales);
        }
    }
}
