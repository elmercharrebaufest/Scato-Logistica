using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ListarCentroConFiltroYCantidadMaxima : IConsulta<ExportacionDeArchivosSelectObjDto>
    {
        private readonly int cantidad;
        private readonly int pagina;
        public ListarCentroConFiltroYCantidadMaxima(int cantidad, int pagina)
        {
            this.cantidad = cantidad;
            this.pagina = pagina;
        }

        public List<ExportacionDeArchivosSelectObjDto> Ejecutar(DbContext contexto)
        {
            var resultado =
                contexto.Set<Centro>()
                        .OrderBy(x => x.Descripcion)
                        .Skip((pagina -1) * cantidad)
                        .Take(cantidad).Select(x => new ExportacionDeArchivosSelectObjDto { Id = x.Id, Descripcion = "(" + x.CodigoSAP + ")" + x.Descripcion }).OrderBy(x => x.Descripcion).ToList();
            return resultado;
        }
    }
}
