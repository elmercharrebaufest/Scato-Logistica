using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Dominio.Dto
{
    public class IngresosEgresosResiduosDto
    {
        public int IdOperaciones { get; set; }
        public int OrdenCargaInterna { get; set; }
        public int? PesadaTara { get; set; }
        public int? PesadaNeto { get; set; }
        public int? PesadaBruto { get; set; }
        public string FechaEntrada { get; set; }
        public string FechaSalida { get; set; }
        public string NroCertificacion { get; set; }
        public string Balanza { get; set; }
        public string UniMedCant { get; set; }
    }
}

