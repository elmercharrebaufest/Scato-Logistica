using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Web.Models
{
    public class MicromuestraModel
    {

        [Display(ResourceType = typeof(Textos), Name = "Centro")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Centro { set; get; }
        public int CentroId { set; get; }
        [Display(ResourceType = typeof(Textos), Name = "Micromuestra_CodigoBarras")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string CodigoBarras { set; get; }
        [Display(ResourceType = typeof(Textos), Name = "Impresora")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int ImpresoraId { set; get; }
    }
}