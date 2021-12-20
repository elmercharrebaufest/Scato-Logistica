using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Sugerencia : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }

        public virtual string TextoSugerencia { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual string NombreUsuario { set; get; }
        public virtual int CentroId { set; get; }
        public virtual string Url { set; get; }
    }
}
