
namespace Molinos.Scato.Dominio.Dto
{
    public sealed class FotoDto
    {
        public byte[] Foto { get; set; }

        public byte[] FotoChica { get; set; }

        public string Actividad { get; set; }

        public string Fecha { get; set; }
        public string Extension { get; set; }
    }
}
