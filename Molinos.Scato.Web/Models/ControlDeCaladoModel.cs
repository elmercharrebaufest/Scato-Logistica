using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Web.Models
{
    public class ControlDeCaladoModel
    {
        [Display(ResourceType = typeof(Textos), Name = "FechaDesde")]
        public System.DateTime FechaDesde { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "FechaHasta")]
        public System.DateTime FechaHasta { get; set; }

        public List<EstadisticasCaladoDto> EstadisticasCalado { get; set; }
        public double CaladosPorHora { get; set; }
        public RecorridoDto UltimoCamion { get; set; }

        public PinchazosPorCaladaDto UltimoCambioPinchazo { get; set; }
        public bool CambiarPinchazos { get; set; }
    }
}