using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class BocaDestino : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }

        public virtual Proveedor Proveedor { get; set; }

        public virtual string NombreBocaDeDestino { get; set; }

        public virtual string CodigoPostal { get; set; }

        public virtual string CodigoONCCA { get; set; }

        public virtual string Domicilio { get; set; }

        public virtual Localidad Localidad { get; set; }

        public virtual Provincia Provincia { get; set; }

        public virtual Pais Pais { get; set; }
    }
}
