using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class BuscarArchivoDeMovimientoDto
    {
        [Display(ResourceType = typeof(Textos), Name = "FechaDesde")]
        public DateTime? FechaDesde { get; set; }
         [Display(ResourceType = typeof(Textos), Name = "FechaHasta")]
        public DateTime? FechaHasta { get; set; }
         [Display(ResourceType = typeof(Textos), Name = "NumeroDocumentoIngreso")]
        public string NumeroDocumentoIngreso { get; set; }
        public int MaterialId { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Material")]
        public string Material { get; set; }
        public int CentroId { get; set; }
    }
}