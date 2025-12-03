using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class FiltroExcepcionPagoTasaMunicipal
    {
        [Required(ErrorMessage = "La patente es obligatoria.")]
        [RegularExpression(
            @"(^[A-Z]{3}[0-9]{3}$)|(^[A-Z]{2}[0-9]{3}[A-Z]{2}$)",
            ErrorMessage = "La patente debe tener el formato AAA123 o AA123AA")]
        [StringLength(10, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [Display(Name = "Patente")]
        public string Patente { get; set; }

        public FiltroEditarExcepcionPagoTasaMunicipal FiltroEditar { get; set; }
    }
}