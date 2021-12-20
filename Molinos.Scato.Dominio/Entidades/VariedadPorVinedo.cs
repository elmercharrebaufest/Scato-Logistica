using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class VariedadPorVinedo : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }

        public virtual string Cosecha { get; set; }

        public virtual Variedad Variedad { get; set; }

        public virtual Vinedo Vinedo { get; set; }

        public virtual decimal Hectareas { get; set; }

        public virtual decimal AvisoCorte { get; set; }

        public virtual decimal TopeHectarea { get; set; }
    }
}
