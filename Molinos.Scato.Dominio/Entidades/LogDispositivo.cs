using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class LogDispositivo : IIdentificable
    {
        [Key]
        public int Id { get; set; }
        public virtual DateTime Fecha { get; set; }
        public string CodigoDispositivo { get; set; }
        public string NombreLog { get; set; }
        public string ValorAnterior { get; set; }
        public string ValorActual { get; set; }
    }
}