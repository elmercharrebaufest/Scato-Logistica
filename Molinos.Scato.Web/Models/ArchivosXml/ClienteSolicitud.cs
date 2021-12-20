using System.Xml;
using System.Xml.Serialization;

namespace Molinos.Scato.Web.Models.ArchivosXml
{
    public class ClienteSolicitud
    {
        [XmlElement("tipocod")]
        public string TipoCod { get; set; }
        [XmlElement("codigocliente")]
        public string CodigoCliente { get; set; }
        [XmlElement("rolcliente")]
        public int RolCliente { get; set; }
        [XmlElement("contrato")]
        public string Contrato { get; set; }
        [XmlIgnore]
        public bool? PresentaSolicitud { get; set; }
        [XmlElement("presentasolicitud")]
        public string PresentaSolicitudSerializado
        {
            get { return PresentaSolicitud.HasValue ? (PresentaSolicitud.Value ? "1" : "0") : null; }
            set { PresentaSolicitud = XmlConvert.ToBoolean(value); }
        }
        [XmlIgnore]
        public bool PagaEnsayos { get; set; }
        [XmlElement("pagaensayos")]
        public string PagaEnsayosSerializado
        {
            get { return PagaEnsayos ? "1" : "0"; }
            set { PagaEnsayos = XmlConvert.ToBoolean(value); }
        }
    }
}