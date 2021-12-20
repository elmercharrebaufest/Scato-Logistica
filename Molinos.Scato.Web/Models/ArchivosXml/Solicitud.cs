using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Molinos.Scato.Web.Models.ArchivosXml
{
    public class Solicitud
    {
        [XmlElement("tiposolicitud")]
        public int TipoSolicitud       { get; set; }
        [XmlElement("producto")]
        public int Producto            { get; set; }
        [XmlElement("productoespecial")]
        public string ProductoEspecial { get; set; }
        [XmlIgnore]
        public bool Reconsideracion { get; set; }
        [XmlElement("reconsideracion")]
        public string ReconsideracionSerializado
        {
            get { return Reconsideracion ? "1" : "0"; }
            set { Reconsideracion = XmlConvert.ToBoolean(value); }
        }

        [XmlElement("puerto")]
        public int Puerto   { get; set; }

        [XmlArray("clientessolicitud")]
        [XmlArrayItem("clientesolicitud")]
        public List<ClienteSolicitud> ClientesSolicitudes { get; set; }

        [XmlArray("clientesrelaciones")]
        [XmlArrayItem("clienterelaciones")]
        public List<ClienteRelacion> ClientesRelaciones { get; set; }

        [XmlArray("camposadicionales")]
        [XmlArrayItem("campoadicional")]
        public List<CampoAdicional> CampoAdicional { get; set; }

        [XmlArray("observaciones")]
        [XmlArrayItem("observacion")]
        public List<Observacion> Observaciones { get; set; }

        [XmlArray("muestras")]
        [XmlArrayItem("muestra")]
        public List<Muestra> Muestras { get; set; }
    }
}