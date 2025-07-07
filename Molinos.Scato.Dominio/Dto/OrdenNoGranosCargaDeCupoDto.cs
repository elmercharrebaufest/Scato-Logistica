using Molinos.Scato.Dominio.Enums;
using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto
{
    public class OrdenNoGranosCargaDeCupoDto
    {
        public int MaterialId { get; set; }
        public string MaterialDescripcion { get; set; }
        public TipoOrdenCargaNoGranos TipoOrden { get; set; }
        public string PatenteAcoplado { get; set; }
    }

    public class OrdenNoGranosCargaDeCupoComparer : IEqualityComparer<OrdenNoGranosCargaDeCupoDto>
    {
        public bool Equals(OrdenNoGranosCargaDeCupoDto x, OrdenNoGranosCargaDeCupoDto y)
        {
            return x.MaterialId == y.MaterialId && x.TipoOrden == y.TipoOrden;
        }

        public int GetHashCode(OrdenNoGranosCargaDeCupoDto obj)
        {
            return obj.MaterialId.GetHashCode() ^ obj.TipoOrden.GetHashCode();
        }
    }
}