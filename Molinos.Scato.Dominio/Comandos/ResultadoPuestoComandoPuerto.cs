using Molinos.Scato.Dominio.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ResultadoPuestoComandoPuerto : Resultado
    {
        [DataMember]
        public List<DatosDeWorkflowDto> Workflows { get; set; }
    }
}
