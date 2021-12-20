using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ObservacionDto
    {
        public int Id { get; set; }
        public Guid WorkflowInstanceId { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Observacion_Observaciones")]
        [MaxLength(1000, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Observaciones { get; set; }
    }
}
