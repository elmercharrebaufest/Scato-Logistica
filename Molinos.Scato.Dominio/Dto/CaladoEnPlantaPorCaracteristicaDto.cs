using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public class CaladoEnPlantaPorCaracteristicaDto
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Textos), Name = "Calado_Caracteristica")]
        public string Caracteristica { get; set; }

        [DataMember]
        public int CaracteristicaId { get; set; }

        [DataMember]
        public string CaracteristicaCodigoSap { get; set; }

        [DataMember]
        public string CaracteristicaDescripcionCorta { get; set; }
        
        [DataMember]
        public bool EsHumedad { get; set; }

        public decimal? ValorCalado { get; set; }

        public decimal? ValorAutomatico { get; set; }

        public int NroDeToma { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Textos), Name = "Calado_Valor")]
        [RegularExpression(@"^[0-9]*(?:\,[0-9]*)?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public decimal? ValorCaladoEnPlanta { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Textos), Name = "Calado_Unidad")]
        public string Unidad { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Textos), Name = "Calado_Rango")]
        public string Rango { get { return RangoMin.ToString("g0") + " - " + RangoMax.ToString("g0") + " " + Unidad; } }

        public bool EsAutomatizable { get; set; }
        public bool AnalisisAutomatico { get; set; }
        public decimal RangoMin { get; set; }
        public decimal RangoMax { get; set; }
        public bool CaladoObligatorio { get; set; }
        public TipoDispositivo Dispositivo { get; set; }
        public string CaracteristicaNombreNirs { get; set; }
        public Modalidad Modalidad { get; set; }
    }
}
