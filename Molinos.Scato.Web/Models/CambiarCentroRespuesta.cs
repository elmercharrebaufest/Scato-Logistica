using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Web.Models
{
    public class SeleccionarCentroRespuesta
    {
        public int CentroId { set; get; }
        [Display(ResourceType = typeof(Textos), Name = "Descripcion" )]
        public string CentroDescripcion { set; get; }
        [Display(ResourceType = typeof(Textos), Name = "Centro_Localidad")]
        public string Localidad { set; get; }
        [Display(ResourceType = typeof(Textos), Name = "Centro_Provincia")]
        public string Provincia { set; get; }
    }
}