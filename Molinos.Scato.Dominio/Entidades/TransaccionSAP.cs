using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    public class TransaccionSAP : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string DescripcionCorta { get; set; }
        public virtual Centro CentroOrigen { get; set; }
        public virtual Material Material { get; set; }
        public virtual TipoComercial TipoComercial { get; set; }
        public virtual FuncionSAP FuncionSAP { get; set; }
    }
}
