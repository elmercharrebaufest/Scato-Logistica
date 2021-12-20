using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoEnviarZE7550 : Resultado
    {
        [DataMember]
        public int TransaccionesTotales { get; set; }
        [DataMember]
        public int CuposReenviados { get; set; }
        [DataMember]
        public int CuposNoPaso24Horas { get; set; }
        [DataMember]
        public int TransaccionesEnviadasOK { get; set; }
        [DataMember]
        public int TransaccionesEnviadasError { get; set; }
    }
}
