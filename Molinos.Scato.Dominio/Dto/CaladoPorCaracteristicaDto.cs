using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public class CaladoPorCaracteristicaDto
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
        public bool NoAceptarSiSeDefineUnValor { get; set; }

        [DataMember]
        public bool EsHumedad { get; set; }

        [DataMember]
        public EnvioACamara CaracteristicaSituacionEnvioACamara { get; set; }

        [DataMember]
        public decimal? CaracteristicaSiSuperaValorCamara { get; set; }

        [DataMember]
        public string TipoDeEnsayo { get; set; }

        [DataMember]
        public TipoAnalisis TipoDeAnalisis { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Textos), Name = "Calado_AnalisisPreliminar")]
        public bool AnalisisPreliminar { get; set; }

        [DataMember]
        public bool HuboExcepcion { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Textos), Name = "Calado_Valor")]
        [RegularExpression(@"^[0-9]*(?:\,[0-9]*)?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public decimal? ValorCalado { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Textos), Name = "Calado_Unidad")]
        public string Unidad { get; set; }

        [DataMember]
        public bool EsModificable { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Textos), Name = "Calado_Rango")]
        public string Rango { get; set; }

        [DataMember]
        public decimal DescuentoEnKg { get; set; }
        [DataMember]
        public decimal DescuentoEnPorcentaje { get; set; }
        [DataMember]
        public bool EsMermaVolatil { get; set; }
        [DataMember]
        public bool EsAutomatizable { get; set; }
        [DataMember]
        public bool AnalisisAutomatico { get; set; }
        [DataMember]
        public bool EnviaASap { get; set; }
        [DataMember]
        public bool EnviaACamara { get; set; }
        
    }
}
