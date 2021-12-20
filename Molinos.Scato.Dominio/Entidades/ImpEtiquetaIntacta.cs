using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("ImpEtiquetaIntacta")]
    public class ImpEtiquetaIntacta : Impresion
    {
        public virtual string Centro { get; set; }
        public virtual string Material { get; set; }
        public virtual string NumeroCartaPorte { get; set; }
        public virtual string TipoDeAnalisis { get; set; }
        public virtual string LaboratiorioCuit { get; set; }
        public virtual string LaboratorioNombre { get; set; }
    }
}
