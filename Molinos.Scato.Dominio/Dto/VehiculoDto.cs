using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class VehiculoDto
    {
        public int Id { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_PatenteCamion")]
        [StringLength(10, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Patente { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_PatenteAcoplado")]
        [StringLength(10, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string PatenteAcoplado { get; set; }
        public string PatenteAcoplado2 { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_PesoTaraOrigen")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public int? PesoTaraOrigen { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_PesoBrutoOrigen")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public int? PesoBrutoOrigen { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_PesoNetoOrigen")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public int? PesoNetoOrigen { get; set; }

        public TipoVehiculo TipoVehiculo { get; set; }
        public int NumeroVehiculo { get; set; }
        public bool Primero { get; set; }
        public string DocumentoInternoSap { get; set; }
        public string NumeroDeDocumentoSap { get; set; }
        public int CartaPorteId { get; set; }
        public string NumCTG { get; set; }
        public string Sucural { get; set; }
        public string NumOrden { get; set; }
    }
}