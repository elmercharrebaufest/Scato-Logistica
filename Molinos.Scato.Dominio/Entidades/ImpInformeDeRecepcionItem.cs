using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ImpInformeDeRecepcionItem : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual int ItemNro { get; set; }
        public virtual string MaterialCodigo { get; set; }
        public virtual string MaterialDescripcion { get; set; }
        public virtual string UniMed { get; set; }
        public virtual string CantidadDescargada { get; set; }
        public virtual string Remito { get; set; }
        public virtual ImpInformeDeRecepcion ImpInformeDeRecepcion { get; set; }
    }
}
