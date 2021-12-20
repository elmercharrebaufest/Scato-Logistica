using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    public class DensidadPorTemperaturaDeMaterial : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual MaterialPuerto MaterialPuerto { get; set; }
        public virtual int Grado { get; set; }
        public virtual decimal Densidad { get; set; }
    }
}
