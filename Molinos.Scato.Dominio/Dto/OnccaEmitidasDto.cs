
namespace Molinos.Scato.Dominio.Dto
{
    public sealed class OnccaEmitidasDto
    {
        public int TipoDeTransporte { get; set; }
        public int TipoDeCartaDePorte { get; set; }
        public long NroCartaDePorte { get; set; }
        public long NumeroDeCee { get; set; }
        public long NumeroDeCTG { get; set; }
        public string FechaDeCarga { get; set; }
        public long CuitTitularCartaDePorte { get; set; }
        public long CuitIntermediario { get; set; }
        public long CuitDelRemitenteComercial { get; set; }
        public long CuitCorredor { get; set; }
        public long CuitRepresentanteEntregador { get; set; }
        public long CuitDestinario { get; set; }
        public long CuitEstablecimientoDestino { get; set; }
        public long CuitTransportista { get; set; }
        public long CuilDelChofer { get; set; }
        public string Cosecha { get; set; }
        public int CodigoDeEspecie { get; set; }
        public int TipoDeGrano { get; set; }
        public string Contrato { get; set; }
        public int TipoDePesado { get; set; }
        public decimal PesoNetoDeCarga { get; set; }
        public int CodigoDeEstablecimientoDeProcedencia { get; set; }
        public int CodigoDeLocalidadDeProcedencia { get; set; }
        public int CodigoDeEstablecimientoDestino { get; set; }
        public int CodigoDeLocalidadDeDestino { get; set; }
        public int KmARecorrer { get; set; }
        public string Patente { get; set; }
        public string AcopladoPatente { get; set; }
        public decimal TarifaPorTonelada { get; set; }
        public decimal FleteTarifaDeReferencia { get; set; }

        public string FechaDeDescarga { get; set; }
        public string FechaDeArriboADestino { get; set; }
        public decimal PesoNetoDeDescarga { get; set; }
        public long CuitEstablecimientoRedestino { get; set; }
    }
}
