using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class MotivoQuiebreBarreraDto
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PuestoDeTrabajo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(35, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string PuestoTrabajo { get; set; }
        public int PuestoTrabajoId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Fecha")]
        public DateTime Fecha { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Hora")]
        public string Hora { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Motivo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(250, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Motivo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Patente")]
        public string Patente { get; set; }

        public bool Apertura { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Transportista")]
        public int TransportistaId { get; set; }
        public string Transportista { get; set; }

        public string FileName { get; set; }
    }
}