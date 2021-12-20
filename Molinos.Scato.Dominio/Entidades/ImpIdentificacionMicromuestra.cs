using System.ComponentModel.DataAnnotations.Schema;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("ImpIdentificacionMicromuestra")]
    public class ImpIdentificacionMicromuestra : Impresion
    {
        public virtual string Centro { get; set; }
        public virtual string Material { get; set; }
        public virtual string NroMuestra { get; set; }
        public virtual string NumeroDeOrden { get; set; }
        public virtual string Proveedor { get; set; }
        public virtual string Humedad { get; set; }
        public virtual string NroCasillero { get; set; }
        public virtual TipoMicromuestra TipoMicromuestra { get; set; }
    }
}
