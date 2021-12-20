using Molinos.Scato.Dominio.Enums;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class CaracteristicasAnalizadas : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual Recorrido Recorrido { get; set; }
        public virtual bool EsHumedad { get; set; }
        public virtual decimal? Humedad { get; set; }
        public virtual bool EsGranosVerdes { get; set; }
        public virtual bool EsGranosDañados { get; set; }
        public virtual bool EsCuerposExtranos { get; set; }
        public virtual bool EsProteinaBaja { get; set; }
        public virtual bool EsProteinaAlta { get; set; }
        public virtual bool TieneDescuentos { get; set; }
        public virtual bool TieneInsectosVivos { get; set; }
        public virtual bool CaracteristicasNoCorrenspodenEspecial { get; set; }
        public virtual decimal? Grado { get; set; }
        public virtual TipoCalidad Calidad { get; set; }
    }
}
