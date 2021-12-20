using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ReciboMunicipalDto
    {
        public int Id { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "ReciboMunicipal_Monto")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [RegularExpression(@"^[0-9]*(?:\,[0-9]*)?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public decimal Monto { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "ReciboMunicipal_Ordenanza")]
        [StringLength(15, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Ordenanza { get; set; }
        public int CentroId { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "ReciboMunicipal_FechaActivacion")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime FechaActivacion { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "ReciboMunicipal_TipoVehiculo")]
        public int? TipoVehiculoId { get; set; }
        public TipoVehiculo? TipoVehiculo { get; set; }

    }
}
