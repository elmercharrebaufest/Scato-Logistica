using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Enums
{

    public enum TipoDeSoja : int
    {
        [Display(ResourceType = typeof(Textos), Name = "Todos")]
        Todos = 0,
        [Display(ResourceType = typeof(Textos), Name = "Sustentable")]
        Si = 1,
        [Display(ResourceType = typeof(Textos), Name = "NoSustentable")]
        No = 2,
    }
}
