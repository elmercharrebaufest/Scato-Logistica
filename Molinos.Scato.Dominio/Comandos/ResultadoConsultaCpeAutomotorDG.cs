using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoConsultaCpeAutomotorDG : Resultado
    {
        [DataMember]
        public long NroOrden { get; set; }

        [DataMember]
        public long NroCTG { get; set; }

        [DataMember]
        public int Sucursal { get; set; }

        [DataMember]
        public string CuitTransportista { get; set; }

        [DataMember]
        public string PatenteCamion { get; set; }

        [DataMember]
        public string PatenteAcoplado { get; set; }

        [DataMember]
        public long CuitChofer { get; set; }

        [DataMember]
        public int PesoBruto { get; set; }

        [DataMember]
        public int PesoTara { get; set; }

        [DataMember]
        public byte[] Pdf { get; set; }

        [DataMember]
        public string PdfBase { get; set; }

        [DataMember]
        public short CodigoGranario { get; set; }

        [DataMember]
        public short CodigoPadre { get; set; }

        [DataMember]
        public string CuitOrigen { get; set; }

        [DataMember]
        public int PlantaDG { get; set; }
    }
}