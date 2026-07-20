using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class CamionPorDiaDto
    {
        public DateTime Fecha { get; set; }
        public int CantidadDiaria { get; set; }
        public int Acumulado { get; set; }
    }
}
