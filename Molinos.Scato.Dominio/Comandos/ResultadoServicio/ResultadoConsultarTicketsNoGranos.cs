using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos.ResultadoServicio
{
    [DataContract]
    public class ResultadoConsultarTicketsNoGranos : Resultado
    {
        [DataMember]
        public List<ResultadoTicketsNoGranos> TicketsNoGranos { get; set; } = new List<ResultadoTicketsNoGranos>();
    }

    [DataContract]
    public class ResultadoTicketsNoGranos
    {
        [DataMember]
        public byte[] CPEDG { get; set; }
        [DataMember]
        public byte[] TicketPesada { get; set; }
        [DataMember]
        public byte[] TicketReciboMunicipal { get; set; }
        [DataMember]
        public string NumeroOrdenOperaciones { get; set; }
    }
}
