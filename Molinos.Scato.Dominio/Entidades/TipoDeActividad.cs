using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class TipoDeActividad : IIdentificable
    {
        [Key]
        public virtual int Id { get; private set; }
        public virtual string Codigo { get; set; }
        public virtual string Descripcion { get; set; }
    }
}
