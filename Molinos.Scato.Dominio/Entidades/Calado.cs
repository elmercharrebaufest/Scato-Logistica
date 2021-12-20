using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Calado : IIdentificable
    {
        [Key]
        public virtual int Id { get; private set; }
        [Required]
        public virtual Guid WorkflowInstanceId { get; set; }
        [Required]
        public virtual int CicloDeCalado { get; set; }
        public virtual int? MuestraConjunto { get; set; }
        public virtual string NumeroOrden { get; set; }
        [InverseProperty("Calado")]
        public virtual ICollection<CaladoPorCaracteristica> CaladosPorCaracteristica { get; set; }
        public virtual bool EstaAutorizadoPorEntregador { get; set; }
        public  virtual DateTime? FechaCreacion { get; set; }
        public virtual CalidadMaterial CalidadMaterial { get; set; }
        public virtual string Usuario { get; set; }
        public virtual string Comentario { get; set; }
    }
}
