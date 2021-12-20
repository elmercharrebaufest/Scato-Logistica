namespace Molinos.Scato.Web.Models.ArchivosTxt
{
    public class Rosario03
    {
        [TxtColumna(Order = 1, Longitud = 15)]
        public string NumeroMuestra { get; set; }
        [TxtColumna(Order = 2, Longitud = 11)]
        public long CuentaOrden { get; set; }
        [TxtColumna(Order = 3, Longitud = 40)]
        public string DescripcionCuentaOrden { get; set; }
        [TxtColumna(Order = 4, Longitud = 3)]
        public int SucursalCuentaOrden { get; set; }
    }
}