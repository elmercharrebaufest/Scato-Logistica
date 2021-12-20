using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    public class DescargaUnidad : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string NroPedido { get; set; }
        public virtual string Observaciones { get; set; }
        public virtual EstadoDescargaUnidad Estado { get; set; }
        [InverseProperty("DescargaUnidad")]
        public virtual ICollection<DescargaUnidadItem> DescargaUnidadItems { get; set; }
        [InverseProperty("DescargaUnidad")]
        public virtual ICollection<DescargaUnidadItemPedido> DescargaUnidadItemPedidos { get; set; }
        public virtual DateTime FechaInicio { get; set; }
        public virtual DateTime? FechaCierre { get; set; }
        [Required]
        public virtual Guid WorkflowInstanceId { get; set; }
        public virtual Proveedor Proveedor { get; set; }
    }
}

