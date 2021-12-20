using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class DSIngresoDto
    {
        public string IngresoxD { get; set; }
        public int CantidadxD { get; set; }
        public decimal TonsxD { get; set; }
        public int CantidadxM { get; set; }
        public decimal TonsxM { get; set; }
        public int CantidadxA { get; set; }
        public decimal TonsxA { get; set; }

    }
}
