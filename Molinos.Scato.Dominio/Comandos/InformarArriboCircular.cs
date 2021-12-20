using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Dominio.Comandos
{
    public class InformarArriboCircular : Comando
    {
        public string Patente { get; set; }
        public int MaterialId { get; set; }
        public int CentroId { get; set; }
        public string CartaPorte { get; set; }
    }
}
