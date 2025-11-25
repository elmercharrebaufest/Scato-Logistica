using Molinos.Scato.Dominio.Comandos;
using Ninject.Extensions.Logging;
using PdfiumViewer;
using System;
using System.Drawing.Printing;
using System.IO;

namespace Molinos.Scato.ModuloImpresor.Procesamiento
{
    public class ProcesadorImprimirFileGenerico : ProcesadorComando<ImprimirFileGenerico>
    {
        public ProcesadorImprimirFileGenerico(ILogger log)
            : base(log)
        {
        }

        public override Resultado Ejecutar(ImprimirFileGenerico comando)
        {
            var resultado = new Resultado();

            Log.Debug($"Iniciando impresión de {comando?.CodigoDocumentoImpresion} en la impresora: {comando.Impresora} - Copias: {comando.CantidadCopias}");
            if (string.IsNullOrEmpty(comando.Impresora))
            {
                Log.Error("Error al imprimir: Impresora no encontrado");
                return resultado;
            }

            if (comando.CantidadCopias <= 0)
            {
                Log.Error("Error al imprimir: Cantidad de copias no válida");
                return resultado;
            }

            try
            {
                PrintPDF(comando.Impresora, comando.File, comando.CantidadCopias);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al imprimir " + comando?.CodigoDocumentoImpresion + " en la impresora: " + comando?.Impresora);
                resultado.Errores.Add("1", $"Error al imprimir el documento : {comando?.CodigoDocumentoImpresion} impresora : {comando?.Impresora}");
            }

            Log.Debug($"Fin impresión de {comando?.CodigoDocumentoImpresion} en la impresora: {comando.Impresora} - Hay error: {resultado.HayErrores}");
            return resultado;
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