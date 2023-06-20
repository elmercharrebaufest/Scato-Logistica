using Molinos.Scato.Dominio.Dto;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoMensajeCartelLed : Resultado
    {
        public ResultadoMensajeCartelLed()
        {
            ListaDeMensajes = new List<MensajeCartelLedDto>();
        }
        [DataMember]
        public string Mensaje { get; set; }
        [DataMember]
        public string NumeroTrama { get; set; }
        [DataMember]
        public string NumeroPrograma { get; set; }
        [DataMember]
        public string NumeroVariable { get; set; }
        [DataMember]
        public int SegundosDeEspera { get; set; }
        public List<MensajeCartelLedDto> ListaDeMensajes { get; set; }
    }
}
