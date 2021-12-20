using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("ImpEtiquetaRubrosAnalizar")]
    public class ImpEtiquetaRubrosAnalizar : Impresion
    {
        public virtual string Centro { get; set; }
        public virtual string NumeroCartaPorte { get; set; }
        public virtual string AnalisisSeleccionados { get; set; }
    }

}
