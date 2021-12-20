using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoEliminarPrimerLectura : Resultado
    {
        [DataMember]
        public string ProximaLectura { get; set; }
        [DataMember]
        public string ProximaPatente { get; set; }
        [DataMember]
        public string ProximaPatenteLeida { get; set; }
        [DataMember]
        public bool OcrActivo { get; set; }
        [DataMember]
        public bool ReconocimientoExitoso { get; set; }
    }
}
