using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class BajaCTGDto
    {
        public int? Id { get; set; }
        public DateTime Fecha { get; set; }
        public int CartaPorteId { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [MaxLength(12, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [Display(ResourceType = typeof(Textos), Name = "BajaCTG_CodigoDeBaja")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string CodigoDeBaja { get; set; }
        public string CodigoDeBajaDefinitivo { get; set; }
        public Guid WorkflowId { get; set; }
        public string WorkflowCodigo { get; set; }
        public string Errores { get; set; }
        public bool Cpe { get; set; }
        public int OrdenDeDescargaFasonId { get; set; }
    }
}