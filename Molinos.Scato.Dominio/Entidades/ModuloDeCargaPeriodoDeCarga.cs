using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ModuloDeCargaPeriodoDeCarga : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual ModuloDeCarga ModuloDeCarga { get; set; }
        public virtual DateTime? FechaAmarro { get; set; }
        public virtual string HoraAmarro { get; set; }
        public virtual string VientoAmarro { get; set; }
        public virtual string DireccionAmarro { get; set; }
        public virtual DateTime? FechaDesamarro { get; set; }
        public virtual string HoraDesamarro { get; set; }
        public virtual string VientoDesamarro { get; set; }
        public virtual string DireccionDesamarro { get; set; }
        public virtual DateTime? FechaHabilitacion { get; set; }
        public virtual string HoraHabilitacion { get; set; }
    }
}