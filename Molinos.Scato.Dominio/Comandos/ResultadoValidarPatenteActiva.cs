using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoValidarPatenteActiva : Resultado
    {
        [DataMember]
        public string PatenteActiva { get; set; }

        [DataMember]
        public int Diferencia { get; set; }

        [DataMember]
        public int? DuracionMs { get; set; }
    }
}