using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.ModuloImpresor.Impresion
{
    public class DocumentoDeEntrada : DocumentoImpresion
    {
        private ImpDocumentoDeEntradaDto parametros;
        private FirmaDto firma;

        public DocumentoDeEntrada(ImpDocumentoDeEntradaDto parametros, string printerName, FirmaDto firma)
        {
            PrinterSettings.PrinterName = printerName;
            this.parametros = parametros;
            this.firma = firma;
        }

        public DocumentoDeEntrada() { }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            int xMargin = DefaultPageSettings.Margins.Left;  //X
            int yMargin = DefaultPageSettings.Margins.Top;  //Y
            var center = DefaultPageSettings.PaperSize.Width / 2;
            var der = DefaultPageSettings.PaperSize.Width - (DefaultPageSettings.PaperSize.Width / 3);

            var fontbold = new Font("Arial", 10, FontStyle.Bold);

            var posY = 0;
            var altoLinea = Font.Height + (Font.Height / 3);

            Text = firma.RazonSocial;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            Text = "Fecha de Emision ";
            ImprimirTexto(Text, der, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = parametros.Centro;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            Text = String.Format(CultureInfo.CurrentCulture, "{0:d/M/yyyy}", parametros.Fecha) + " " + String.Format(CultureInfo.CurrentCulture, "{0:HH:mm:ss tt}", parametros.Fecha);
            ImprimirTexto(Text, der, yMargin + posY, Font, e);
            posY += altoLinea*2;

            Text = "Tarjeta de Acceso";
            ImprimirTexto(Text, xMargin, yMargin + posY, fontbold, e, StringAlignment.Center);
            posY += altoLinea*2;

            Text = "Nro. Ingreso: " + parametros.NumeroDeIngreso;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e, StringAlignment.Center);
            posY += altoLinea * 3;


            ImprimirTexto("Chapa", xMargin, yMargin + posY, Font, e);
            ImprimirTexto("Fecha Ingreso", xMargin  * 3, yMargin + posY, Font, e);
            ImprimirTexto("Hora Ingreso", der, yMargin + posY, Font, e);
            posY += altoLinea;

            ImprimirTexto(parametros.Patente, xMargin, yMargin + posY, Font, e);
            ImprimirTexto(String.Format(CultureInfo.CurrentCulture, "{0:d/M/yyyy}", parametros.FechaDocumentoDeIngreso), xMargin * 3, yMargin + posY, Font, e);
            ImprimirTexto(String.Format(CultureInfo.CurrentCulture, "{0:HH:mm:ss}", parametros.FechaDocumentoDeIngreso), der, yMargin + posY, Font, e);

        }

        public override void Imprimir(object dto, string printerName, FirmaDto firmaDto)
        {
            parametros = (ImpDocumentoDeEntradaDto)dto;
            firma = firmaDto;
            if (!String.IsNullOrEmpty(printerName))
            {
                PrinterSettings.PrinterName = printerName;
                Print();
            }
        }
    }
}
