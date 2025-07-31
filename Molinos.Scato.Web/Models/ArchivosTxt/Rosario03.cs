namespace Molinos.Scato.Web.Models.ArchivosTxt
{
    public class Rosario03 : Rosario03Base
    {
        [TxtColumna(Order = 2, Longitud = 11)]
        public long CuentaOrden { get; set; }

        [TxtColumna(Order = 3, Longitud = 40)]
        public string DescripcionCuentaOrden { get; set; }
    }

    public class Rosario03VentaSecundaria : Rosario03Base
    {
        [TxtColumna(Order = 2, Longitud = 11)]
        public long CuitRteComercialVentaSecundaria { get; set; }
     
        [TxtColumna(Order = 3, Longitud = 40)]
        public string RteComercialVentaSecundaria { get; set; }
    }

    public class Rosario03VentaSecundaria2 : Rosario03Base
    {
        [TxtColumna(Order = 2, Longitud = 11)]
        public long CuitRteComercialVentaSecundaria2 { get; set; }

        [TxtColumna(Order = 3, Longitud = 40)]
        public string RteComercialVentaSecundaria2 { get; set; }
    }

    public class Rosario03MercadoATermino : Rosario03Base
    {
        [TxtColumna(Order = 2, Longitud = 11)]
        public long CuitMercadoATermino { get; set; }

        [TxtColumna(Order = 3, Longitud = 40)]
        public string MercadoATérmino { get; set; }
    }

    public class Rosario03CorredorVentaSecundaria : Rosario03Base
    {
        [TxtColumna(Order = 2, Longitud = 11)]
        public long CuitCorredorVentaSecundaria { get; set; }

        [TxtColumna(Order = 3, Longitud = 40)]
        public string CorredorVentaSecundaria { get; set; }
    }

    public class Rosario03RepresentanteRecibidor : Rosario03Base
    {
        [TxtColumna(Order = 2, Longitud = 11)]
        public long CuitRepresentanteRecibidor { get; set; }

        [TxtColumna(Order = 3, Longitud = 40)]
        public string RepresentanteRecibidor { get; set; }
    }

    public class Rosario03Destino : Rosario03Base
    {
        [TxtColumna(Order = 2, Longitud = 11)]
        public long CuitDestino { get; set; }

        [TxtColumna(Order = 3, Longitud = 40)]
        public string Destino { get; set; }
    }
}