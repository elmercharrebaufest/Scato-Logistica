using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class AutorizacionTiempoEnTransitoDto
    {
        public int Id { get; set; }
        public Guid WorkflowInstanceId { get; set; }
        public string NombreUsuario { get; set; }
        public string Actividad { get; set; }
        [MaxLength(100, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Mensaje { get; set; }
        public bool Decision { get; set; }
        public DateTime Fecha { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "DocumentoIngreso")]
        public string DocumentoIngreso { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "NroDocumentoIngreso")]
        public string NroDocumentoIngreso { get; set; }
        public string Patente { get; set; }
        public string Material { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "ExcesoDeTiempo_Actividad1")]
        public string Actividad1 { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "ExcesoDeTiempo_Actividad2")]
        public string Actividad2 { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "ExcesoDeTiempo_TiempoAceptado")]
        public string TiempoAceptado { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "ExcesoDeTiempo_TiempoEnTransito")]
        public string TiempoEnTransito { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "ExcesoDeTiempo_Fecha1")]
        public string FechaActividad1 { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "ExcesoDeTiempo_Fecha2")]
        public string FechaActividad2 { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "ExcesoDeTiempo_DiferenciaTiempo")]
        public string DiferenciaTiempo { get; set; }
        [RegularExpression(@"^.{100,}$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMinimo100")]
        [StringLength(1000, MinimumLength = 10, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Comentario { get; set; }
        public string Error { get; set; }
    }
}
