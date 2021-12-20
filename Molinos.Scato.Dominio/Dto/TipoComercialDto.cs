using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class TipoComercialDto
    {
        public int? Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Descripcion")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(40, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "TipoComercial_CodigoSap")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(3, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string CodigoSap { get; set; }

        
        [Display(ResourceType = typeof(Textos), Name = "TipoComercial_Sentido")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(1, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Sentido { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "TipoComercial_PesoEsperado")]
        [RegularExpression(@"[0-9]*$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public int? PesoEsperado { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "TipoComercial_ToleranciaDifPeso")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [RegularExpression(@"[0-9]*$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public int? ToleranciaDifPesoE { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "TipoComercial_UsaBinPallet")]
        public bool UsaBinPallet { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "TipoComercial_ValidaPatente")]
        public bool ValidaPatente { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "TipoComercial_TransportistaEsProveedor")]
        public bool TransportistaEsProveedor { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "TipoComercial_PesoMaximoDocumentoIngreso")]
        [RegularExpression(@"[0-9]*$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public int? PesoMaximoDocumentoIngreso { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "TipoComercial_NoRechazaEnCalado")]
        public bool NoRechazaEnCalado { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "TipoComercial_EsParaUva")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public TipoUva EsParaUva { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "TipoComercial_ValidaStockEPA")]
        public bool ValidaStockEPA { get; set; }
    }
}