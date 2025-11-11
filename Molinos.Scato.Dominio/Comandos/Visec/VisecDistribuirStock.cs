namespace Molinos.Scato.Dominio.Comandos
{
    public class VisecDistribuirStock : Comando
    {
        public int Producto { get; set; }
        public int RUCAOrigenEgreso { get; set; }
        public int StockSolicitado { get; set; }
    }
}