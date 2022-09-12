using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class LoteInase : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public string NumeroDeLote { get; set; }
        public virtual Centro Centro { get; set; }
        public virtual ICollection<MuestraDeInase> Muestras { get; set; }
        public DateTime Fecha { get; set; }
        public virtual string NombreUsuario { get; set; }
    }
}
