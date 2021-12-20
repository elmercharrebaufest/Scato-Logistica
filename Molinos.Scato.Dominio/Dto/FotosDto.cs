
using System;
using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class FotosDto
    {
        public FotosDto()
        {
            Fotos = new List<FotoDto>();
        }
        public List<FotoDto> Fotos { get; set; }

        public string Patente { get; set; }

        public string NumeroDocumentoIngreso { get; set; }

        public string TipoDocumento { get; set; }

        public int RecorridoId { get; set; }

        public string Material { get; set; }

        public DateTime FechaInicio { get; set; }
    }
}
