using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class CaracteristicaDeCalidadPorWorkflow : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual CaracteristicaDeCalidad CaracteristicaDeCalidad { get; set; }
        public virtual Workflow Workflow { get; set; }
    }
}
