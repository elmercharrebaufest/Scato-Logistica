using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class RamalFerroviarioDto
    {
        public int Id { get; set; }
        public int CodigoAfip { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Deshabilitada { get; set; }
    }
}
