using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoActualizarValoresSap : Resultado
    {
        [DataMember]
        public string NumeroDeDocumentoSap { get; set; }
        [DataMember]
        public string DocumentoInternoSap { get; set; }
    }
}
