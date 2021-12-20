using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class LogSincronizacion : IIdentificable
    {

        [Key]
        public virtual int Id { get; set; }
        public virtual string NombreInterface { get; set; }
        public virtual DateTime FechaEjecucion { get; set; }
        public virtual bool Completa { get; set; }
        public virtual bool Correcta { get; set; }
        public virtual string Mensaje { get; set; }
    }
}