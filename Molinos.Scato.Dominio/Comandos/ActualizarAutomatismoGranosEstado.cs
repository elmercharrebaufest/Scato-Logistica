using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ActualizarAutomatismoGranosEstado : Comando
    {
        public bool Estado { get; set; }
    }
}
