using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto
{
    public class ModuloDeCargaPlanillaDeTurnosTurnosDto
    {
        public int Id { get; set; }
        public TurnoPuertoDto TurnoPuerto { get; set; }
        public bool Cerrado { get; set; }
        public bool Enviado { get; set; }
        public IList<ModuloDeCargaPlanillaDeTurnosTurnosDetallesDto> ModuloDeCargaPlanillaDeTurnosTurnosDetalles { get; set; }
        public IList<ModuloDeCargaPlanillaDeTurnosTurnosCortesDto> ModuloDeCargaPlanillaDeTurnosTurnosCortes { get; set; }
    }
}