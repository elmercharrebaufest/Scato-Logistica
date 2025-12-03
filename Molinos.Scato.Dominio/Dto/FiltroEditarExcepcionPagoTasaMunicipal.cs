using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class FiltroEditarExcepcionPagoTasaMunicipal
    {
        [RegularExpression(@"(^[A-Z]{3}[0-9]{3}$)|(^[A-Z]{2}[0-9]{3}[A-Z]{2}$)",
        ErrorMessage = "La patente debe tener el formato AAA123 o AA123AA")]
        [StringLength(10, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [Display(Name = "Patente")]
        public string PatenteActual { get; set; }

        public int Id { get; set; }
    }
}