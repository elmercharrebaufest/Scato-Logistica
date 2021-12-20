using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Dominio.Dto
{
    public class ReportePesadaSeisHorasDto
    {
        public string Vapor { get; set; }
        public string Material { get; set; }
        public string Exportador { get; set; }
        public DateTime Fecha { get; set; }
        public string Bodega { get; set; }
        public int Total { get; set; }
        public int RangoCeroASeis { get; set; }
        public int RangoSeisADoce { get; set; }
        public int RangoDoceADieciseis { get; set; }
        public int RangoDieciseisAveinticuatro { get; set; }
    }
}
