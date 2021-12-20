using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class MovimientoDeTerceros : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }

        public virtual ArchivoDeMovimientos ArchivoDeMovimientos { get; set; }

        public virtual Recorrido Recorrido { get; set; }

    }
}
