using System;

namespace Molinos.Scato.Web.Models.ArchivosTxt
{
    public class MuestraInase
    {
        [TxtColumna(Order = 1, Longitud = 15)]
        public string NumeroMuestra { get; set; }
        [TxtColumna(Order = 2, Longitud = 4)]
        public int CodigoProducto { get; set; }
        [TxtColumna(Order = 3, Longitud = 25)]
        public string NombreProducto { get; set; }
        [TxtColumna(Order = 4, Longitud = 11)]
        public long CuitDestinatario { get; set; }
        [TxtColumna(Order = 5, Longitud = 11)]
        public long CuitRtteComercial { get; set; }
        [TxtColumna(Order = 6, Longitud = 11)]
        public long CuitCorredor { get; set; }
        [TxtColumna(Order = 7, Longitud = 1)]
        public long CodigoPagador { get; set; }
        [TxtColumna(Order = 8, Longitud = 4)]
        public int CodigoProcedencia { get; set; }
        [TxtColumna(Order = 9, Longitud = 2)]
        public int SubCodigoProcedencia { get; set; }
        [TxtColumna(Order = 10, Longitud = 3)]
        public int CodigoPuerto { get; set; }
        [TxtColumna(Order = 11, Longitud = 9)]
        public long PesoNetoSeco { get; set; }
        [TxtColumna(Order = 12, Longitud = 1)]
        public string Lacrada { get; set; }
        [TxtColumna(Order = 13, Longitud = 15)]
        public string ContratoVendedor { get; set; }
        [TxtColumna(Order = 14, Longitud = 15)]
        public string ContratoComprador { get; set; }
        [TxtColumna(Order = 15, Longitud = 6)]
        public DateTime FechaDescarga { get; set; }
        [TxtColumna(Order = 16, Longitud = 12)]
        public string NumeroRecibo { get; set; }
        [TxtColumna(Order = 17, Longitud = 2)]
        public int CodigoGrupo { get; set; }
        [TxtColumna(Order = 18, Longitud = 1)]
        public string ServicioLacrado { get; set; }
        [TxtColumna(Order = 19, Longitud = 8)]
        public string Patente { get; set; }
        [TxtColumna(Order = 20, Longitud = 40)]
        public string RtteComercial { get; set; }
        [TxtColumna(Order = 21, Longitud = 3)]
        public int CodigoSucursalComprador { get; set; }
        [TxtColumna(Order = 22, Longitud = 3)]
        public int CodigoSucursalVendedor { get; set; }
        [TxtColumna(Order = 23, Longitud = 3)]
        public int CodigoSucursalCorredor { get; set; }
        [TxtColumna(Order = 24, Longitud = 13)]
        public long CartaDePorte { get; set; }
        [TxtColumna(Order = 25, Longitud = 12)]
        public long NumeroCTG { get; set; }
        [TxtColumna(Order = 26, Longitud = 11)]
        public long CuitTitularCartaPorte { get; set; }
        [TxtColumna(Order = 27, Longitud = 40)]
        public string TitularCartaPorte { get; set; }
        [TxtColumna(Order = 28, Longitud = 2)]
        public string TecnologiaDeclarada { get; set; }
        [TxtColumna(Order = 29, Longitud = 40)]
        public string Establecimiento { get; set; }
        [TxtColumna(Order = 30, Longitud = 60)]
        public string DireccionPostalDestino { get; set; }
        [TxtColumna(Order = 31, Longitud = 5)]
        public int CodigoLocalidadONCCAProcedencia { get; set; }
        [TxtColumna(Order = 32, Longitud = 5)]
        public int CodigoLocalidadONCCADestino { get; set; }
        [TxtColumna(Order = 33, Longitud = 2)]
        public string TipoDeTransporte { get; set; }
        [TxtColumna(Order = 34, Longitud = 2)]
        public int CantidadVagones { get; set; }
        [TxtColumna(Order = 35, Longitud = 15)]
        public string IdentificadorVagon { get; set; }
        [TxtColumna(Order = 36, Longitud = 10)]
        public long CodigoPlantaONCCADestino { get; set; }
        [TxtColumna(Order = 37, Longitud = 40)]
        public string RazonSocialCorredor { get; set; }
        [TxtColumna(Order = 38, Longitud = 11)]
        public long CuitIntermediario { get; set; }
        [TxtColumna(Order = 39, Longitud = 40)]
        public string RazonSocialIntermediario { get; set; }
        [TxtColumna(Order = 40, Longitud = 11)]
        public long CuitRepresentante { get; set; }
        [TxtColumna(Order = 41, Longitud = 40)]
        public string RazonSocialRepresentante { get; set; }
        [TxtColumna(Order = 42, Longitud = 4)]
        public long Cosecha { get; set; }

    }
}