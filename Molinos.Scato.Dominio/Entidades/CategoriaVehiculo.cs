using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    public class CategoriaVehiculo : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string Patente { get; set; }
        public virtual string PatenteAcoplado { get; set; }
        public virtual string PatenteAcoplado2 { get; set; }
        public virtual int TipoVehiculo { get; set; }
    }
}
