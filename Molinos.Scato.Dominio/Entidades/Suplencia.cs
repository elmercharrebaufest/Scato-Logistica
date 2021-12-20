using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Suplencia : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual Usuario UsuarioASuplantar { get; set; }
        public virtual Usuario UsuarioSuplente { get; set; }
        public virtual DateTime FechaDesde { get; set; }
        public virtual DateTime FechaHasta { get; set; }
    }
}
