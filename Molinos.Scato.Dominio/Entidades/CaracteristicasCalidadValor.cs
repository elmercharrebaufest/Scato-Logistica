using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class CaracteristicasCalidadValor : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string Key { get; set; }
        public virtual string Data { get; set; }
        public virtual string Descuento { get; set; }
        public virtual string EnvioCamara { get; set; }
        public virtual ImpConstanciaDeEntregaLaser ImpConstanciaDeEntregaLaser { get; set; }
    }
}
