using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class DescargaDeBines
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual Material Tipo { get; set; }
        public virtual Cuartel Cuartel { get; set; }
        public virtual int CantidadBines { get; set; }
        public virtual RemitoBodegaUva RemitoBodegaUva { get; set; }
    }
}
