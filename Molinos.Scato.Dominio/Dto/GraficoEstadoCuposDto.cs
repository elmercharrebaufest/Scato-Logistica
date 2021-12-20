using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class GraficoEstadoCuposDto
    {
        public int Cupeados { get; set; }
        public int Arribados { get; set; }
        public int Descargados { get; set; }

        public int PorcentajeEnPlanta { get { return Cupeados > 0 ? (Arribados- Descargados) * 100 / Cupeados : 0; } }
        public int PorcentajeDescargados { get { return Cupeados > 0 ? Descargados * 100 / Cupeados : 0; } }



        public List<EstadoMaterialDto> Materiales { get; set; }
    }
}
