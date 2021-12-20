using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Camara : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual string DescripcionCorta { get; set; }
        public virtual string CodigoSAP { get; set; }
        public virtual CamaraFormatoDeArchivo? FormatoDeArchivo { get; set; }
        public virtual string Email { get; set; }

    }
}
