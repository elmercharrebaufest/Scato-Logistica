using Molinos.Scato.Dominio.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class CallePorRecorrido : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual Calle Calle { get; set; }
        public virtual Recorrido Recorrido { get; set; }
        public virtual CargaDeCupo CargaDeCupo { get; set; }
        public virtual DateTime FechaIngeso { get; set; }
        public virtual DateTime? FechaEgreso { get; set; }
        public virtual bool UltimoDeLaFila { get; set; }
    }
}
