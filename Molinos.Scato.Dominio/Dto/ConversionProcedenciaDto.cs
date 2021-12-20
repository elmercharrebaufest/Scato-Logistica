using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ConversionProcedenciaDto
    {
        public int? Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "Camara")]
        public int CamaraId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Camara")]
        public string CamaraDesc { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "Procedencia")]
        public int? ProcedenciaId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Procedencia")]
        public string ProcedenciaDesc { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "TablaConversion_CodigoCamara")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [MaxLength(10, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string CodigoCamara { get; set; }
    }
}

