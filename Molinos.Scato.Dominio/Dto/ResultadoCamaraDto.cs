namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ResultadoCamaraDto
    {
        public string NumeroMuestra { get; set; }
        public string NumeroCp { get { return NumeroMuestra.Length >= 10 ? "1" +NumeroMuestra.Substring(NumeroMuestra.Length - 10) : ""; } }
        public decimal? Valor { get; set; }
    }
}
