using Molinos.Scato.Dominio.Dto;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoTickets : Resultado
    {
        [DataMember]
        public string CP { get; set; }
        [DataMember]
        public string Patente { get; set; }
        [DataMember]
        public FotosDto FotoCP { get; set; }
        [DataMember]
        public byte[] TicketReciboMunicipal { get; set; }
        [DataMember]
        public byte[] TicketPesada { get; set; }
        [DataMember]
        public byte[] CertificadoCP { get; set; }
    }
}
