using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ControlDeTiempo : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual Workflow Workflow { get; set; }
        [Required]
        public virtual string ActividadDesde { get; set; }
        [Required]
        public virtual string ActividadHasta { get; set; }
        [Required]
        public virtual int TiempoMaximo { get; set; }
        [Required]
        public virtual string CodigoControl { get; set; }
    }
}
