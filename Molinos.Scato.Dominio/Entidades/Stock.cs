using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Stock : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual Material Material { get; set; }
        public virtual Proveedor Proveedor { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual decimal Cantidad { get; set; }
        public virtual Recorrido Recorrido { get; set; }
    }
}
