using System;
using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto
{
    public class ModuloDeCargaListadoDto
    {
        public int Id { get; set; }
        public DateTime? FechaDeFinalizacion { get; set; }
        public IList<ModuloDeCargaHabilitacionDeTanquesDto> ModuloDeCargaHabilitacionDeTanques { get; set; }
        public bool Enviado { get; set; }
    }
}