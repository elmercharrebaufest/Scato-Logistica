using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.ModuloImpresor.Impresion
{
    public class PaginaPrueba : DocumentoImpresion
    {
        private string nombreUsuario;
        private string nombreServidor;

        public PaginaPrueba(string nombreUsuario, string nombreServidor, string nombreImpresora)
        {
            PrinterSettings.PrinterName = nombreImpresora;
            this.nombreUsuario = nombreUsuario;
            this.nombreServidor = nombreServidor;
        }

        public PaginaPrueba()
        {
        }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            int xMargin = DefaultPageSettings.Margins.Left; //X
            int yMargin = DefaultPageSettings.Margins.Top; //Y

            var font24Bold = new Font("Arial", 24, FontStyle.Bold);
            var font10Bold = new Font("Arial", 10, FontStyle.Bold);
            var posY = 0;
            var altoLinea = Font.Height + (Font.Height/3);

            Text = "Impresión de prueba";
            ImprimirTexto(Text, xMargin, yMargin + posY, font24Bold, e);
            posY += altoLinea*2;

            Text = "Usuario: " + nombreUsuario;
            ImprimirTexto(Text, xMargin, yMargin + posY, font10Bold, e);
            posY += altoLinea;
            
            Text = "Servidor: " + nombreServidor;
            ImprimirTexto(Text, xMargin, yMargin + posY, font10Bold, e);
            posY += altoLinea;
            
            Text = "Impresora: " + PrinterSettings.PrinterName;
            ImprimirTexto(Text, xMargin, yMargin + posY, font10Bold, e);
        }

        public override void Imprimir(object dto, string printerName, FirmaDto firmaDto)
        {
            PrinterSettings.PrinterName = printerName;
            Print();
        }
    }
}
