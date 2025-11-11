namespace Molinos.Scato.Dominio.Comandos
{
    public class ConsultarStockRUCA : Comando
    {
        public string CUITEmpresaResponsable { get; set; }
        public int NumeroRUCATitular { get; set; }
        public int CodigoProductoAFIP { get; set; }
        public string Campania { get; set; }
        public int Volumen { get; set; }
    }
}