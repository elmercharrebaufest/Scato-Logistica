using System;
using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto
{
    public class ModuloDeCargaPlanillaDeTurnosDto
    {
        public int Id { get; set; }
        public DateTime? Fecha { get; set; }
        public IList<ModuloDeCargaPlanillaDeTurnosTurnosDto> ModuloDeCargaPlanillaDeTurnosTurnos { get; set; }
    }
}