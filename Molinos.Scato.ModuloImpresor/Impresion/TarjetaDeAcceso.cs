using System;
using System.Drawing;
using System.Drawing.Printing;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.ModuloImpresor.Impresion
{
    public class TarjetaDeAcceso : DocumentoImpresion
    {
        private ImpTarjetaDeAccesoDto parametros;

        public TarjetaDeAcceso(ImpTarjetaDeAccesoDto parametros, string printerName)
            : base(printerName)
        {
            this.parametros = parametros;
        }

        public TarjetaDeAcceso() { }

        public string GenerarZpl()
        {
            return "^XA\r\n~TA000~JSN^LT0^MNW^MTT^PON^PMN^LH0,0^JMA^PR5,5^MD15^LRN^CI0\r\n^MMT\r\n^PW392\r\n^LL0232\r\n^LS0\r\n^BY3,3,83^FT26,116^BCN,,N,N\r\n^FD>;" + parametros.Numero + ">69^FS\r\n^FT41,205^A0N,28,28^FH\\^FD" + parametros.Fecha + "^FS\r\n^FT241,204^A0N,28,28^FH\\^FD" + DateTime.Now.ToString("HH:mm:ss") + "^FS\r\n^FT89,163^A0N,45,45^FH\\^FD" + parametros.Numero + "^FS\r\n^XZ\r\n";
        }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            var font = new Font("Arial", 10);
            ImprimirTextoTicket(GenerarZpl(), 0, 0, font, e);
        }

        public override void Imprimir(object dto, string printerName, FirmaDto firmaDto)
        {
            PrinterSettings.PrinterName = printerName;
            parametros = (ImpTarjetaDeAccesoDto)dto;
            Print();
        }
    }
}
