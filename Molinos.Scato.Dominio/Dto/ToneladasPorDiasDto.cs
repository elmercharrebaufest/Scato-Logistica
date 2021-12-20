using System;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ToneladasPorDiaDto
    {
        public string Fecha { get; set; }

        public DateTime Orden { get; set; }

        public int Toneladas { get; set; }

        public string Codigo { get; set; }
    }



}