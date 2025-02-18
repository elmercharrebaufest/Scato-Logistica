using Molinos.Scato.Dominio.Entidades;
using NPOI.POIFS.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Dominio.Comandos
{
    public class VerificarEnCompliance : Comando
    {
        public string Cuit { get; set; }
        public string Dni { get; set; }
        public string Patente { get; set; }
        public string Planta { get; set; }
    }
}
