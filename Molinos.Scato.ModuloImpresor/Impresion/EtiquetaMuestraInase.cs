using System;
using System.Drawing;
using System.Drawing.Printing;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.ModuloImpresor.Impresion
{
    public class EtiquetaMuestraInase : DocumentoImpresion
    {
        private ImpEtiquetaMuestraInaseDto parametros;

        public EtiquetaMuestraInase(ImpEtiquetaMuestraInaseDto parametros, string printerName)
        {
            PrinterSettings.PrinterName = printerName;
            this.parametros = parametros;
            this.EsZebra = true;

        }

        public EtiquetaMuestraInase() { }

        public string GenerarZpl()
        {
            return "^XA~TA000~JSN^LT0^MNW^MTT^PON^PMN^LH0,0^JMA^PR6,6~SD15^JUS^LRN^CI0^XZ\r\n^XA\r\n^MMT\r\n^PW609\r\n^LL0406\r\n^LS0\r\n" +
                "^FT30,64^A0N,28,28^FH\\^FD Resol 37/22 Muestra INASE ^FS\r\n" +
                "^FT30,100^A0N,28,28^FH\\^FD Fecha/Hora:" + DateTime.Now.ToString("dd/MM/yyyy hh:mm") + "^FS\r\n" +
                "^FT30,137^A0N,28,28^FH\\^FD CUIT Productor:" + parametros.CuitProductor + "^FS\r\n" +
                "^FT30,167^A0N,25,24^FH\\^FD Nro CPE: " + parametros.NumeroCartaPorte + "^FS\r\n" +
                "^FT30,197^A0N,25,24^FH\\^FD Producto: " + parametros.Material + "^FS\r\n" +
                "^FT30,227^A0N,25,24^FH\\^FD Patente: " + parametros.Patente + "^FS\r\n" +
                "^BY2,3,83^FT25,330^B3N,Y,,N,N^FD" + parametros.NroMuestra + "^FS\r\n" +
                "^FT146,370^A0N,36,35^FH\\^FD" + parametros.NroMuestra + "^FS\r\n" +
                "^PQ1,0,1,Y^XZ\r\n"; 
        }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            var font = new Font("Arial", 10);
            
            ImprimirTextoTicket(GenerarZpl(), 0, 0, font, e);
        }

        public override void Imprimir(object dto, string printerName, FirmaDto firmaDto)
        {
            parametros = (ImpEtiquetaMuestraInaseDto)dto;
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
