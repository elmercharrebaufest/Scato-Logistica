using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoConsultarImagenCpe : Resultado
    {
        [DataMember]
        public byte[] PdfImage { get; set; }
    }
}
