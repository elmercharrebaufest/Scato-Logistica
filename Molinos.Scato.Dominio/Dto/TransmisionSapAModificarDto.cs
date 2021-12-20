using System.Collections.Generic;
using Molinos.Scato.Dominio.Enums;


namespace Molinos.Scato.Dominio.Dto
{
    public class TransmisionSapAModificarDto
    {
        public int Id { get; set; }
        public IDictionary<string, string> Campos { get; set; }
        public EstadoTransmisionASap EstadoTransmision { get; set; }
    }
}
