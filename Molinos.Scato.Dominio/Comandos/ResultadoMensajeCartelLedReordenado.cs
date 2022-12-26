using Molinos.Scato.Dominio.Dto;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoMensajeCartelLedReordenado : Resultado
    {
        public ResultadoMensajeCartelLedReordenado()
        {
            ListaDeMensajes = new List<MensajeCartelLedDto>();
        }
        [DataMember]
        public List<MensajeCartelLedDto> ListaDeMensajes { get; set; }
    }
}
