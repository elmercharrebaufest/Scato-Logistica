namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ModuloDeCargaBalanzasDto
    {
        public int Id { get; set; }
        public MotivosFallasBalanzaDto MotivosFallasBalanza { get; set; }
        public string Observaciones { get; set; }

     //   public BalanzadaDto Balanzada { get; set; }
    }
}
