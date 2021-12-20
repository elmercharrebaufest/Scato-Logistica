using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using PrintDoc2Pdf;

namespace Molinos.Scato.ModuloImpresor.Impresion
{
    public class DocumentoImpresion : PrintDocument
    {
        public bool EsZebra { get; set; }
        public string ZplCode { get; set; }
        public Font Font { get; set; }
        public string Text { get; set; }
        public CultureInfo CurrentUICulture { get; set; }

        public DocumentoImpresion()
        {
            Text = string.Empty;
            CurrentUICulture = System.Threading.Thread.CurrentThread.CurrentUICulture;
        }

        public DocumentoImpresion(string printerName)
        {
            PrinterSettings.PrinterName = printerName;
            CurrentUICulture = System.Threading.Thread.CurrentThread.CurrentUICulture; 
        }

        protected override void OnBeginPrint(PrintEventArgs e)
        {
            base.OnBeginPrint(e);

            if (Font == null)
            {
                Font = new Font("Arial", 10);
            }
        }

        public int RemoveZeros(int value, string texto)
        {
            if (String.IsNullOrEmpty(texto))
            {
                return 0;
            }
            while (texto[value] == '\0')
            {
                value++;
                if (texto.Length == value)
                {
                    return value;
                }
            }
            return value;
        }


        protected void ImprimirTextoTicket(string texto, int x, int y, Font fuente, PrintPageEventArgs e, StringAlignment alineacion = StringAlignment.Near, StringFormatFlags direccion = StringFormatFlags.LineLimit)
        {
            if (String.IsNullOrEmpty(texto))
            {
                return;
            }
            texto = texto.Trim();

            var altoPagina = 900;
            var anchoPagina = 650;

            var format = new StringFormat(direccion) { Alignment = alineacion };

            var printArea = new RectangleF(x, y, anchoPagina, altoPagina);

            e.Graphics.DrawString(texto, fuente, Brushes.Black, printArea, format);
        }

        protected void ImprimirTexto(string texto, int x, int y, Font fuente, PrintPageEventArgs e, StringAlignment alineacion = StringAlignment.Near, StringFormatFlags direccion = StringFormatFlags.LineLimit)
        {
            if (String.IsNullOrEmpty(texto))
            {
                return;
            }
            texto = texto.Trim();

            var format = new StringFormat(direccion) { Alignment = alineacion };

            var printArea = new RectangleF(x, y, 900, 900);

            e.Graphics.DrawString(texto.Substring(RemoveZeros(0, texto)), fuente, Brushes.Black, printArea, format);
        }

        protected void ImprimirTexto(string texto, int x, int y, int ancho,int alto,Font fuente, PrintPageEventArgs e, StringAlignment alineacion = StringAlignment.Near, StringFormatFlags direccion = StringFormatFlags.LineLimit)
        {
            if (String.IsNullOrEmpty(texto))
            {
                return;
            }
            texto = texto.Trim();

            var format = new StringFormat(direccion) { Alignment = alineacion };

            var printArea = new RectangleF(x, y, ancho, alto);

            e.Graphics.DrawString(texto.Substring(RemoveZeros(0, texto)), fuente, Brushes.Black, printArea, format);
        }

        protected void ImprimirTexto(string texto, int fila, int columna, Font fuente, FormatoDeImpresionDto formato, PrintPageEventArgs e, StringAlignment alineacion = StringAlignment.Near, float? sizeWidth = null, int? textWidth = null)
        {
            if (String.IsNullOrEmpty(texto))
            {
                return;
            }
            texto = texto.Trim();

            if (textWidth.HasValue && textWidth < texto.Length)
            {
                texto = texto.Substring(0, textWidth.Value);
            }

            var anchoDelPapel = formato.FormatoDePapelAncho;
            var altoDelPapel = formato.FormatoDePapelAlto;

            var anchoDeColumna = anchoDelPapel / formato.Columnas;
            var altoDeFila = altoDelPapel / formato.Filas;

            if (formato.Posicion == Posicion.Horizontal)
            {
                altoDeFila = anchoDelPapel / formato.Filas;
                anchoDeColumna = altoDelPapel / formato.Columnas;
                e.Graphics.TranslateTransform(0, altoDelPapel);
                e.Graphics.RotateTransform(270);
            }
            e.Graphics.PageUnit = GraphicsUnit.Millimeter;

            var format = new StringFormat(StringFormatFlags.LineLimit) { Alignment = alineacion };
            var size = e.Graphics.MeasureString(texto, fuente);

            var printArea = new RectangleF(columna * anchoDeColumna + formato.MargenIzquierdo, fila * altoDeFila + formato.MargenSuperior, (sizeWidth ?? size.Width) < anchoDeColumna ? anchoDeColumna : (sizeWidth ?? size.Width), altoDeFila);

            e.Graphics.DrawString(texto.Substring(RemoveZeros(0, texto)), fuente, Brushes.Black, printArea, format);
            e.Graphics.ResetTransform();
        }

        public virtual void Imprimir(object dto, string printerName, FirmaDto firma) { }

        public virtual void Imprimir(object dto, string printerName, FormatoDeImpresionDto formato) { }
    }
}
