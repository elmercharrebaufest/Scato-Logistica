using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public class CaladoDto
    {
        public int Id { get; set; }
        public Guid WorkflowInstanceId { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Calado_Ciclo")]
        public int CicloDeCalado { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Calado_MuestraConjunto")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public int? MuestraConjunto { get; set; }
        public string NumeroOrden { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Calado_EstaAutorizadoPorEntregador")]
        public bool EstaAutorizadoPorEntregador { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public IList<CaladoPorCaracteristicaDto> CaladosPorCaracteristica { get; set; }
        public AnalisisDeCalidadDto AnalisisDeCalidad { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Calidad")]
        public int CalidadMaterialId { get; set; }
        public string CalidadMaterialDescripcion { get; set; }
        public string Usuario { get; set; }
        public string Comentario { get; set; }

        public bool PideAnalisis {
            get
            {
                return CaladosPorCaracteristica.Any(caladoPorCaracteristicaDto => caladoPorCaracteristicaDto.AnalisisPreliminar);
            }
        }
        public bool TieneDescuentos {
            get { return CaladosPorCaracteristica.Any(x => x.DescuentoEnPorcentaje > 0 || x.DescuentoEnKg > 0); }
        }
    }
}
