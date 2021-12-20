using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ControlDeBalanza : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual Recorrido Recorrido { get; set; }
        public virtual string Observaciones { get; set; }
        public virtual TipoPesada TipoPesada { get; set; }
        [InverseProperty("ControlDeBalanza")]
        public virtual ICollection<ControlDeBalanzaPesada> ControlesDeBalanzasPesadas { get; set; }
    }
}