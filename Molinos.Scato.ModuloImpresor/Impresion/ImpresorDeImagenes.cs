using System;
using System.Drawing;
using System.Drawing.Printing;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.ModuloImpresor.Impresion
{
    public class ImpresorDeImagenes : DocumentoImpresion
    {
        private readonly Image imagen;

        public ImpresorDeImagenes(Image imagen)
        {
            this.imagen = imagen;
        }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            Rectangle m = e.MarginBounds;

            if ((double)imagen.Width / (double)imagen.Height > (double)m.Width / (double)m.Height) // image is wider
            {
                m.Height = (int)((double)imagen.Height / (double)imagen.Width * (double)m.Width);
            }
            else
            {
                m.Width = (int)((double)imagen.Width / (double)imagen.Height * (double)m.Height);
            }

            e.Graphics.DrawImage(imagen, m);
        }

    }
}
