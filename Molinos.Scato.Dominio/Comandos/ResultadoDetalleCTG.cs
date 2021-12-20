using System.Runtime.Serialization;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoDetalleCTG : Resultado
    {
        [DataMember]
        public CartaPorteDto CartaPorte { get; set; }
    }
}
