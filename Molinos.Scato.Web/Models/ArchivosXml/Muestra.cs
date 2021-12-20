using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Molinos.Scato.Web.Models.ArchivosXml
{
    public class Muestra
    {
        [XmlElement("ordenmta")]
        public long OrdenMta { get; set; }

        [XmlElement("kilajereal")]
        public int KilajeReal { get; set; }

        [XmlElement("kilajeensayo")]
        public int KilajeEnsayo { get; set; }

        [XmlArray("conjuntos")]
        [XmlArrayItem("conjunto")]
        public List<Conjunto> Conjuntos { get; set; }

        [XmlIgnore]
        public DateTime? FechaDescarga { get; set; }
        [XmlElement("fechadescarga")]
        public string FechaDescargaSerializado
        {
            get { return FechaDescarga.HasValue ? FechaDescarga.Value.ToString("yyyy-MM-dd") : string.Empty; }
            set { FechaDescarga = XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Unspecified); }
        }

        [XmlIgnore]
        public DateTime? FechaOperacion { get; set; }
        [XmlElement("fechaoperacion")]
        public string FechaOperacionSerializado
        {
            get { return FechaOperacion.HasValue ? FechaOperacion.Value.ToString("yyyy-MM-dd") : string.Empty; }
            set { FechaOperacion = XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Unspecified); }
        }

        [XmlElement("agrupamiento")]
        public int Agrupamiento { get; set; }

        [XmlElement("aniocosecha1")]
        public int AnioCosecha1 { get; set; }

        [XmlElement("aniocosecha2")]
        public int AnioCosecha2   { get; set; }

        [XmlElement("camion")]
        public string Camion { get; set; }

        [XmlIgnore]
        public DateTime? FechaCertificadoOriginal { get; set; }
        [XmlElement("fechacertificadooriginal")]
        public string FechaCertificadoOriginalSerializado
        {
            get { return FechaCertificadoOriginal.HasValue ? FechaCertificadoOriginal.Value.ToString("yyyy-MM-dd") : string.Empty; }
            set { FechaCertificadoOriginal = XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Unspecified); }
        }

        [XmlElement("nrocertificadooriginal")]
        public string NroCertificadoOriginal { get; set; }

        [XmlElement("localidadprocedencia")]
        public string LocalidadProcedencia { get; set; }

        [XmlElement("localidaddescarga")]
        public string LocalidadDescarga { get; set; }

        [XmlElement("caratula")]
        public int Caratula { get; set; }

        [XmlElement("cantidadmuestrasoriginales")]
        public int CantidadMuestrasOriginales { get; set; }

        [XmlArray("cartasdeporte")]
        [XmlArrayItem("cartaporte")]
        public List<string> CartasDePorte { get; set; }

        [XmlElement("grupoensayos")]
        public string GrupoEnsayos { get; set; }

        public Muestra()
        {
            Camion = string.Empty;
        }
    }
}