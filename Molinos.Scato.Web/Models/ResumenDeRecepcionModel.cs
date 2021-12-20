using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Web.Models
{
    public class ResumenDeRecepcionModel
    {
        [Display(ResourceType = typeof(Textos), Name = "ResumenDeRecepcion_NumeroPedido")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Range(-1, 9999999999, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "NumeroInvalido")]
        public string NroPedido { set; get; }
        [Display(ResourceType = typeof(Textos), Name = "ResumenDeRecepcion_TipoDocumento")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string TipoDocumento { set; get; }
    }
}