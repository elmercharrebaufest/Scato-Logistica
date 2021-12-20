using System;
using System.Drawing;
using System.Drawing.Printing;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.ModuloImpresor.Impresion
{
    public class EtiquetaAuditoriaCamara : DocumentoImpresion
    {
        private ImpEtiquetaAuditoriaCamaraDto parametros;

        public EtiquetaAuditoriaCamara(ImpEtiquetaAuditoriaCamaraDto parametros, string printerName)
        {
            PrinterSettings.PrinterName = printerName;
            this.parametros = parametros;
            this.EsZebra = true;

        }

        public EtiquetaAuditoriaCamara() { }

        public string GenerarZpl()
        {
            return "^XA~TA000~JSN^LT0^MNW^MTT^PON^PMN^LH0,0^JMA^PR6,6~SD15\r\n^JUS^LRN^CI0^XZ\r\n^XA\r\n^MMT\r\n^PW625\r\n^LL0406\r\n^LS0\r\n^FO0,395^GB620,0,5^FS\r\n^FO0,133^GB620,0,5^FS\r\n^FO0,10^GB620,0,5^FS\r\n^FO0,10^GB0,390,5^FS\r\n^FO620,10^GB0,390,5^FS\r\n^FT90,50^A0N,28,28^FH\\^FDAnalisis aleatorio - " +
                parametros.Centro + "^FS\r\n^FT90,100^A0N,28,28^FH\\^FDMaterial: " +
                parametros.Material + "^FS\r\n^BY2,3,83^FT25,244^B3N,Y,,N,N\r\n^FD" + parametros.NumeroCartaPorte +
                "^FS\r\n^FT66,392^A0N,25,24^FH\\^FD " +
                "^FS\r\n^FT66,353^A0N,28,28^FH\\^FDChapa: " + parametros.TipoDeAnalisis + "^FS\r\n^FT66,310^A0N,56,55^FH\\^FD" + parametros.NumeroCartaPorte +
                "^FS\r\n^PQ1,0,1,Y^XZ\r\n";
        }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            var font = new Font("Arial", 10);
            
            ImprimirTextoTicket(GenerarZpl(), 0, 0, font, e);
        }

        public override void Imprimir(object dto, string printerName, FirmaDto firmaDto)
        {
            parametros = (ImpEtiquetaAuditoriaCamaraDto)dto;
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
