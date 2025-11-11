namespace Molinos.Scato.Dominio.Comandos
{
    public class ConsultarStockUP : Comando
    {
        public string CUITEmpresaResponsable { get; set; }
        public string NumeroRENSPA { get; set; }
        public int CodigoProductoAFIP { get; set; }
        public string Campania { get; set; }
        public int Volumen { get; set; }
    }
}