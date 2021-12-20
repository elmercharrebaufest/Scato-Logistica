using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto

{
    public class TalonarioDto
    {

        public int Id { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Talonario_Descripcion")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Talonario_Sucursal")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int Sucursal { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Talonario_PrimerNumero")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int PrimerNumero { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Talonario_UltimoNumero")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int UltimoNumero { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Talonario_ProximoNumero")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int ProximoNumero { get; set; }

        public int CentroId { get; set; }
    }
}
