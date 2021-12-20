using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class GraficoCamionesHoraDto
    {
        public int Id { get; set; }
        
        [Display(ResourceType = typeof(Textos), Name = "Material")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string MaterialDescripcion { get; set; }
        
        public int MaterialId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "FechaAComparar")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime FechaVieja { get; set; }

        public int[] CamionesActuales { get; set; }
        
        public int[] CamionesHistorico { get; set; }
    }
}