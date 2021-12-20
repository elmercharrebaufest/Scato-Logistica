using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Dominio.Comandos
{
    public class EnviarMensajeCamioneroCircular : Comando
    {
        public string Mensaje { get; set; }
        public string CartaPorte { get; set; }

        public bool SePuedeDesactivar { get; set; }
    }
}
