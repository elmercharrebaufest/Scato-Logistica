namespace Molinos.Scato.Web.Models.ArchivosTxt
{
    public class Rosario03Base
    {
        [TxtColumna(Order = 1, Longitud = 15)]
        public string NumeroMuestra { get; set; }
       
        [TxtColumna(Order = 4, Longitud = 3)]
        public int SucursalCuentaOrden { get; set; }
    }
}