using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ModuloDeCargaPlanillaDeTurnos : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual ModuloDeCarga ModuloDeCarga { get; set; }
        public virtual DateTime? Fecha { get; set; }
        public virtual ICollection<ModuloDeCargaPlanillaDeTurnosTurnos> ModuloDeCargaPlanillaDeTurnosTurnos { get; set; }
    }
}