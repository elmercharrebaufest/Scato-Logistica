using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class FiltroArchivoDeMovimientosDto
    {
        [Display(ResourceType = typeof(Textos), Name = "Material")]
        public int MaterialId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Material { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Movimiento")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public TipoDeWorkflow? TipoDeWorkflow { get; set; }

        public string NombreUsuario { get; set; }

        public int CentroId { get; set; }

        public string CentroDesc { get; set; }

        public System.DateTime Fecha { get; set; }
    }
}
