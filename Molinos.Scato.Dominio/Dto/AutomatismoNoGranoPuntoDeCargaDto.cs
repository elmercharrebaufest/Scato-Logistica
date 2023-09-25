using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Dominio.Dto
{
    public class AutomatismoNoGranoPuntoDeCargaDto
    {
        public int Id { get; set; }

        public int PuntoDeCargaId { get; set; }
        public int AutomatismoNoGranoId { get; set; }
    }
}
