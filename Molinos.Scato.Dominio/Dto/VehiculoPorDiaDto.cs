using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class VehiculoPorDiaDto
    {
        public DateTime Fecha { get; set; }
        public string NombreDia { get; set; }
        public int Presente { get; set; }
        public int NoPresente { get; set; }
    }
}
