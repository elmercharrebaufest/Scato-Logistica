
namespace Molinos.Scato.Dominio.Dto
{
    public sealed class HojaDeRutaYerbateraValidaResponseDto
    {
        public bool Valida { get; set; }
        public int CodigoDeError { get; set; }
        public string Error { get; set; }
    }
}