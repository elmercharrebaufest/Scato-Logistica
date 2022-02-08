using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoConsultarPDFCpe : Resultado
    {
        [DataMember]
        public byte[] Pdf { get; set; }
    }
}
