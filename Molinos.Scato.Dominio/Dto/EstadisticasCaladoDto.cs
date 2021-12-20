
using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class EstadisticasCaladoDto
    {
        //public bool Rechazado { get; set; }
        //public bool EnAnalisis { get; set; }
        //public bool Recalado { get; set; }
        //public string Calador { get; set; }
        public string Material { get; set; }
        public int CantidadRechazados { get; set; }
        public int CantidadEnAnalisis { get; set; }
        public int CantidadReclado { get; set; }
        public int Total { get; set; }
        public List<CaladorCantidadDto> Caladores { get; set; }
        public int CantidadConforme {
            get { return Total - (CantidadRechazados + CantidadEnAnalisis); }
        }
    }
}
