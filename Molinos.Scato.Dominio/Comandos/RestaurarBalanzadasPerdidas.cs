using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Dominio.Comandos
{
    public class RestaurarBalanzadasPerdidas : Comando
    {
        public string NumeroBalanza { get; set; }
        public int Desde { get; set; }
        public int Hasta { get; set; }
    }
}
