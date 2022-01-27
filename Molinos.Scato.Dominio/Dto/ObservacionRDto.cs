using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ObservacionRDto
    {
        public int Id { get; set; }
        public Guid WorkflowInstanceId { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Observacion_Observaciones")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [RegularExpression(@"^.{10,}$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMinimo")]
        [StringLength(1000, MinimumLength = 10, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Observaciones { get; set; }
        public bool Rechazado{ get; set; }
    }
}
