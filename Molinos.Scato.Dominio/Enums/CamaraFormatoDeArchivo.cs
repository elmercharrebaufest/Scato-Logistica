using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Enums
{

    public enum CamaraFormatoDeArchivo : int
    {
        [Display(Description = "")]
        NoEspecificado = 0,
        [Display(ResourceType = typeof(Textos), Name = "Rosario")]
        Rosario = 1,
        [Display(ResourceType = typeof(Textos), Name = "BahiaBlanca")]
        BahiaBlanca = 2,
        [Display(ResourceType = typeof(Textos), Name = "BuenosAires")]
        BuenosAires = 3
    }
}
