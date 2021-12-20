using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class LiberacionDeCasillerosDto
    {
        [Display(ResourceType = typeof(Textos), Name = "LiberacionDeCasilleros_DiasDeAntiguedad")]
        //[Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string DiasDeAntiguedad { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro")]
        //[Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Centro { set; get; }
        public int CentroId { set; get; }

        [Display(ResourceType = typeof(Textos), Name = "LiberacionDeCasilleros_Desde")]
        //[Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Desde { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "LiberacionDeCasilleros_Hasta")]
        public string Hasta { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "LiberacionDeCasilleros_ImprimirMuestrasAEliminar")]
        public bool ImprimirMuestraAEliminar { get; set; }

        public int Id { get; set; }
        public string NDeCasillero { get; set; }
        public string TipoDocumentoIngreso { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "NumeroDocumentoIngreso")]
        public string NumeroDocumento { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Patente")]
        [StringLength(10, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Patente { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "TipoDocumentoIngreso")]
        public TipoDocumentoIngreso? TipoDocumento { get; set; } 
    }
}