using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    [DataContract]
    public sealed class CalidadCaracteristicaDto
    {
        [Display(ResourceType = typeof(Textos), Name = "Calado_Caracteristica")]
        public string Caracteristica { get; set; }

        public string CaracteristicaId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CargaDeAnalisisDeCalidad_ValorCalado")]
        public decimal? ValorCalado { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CargaDeAnalisisDeCalidad_ValorAnalisis")]
        public decimal? ValorAnalisis { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Calado_Unidad")]
        public string Unidad { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Calidad_MaximoPermitido")]
        public string MaximoPermitido { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Calidad_DescuentoKg")]
        public decimal DescuentoEnKg { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Calidad_DescuentoPorcentaje")]
        public decimal DescuentoEnPorcentaje { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Calidad_SeEnvioACamara")]
        public bool SeEnvioACamara { get; set; }

        public string CodigoSap { get; set; }
    }
}