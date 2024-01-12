using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class AutomatismoTipoLlamado : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }

        public virtual string Codigo { get; set; }

        public virtual string Descripcion { get; set; }

        public virtual bool Activo { get; set; }

    }
}