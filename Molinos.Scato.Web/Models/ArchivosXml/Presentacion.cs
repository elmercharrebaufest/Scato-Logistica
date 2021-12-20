using System.Collections.Generic;
using System.Xml.Serialization;

namespace Molinos.Scato.Web.Models.ArchivosXml
{
    [XmlRoot("presentacion")]
    public class Presentacion
    {
        [XmlArray("solicitudes")]
        [XmlArrayItem("solicitud")]
        public List<Solicitud> Solicitudes { get; set; }

        [XmlArray("clientes")]
        [XmlArrayItem("cliente")]
        public List<Cliente> Clientes { get; set; }

        [XmlElement("resumen")]
        public Resumen Resumen { get; set; }
    }
}