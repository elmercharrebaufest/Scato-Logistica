
namespace Molinos.Scato.Dominio.Dto
{
    public sealed class VideoCamaraDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Directorio { get; set; }
        public int? Posicion { get; set; }
    }
}