using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public class EstablecimientoProveedorDto
    {
        [Required]
        public int EstablecimientoId { get; set; }

        [Required]
        public int ProveedorId { get; set; }

    }
}