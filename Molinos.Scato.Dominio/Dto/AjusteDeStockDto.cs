using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class AjusteDeStockDto
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "TipoDocumentoIngreso")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int TipoComprobanteOnccaId { get; set; }
        public string TipoComprobanteOnccaDesc { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "NumeroDocumentoIngreso")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string NumeroDocumentoIngreso { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CTG")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string NumeroCTG { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Fecha")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime Fecha { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int MaterialId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "Material")]
        public string MaterialDesc { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "AjusteDeStock_PesoBrutoIngreso")]
        [RegularExpression(@"^[0-9]*(?:\,[0-9]*)?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public decimal? PesoBrutoIngreso { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "AjusteDeStock_PesoNetoIngreso")]
        [RegularExpression(@"^[0-9]*(?:\,[0-9]*)?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public decimal? PesoNetoIngreso { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "AjusteDeStock_PesoNetoEgreso")]
        [RegularExpression(@"^[0-9]*(?:\,[0-9]*)?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public decimal? PesoNetoEgreso { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Romaneo_Observaciones")]
        [StringLength(30, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Observaciones { get; set; }

        public int CentroId { get; set; }
        public string NombreUsuario { get; set; }
    }
}
