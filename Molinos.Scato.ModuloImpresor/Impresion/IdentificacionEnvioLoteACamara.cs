using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.ModuloImpresor.Impresion
{
    public class IdentificacionEnvioLoteACamara : DocumentoImpresion
    {
        private ImpIdentificacionEnvioLoteACamaraDto parametros;

        public IdentificacionEnvioLoteACamara(ImpIdentificacionEnvioLoteACamaraDto parametros, string printerName) :base(printerName)
        {
            this.parametros = parametros;
            this.EsZebra = true;
        }

        public IdentificacionEnvioLoteACamara() { }

        public string GenerarZpl()
        {
            return "^XA~TA000~JSN^LT0^MNW^MTT^PON^PMN^LH0,0^JMA^PR6,6~SD15^JUS^LRN^CI0^XZ\r\n^XA\r\n^MMT\r\n^PW625\r\n^LL0406\r\n^LS0\r\n^FO0,395^GB620,0,5^FS\r\n^FO0,133^GB620,0,5^FS\r\n^FO0,10^GB620,0,5^FS\r\n^FO0,10^GB0,390,5^FS\r\n^FO620,10^GB0,390,5^FS\r\n^FT90,50^A0N,28,28^FH\\^FD" + parametros.Centro +
                "^FS\r\n^BY2,3,83^FT25,235^B3N,Y,,N,N\r\n^FD" + parametros.NumeroDeMuestra +
                "^FS\r\n^FT66,385^A0N,28,28^FH\\^FDPrecinto: " + parametros.Precinto +
                "^FS\r\n^FT66,353^A0N,28,28^FH\\^FDOrden/Chapa: " + parametros.NumeroDeOrden + " - " + parametros.Patente +
                "^FS\r\n^FT66,266^A0N,28,28^FH\\^FDFecha/Hora Calado: " + String.Format(CultureInfo.CurrentCulture, "{0:d/M/yy HH:mm}", parametros.FechaCalado) +
                "^FS\r\n^FT66,315^A0N,56,55^FH\\^FD" + parametros.NumeroDeMuestra +
                "^FS\r\n^PQ1,0,1,Y^XZ\r\n";
        }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            var font = new Font("Arial", 10);
            
            ImprimirTextoTicket(GenerarZpl(), 0, 0, font, e);
        }

        public override void Imprimir(object dto, string printerName, FirmaDto firmaDto)
        {
            parametros = (ImpIdentificacionEnvioLoteACamaraDto)dto;
            if (!String.IsNullOrEmpty(printerName))
            {
                PrinterSettings.PrinterName = printerName;
                Print();
            }
            else
            {
                ZplCode = GenerarZpl();
            }
        }
    }
}
