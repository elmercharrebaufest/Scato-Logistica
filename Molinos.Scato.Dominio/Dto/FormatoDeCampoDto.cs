using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class FormatoDeCampoDto
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Campo")]
        public string CampoDescripcion { get; set; }
        public string CampoDireccion { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int CampoId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int FormatoDeImpresionId { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "FormatoDeImpresion")]
        public string FormatoDeImpresionDescripcion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "FormatoDeCampo_Letra")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string LetraDescripcion { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int LetraId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "FormatoDeCampo_Alineacion")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public Alineacion Alineacion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "FormatoDeCampo_Fila")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int Fila { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "FormatoDeCampo_Columna")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int Columna { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "FormatoDeCampo_Tamaño")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int Tamaño { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "FormatoDeCampo_Negrita")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public bool Negrita { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "FormatoDeCampo_Cursiva")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public bool Cursiva { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "FormatoDeCampo_Subrayado")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public bool Subrayado { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "FormatoDeCampo_EsColumna")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public bool EsColumna { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "FormatoDeCampo_Texto")]
        public string Texto { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "FormatoDeCampo_TituloOncca")]
        public string TituloOncca { get; set; }

        public bool _destroy { get; set; }
        public bool EsNuevo { get; set; }
        public TipoDeCampo TipoDeCampo { get; set; }
    }
}
