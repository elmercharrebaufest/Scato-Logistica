using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Romaneo : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string NroPedido { get; set; }
        public virtual string Observaciones { get; set; }
        public virtual EstadoRomaneo Estado { get; set; }
        [InverseProperty("Romaneo")]
        public virtual ICollection<RomaneoItem> RomaneoItems { get; set; }
        [InverseProperty("Romaneo")]
        public virtual ICollection<RomaneoItemPedido> RomaneoItemsPedidos { get; set; }
        public virtual DateTime FechaInicio { get; set; }
        public virtual DateTime? FechaCierre { get; set; }
        [Required]
        public virtual Guid WorkflowInstanceId { get; set; }
        public virtual Proveedor Proveedor { get; set; }
    }
}

