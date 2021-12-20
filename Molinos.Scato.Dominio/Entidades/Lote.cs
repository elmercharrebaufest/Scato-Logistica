using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Lote : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public string NumeroDeLote { get; set; }
        public virtual Camara Camara { get; set; }
        public virtual IList<MuestraEnvioACamara> Muestras { get; set; }
        public DateTime Fecha { get; set; }
    }
}
