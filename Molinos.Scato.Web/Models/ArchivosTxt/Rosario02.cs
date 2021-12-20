namespace Molinos.Scato.Web.Models.ArchivosTxt
{
    public class Rosario02
    {
        [TxtColumna(Order = 1, Longitud = 15)]
        public string NumeroMuestra { get; set; }
        [TxtColumna(Order = 2, Longitud = 1)]
        public string CodigoTiposEnsayo { get; set; }
        [TxtColumna(Order = 3, Longitud = 3)]
        public int CodigoEnsayo { get; set; }
    }
}