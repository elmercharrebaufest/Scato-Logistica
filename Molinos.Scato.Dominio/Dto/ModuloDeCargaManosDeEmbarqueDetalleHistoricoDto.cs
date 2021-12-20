namespace Molinos.Scato.Dominio.Dto
{
    public class ModuloDeCargaManosDeEmbarqueDetalleHistoricoDto
    {
        public int Id { get; set; }
        public CeldaManoDeEmbarqueDto CeldaManoDeEmbarque { get; set; }
        public SentidoManoDeEmbarqueDto SentidoManoDeEmbarque { get; set; }
        public int? PorcentajePorMano { get; set; }
        public bool? AperturaPorton { get; set; }
        public bool? MasProduccion { get; set; }
        public MaterialPuertoDto MaterialPuerto { get; set; }
    }
}
