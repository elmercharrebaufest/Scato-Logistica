using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto
{
    public class ModuloDeCargaManosDeEmbarqueHistoricoDto
    {
        public int Id { get; set; }
        public int Mano { get; set; }
        public string Observaciones { get; set; }
        public IList<ModuloDeCargaManosDeEmbarqueDetalleHistoricoDto> ModuloDeCargaManosDeEmbarqueDetalleHistorico { get; set; }
    }
}