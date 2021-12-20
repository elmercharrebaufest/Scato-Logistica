using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Web.Models
{
    public class ImpresionTarjetaDeAccesoModel
    {
        [Display(ResourceType = typeof(Textos), Name = "NumeroTarjeta")]
        [RegularExpression(@"\d{10,10}", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Solo10Digitos")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(10, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Numero { set; get; }
    }
}