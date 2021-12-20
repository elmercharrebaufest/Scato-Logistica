using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class VinedoPropio : Vinedo
    {
        [InverseProperty("VinedoPropio")]
        public virtual IList<Cuartel> Cuarteles { get; set; }
        public virtual string CentroOperativo { get; set; }

        public override string Vinatero()
        {
            return Proveedor.Descripcion;
        }

        public override string VinateroCuit()
        {
            return Proveedor.Cuil;
        }

        public override string CentroOperativoCodigoSap()
        {
            return CentroOperativo;
        }
    }
}
