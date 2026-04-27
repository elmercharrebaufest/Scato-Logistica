using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Dominio.Dto
{
    public class RecorridoCpeDto
    {
        public Guid InstanciaWorkflow { get; set; }
        public int Id { get; set; }
        public DateTime? FechaCacheado { get; set; }
        public byte[] Pdf { get; set; }
    }
}
