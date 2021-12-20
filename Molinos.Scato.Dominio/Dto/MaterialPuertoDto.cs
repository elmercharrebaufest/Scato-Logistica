using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class MaterialPuertoDto
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material_CodigoSAP")]
        [StringLength(100, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string CodigoSAP { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Descripcion")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(200, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Descripcion { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string DescripcionCorta { get; set; }
        
        [Display(ResourceType = typeof(Textos), Name = "Material_AlmacenOrigen")]
        public int? Almacen_Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material_AlmacenOrigen")]
        public string AlmacenDesc { get; set; }

        public bool EsLiquido { get; set; }
        public string Color { get; set; }
    }
}