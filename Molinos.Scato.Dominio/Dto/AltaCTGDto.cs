using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class AltaCTGDto
    {
        public int? Id { get; set; }
        public DateTime Fecha { get; set; }
        public int CartaPorteId { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [StringLength(12, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo", MinimumLength = 8)]
        [Display(ResourceType = typeof(Textos), Name = "AltaCTG_CodigoDeAlta")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string CodigoCTG { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_TarifaReferencia")]
        [RegularExpression(@"^[0-9]*(?:\,[0-9]*)?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public decimal? TarifaReferencia { get; set; }

        public Guid WorkflowId { get; set; }
        public string WorkflowCodigo { get; set; }
        public string Errores { get; set; }
        public bool Cpe { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Sucursal_CPE")]
        public string Sucursal { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Numero_DeOrden")]
        public string NroOrden { get; set; }
        public int RecorridoId { get; set; }

    }
}