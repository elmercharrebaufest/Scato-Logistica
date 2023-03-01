using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class BajaCTG : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual CartaPorte CartaPorte { get; set; }
        public virtual string CodigoDeBaja { get; set; }
        public virtual string CodigoDeBajaDefinitivo { get; set; }
        public virtual Guid WorkflowId { get; set; }
        public virtual OrdenDeDescargaFason OrdenDeDescargaFason { get; set; }
    }
}
