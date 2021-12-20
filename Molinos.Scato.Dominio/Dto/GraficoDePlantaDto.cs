
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class GraficoDePlantaDto
    {
        public int Id { get; set; }
        public int CentroId { get; set; }
        public string NombreActividad { get; set; }
        public string NombreActividadDesc { get; set; }
        public int CantidadCamionesNoDemorados { get; set; }
        public int CantidadCamionesDemorados { get; set; }

        public string Color { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Range(0, 9999, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public int Rango { get; set; }

        public SectorEnum Sector { get; set; }
    }
}
