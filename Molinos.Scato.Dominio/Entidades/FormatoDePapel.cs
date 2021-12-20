using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class FormatoDePapel : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual int CodigoTipoPapel { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual int Ancho { get; set; }
        public virtual int Alto { get; set; }
    }
}
