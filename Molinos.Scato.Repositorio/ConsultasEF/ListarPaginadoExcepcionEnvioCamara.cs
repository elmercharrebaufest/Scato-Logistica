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
    public class ListarPaginadoExcepcionEnvioCamara : IConsultaPaginada<ExcepcionEnvioCamaraDto>
    {
        private readonly string filtro;
        private readonly Paginacion paginacion;


        public ListarPaginadoExcepcionEnvioCamara(string filtro, Paginacion paginacion)
        {
            this.filtro = filtro;
            this.paginacion = paginacion;
        }

        public virtual ListaPaginada<ExcepcionEnvioCamaraDto> Ejecutar(DbContext contexto)
        {
            
            ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            IQueryable<ExcepcionEnvioCamara> query = contexto.Set<ExcepcionEnvioCamara>();

            var resultado = query.Where(x => x.Material.Descripcion.Contains(filtro)
                                                        || x.TipoComercial.Descripcion.Contains(filtro)
                                                        || x.Proveedor.RazonSocial.Contains(filtro)
                                                        || x.Entregador.RazonSocial.Contains(filtro)
                                                        || filtro == null
                                                        || filtro == "")
                                                        .GroupBy(x => new { x.Material, x.Entregador, x.Proveedor, x.TipoComercial })
                                                        .Select(x => new ExcepcionEnvioCamaraDto
                                                        {
                                                            Id = x.FirstOrDefault().Id,
                                                            ProveedorId = x.Key.Proveedor.Id,
                                                            EntregadorId = x.Key.Entregador.Id,
                                                            TipoComercialId = x.Key.TipoComercial.Id,
                                                            EntregadorDesc = x.Key.Entregador.RazonSocial,
                                                            MaterialDesc = x.Key.Material.Descripcion,
                                                            MaterialId = x.Key.Material.Id,
                                                            ProveedorDesc = x.Key.Proveedor.RazonSocial,
                                                            TipoComercialDesc = x.Key.TipoComercial.Descripcion
                                                        });

            if (paginacion.OrdenarPor != null)
            {
                var selectorOrden = Expresiones.Propiedad<ExcepcionEnvioCamaraDto>(paginacion.OrdenarPor);
                resultado = paginacion.DireccionOrden == DirOrden.Asc
                                 ? resultado.OrderBy(selectorOrden)
                                 : resultado.OrderByDescending(selectorOrden);
            }
            var itemsTotales = resultado.Count();

            var resultadoMaterializado = resultado.Skip((paginacion.Pagina - 1) * paginacion.ItemsPorPagina).Take(paginacion.ItemsPorPagina).ToList();


            return new ListaPaginada<ExcepcionEnvioCamaraDto>(resultadoMaterializado, paginacion.Pagina, paginacion.ItemsPorPagina, itemsTotales);
        }
    }
}
