using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Web.Models
{
    public class FiltroRecorridoModel
    {
        [Display(ResourceType = typeof(Textos), Name = "TipoDocumentoIngreso")]
        public TipoDocumentoIngreso TipoDocumentoIngreso { set; get; }
        [Display(ResourceType = typeof(Textos), Name = "NumeroDocumentoIngreso")]
        public string NumeroDocumentoIngreso { set; get; }
        [Display(ResourceType = typeof(Textos), Name = "Camion_Patente")]
        public string Patente { set; get; }
    }
}