using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Dominio.Comandos
{
    public class InformarCaladoCircular : Comando
    {
        public Guid WorkflowInstanceId { get; set; }
        public bool EsPostCalado { get; set; }
    }
}
