using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class CuposOtorgadosDto
    {     
        public int Cupos { set; get; }

        public DateTime Fecha { get; set; }

        public int CentroId { get; set; }

        public int MaterialId { get; set; }
        public bool Especial { get; set; }
    }
}
