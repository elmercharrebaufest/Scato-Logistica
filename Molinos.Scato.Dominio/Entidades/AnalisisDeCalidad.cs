using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class AnalisisDeCalidad : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual string NumeroOrden { get; set; }
        [Required]
        public virtual Guid WorkflowInstanceId { get; set; }
        public virtual IList<AnalisisPorCaracteristica> CaracteristicasAnalizadas { get; set; }

        public virtual Calado Calado { get; set; }
        public virtual DateTime FechaCreacion { get; set; }
        public virtual string Usuario { get; set; }
    }
}
