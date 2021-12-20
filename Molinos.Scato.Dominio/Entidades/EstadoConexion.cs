using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class EstadoConexion : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual PuestoDeTrabajo PuestoDeTrabajo { get; set; }
        public virtual bool Estado { get; set; }
        public virtual string Mensaje { get; set; }
    }
}
