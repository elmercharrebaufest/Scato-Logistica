using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Domicilio : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual int Tipo { get; set; }
        public virtual int Orden { get; set; }
        public virtual string Descripcion { get; set; }
    }
}
