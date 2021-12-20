using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    [DataContract]
    public sealed class AnalisisPorCaracteristicaDto
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Textos), Name = "Calado_Caracteristica")]
        public string Caracteristica { get; set; }

        [DataMember]
        public int CaracteristicaId { get; set; }

        [DataMember]
        public bool NoAceptarSiSeDefineUnValor { get; set; }

        [DataMember]
        public decimal? ToleranciaSinAnalisis { get; set; }

        [DataMember]
        public decimal? ToleranciaSinMensaje { get; set; }

        [DataMember]
        public decimal CaladoMinimo { get; set; }

        [DataMember]
        public bool EsHumedad { get; set; }

        [DataMember]
        public string TipoDeEnsayo { get; set; }

        [DataMember]
        public TipoAnalisis TipoDeAnalisis { get; set; }

        [DataMember]
        public string CaracteristicaCodigoSap { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Textos), Name = "CargaDeAnalisisDeCalidad_ValorCalado")]
        [RegularExpression(@"^[0-9]*(?:\,[0-9]*)?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public decimal? ValorCalado { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Textos), Name = "CargaDeAnalisisDeCalidad_ValorAnalisis")]
        [RegularExpression(@"^[0-9]*(?:\,[0-9]*)?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public decimal? ValorAnalisis { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Textos), Name = "Calado_Unidad")]
        public string Unidad { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Textos), Name = "Calado_Rango")]
        public string Rango { get; set; }

        [DataMember]
        public decimal DescuentoEnKg { get; set; }
        [DataMember]
        public decimal DescuentoEnPorcentaje { get; set; }

        [DataMember]
        public int AnalisisDeCalidadId { get; set; }

        public decimal Valor { 
            get { return ValorAnalisis ?? ValorCalado ?? 0; }
        }

        public bool HuboExcepcion { get; set; }
        public bool EsMermaVolatil { get; set; }

        public bool EsInsectosVivos { get; set; }
        [DataMember]
        public bool EnviaASap { get; set; }
        [DataMember]
        public bool EnviaACamara { get; set; }
    }
}