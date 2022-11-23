using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ConfirmacionCargaDescarga : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string NombreUsuario { get; set; }
        public virtual DateTime FechaConfirmacion { get; set; }
        public virtual Recorrido Recorrido { get; set; }
    }
}
