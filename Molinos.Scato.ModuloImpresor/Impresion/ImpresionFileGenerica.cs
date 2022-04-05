using Molinos.Scato.Dominio.Comandos;
using PdfiumViewer;
using System;
using System.Drawing.Printing;
using System.IO;

namespace Molinos.Scato.ModuloImpresor.Impresion
{
    public class ImpresionFileGenerica : DocumentoImpresion
    {
        private ImprimirFileGenerico impresion;

        public ImpresionFileGenerica(ImprimirFileGenerico impresion)
        {
            this.impresion = impresion;
        }

        public ImpresionFileGenerica()
        {
        }

        public bool PrintPDF(string szPrinterName, byte[] file, int copies)
        {
            try
            {
                // Create the printer settings for our printer
                var printerSettings = new PrinterSettings
                {
                    PrinterName = szPrinterName,
                    Copies = (short)copies,
                };

                // Create our page settings for the paper size selected
                var pageSettings = new PageSettings(printerSettings)
                {
                    Margins = new Margins(0, 0, 0, 0),
                };
                foreach (PaperSize paperSize in printerSettings.PaperSizes)
                {
                    if (paperSize.PaperName == "A4")
                    {
                        pageSettings.PaperSize = paperSize;
                        break;
                    }
                }

                // Now print the PDF document
                using (var document = PdfDocument.Load(new MemoryStream(file)))
                {
                    using (var printDocument = document.CreatePrintDocument())
                    {
                        printDocument.PrinterSettings = printerSettings;
                        printDocument.DefaultPageSettings = pageSettings;
                        printDocument.PrintController = new StandardPrintController();
                        printDocument.Print();
                    }
                }
                return true;
            }
            catch
            {
                throw;
            }
        }
    }
}