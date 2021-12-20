using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Contingencia : IIdentificable
    {
        [Key]
        public int Id { get; set; }
        public string TipoContingencia { get; set; }
        public DateTime Fecha { get; set; }
        public string Usuario { get; set; }
        public bool Activado { get; set; }
        public string Motivo { get; set; }

    }
}
