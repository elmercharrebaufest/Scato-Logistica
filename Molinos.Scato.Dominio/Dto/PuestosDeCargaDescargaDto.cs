using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class PuestosDeCargaDescargaDto
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Descripcion")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(35, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Nombre { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Codigo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(5, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Codigo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PuestoDeTrabajo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int PuestoDeTrabajoId { get; set; }

        public string PuestoDeTrabajo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "EsSojaSustentable")]
        public bool EsSojaSustentable { get; set; }

        public int CentroId { get; set; }
    }
}