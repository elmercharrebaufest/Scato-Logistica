using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.ModuloImpresor.Impresion
{
    public class SolicitudDeAnalisis : DocumentoImpresion
    {
        private ImpSolicitudDeAnalisisDto parametros;

        public SolicitudDeAnalisis(ImpSolicitudDeAnalisisDto parametros, string printerName)
        {
            PrinterSettings.PrinterName = printerName;
            this.parametros = parametros;
        }

        public SolicitudDeAnalisis() { }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            int xMargin = DefaultPageSettings.Margins.Left;  //X
            int yMargin = DefaultPageSettings.Margins.Top;  //Y
            var center = DefaultPageSettings.PaperSize.Width / 2;
            var der = DefaultPageSettings.PaperSize.Width - (DefaultPageSettings.PaperSize.Width / 2);

            var font9Normal = new Font("Arial", 9, FontStyle.Regular);
            var fontNormal = new Font("Arial", 10, FontStyle.Regular);
            var fontBold = new Font("Arial", 10, FontStyle.Bold);
            var posY = 0;
            var altoLinea = Font.Height + (Font.Height / 3);

            ImprimirTexto(parametros.Centro, xMargin, yMargin + posY, fontBold, e);
            posY += altoLinea;

            Text = "SOLICITUD ANALISIS FISICO Nº:";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            ImprimirTexto(parametros.NumeroAnalisis, xMargin + 220, yMargin + posY, fontBold, e);
            posY += altoLinea;

            Text = "SE ADJUNTA MUESTRA PARA LA REALIZACION DEL ANALISIS CORRESPONDIENTE.";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea * 5;

            Text = "_______________________\n Firma Solicitante";
            ImprimirTexto(Text, xMargin + der, yMargin + posY, font9Normal, e);
            posY +=  altoLinea * 3;

            Text = "PRODUCTO: " + parametros.Material.ToUpper();
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "----------------------------------------------------------------------------------------------------------------------------";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "CONCEPTO                          PORCEN.    KILOS       OBSERVACIONES";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "----------------------------------------------------------------------------------------------------------------------------";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;

            foreach (var c in parametros.CaracteristicaDeCalidad)
            {
                ImprimirTexto(c, xMargin, yMargin + posY, Font, e);
                posY += altoLinea + altoLinea;
            }

            posY = DefaultPageSettings.PaperSize.Height - altoLinea*11;
            Text = "FIRMA LABORATORIO     : _____________________________________________";
            ImprimirTexto(Text, xMargin, posY, Font, e);
            posY += altoLinea * 4;

            Text = "FIRMA JEFE MAT. PRIMAS: _____________________________________________";
            ImprimirTexto(Text, xMargin, posY, Font, e);
            posY += altoLinea * 2;

            Text = "----------------------------------------------------------------------------------------------------------------------------";
            ImprimirTexto(Text, xMargin, posY, Font, e);
            posY += altoLinea;

            Text = "CALADO: " + String.Format(CultureInfo.CurrentCulture, "{0:d/M/yyyy}", parametros.FechaCalado) + "    Hora: " + String.Format(CultureInfo.CurrentCulture, "{0:HH:mm:ss}", parametros.FechaCalado) + "   ORDEN: " +
                    parametros.NumeroDeOrden + "   CHAPA: " + parametros.Patente;
            ImprimirTexto(Text, xMargin, posY, Font, e);
        }

        public override void Imprimir(object dto, string printerName, FirmaDto firmaDto)
        {
            parametros = (ImpSolicitudDeAnalisisDto)dto;
            if (!String.IsNullOrEmpty(printerName))
            {
                PrinterSettings.PrinterName = printerName;
                Print();
            }
        }
    }
}
