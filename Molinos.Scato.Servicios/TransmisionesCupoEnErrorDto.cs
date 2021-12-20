using System;
using Molinos.Scato.Servicios.ServiciosSap;

namespace Molinos.Scato.Servicios
{
    public sealed class TransmisionesCupoEnErrorDto
    {
        public Guid InstanciaWorkflow { get; set; }
        public Z_SDMF_Z2200N Z2200 { get; set; }
        
    }
}
