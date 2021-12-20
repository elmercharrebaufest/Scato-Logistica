
namespace Molinos.Scato.Dominio.Dto
{
    public sealed class CartaPorteResponseDto
    {
        public CartaPorteDto CartaPorte { get; set; }
        public int CodigoDeError { get; set; }
        public string Error { get; set; }
        public bool EsRedespacho { get; set; }
        public bool EstaDemorado { get; set; }
    }
}