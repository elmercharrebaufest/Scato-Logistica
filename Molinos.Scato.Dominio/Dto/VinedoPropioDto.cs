using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class VinedoPropioDto
    {
        public int Id { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "VinedoPropio_NumeroINV")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [RegularExpression(@"^[\w\-]*$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloAlfanumerico")]
        [StringLength(8, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string NumeroINV { get; set; }
        public List<CuartelDto> Cuarteles { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "VinedoPropio_Descripcion")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(30, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Descripcion { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_VinedoPropio_CuartelRequerido")]
        public string CuartelesJson { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "VinedoPropio_IngresosBrutos")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [RegularExpression(@"^\d*?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [StringLength(10, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string IngresosBrutos { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "SubZona")]
        public string SubZona { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "SubZona")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int? SubZonaId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Zona")]
        public string Zona { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Zona")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int? ZonaId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(40, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [Display(ResourceType = typeof(Textos), Name = "Calidad")]
        public string Calidad { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "VinedoPropio_CentroOperativo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(10, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string CentroOperativo { get; set; }
    }
}
