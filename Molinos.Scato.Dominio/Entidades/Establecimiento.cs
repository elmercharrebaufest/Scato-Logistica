using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Establecimiento : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }

        public virtual Proveedor Proveedor { get; set; }
        [Required]
        public virtual string NombreDeEstablecimiento { get; set; }

        public virtual string CodigoDeEstablecimiento { get; set; }

        public virtual string Domicilio { get; set; }

        public virtual string CodigoPostal { get; set; }

        public virtual bool Anulado { get; set; }

        public virtual Localidad Localidad { get; set; }

        public virtual Provincia Provincia { get; set; }

        public virtual bool EPA { get; set; }
    }
}
