using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Web.Models
{
    public class CasilleroModel
    {
        public CasilleroDto Casillero { get; set; }
        public string Hasta { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Casillero_CrearMasivo")]
        public bool Masivo { get; set; }
    }
}