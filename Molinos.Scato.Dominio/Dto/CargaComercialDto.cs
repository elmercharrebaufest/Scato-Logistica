using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Dominio.Dto
{
    public class CargaComercialDto
    {
        public int Id { get; set; }
        public ExportadorDto Exportador { get; set; }
        public MaterialPuertoDto MaterialPuerto { get; set; }
        public int Cantidad { get; set; }
    }
}
