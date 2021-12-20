using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoGuardarFoto : Resultado
    {
        [DataMember]
        public string Path { get; set; }
    }
}
