using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class RangosDeRedondeo : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual decimal ValorDesde { get; set; }
        public virtual decimal ValorHasta { get; set; }
        public virtual decimal ValorRedondeado { get; set; }
        public virtual MaterialPorCentro MaterialPorCentro { get; set; }
    }
}
