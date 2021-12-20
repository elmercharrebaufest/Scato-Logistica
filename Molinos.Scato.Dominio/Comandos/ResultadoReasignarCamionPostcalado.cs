using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoReasignarCamionPostcalado : Resultado
    {
        [DataMember]
        public int Disponibilidad { get; set; }
        [DataMember]
        public int CalleId { get; set; }
    }
}
