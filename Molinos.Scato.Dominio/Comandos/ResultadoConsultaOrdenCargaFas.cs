using Molinos.Scato.Dominio.Dto;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoConsultaOrdenCargaFas : Resultado
    {
        [DataMember]
        public List<OrdenCargaFasDto> Orden { get; set; }

    }
}