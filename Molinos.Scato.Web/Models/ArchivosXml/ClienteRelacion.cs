using System.Xml.Serialization;

namespace Molinos.Scato.Web.Models.ArchivosXml
{
    public class ClienteRelacion
    {
        [XmlElement("rolcliente1")]
        public int RolCliente1 { get; set; }

        [XmlElement("rolcliente2")]
        public int RolCliente2 { get; set; }

        [XmlElement("rolclientenro1")]
        public int RolClienteNro1 { get; set; }

        [XmlElement("rolclientenro2")]
        public int RolClienteNro2 { get; set; }

        [XmlElement("relacionclientes")]
        public int RelacionClientes { get; set; }
    }
}