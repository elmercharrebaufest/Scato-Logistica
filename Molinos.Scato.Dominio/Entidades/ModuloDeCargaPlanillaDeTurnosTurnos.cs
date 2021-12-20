using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ModuloDeCargaPlanillaDeTurnosTurnos : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual ModuloDeCargaPlanillaDeTurnos ModuloDeCargaPlanillaDeTurnos { get; set; }
        public virtual TurnoPuerto TurnoPuerto { get; set; }
        public virtual bool Cerrado { get; set; }
        public virtual bool Enviado { get; set; }
        public virtual ICollection<ModuloDeCargaPlanillaDeTurnosTurnosDetalles> ModuloDeCargaPlanillaDeTurnosTurnosDetalles { get; set; }
        public virtual ICollection<ModuloDeCargaPlanillaDeTurnosTurnosCortes> ModuloDeCargaPlanillaDeTurnosTurnosCortes { get; set; }
    }
}