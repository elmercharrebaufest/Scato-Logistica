using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class AnalisisObligatorio : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual Material Material { get; set; }
        public virtual Centro Centro { get; set; }
        public virtual int IntervaloDeAnalisis { get; set; }
        public virtual DateTime? UltimoAnalisis { get; set; }

        [InverseProperty("AnalisisObligatoriosAsociados")]
        public virtual IList<PuestoDeTrabajo> PuestosDeTrabajoAsociados { get; set; }
        public virtual int AlertarAnalisisIntervalo { get; set; }
    }
}
