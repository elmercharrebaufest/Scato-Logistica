using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class AnalisisDeCalidadDto
    {
        public int Id { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "CargaDeAnalisisDeCalidad_NroOrden")]
        public string NumeroOrden { get; set; }
        public Guid WorkflowInstanceId { get; set; }
        public IList<AnalisisPorCaracteristicaDto> CaracteristicasAnalizadas { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string Usuario { get; set; }
    }
}