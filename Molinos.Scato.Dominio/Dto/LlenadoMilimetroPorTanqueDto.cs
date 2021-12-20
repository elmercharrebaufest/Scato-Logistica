namespace Molinos.Scato.Dominio.Dto
{
    public sealed class LlenadoMilimetroPorTanqueDto
    {
        public int Id { get; set; }
        public string TanqueNum { get; set; }
        public string Cm { get; set; }
        public string Mm { get; set; }
        public string LlenadoMm { get; set; }
    }
}
