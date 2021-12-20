using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades

{
    public class Talonario:IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual int Sucursal { get; set; }
        [Required]
        public virtual string Descripcion { get; set; }
        [Required]
        public virtual int PrimerNumero { get; set; }
        [Required]
        public virtual int UltimoNumero { get; set; }
        [Required]
        public virtual int ProximoNumero { get; set; }
        public virtual Centro Centro { get; set; }
    }
}
