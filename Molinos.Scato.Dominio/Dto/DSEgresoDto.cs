using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class DSEgresoDto
    {
        public string EgresoxD { get; set; }
        public int CantidadxD { get; set; }
        public double TonsxD { get; set; }
        public int CantidadxM { get; set; }
        public double TonsxM { get; set; }
        public int CantidadxA { get; set; }
        public double TonsxA { get; set; }

    }
}
