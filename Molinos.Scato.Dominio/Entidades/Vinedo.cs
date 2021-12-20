using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{

    public abstract class Vinedo : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string NumeroINV { get; set; }
        public virtual Proveedor Proveedor { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual string IngresosBrutos { get; set; }
        [InverseProperty("Vinedo")]
        public virtual ICollection<VariedadPorVinedo> VariedadesPorVinedo { get; set; }
        public virtual SubZona SubZona { get; set; }
        public virtual string Calidad { get; set; }

        public abstract string Vinatero();
        public abstract string VinateroCuit();
        public abstract string CentroOperativoCodigoSap();
    }
}
