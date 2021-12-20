using System;
using System.Xml;
using System.Xml.Serialization;

namespace Molinos.Scato.Web.Models.ArchivosXml
{
    public class Resumen
    {
        [XmlElement("totalkilosreal")]
        public long TotalKilosReal { get; set; }

        [XmlElement("totalkilosensayo")]
        public long TotalKilosEnsayo { get; set; }

        [XmlElement("cantidadsolicitudes")]
        public int CantidadSolicitudes { get; set; }

        [XmlElement("cantidadmuestras")]
        public int CantidadMuestras { get; set; }

        [XmlElement("cantidadclientes")]
        public int CantidadClientes { get; set; }

        [XmlIgnore]
        public DateTime? FechaEnvioCamara { get; set; }

        [XmlElement("fechaenviocamara")]
        public string FechaEnvioCamaraSerializado
        {
            get { return FechaEnvioCamara.HasValue ? FechaEnvioCamara.Value.ToString("yyyy-MM-dd") : string.Empty; }
            set { FechaEnvioCamara = XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Unspecified); }
        }

        [XmlElement("origenarchivosolicitudes")]
        public int OrigenArchivoSolicitudes { get; set; }

        [XmlElement("centrodeensayos")]
        public int CentroDeEnsayos { get; set; }

        [XmlElement("nrosecarchivosolicitudes")]
        public int NroSecArchivoSolicitudes { get; set; }
    }
}