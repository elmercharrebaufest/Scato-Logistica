using Molinos.Scato.Dominio.Dto;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoConsultaDomiciliosDG : Resultado
    {
        [DataMember]
        public List<DomicilioDto> Domicilios { get; set; }
    }

    
}
