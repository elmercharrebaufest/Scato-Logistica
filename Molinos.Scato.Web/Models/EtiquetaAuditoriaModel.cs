using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Web.Models
{
    public class EtiquetaAuditoriaModel
    {
        [Display(ResourceType = typeof(Textos), Name = "Centro")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Centro { set; get; }
        public int CentroId { set; get; }
        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_NroCartaPorte")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string NumeroCartaPorte { set; get; }
        [Display(ResourceType = typeof(Textos), Name = "Impresora")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int ImpresoraId { set; get; }
    }
}