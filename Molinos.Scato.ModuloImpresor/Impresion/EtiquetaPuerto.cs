using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.ModuloImpresor.Impresion
{
    public class EtiquetaPuerto : DocumentoImpresion
    {
        private ImpEtiquetaPuertoDto parametros;
        private string printerName;
        private FirmaDto firma;

        public EtiquetaPuerto(ImpEtiquetaPuertoDto parametros, string printerName, int xPos, int yPos, FirmaDto firmaDto)
            :base(printerName)
        {
            this.parametros = parametros;
            this.printerName = printerName;
            this.firma = firmaDto;
            this.EsZebra = true;
            this.ZplCode = GenerarZpl();
        }

        public EtiquetaPuerto() { }

        public string GenerarZpl()
        {
            var disenio = "^XA\r\n~TA000~JSN^LT0^MNW^MTT^PON^PMN^LH0,0^JMA^PR5,5^MD15^LRN^CI0\r\n^MMT\r\n^PW609\r\n^LL0406\r\n^LS0\r\n^FO8,12^GB593,350,4^FS\r\n^FT66,272^A0N,28,28^FH\r\n^FO440,30^GFA,935,935,17,,:gG01F,W0LF,W0FFC3FF,U0707F03FF,T01E07F07FF,T07E07F07FF,T0FE07E07FF,S01FE07807FE,S03FE0780FFE,S07FC0700FFC,S0FF98710FFC,S0FF18630FF8,R01FE18470FF8,R01FE380E0FF,R03FE380E0FF,R03FE781E1FE,R03FC781E1FC,R03F1F87E1F,R03F1F8FE1E,R03F1F8FC1C,R03F1F9FC18,R07LF,R07KF,R078,,::::::::038038,0380F07E0E038E0607E0F801F81FC671F803C1F0FE0E030E061FF0FC07F87FC667F807C1E1EF0E030E0E1FF1EC0FF8FFC6E7F807C3E3C30E071F0E3871C00E38F1C7CE3807C3C7039C061F1E7071C03839C1CE1C1C06C4C7039C0E1D9E7071E03031818E181C06IC7039C0E1D9E6070F03073818C38180CDDC707180E18FCE0F0F07073031838380CFBC70E381E38FCE0F03070F7071838381CF3C70E381E387CE0F03070F38F3838781C63C7FE3FDE38387FCFF07FF3FF381FF,3C63C3FC3F9C30383F8FF03F61F7381FE,3C43C3F03F9C70103F07C03EE0FF380FC,gM0E,:gK063C,gK07F8,,:^FS\r\n^FT21,106^A0N,28,28^FH\\^FDVapor: {0}^FS\r\n^FT21,139^A0N,28,28^FH\\^FDCargador: {1}^FS\r\n^FT21,172^A0N,28,28^FH\\^FDMercaderia: {2}^FS\r\n^FT21,205^A0N,28,28^FH\\^FDDestino: {3}^FS\r\n^FT21,238^A0N,28,28^FH\\^FDKg: {4}^FS\r\n^FT21,271^A0N,28,28^FH\\^FDNro de Lote: {5}^FS\r\n^FT350,271^A0N,28,28^FH\\^FDBodega: {6}^FS\r\n^FT21,304^A0N,28,28^FH\\^FDControl: {7}^FS\r\n^FT21,337^A0N,28,28^FH\\^FDFecha: {8}^FS\r\n^XZ\r\n";
            return string.Format(disenio, 
                parametros.Vapor,
                parametros.Cargador,
                parametros.Mercaderia,
                parametros.Destino,
                parametros.Kg,
                parametros.NumeroLote,
                parametros.Bodega,
                parametros.Control,
                parametros.Fecha?.ToString("dd/MM/yyyy"));
        }
        protected override void OnBeginPrint(PrintEventArgs e)
        {
            RawPrinterHelper.SendStringToPrinter(printerName, GenerarZpl());
        }
    }
}
