using System.Collections.Generic;
using System.Runtime.Serialization;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoPuestoComando : Resultado
    {
        [DataMember]
        public List<DatosDeWorkflowDto> Workflows { get; set; }
    }
}
