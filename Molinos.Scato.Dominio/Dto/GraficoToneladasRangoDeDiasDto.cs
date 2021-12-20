using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class GraficoToneladasRangoDeDiasDto
    {
        public int Id { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Material")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string MaterialDescripcion { get; set; }
        public int MaterialId { get; set; }

        public List<ToneladasPorDiaDto> Toneladas { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Desde")]
        public DateTime FechaDesde { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Hasta")]
        public DateTime FechaHasta { get; set; }
    }


}