using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Impresion : IIdentificable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }

        public virtual string Impresora { get; set; }
        public virtual TipoImpresion TipoImpresion { get; set; }
        public virtual DateTime FechaImpresion { get; set; }
        public virtual Guid? WorkflowId { get; set; }
        public virtual string Patente { get; set; }
        public virtual string Codigo { get; set; }
        public virtual bool Eliminada { get; set; }
    }
}
