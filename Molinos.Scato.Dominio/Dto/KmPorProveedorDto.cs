using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class KmPorProveedorDto
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Cliente")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int ClienteId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Cliente")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string ClienteDescripcion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_Provincia")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int ProvinciaId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_Provincia")]
        public string ProvinciaDescripcion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_Localidad")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int LocalidadId { get; set; }
        
        [Display(ResourceType = typeof(Textos), Name = "Centro_Localidad")]
        public string LocalidadDescripcion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int CentroId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro")]
        public string CentroDescripcion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "AdministracionDistancia_KmARecorrer")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [StringLength(4, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string KmARecorrer { get; set; }

    }
}