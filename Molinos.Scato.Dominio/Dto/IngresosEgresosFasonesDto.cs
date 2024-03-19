using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Dominio.Dto
{
    public class IngresosEgresosFasonesDto
    {
        public int FasonId { get; set; }
        public int Cantidad { get; set; }
        public string FechaIngreso { get; set; }
        public string FechaEgreso { get; set; }
        public string NroRemito { get; set; }
        public string UniMedCant { get; set; }
    }
}
