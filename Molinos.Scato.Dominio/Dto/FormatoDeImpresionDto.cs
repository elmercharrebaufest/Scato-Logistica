using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class FormatoDeImpresionDto
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "FormatoDeImpresion_Descripcion")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(50, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Descripcion { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "FormatoDePapel")]
        public int FormatoDePapelId { get; set; }
        public string FormatoDePapelDescripcion { get; set; }
        public int FormatoDePapelAlto { get; set; }
        public int FormatoDePapelAncho { get; set; }
        public int FormatoDePapelCodigoTipoPapel { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "FormatoDeImpresion_Posicion")]
        public Posicion Posicion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "FormatoDeImpresion_MargenIzquierdo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public int MargenIzquierdo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "FormatoDeImpresion_MargenSuperior")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public int MargenSuperior { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "FormatoDeImpresion_Filas")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public int Filas { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "FormatoDeImpresion_Columnas")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public int Columnas { get; set; }

        public List<FormatoDeCampoDto> FormatosDeCampo { get; set; }
    }
}
