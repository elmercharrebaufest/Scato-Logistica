using System;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.ModuloImpresor.Impresion
{
    public class CartaPorteUrenport : DocumentoImpresion
    {
        private ImpCartaPorteUrenportDto parametros;

        public CartaPorteUrenport(ImpCartaPorteUrenportDto parametros, string printerName)
            : base(printerName)
        {
            this.parametros = parametros;
        }

        public CartaPorteUrenport() { }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            using (var ms = new MemoryStream(ObtenerImagen(parametros.FotoRutaDestino)))
            {
                var m = e.MarginBounds;
                var i = Image.FromStream(ms);

                e.Graphics.DrawImage(i, 0, 0, e.PageBounds.Width, e.PageBounds.Height);
            }
        }

        private byte[] ObtenerImagen(string path)
        {
            byte[] buffer = null;
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                buffer = new byte[fs.Length];
                fs.Read(buffer, 0, (int)fs.Length);
            }
            return buffer;
        }

        public override void Imprimir(object dto, string printerName, FirmaDto firmaDto)
        {
            parametros = (ImpCartaPorteUrenportDto)dto;
            if (!String.IsNullOrEmpty(printerName))
            {
                PrinterSettings.PrinterName = printerName;
                Print();
            }
        }
    }
}
