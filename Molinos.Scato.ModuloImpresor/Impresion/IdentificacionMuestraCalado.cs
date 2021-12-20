using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.ModuloImpresor.Impresion
{
    public class IdentificacionMuestraCalado : DocumentoImpresion
    {
        private ImpIdentificacionMuestraCaladoDto parametros;

        public IdentificacionMuestraCalado(ImpIdentificacionMuestraCaladoDto parametros, string printerName)
        {
            PrinterSettings.PrinterName = printerName;
            this.parametros = parametros;
            this.EsZebra = true;
        }

        public IdentificacionMuestraCalado() { }

        public string GenerarZpl()
        {
            return "^XA~TA000~JSN^LT0^MNW^MTT^PON^PMN^LH0,0^JMA^PR6,6~SD15^JUS^LRN^CI0\r\n^XZ\r\n^XA\r\n^MMT\r\n^PW609\r\n^LL0406\r\n^LS0\r\n^FO45,395^GB595,0,5^FS\r\n^FO45,133^GB595,0,5^FS\r\n^FO45,10^GB595,0,5^FS\r\n^FO45,10^GB0,390,5^FS\r\n^FO605,10^GB0,390,5^FS\r\n^FT90,100^A0N,28,28^FH\\^FD" + parametros.Centro + "^FS\r\n^BY3,3,48^FT202,208^BCN,,N,N\r\n^FD>;" + parametros.NumeroCartaPorte + "^FS\r\n^FT66,272^A0N,28,28^FH\\^FDProcedencia: " + parametros.Procedencia.ToUpper() + "^FS\r\n^FT66,380^A0N,28,28^FH\\^FDOrden/Chapa: " + parametros.NumeroDeOrden + " - " + parametros.Patente + "^FS\r\n^FT66,344^A0N,28,28^FH\\^FDUsuario Calada: " + parametros.NombreUsuario.ToUpper() + "^FS\r\n^FT66,308^A0N,28,28^FH\\^FDFecha Calado: " + String.Format(CultureInfo.CurrentCulture, "{0:yyyy-M-d HH:mm:ss}", parametros.FechaCalado) + "^FS\r\n^FT66,235^A0N,28,28^FH\\^FDNeto Origen: " + parametros.PesoNeto + "   Humedad: " + parametros.Humedad + "^FS^PQ1,0,1,Y^XZ\r\n";
        }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            var font = new Font("Arial", 10);
            
            ImprimirTextoTicket(GenerarZpl(), 0, 0, font, e);
        }

        public override void Imprimir(object dto, string printerName, FirmaDto firmaDto)
        {
            parametros = (ImpIdentificacionMuestraCaladoDto)dto;
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
