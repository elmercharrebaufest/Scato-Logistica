using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ListarMaterialConFiltroYCantidadMaxima : IConsulta<ExportacionDeArchivosSelectObjDto>
    {
        private readonly int cantidad;
        private readonly int pagina;
        public ListarMaterialConFiltroYCantidadMaxima(int cantidad, int pagina)
        {
            this.pagina = pagina;
            this.cantidad = cantidad;
        }

        public List<ExportacionDeArchivosSelectObjDto> Ejecutar(DbContext contexto)
        {
            var resultado =
                contexto.Set<Material>()
                        .Where(x => contexto.Set<Recorrido>().Any(y => y.Material.Id == x.Id))
                        .OrderBy(x => x.Descripcion)
                        .Skip((pagina - 1) * cantidad)
                        .Take(cantidad).Select(x => new ExportacionDeArchivosSelectObjDto { Id = x.Id, Descripcion = x.Descripcion }).OrderBy(x => x.Descripcion).ToList();
            return resultado;
        }
    }
}
