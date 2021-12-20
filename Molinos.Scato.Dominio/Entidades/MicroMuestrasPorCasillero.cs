using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class MicroMuestrasPorCasillero : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual MuestraEnvioACamara Muestra { get; set; }
        public virtual Casillero Casillero { get; set; }
        public DateTime Fecha { get; set; }
    }
}
