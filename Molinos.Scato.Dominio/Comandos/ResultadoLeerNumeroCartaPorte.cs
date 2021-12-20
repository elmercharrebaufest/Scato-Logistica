using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoLeerNumeroCartaPorte : Resultado
    {
        [DataMember]
        public string NumeroCartaPorte { get; set; }
    }
}
