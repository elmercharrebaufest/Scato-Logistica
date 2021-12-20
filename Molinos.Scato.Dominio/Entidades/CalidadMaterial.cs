using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class CalidadMaterial : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual MaterialPorCentro MaterialPorCentro { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual bool TieneAnalisis { get; set; }
        public virtual decimal ValorDesde { get; set; }
        public virtual decimal ValorHasta { get; set; }
    }
}
