using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class RomaneoItemPedido:IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual Romaneo Romaneo { get; set; }
        [Required]
        public virtual Material Material { get; set; }
        public virtual string Ebelp { get; set; }
        public virtual string CantPedido { get; set; }
        public virtual string FecEntrega { get; set; }
        public virtual string PorcentajeExc { get; set; }
        public virtual string Werks { get; set; }
    }
}