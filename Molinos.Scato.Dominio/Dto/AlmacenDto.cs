using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class AlmacenDto
    {
        public int Id { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Almacen_CodigoSAP")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloAlfanumerico")]
        [StringLength(20, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string CodigoSAP { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Descripcion")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(30, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Descripcion { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "DescripcionCorta")]
        [StringLength(8, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string DescripcionCorta { get; set; }
        public int CentroId { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Material_CodigoONCCA")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [StringLength(6, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string CodigoONCCA { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Almacen_EsTanqueVino")]
        public bool EsTanqueVino { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "EsSojaSustentable")]
        public bool EsSojaSustentable { get; set; }
        public bool? EsNuevo { get; set; }
        public bool FueEliminado { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Establecimiento_SojaEPA")]
        public bool EsSojaEPA { get; set; }
    }
}
