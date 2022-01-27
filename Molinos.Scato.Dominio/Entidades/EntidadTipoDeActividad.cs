using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class EntidadTipoDeActividad : IIdentificable
    {
        [Key]
        public virtual int Id { get; private set; }
        public virtual Entidad Entidad { get; set; }
        public virtual TipoDeActividad TipoDeActividad { get; set; }
    }
}
