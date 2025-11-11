using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Workflow : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual string Codigo { get; set; }
        [Required]
        public virtual string Descripcion { get; set; }
        [InverseProperty("Workflow")]
        public virtual IList<WorkflowDefinicion> Definiciones { get; set; }
        [InverseProperty("WorkflowsAsociados")]
        public virtual IList<TipoComercial> TiposComercialesAsociados { get; set; }
        public virtual TipoDeWorkflow TipoDeWorkflow { get; set; }
        public virtual Centro Centro { get; set; }
        public virtual bool Activo { get; set; }
        public virtual bool PendienteNoGranos { get; set; }
        public virtual bool AplicaConsultaCupoDataAgro { get; set; }
    }
}
