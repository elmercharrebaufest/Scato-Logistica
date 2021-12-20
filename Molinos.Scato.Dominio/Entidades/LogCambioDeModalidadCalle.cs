using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class LogCambioDeModalidadCalle : IIdentificable
    {
        [Key]
        public int Id { get; set; }
        public string Motivo { get; set; }
        public string Usuario { get; set; }
        public DateTime Fecha { get; set; }
        public Calle Calle { get; set; }
        public bool Automatico { get; set; }
    }
}
