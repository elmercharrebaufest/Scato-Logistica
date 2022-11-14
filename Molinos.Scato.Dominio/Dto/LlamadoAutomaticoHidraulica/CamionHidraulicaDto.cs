using System;
using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class CamionHidraulicaDto
    {
        public string Patente { get; set; }
        public List<int> HidraulicasId { get; set; }
    }
}
