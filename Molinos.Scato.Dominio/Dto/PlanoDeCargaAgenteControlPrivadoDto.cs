using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto
{
    public class PlanoDeCargaAgenteControlPrivadoDto
    {
        public int? Id { get; set; }
        public PlanoDeCargaDto PlanoDeCarga { get; set; }
        public AgenteControlPrivadoDto AgenteControlPrivado { get; set; }

    }
}
