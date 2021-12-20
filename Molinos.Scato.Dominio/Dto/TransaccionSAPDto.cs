using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class TransaccionSAPDto
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "DescripcionCorta")]
        [StringLength(8, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string DescripcionCorta { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "TransaccionSAP_Centro")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string CentroOrigenId { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "TransaccionSAP_Centro")]
        public string CentroOrigenDesc { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string MaterialDesc { get; set; }
        public int MaterialId { get; set; }
        public string MaterialCodigoSAP { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "TipoComercial")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int TipoComercialId { get; set; }
        public string TipoComercialDesc { get; set; }
        
        [Display(ResourceType = typeof(Textos), Name = "FuncionSAP")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public FuncionSAP FuncionSAP { get; set; }
    }
}
