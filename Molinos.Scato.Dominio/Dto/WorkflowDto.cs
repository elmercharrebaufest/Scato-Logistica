using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class WorkflowDto
    {
        public int Id { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Codigo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [MaxLength(40, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Codigo { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Descripcion")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Descripcion { get; set; }

        public IList<TipoComercialDto> TiposComercialesAsociados { get; set; }
        public TipoDeWorkflow TipoDeWorkflow { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaActivacion { get; set; }
        public int CentroId { get; set; }
        public bool AplicaConsultaCupoDataAgro { get; set; }
    }
}