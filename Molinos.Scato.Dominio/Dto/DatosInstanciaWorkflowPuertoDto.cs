using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class DatosInstanciaWorkflowPuertoDto
    {
        public IList<LineUpDto> LineUps { get; set; }
        public IList<EmbarqueDto> Embarques { get; set; }
    }
}