using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class PuntoDeCarga : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }

        [StringLength(100)]
        public virtual string Descripcion { get; set; }

        public virtual bool? EstadoAutomatismo { get; set; }
        public virtual int? CantidadMaximaDeCamiones { get; set; }
        public virtual bool Borrado { get; set; }

        public virtual DateTime FechaCreacion { get; set; }

        public virtual DateTime? FechaModificacion { get; set; }

        [StringLength(50)]
        public virtual string CreadoPor { get; set; }

        [StringLength(50)]
        public virtual string ModificadoPor { get; set; }

        [InverseProperty("PuntosDeCargaAsociados")]
        public virtual IList<AutomatismoNoGrano> AutomatismoNoGranoAsociados { get; set; }
    }
}