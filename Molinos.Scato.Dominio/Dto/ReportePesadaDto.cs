using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Dominio.Dto
{
    public class ReportePesadaDto
    {
        public int IdCarga { get; set; }
        public string NumeroBalanza { get; set; }
        public int TotalEmbarcado { get; set; }
        public string Commodity { get; set; }
        public string Bodega { get; set; }
        public string Destino { get; set; }
        public DateTime? Fecha { get; set; }
        public string Exportador { get; set; }
        public string Vapor { get; set; }
        public int PesoProgramado { get; set; }



    }
}
