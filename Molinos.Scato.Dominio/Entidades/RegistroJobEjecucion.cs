using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class RegistroJobEjecucion : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }

        public virtual string NombreProceso { get; set; }

        public virtual string Descripcion { get; set; }

        public virtual DateTime FechaEjecucion { get; set; }
    }
}