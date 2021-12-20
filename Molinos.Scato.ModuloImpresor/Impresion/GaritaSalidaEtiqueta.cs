using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.ModuloImpresor.Impresion
{
    public class GaritaSalidaEtiqueta : DocumentoImpresion
    {
        private ImpGaritaSalidaDto parametros;
        private FirmaDto firma;

        public GaritaSalidaEtiqueta(ImpGaritaSalidaDto parametros, string printerName, FirmaDto firma)
            : base(printerName)
        {
            this.parametros = parametros;
            this.firma = firma;
            this.EsZebra = true;
        }

        public GaritaSalidaEtiqueta() { }

        public static string FrontBack(string str)
        {
            int len = str.Length;
            return str[0] + "" + str[len - 1];
        }

        public string GenerarZpl()
        {
            return "^XA\r\n~TA000~JSN^LT0^MNW^MTT^PON^PMN^LH0,0^JMA^PR5,5^MD15^LRN^CI0\r\n^MMT\r\n^PW609\r\n^LL0406\r\n^LS0\r\n^FO8,12^GB593,384,4" + 
                "^FS\r\n^FT21,50^A0N,28,28^FH\\^FDPatentes: " +
                "^FS\r\n^FT21,52^A0N,35,35^FH\\^FD             " + parametros.Patente + "  " + parametros.PatenteAcoplado +
                "^FS\r\n^FT21,90^A0N,28,28^FH\\^FDTarjeta: " + parametros.NumeroDeTarjetaAsignada + "  " + parametros.MaterialDesc +
                "^FS\r\n^FT21,120^A0N,28,28^FH\\^FDNum de Doc: " + parametros.NumeroDocumento + 
                        "^FS\r\n^FT21,160^A0N,28,28^FH\\^FDCalle: "+
                        "^FS\r\n^FT21,162^A0N,35,35^FH\\^FD       " + parametros.Calle +
                "^FS\r\n^FT21,200^A0N,28,28^FH\\^FDHidraulicas: " + 
                "^FS\r\n^FT21,202^A0N,35,35^FH\\^FD                " + parametros.Hidraulicas.Aggregate((i, j) => FrontBack(i) + ", " + FrontBack(j)) +
                "^FS\r\n^FT21,230^A0N,28,28^FH\\^FDCalidad: " + parametros.Calidad +
                "^FS\r\n^FT21,260^A0N,28,28^FH\\^FDAlmacen: " + parametros.Almacen +
                "^FS\r\n^FT21,290^A0N,28,28^FH\\^FDFecha y hora de calado: " + parametros.FechaCalado +
                "^FS\r\n^FT21,320^A0N,28,28^FH\\^FDHumedad: " + parametros.Humedad +
                (!string.IsNullOrEmpty(parametros.ProteinaAlta) ? "^FS\r\n^FT21,350^A0N,28,28^FH\\^FDProteina: " + parametros.ProteinaAlta :
                !string.IsNullOrEmpty(parametros.ProteinaBaja) ? "^FS\r\n^FT21,350^A0N,28,28^FH\\^FDProteina: " + parametros.ProteinaBaja : string.Empty
                ) +
                "^FS\r\n^FT21,380^A0N,28,28^FH\\^FD" + firma.Descripcion + " - Planta San Lorenzo^FS\r\n^XZ\r\n";
        }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            var font = new Font("Arial", 10);
            ImprimirTextoTicket(GenerarZpl(), 0, 0, font, e);
        }

        public override void Imprimir(object dto, string printerName, FirmaDto firmaDto)
        {
            parametros = (ImpGaritaSalidaDto)dto;
            firma = firmaDto;
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
