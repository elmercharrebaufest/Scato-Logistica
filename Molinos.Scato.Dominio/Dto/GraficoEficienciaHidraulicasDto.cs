using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class GraficoEficienciaHidraulicasDto
    {
        public int Id { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Material")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string MaterialDescripcion { get; set; }
        public int MaterialId { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "FechaABuscar")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime Fecha { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "FechaDesde")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime FechaHoraDesde { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "FechaHasta")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime FechaHoraHasta { get; set; }
        public List<ClaveStringValorMaterialDto> CamionesPorHidraulicaMaterial { get; set; }
        public string[] HidraulicasCodes { get; set; }
    }
}