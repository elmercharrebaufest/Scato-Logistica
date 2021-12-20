using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class BalanzaModificacionModalidadDto
    {
        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "Balanza_Modalidad")]
        public Modalidad Modalidad { get; set; }

        [Required(ErrorMessageResourceType = typeof (Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int BalanzaId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Balanza_Descripcion")]
        public string BalanzaNombre { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Motivo { get; set; }

        public string NombreUsuarioResponsable { get; set; }
    }
}
 