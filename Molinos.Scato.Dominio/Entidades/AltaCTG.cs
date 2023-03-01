using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class AltaCTG : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual CartaPorte CartaPorte { get; set; }
        public virtual string CodigoCTG { get; set; }
        public virtual Guid WorkflowId { get; set; }
        public virtual CartaPorteDerivadoGranario CartaPorteDerivadoGranario { get; set; }
    }
}
