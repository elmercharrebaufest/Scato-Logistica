using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoConsultaCpeAutomotorDG : Resultado
    {
        [DataMember]
        public long nroOrden { get; set; }

        [DataMember]
        public long nroCTG { get; set; }

        [DataMember]
        public int sucursal { get; set; }

        [DataMember]
        public string cuitTransportista { get; set; }

        [DataMember]
        public string patenteCamion { get; set; }

        [DataMember]
        public string patenteAcoplado { get; set; }  
        
        [DataMember]
        public long cuitChofer { get; set; }
        
        [DataMember]
        public int pesoBruto { get; set; }
        
        [DataMember]
        public int pesoTara { get; set; }

        [DataMember]
        public byte[] pdf { get; set; }

        [DataMember]
        public string pdfBase { get; set; }

    }

    
}
