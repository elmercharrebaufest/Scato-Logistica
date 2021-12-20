using Molinos.Scato.Dominio.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Dominio.Comandos
{
    public class InformarPesadaCircular : Comando
    {
        public Guid WorkflowInstanceId { get; set; }
        public int Peso { get; set; }
        public TipoPesada TipoPesada { get; set; }
    }
}
