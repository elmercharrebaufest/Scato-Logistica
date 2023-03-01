using Molinos.Scato.Dominio.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class LlamadoAutomaticoHidraulica : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual EstadoHidraulica Estado { get; set; }
        public virtual PuestosDeCargaDescarga Hidraulica { get; set; }
        public virtual string UltimaPatenteLlamada { get; set; }
        public virtual DateTime? FechaUltimaModificacionEstado { get; set; }
        public virtual string UltimoCartelLlamado { get; set; }
    }
}
