using System.Xml.Serialization;

namespace Molinos.Scato.Web.Models.ArchivosXml
{
    public class Observacion
    {
        [XmlElement("observacionlibre")]
        public string ObservacionLibre { get; set; }
        [XmlElement("observacionid")]
        public int ObservacionId { get; set; }
    }
}