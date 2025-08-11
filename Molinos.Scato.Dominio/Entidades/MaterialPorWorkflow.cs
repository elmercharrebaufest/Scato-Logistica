using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class MaterialPorWorkflow : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual Material Material { get; set; }
        public virtual Centro Centro { get; set; }
        public virtual Workflow Workflow { get; set; }
        public virtual bool ImprimirTicketPesada { get; set; }
        public bool EnviaASapAlmacenPredeterminado { get; set; }
        public virtual bool ImprimirAsignacionRuta { get; set; }
        public virtual Cliente Cliente { get; set; }
        public virtual bool ImprimirMuestraInase { get; set; }

    }
}
