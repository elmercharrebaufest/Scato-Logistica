using Molinos.Scato.Dominio.Enums;
using System;

namespace Molinos.Scato.Web.Models
{
    public class TemplateViewModel
    {
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public int CentroId { get; set; }
        public NivelDetalleTemplate NivelDetalle { get; set; }
        public TipoDeWorkflow TipoMovimiento { get; set; }
    }

    public enum NivelDetalleTemplate
    {
        Registracion,
        ActualizacionTrazabilidad
    }
}