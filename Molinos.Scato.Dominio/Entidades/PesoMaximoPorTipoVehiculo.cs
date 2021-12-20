using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    public class PesoMaximoPorTipoVehiculo : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual TipoVehiculo TipoVehiculo { get; set; }
        public virtual int PesoMaxIngreso { get; set; }
        public virtual int PesoMaxEgreso { get; set; }
        public virtual int? PesoNetoMaxPlanta { get; set; }
        public virtual int? PesoNetoMinimo { get; set; }
        public virtual bool Activo { get; set; }
        public virtual Centro Centro { get; set; }
        
    }
}
