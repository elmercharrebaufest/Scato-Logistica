
namespace Molinos.Scato.Dominio.Dto
{
    public sealed class AnalisisVagonDto
    {
        public int NumeroVagon { get; set; }
        public CaladoDto Calado { get; set; }
        public AnalisisDeCalidadDto AnalisisDeCalidad { get; set; }
        public int PesoNeto { get; set; }
    }
}