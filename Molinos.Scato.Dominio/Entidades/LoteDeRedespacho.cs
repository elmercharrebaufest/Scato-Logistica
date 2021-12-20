using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class LoteDeRedespacho : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }

        public virtual Recorrido Recorrido { get; set; }

        public virtual string Almacen { get; set; }

        public virtual string Centro { get; set; }

        public virtual string Lote { get; set; }

        public virtual string Material { get; set; }

        public virtual string Stock { get; set; }
    }
}
