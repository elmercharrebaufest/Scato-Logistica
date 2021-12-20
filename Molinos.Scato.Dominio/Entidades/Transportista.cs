using Molinos.Scato.Dominio.Enums;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Transportista : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual string Cuit { get; set; }
        [Required]
        public virtual string RazonSocial { get; set; }

        public virtual string Domicilio { get; set; }

        public virtual Localidad Localidad { get; set; }

        public virtual Provincia Provincia { get; set; }
        [Required]
        public virtual MedioDePago MedioDePago { get; set; }
    }
}
