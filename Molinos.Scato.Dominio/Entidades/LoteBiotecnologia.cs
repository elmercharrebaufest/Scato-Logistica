using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class LoteBiotecnologia : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string NombreUsuario { get; set; }
        public virtual Camara Camara { get; set; }
        public virtual Centro Centro { get; set; }
        public virtual IList<MuestraEnvioACamaraBiotecnologia> Muestras { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual string NumeroDeLote { get; set; }
        public virtual DateTime FechaDesde { get; set; }
        public virtual DateTime FechaHasta { get; set; }
        public virtual Material Material { get; set; }
    }
}

