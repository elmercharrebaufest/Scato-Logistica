using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ActividadConCargaAutomatica : IIdentificable
    {
        [Key]
        public int Id { get; set; }
        public virtual Workflow Workflow { get; set; }
        public virtual string Actividad { get; set; }
    }
}
