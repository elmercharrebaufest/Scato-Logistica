using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class CaracteristicasAnalizadasDto
    {
        public int Id { get; set; }
        public bool EsHumedad { get; set; }
        public decimal? Humedad { get; set; }
        public bool EsGranosVerdes { get; set; }
        public bool EsGranosDañados { get; set; }
        public bool EsCuerposExtranos { get; set; }
        public bool EsProteinaBaja { get; set; }
        public bool EsProteinaAlta { get; set; }
        public bool TieneDescuentos { get; set; }
        public bool TieneInsectosVivos { get; set; }
        public bool CaracteristicasNoCorrenspodenEspecial { get; set; }
    }
}
