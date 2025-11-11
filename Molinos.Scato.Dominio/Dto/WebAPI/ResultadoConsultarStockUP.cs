namespace Molinos.Scato.Dominio.Dto.WebAPI
{
    public class ResultadoConsultarStockUP
    {
        public bool StockDisponible { get; set; }
        public string Mensaje { get; set; } = string.Empty;
    }
}