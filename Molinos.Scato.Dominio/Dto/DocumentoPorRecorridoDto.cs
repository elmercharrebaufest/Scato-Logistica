using Molinos.Scato.Dominio.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Dominio.Dto
{
    public class DocumentoPorRecorridoDto
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public string Extension { get; set; }
        public TipoImpresion Tipo { get; set; }
        public DateTime FechaDeGuardado { get; set; }
    }
}
