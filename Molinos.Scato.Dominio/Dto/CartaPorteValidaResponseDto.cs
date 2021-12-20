
namespace Molinos.Scato.Dominio.Dto
{
    public sealed class CartaPorteValidaResponseDto
    {
        public bool Valida { get; set; }
        public int CodigoDeError { get; set; }
        public string Error { get; set; }
    }
}