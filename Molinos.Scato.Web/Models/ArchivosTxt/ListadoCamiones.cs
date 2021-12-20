
namespace Molinos.Scato.Web.Models.ArchivosTxt
{
    public class ListadoCamiones
    {
        [TxtColumna(Order = 1, Longitud = 12)]
        public long NroCartaDePorte { get; set; }
        [TxtColumna(Order = 2, Longitud = 10)]
        public string FechaDeCarga { get; set; }
        [TxtColumna(Order = 3, Longitud = 50)]
        public string LocalidadDeOrigen { get; set; }
        [TxtColumna(Order = 4, Longitud = 50)]
        public string ProvinciaDeOrigen { get; set; }
        [TxtColumna(Order = 5, Longitud = 50)]
        public string LocalidadDeDestino { get; set; }
        [TxtColumna(Order = 6, Longitud = 50)]
        public string ProvinciaDeDestino { get; set; }
        [TxtColumna(Order = 7, Longitud = 11)]
        public long CuitTransportista { get; set; }
        [TxtColumna(Order = 8, Longitud = 30)]
        public string NombreTransportista { get; set; }
        [TxtColumna(Order = 9, Longitud = 11)]
        public long CuilDelChofer { get; set; }
        [TxtColumna(Order = 10, Longitud = 50)]
        public string NombreChofer { get; set; }
    }
}