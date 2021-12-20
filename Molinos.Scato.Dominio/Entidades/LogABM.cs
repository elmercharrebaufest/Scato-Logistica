using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    public class LogABM : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string Pantalla { get; set; }
        public virtual string Usuario { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual EventoABM Evento { get; set; }
        public virtual string Entidad { get; set; }
        
    }
}
