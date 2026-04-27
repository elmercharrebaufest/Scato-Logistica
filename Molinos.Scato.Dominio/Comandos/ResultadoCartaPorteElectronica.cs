using Molinos.Scato.Dominio.Dto;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoCartaPorteElectronica : Resultado
    {
        [DataMember]
        public CartaPorteDto Cpe { get; set; }

        [DataMember]
        public byte[] PdfImage { get; set; }

        public List<long> CTGsDeOperativo { get; set; }

        [DataMember]
        public byte[] PdfImageSustentable { get; set; }

        [DataMember]
        public string RutaImagen { get; set; }

        [DataMember]
        public List<MaterialDuplicadoCPE> Duplicados { get; set; } = new List<MaterialDuplicadoCPE>();
        
        [DataMember]
        public byte[] Pdf { get; set; }
    }

    public class MaterialDuplicadoCPE
    {
        public int MaterialId { get; set; }
        public string MaterialDescripcion { get; set; }
        public string CTG { get; set; }
    }
}