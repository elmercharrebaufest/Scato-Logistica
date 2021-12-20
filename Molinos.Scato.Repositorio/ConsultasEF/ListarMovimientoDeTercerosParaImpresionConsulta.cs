using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ListarMovimientoDeTercerosParaImpresionConsulta : IConsultaPaginada<MovimientoDeTercerosListaDto>
    {
        private readonly int loteId;
        private Paginacion paginacion;

        public ListarMovimientoDeTercerosParaImpresionConsulta(int loteId, Paginacion paginacion = null)
        {
            this.loteId = loteId;
            this.paginacion = paginacion;
            
        }

        public ListaPaginada<MovimientoDeTercerosListaDto> Ejecutar(DbContext contexto)
        {

            ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var muestras = (from muestra in contexto.Set<MovimientoDeTerceros>()

                    join ordenEntrePlantas in contexto.Set<OrdenEntrePlantas>() on muestra.Recorrido.Id equals ordenEntrePlantas.Recorrido.Id into
                                 ordenEntrePlantasJoined
                    from ordenEntrePlantas in ordenEntrePlantasJoined.DefaultIfEmpty()

                    join remito in contexto.Set<Remito>() on muestra.Recorrido.Id equals remito.Recorrido.Id into
                                 remitoJoined
                    from remito in remitoJoined.DefaultIfEmpty()

                    where muestra.ArchivoDeMovimientos.Id == loteId
                    orderby muestra.Recorrido.Id
                            select new MovimientoDeTercerosListaDto
                    {
                        Id = muestra.Id,
                        Material = muestra.Recorrido.Material.Descripcion,
                        FechaDescarga = muestra.Recorrido.PesoTaraFecha.HasValue ? muestra.Recorrido.PesoTaraFecha.Value : muestra.Recorrido.FechaInicio,
                        PesoNeto = (muestra.Recorrido.PesoBruto ?? 0) - (muestra.Recorrido.PesoTara ?? 0),
                        NroCartaPorte = remito != null ? remito.OrdenRemito : muestra.Recorrido.NumeroDocumentoIngreso,
                        CentroDestino = ordenEntrePlantas.CentroDestino.CodigoSAP ?? muestra.Recorrido.Vehiculo.CartaPorte.CentroDestino.CodigoSAP,
                        Patente = muestra.Recorrido.Patente,
                        TipoDocumentoIngreso = muestra.Recorrido.TipoDocumentoIngreso,
                        Transportista = muestra.Recorrido.Transportista.RazonSocial
                        
                    });
            
            var itemsTotales = muestras.Count();
            if (paginacion == null)
            {
                paginacion = new Paginacion(itemsPorPagina: itemsTotales);
            }
            if (paginacion.OrdenarPor != null)
            {
                var selectorOrden = Expresiones.Propiedad<MovimientoDeTercerosListaDto>(paginacion.OrdenarPor);
                muestras = paginacion.DireccionOrden == DirOrden.Asc
                                 ? muestras.OrderBy(selectorOrden)
                                 : muestras.OrderByDescending(selectorOrden);
            }
            
            muestras = muestras.Skip((paginacion.Pagina - 1) * paginacion.ItemsPorPagina).Take(paginacion.ItemsPorPagina);

            return new ListaPaginada<MovimientoDeTercerosListaDto>(muestras.ToList(), paginacion.Pagina, paginacion.ItemsPorPagina, itemsTotales);
        }
    }
}
