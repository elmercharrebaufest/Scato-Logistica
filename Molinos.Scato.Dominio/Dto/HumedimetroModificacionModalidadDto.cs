using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class HumedimetroModificacionModalidadDto
    {
        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "Balanza_Modalidad")]
        public Modalidad Modalidad { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int HumedimetroId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Balanza_Descripcion")]
        public string HumedimetroDescripcion { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime Fecha { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Humedimetro_MotivoModalidad")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(500, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Motivo { get; set; }

        public string NombreUsuarioResponsable { get; set; }
    }
}
