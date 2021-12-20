
namespace Molinos.Scato.Web.Models.ArchivosTxt
{
    public class OnccaEmitidas
    {
        [TxtColumna(Order = 1, Longitud = 1)]
        public int TipoDeTransporte { get; set; }
        [TxtColumna(Order = 2, Longitud = 1)]
        public int TipoDeCartaDePorte { get; set; }
        [TxtColumna(Order = 3, Longitud = 12)]
        public long NroCartaDePorte { get; set; }
        [TxtColumna(Order = 4, Longitud = 14)]
        public long NumeroDeCee { get; set; }
        [TxtColumna(Order = 5, Longitud = 8)]
        public long NumeroDeCTG { get; set; }
        [TxtColumna(Order = 6, Longitud = 8)]
        public string FechaDeCarga { get; set; }
        [TxtColumna(Order = 7, Longitud = 11)]
        public long CuitTitularCartaDePorte { get; set; }
        [TxtColumna(Order = 8, Longitud = 11)]
        public long CuitIntermediario { get; set; }
        [TxtColumna(Order = 9, Longitud = 11)]
        public long CuitDelRemitenteComercial { get; set; }
        [TxtColumna(Order = 10, Longitud = 11)]
        public long CuitCorredor { get; set; }
        [TxtColumna(Order = 11, Longitud = 11)]
        public long CuitRepresentanteEntregador { get; set; }
        [TxtColumna(Order = 12, Longitud = 11)]
        public long CuitDestinario { get; set; }
        [TxtColumna(Order = 13, Longitud = 11)]
        public long CuitEstablecimientoDestino { get; set; }
        [TxtColumna(Order = 14, Longitud = 11)]
        public long CuitTransportista { get; set; }
        [TxtColumna(Order = 15, Longitud = 11)]
        public long CuilDelChofer { get; set; }
        [TxtColumna(Order = 16, Longitud = 5)]
        public string Cosecha { get; set; }
        [TxtColumna(Order = 17, Longitud = 3)]
        public int CodigoDeEspecie { get; set; }
        [TxtColumna(Order = 18, Longitud = 2)]
        public int TipoDeGrano { get; set; }
        [TxtColumna(Order = 19, Longitud = 20)]
        public string Contrato { get; set; }
        [TxtColumna(Order = 20, Longitud = 1)]
        public int TipoDePesado { get; set; }
        [TxtColumna(Order = 21, Longitud = 11)]
        public decimal PesoNetoDeCarga { get; set; }
        [TxtColumna(Order = 22, Longitud = 6)]
        public int CodigoDeEstablecimientoDeProcedencia { get; set; }
        [TxtColumna(Order = 23, Longitud = 5)]
        public int CodigoDeLocalidadDeProcedencia { get; set; }
        [TxtColumna(Order = 24, Longitud = 6)]
        public int CodigoDeEstablecimientoDestino { get; set; }
        [TxtColumna(Order = 25, Longitud = 5)]
        public int CodigoDeLocalidadDeDestino { get; set; }
        [TxtColumna(Order = 26, Longitud = 4)]
        public int KmARecorrer { get; set; }
        [TxtColumna(Order = 27, Longitud = 11)]
        public string Patente { get; set; }
        [TxtColumna(Order = 28, Longitud = 11)]
        public string AcopladoPatente { get; set; }
        [TxtColumna(Order = 29, Longitud = 8)]
        public decimal TarifaPorTonelada { get; set; }
        [TxtColumna(Order = 30, Longitud = 8)]
        public decimal FleteTarifaDeReferencia { get; set; }
    }
}