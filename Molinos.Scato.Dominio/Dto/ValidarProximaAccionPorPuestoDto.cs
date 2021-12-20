
using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ValidarProximaAccionPorPuestoDto : ValidarProximaAccionDto
    {
        public IEnumerable<string> Entrada { get; set; }
        public List<VideoCamaraDto> VideoCamaras { get; set; }
    }
}
