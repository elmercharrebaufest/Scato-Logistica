using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class PesoVagonesDto
    {
        public int PesoBruto { get; set; }
        public int PesoTara { get; set; }
        public int PesoNeto { get; set; }
        public decimal PesoDescuento { get; set; }
        
        public IList<RecorridoDto> Vagones { get; set; }
    }
}

