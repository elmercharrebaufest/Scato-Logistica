using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class CaladoEnPlanta : IIdentificable
    {
        [Key]
        public virtual int Id { get; private set; }
        public virtual Guid WorkflowInstanceId { get; set; }
        [InverseProperty("CaladoEnPlanta")]
        public virtual ICollection<CaladoEnPlantaPorCaracteristica> CaladosEnPlantaPorCaracteristica { get; set; }
        public  virtual DateTime? FechaCreacion { get; set; }
        public virtual string Usuario { get; set; }
        public virtual string Comentario { get; set; }

    }
}
