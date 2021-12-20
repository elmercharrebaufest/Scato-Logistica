using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("RegistroBalanzaPuerto")]
    public class RegistroBalanzaPuerto
    {
        [Key, Column(Order = 0)]
        public virtual int Id { get; set; }
        [Key, Column(Order = 1)]
        public virtual string NumeroBalanza { get; set; }
        public virtual string Tipo { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual bool EnviadoASap { get; set; }

    }
}
