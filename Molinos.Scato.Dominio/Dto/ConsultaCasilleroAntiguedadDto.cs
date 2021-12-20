using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ConsultaCasilleroAntiguedadDto
    {
        //Casilleros por Antiguedad
        [Display(ResourceType = typeof(Textos), Name = "ConsultaCasillero_DiasDeAntiguedad")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string DiasDeAntiguedad { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro")]
        public string Centro { set; get; }

        [Display(ResourceType = typeof(Textos), Name = "ConsultaCasillero_OcupadosMas")]
        public string OcupadosMas { set; get; }

        [Display(ResourceType = typeof(Textos), Name = "ConsultaCasillero_OcupadosMenos")]
        public string OcupadosMenos { set; get; }

        [Display(ResourceType = typeof(Textos), Name = "ConsultaCasillero_Libres")]
        public string Libres { set; get; }

        public DateTime Fecha { set; get; }
        public string Casillero { set; get; }
    }
}
