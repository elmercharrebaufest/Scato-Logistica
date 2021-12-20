namespace Molinos.Scato.Dominio.Dto
{
    public sealed class PagoConMercadoPagoDto
    {
        public int Id { get; set; }
        public RecorridoDto Recorrido { get; set; }
        public string MercadoPagoId { get; set; }
        public decimal MontoCobrado { get; set; }
        public string Estado { get; set; }
        public string DetalleDelEstado { get; set; }
        public int RecorridoId { get; set; }
        public bool Devuelto { get; set; }

        public bool Reembolsable { get { return !Devuelto && !string.IsNullOrEmpty(MercadoPagoId); } }
    }
}