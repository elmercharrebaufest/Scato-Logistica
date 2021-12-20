using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ConsultaCasilleroDto
    {
        //Casilleros por Casillero
        [Display(ResourceType = typeof(Textos), Name = "ConsultaCasillero_Desde")]
        public string Desde { set; get; }

        [Display(ResourceType = typeof(Textos), Name = "ConsultaCasillero_Hasta")]
        public string Hasta { set; get; }

        public int CentroId { set; get; }
        public string NDeCasillero { get; set; }
        public string CantMuestra { get; set; }
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string Patente { get; set; }

    }
}
