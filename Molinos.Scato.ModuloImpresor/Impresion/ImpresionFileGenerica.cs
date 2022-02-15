using Molinos.Scato.Dominio.Comandos;
using System.Drawing.Printing;

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

        protected override void OnBeginPrint(PrintEventArgs e)
        {
            RawPrinterHelper.SendFileBytesPrinter(impresion.Impresora, impresion.File);
        }
    }
}