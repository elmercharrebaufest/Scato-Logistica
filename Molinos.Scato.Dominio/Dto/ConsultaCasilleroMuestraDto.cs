using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ConsultaCasilleroMuestraDto
    {
        [Display(ResourceType = typeof(Textos), Name = "Centro")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Centro { set; get; }
        public int CentroId { set; get; }

        [Display(ResourceType = typeof(Textos), Name = "ConsultaCasillero_TipoDocumento")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public TipoDocumentoIngreso? TipoDocumento { set; get; }

        [Display(ResourceType = typeof(Textos), Name = "ConsultaCasillero_NumeroDocumento")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string NumeroDocumento { set; get; }

        [Display(ResourceType = typeof(Textos), Name = "ConsultaCasillero_Patente")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Patente { set; get; }

        public string NDeCasillero { set; get; }
        public string CantMuestra { set; get; }
    }
}
