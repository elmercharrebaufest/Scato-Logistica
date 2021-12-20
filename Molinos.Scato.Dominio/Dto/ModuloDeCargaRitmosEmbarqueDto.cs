using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Dominio.Dto
{
    public class ModuloDeCargaRitmosEmbarqueDto
    {
        public int Id { get; set; }
        public DateTime FechaHoraArranqueBalanza7 { get; set; }
        public DateTime UltimaBalanzadaBalanza7 { get; set; }
        public int tnTotalesBalanza7 { get; set; }
        public DateTime FechaHoraArranqueBalanza8 { get; set; }
        public DateTime UltimaBalanzadaBalanza8 { get; set; }
        public int tnTotalesBalanza8 { get; set; }
    }
}
