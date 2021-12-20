using Molinos.Scato.Dominio.Enums;
using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoRetransmitirTransmisionASap : Resultado
    {
        [DataMember]
        public TipoAlerta Resultado { get; set; }
    }
}
