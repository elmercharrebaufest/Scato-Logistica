namespace Molinos.Scato.Web.Models.ArchivosTxt
{
    public class BahiaBlanca
    {
        [TxtColumna(Order = 1, Longitud = 10)]
        public long NumeroMuestra { get; set; }
        [TxtColumna(Order = 2, Longitud = 5)]
        public long CodigoProducto { get; set; }
        [TxtColumna(Order = 3, Longitud = 60)]
        public string NombreExportador { get; set; }
        [TxtColumna(Order = 4, Longitud = 60)]
        public string NombreVendedor { get; set; }
        [TxtColumna(Order = 5, Longitud = 60)]
        public string NombreCorredor { get; set; }
        [TxtColumna(Order = 6, Longitud = 60)]
        public string NombreEntregador { get; set; }
        [TxtColumna(Order = 7, Longitud = 12)]
        public long CartaDePorte { get; set; }
        [TxtColumna(Order = 8, Longitud = 40)]
        public string NombreProcedencia { get; set; }
        [TxtColumna(Order = 9, Longitud = 4)]
        public int AñoFechaDescarga { get; set; }
        [TxtColumna(Order = 10, Longitud = 2)]
        public int MesFechaDescarga { get; set; }
        [TxtColumna(Order = 11, Longitud = 2)]
        public int DiaFechaDescarga { get; set; }
        [TxtColumna(Order = 12, Longitud = 2)]
        public int Cosecha { get; set; }
        [TxtColumna(Order = 13, Longitud = 9)]
        public int Kilos { get; set; }
        [TxtColumna(Order = 14, Longitud = 10)]
        public int NumeroContrato { get; set; }
        [TxtColumna(Order = 15, Longitud = 1)]
        public string TipoTrans { get; set; }
        [TxtColumna(Order = 16, Longitud = 8)]
        public int NroVagon { get; set; }
        [TxtColumna(Order = 17, Longitud = 12)]
        public long NumeroCTG { get; set; }
        [TxtColumna(Order = 18, Longitud = 13)]
        public long CPE { get; set; }

    }
}