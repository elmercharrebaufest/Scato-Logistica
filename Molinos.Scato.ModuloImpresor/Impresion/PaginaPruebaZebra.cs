using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.ModuloImpresor.Impresion
{
    public class PaginaPruebaZebra : DocumentoImpresion
    {
        private string nombreUsuario;
        private string nombreServidor;

        public PaginaPruebaZebra(string nombreUsuario, string nombreServidor, string nombreImpresora)
        {
            PrinterSettings.PrinterName = nombreImpresora;
            this.nombreUsuario = nombreUsuario;
            this.nombreServidor = nombreServidor;
        }

        public PaginaPruebaZebra()
        {
        }

        public string GenerarZpl()
        {
            return "^XA^FO50,50^GB700,1,3^FS^FX^CFA,30^FO50,100^FDImpresióndeprueba^FS^FO50,150^FDUsuario:"
            + nombreUsuario + "^FS^FO50,200^FDServidor:"
            + nombreServidor + "^FS^FO50,250^FDImpresora:"
            + PrinterSettings.PrinterName + "^FS^CFA,15^FO50,330^GB700,1,3^FS^XZ";
        }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            var font = new Font("Arial", 10);

            ImprimirTextoTicket(GenerarZpl(), 0, 0, font, e);
        }

        public override void Imprimir(object dto, string printerName, FirmaDto firmaDto)
        {

            PrinterSettings.PrinterName = printerName;
            Print();
        }
    }
}
