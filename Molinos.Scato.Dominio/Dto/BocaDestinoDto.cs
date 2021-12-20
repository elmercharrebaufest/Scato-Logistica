using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class BocaDestinoDto
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "BocaDestino_Entregador")]
        public string Proveedor { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int ProveedorId { get; set; }

        [Display(ResourceType = typeof (Textos), Name = "BocaDestino_Nombre")]
        [MaxLength(30, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [Required(ErrorMessageResourceType = typeof (Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string NombreBocaDeDestino { get; set; }

        [Display(ResourceType = typeof (Textos), Name = "BocaDestino_CodigoPostal")]
        [MaxLength(10, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public string CodigoPostal { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material_CodigoONCCA")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [MaxLength(6, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public string CodigoONCCA { get; set; }

        [Display(ResourceType = typeof (Textos), Name = "Transportista_Domicilio")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [MaxLength(35, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Domicilio { get; set; }

        [Display(ResourceType = typeof (Textos), Name = "Transportista_Localidad")]
        public string Localidad { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int LocalidadId { get; set; }

        [Display(ResourceType = typeof (Textos), Name = "Transportista_Provincia")]
        public string Provincia { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int ProvinciaId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Transportista_Pais")]
        public string Pais { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int PaisId { get; set; }
    }
}
