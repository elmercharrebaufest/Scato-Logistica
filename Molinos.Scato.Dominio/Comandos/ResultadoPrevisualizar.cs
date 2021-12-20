using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoPrevisualizar : Resultado
    {
        [DataMember]
        public byte[] Archivo { get; set; }
    }
}
