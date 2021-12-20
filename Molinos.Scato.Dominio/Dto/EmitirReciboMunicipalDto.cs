using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class EmitirReciboMunicipalDto
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "TipoDocumentoIngreso")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int TipoDocumentoIngresoId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "NumeroDocumentoIngreso")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string NumeroDocumentoIngreso { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "NroTarjetaAcceso")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(10, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string NroTarjetaAcceso { get; set; }

        [NotMapped]
        [Display(ResourceType = typeof(Textos), Name = "NroTarjetaAcceso")]
        public bool NroTarjetaAccesoCB { get; set; }

        [NotMapped]
        [Display(ResourceType = typeof(Textos), Name = "NumeroDocumentoIngreso")]
        public bool NumeroDocumentoIngresoCB { get; set; }
       }
}
