using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto
{
    public class PlanoDeCargaAgenteControlPrivadoHistoricoDto
    {
        public int? Id { get; set; }
        public PlanoDeCargaHistoricoDto PlanoDeCargaHistorico { get; set; }
        public AgenteControlPrivadoDto AgenteControlPrivado { get; set; }

    }
}