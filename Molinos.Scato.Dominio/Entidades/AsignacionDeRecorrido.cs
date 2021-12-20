using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class AsignacionDeRecorrido : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }

        [Required] 
        public virtual DateTime FechaDesde { get; set; }

        [Required]
        public virtual DateTime FechaHasta { get; set; }

        [Required]
        public virtual MaterialPorCentro MaterialPorCentro { get; set; }

        public virtual Workflow Workflow { get; set; }

        public virtual CalidadMaterial Calidad { get; set; }

        [Required]
        public virtual Calle Calle { get; set; }

        public virtual Balanza BalanzaBruto { get; set; }

        
        public virtual Balanza BalanzaTara { get; set; }

        [Required]
        public virtual Almacen AlmacenDestino { get; set; }

        [Required]
        public virtual Centro Centro { get; set; }

        [InverseProperty("AsignacionesDeRecorrido")]
        public virtual ICollection<PuestosDeCargaDescarga> PuestosDeCargaDescargas { get; set; }
    }
}
