using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ModuloDeCargaUmap : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual ModuloDeCarga ModuloDeCarga { get; set; }
        public virtual DateTime? FechaEncendido { get; set; }
        public virtual string HoraEncendido { get; set; }
        public virtual DateTime? FechaApagado { get; set; }
        public virtual string HoraApagado { get; set; }
    }
}