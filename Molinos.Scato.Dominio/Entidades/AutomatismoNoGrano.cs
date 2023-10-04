using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class AutomatismoNoGrano : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }

        public virtual Calle CallePlanta { get; set; }

        public virtual Calle CallePlayaInterna { get; set; }

        public virtual bool Activo { get; set; }

        public virtual Almacen Almacen { get; set; }

        public virtual PuntoDeCarga PuntoDeCarga { get; set; }
    }
}