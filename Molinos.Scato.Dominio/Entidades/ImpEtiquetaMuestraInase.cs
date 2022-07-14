using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("ImpEtiquetaMuestraInase")]
    public class ImpEtiquetaMuestraInase : Impresion
    {
        public virtual string Centro { get; set; }
        public virtual string ProductorCuit { get; set; }
        public virtual string Cpe { get; set; }
        public virtual string Material { get; set; }
    }
}
