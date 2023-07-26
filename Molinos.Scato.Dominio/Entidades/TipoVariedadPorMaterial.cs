using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class TipoVariedadPorMaterial : AuditoriaBase, IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }

        public virtual TipoVariedad TipoVariedad { get; set; }
        public virtual Material Material { get; set; }
    }
}