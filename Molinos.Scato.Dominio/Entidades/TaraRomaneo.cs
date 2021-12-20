using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades

{
    public class TaraRomaneo : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual int Codigo { get; set; }
        [Required]
        public virtual string Descripcion { get; set; }
        [Required]
        public virtual decimal Peso { get; set; }
        public virtual bool Importacion { get; set; }
        public virtual bool CargaPesoManual { get; set; }
        public virtual Centro Centro { get; set; }
    }
}
