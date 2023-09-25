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

        [InverseProperty("AutomatismoNoGranoAsociados")]
        public virtual IList<Almacen> AlmacenesAsociados { get; set; }

        [InverseProperty("AutomatismoNoGranoAsociados")]
        public virtual IList<PuntoDeCarga> PuntosDeCargaAsociados { get; set; }
    }
}