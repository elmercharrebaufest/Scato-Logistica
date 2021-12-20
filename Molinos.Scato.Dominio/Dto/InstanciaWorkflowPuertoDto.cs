using Molinos.Scato.Dominio.Recursos;
using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class InstanciaWorkflowPuertoDto
    {
        [Display(ResourceType = typeof(Textos), Name = "Workflow_InstanceId")]
        public Guid Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Workflow_ProximaAccion")]
        public string ProximaAccion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Workflow_FechaUltimaModificacion")]
        public DateTime? FechaUltimaModificacion { get; set; }
        public bool Rechazado { get; set; }

        public EmbarqueDto Embarque { get; set; }
        public LineUpDto LineUp { get; set; }
    }
}