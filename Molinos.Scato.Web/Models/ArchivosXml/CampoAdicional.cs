using System.Xml.Serialization;

namespace Molinos.Scato.Web.Models.ArchivosXml
{
    public class CampoAdicional
    {
        [XmlElement("tipocampoadic")]
        public int TipoCampoAdic { get; set; }

        [XmlElement("tipovalor")]
        public int TipoValor { get; set; }

        [XmlElement("valor")]
        public string Valor { get; set; }
    }
}