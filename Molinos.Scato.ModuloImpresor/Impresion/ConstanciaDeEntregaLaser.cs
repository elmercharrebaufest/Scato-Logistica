using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.ModuloImpresor.Impresion
{
    public class ConstanciaDeEntregaLaser : DocumentoImpresion
    {
        private ImpConstanciaDeEntregaLaserDto parametros;
        private FirmaDto firma;

        public ConstanciaDeEntregaLaser(ImpConstanciaDeEntregaLaserDto parametros, string printerName, FirmaDto firma)
        {
            PrinterSettings.PrinterName = printerName;
            this.parametros = parametros;
            this.firma = firma;
        }

        public ConstanciaDeEntregaLaser() { }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            int xMargin = DefaultPageSettings.Margins.Left;  //X
            int yMargin = DefaultPageSettings.Margins.Top;  //Y
            var center = DefaultPageSettings.PaperSize.Width / 2;
            var der = DefaultPageSettings.PaperSize.Width - (DefaultPageSettings.PaperSize.Width / 3);
            var col2 = (DefaultPageSettings.PaperSize.Width / 4);
            var col3 = DefaultPageSettings.PaperSize.Width - (DefaultPageSettings.PaperSize.Width / 2);
            var col4 = DefaultPageSettings.PaperSize.Width - (DefaultPageSettings.PaperSize.Width / 3);

            var despl = DefaultPageSettings.PaperSize.Width / 8;

            var font8Normal = new Font("Arial", 8, FontStyle.Regular);
            var fontsub = new Font("Arial", 10, FontStyle.Underline);
            var fontBoldSub = new Font("Arial", 10, FontStyle.Underline | FontStyle.Bold);
            var posY = 0;
            var altoLinea = Font.Height + (Font.Height / 3);

            Text = firma.Descripcion;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            Text = "Fecha: " + String.Format(CultureInfo.CurrentCulture, "{0:d/M/yyyy}", parametros.Fecha);
            ImprimirTexto(Text, der, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = firma.DescripcionCorta + " – " + parametros.Centro;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            Text = "Hora: " + String.Format(CultureInfo.CurrentCulture, "{0:HH:mm}", parametros.Fecha);
            ImprimirTexto(Text, der, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = parametros.CentroDireccion + "-" + parametros.CentroLocalidad + "-" + parametros.CentroProvincia;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            Text = "Número: " + parametros.NumeroCertificacion;
            ImprimirTexto(Text, der, yMargin + posY, Font, e);
            posY += altoLinea*2;

            Text = "CERTIFICACIÓN HOJA DE RUTA / CARTA DE PORTE";
            ImprimirTexto(Text, xMargin, yMargin + posY, fontsub, e, StringAlignment.Center);
            posY += altoLinea*2;

            Text = "Nro. Orden de Ingreso: ";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            Text = parametros.NumeroDeOrden;
            ImprimirTexto(Text, center, yMargin + posY, font8Normal, e);
            posY += altoLinea;

            Text = "Producto: ";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            Text = parametros.Material.ToUpper();
            ImprimirTexto(Text, center, yMargin + posY, font8Normal, e);
            posY += altoLinea;

            Text = "Corredor: ";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            Text = parametros.Corredor.ToUpper();
            ImprimirTexto(Text, center, yMargin + posY, font8Normal, e);
            posY += altoLinea;

            Text = "Vendedor: ";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            Text = parametros.Vendedor.ToUpper();
            ImprimirTexto(Text, center, yMargin + posY, font8Normal, e);
            posY += altoLinea;

            Text = "Entregador: ";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            Text = parametros.Entregador.ToUpper();
            ImprimirTexto(Text, center, yMargin + posY, font8Normal, e);
            posY += altoLinea;

            Text = "Procedencia: ";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            Text = parametros.Procedencia.ToUpper();
            ImprimirTexto(Text, center, yMargin + posY, font8Normal, e);
            posY += altoLinea;

            Text = "Carta de Porte / Remito Nº: ";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            Text = parametros.NumeroCartaPorte;
            ImprimirTexto(Text, center, yMargin + posY, font8Normal, e);
            posY += altoLinea*2;

            ImprimirTexto("Peso Bruto: ", xMargin, yMargin + posY, Font, e);
            ImprimirTexto(parametros.PesoBruto + " " + parametros.BalanzaBruto + " " + parametros.ModeloBalanzaBruto + " " + parametros.NroSerieBalanzaBruto, col2, yMargin + posY, font8Normal, e);
            ImprimirTexto("Fecha Descarga: ", col3 + despl, yMargin + posY, Font, e);
            ImprimirTexto(String.Format(CultureInfo.CurrentCulture, "{0:d/M/yyyy}", parametros.FechaCartaPorte), col4 + despl, yMargin + posY, font8Normal, e);
            posY += altoLinea;

            ImprimirTexto("Peso Tara: ", xMargin, yMargin + posY, Font, e);
            ImprimirTexto(parametros.PesoTara + " " + parametros.BalanzaTara + " " + parametros.ModeloBalanzaTara + " " + parametros.NroSerieBalanzaTara, col2, yMargin + posY, font8Normal, e);
            ImprimirTexto("Patente camión: ", col3+despl, yMargin + posY, Font, e);
            ImprimirTexto(parametros.Patente, col4+despl, yMargin + posY, font8Normal, e);
            posY += altoLinea;

            ImprimirTexto("Peso Neto: ", xMargin, yMargin + posY, Font, e);
            ImprimirTexto(parametros.PesoNeto, col2, yMargin + posY, font8Normal, e);
            ImprimirTexto("Patente acopl.: ", col3+despl, yMargin + posY, Font, e);
            ImprimirTexto(parametros.PatenteAcoplado, col4+despl, yMargin + posY, font8Normal, e);
            posY += altoLinea*2;

            ImprimirTexto("Observaciones: ", xMargin, yMargin + posY, Font, e);
            posY += altoLinea;
            ImprimirTexto(parametros.Observaciones, xMargin, yMargin + posY, font8Normal, e);
            posY += altoLinea * 5;

            ImprimirTexto("Observaciones Calada: ", xMargin, yMargin + posY, Font, e);
            posY += altoLinea;
            ImprimirTexto(parametros.ObservacionesCalado, xMargin, yMargin + posY, font8Normal, e);
            posY += altoLinea * 5;

            Text = "KILOS NETOS SUJETOS A LAS CONDICIONES COMERCIALES ACORDADAS";
            ImprimirTexto(Text, xMargin, yMargin + posY, fontBoldSub, e);
            posY += altoLinea * 2;

            ImprimirTexto("Rubro", xMargin, yMargin + posY, fontsub, e);
            ImprimirTexto("Cant. Observado", (xMargin + center) / 2 + despl / 2, yMargin + posY, fontsub, e);
            ImprimirTexto("Descuento Kg", center + despl / 2, yMargin + posY, fontsub, e);
            ImprimirTexto("A Camara", center + 2 * despl, yMargin + posY, fontsub, e);
            posY += altoLinea;

            foreach (var param in parametros.CaracteristicasCalidadValor)
            {
                ImprimirTexto(param.Key.ToUpper(), xMargin, yMargin + posY, font8Normal, e);
                ImprimirTexto(param.Data, (xMargin + center) / 2 + despl / 2, yMargin + posY, font8Normal, e);
                ImprimirTexto(param.Descuento, center + despl / 2, yMargin + posY, font8Normal, e);
                ImprimirTexto(param.EnvioCamara, center + 2 * despl, yMargin + posY, font8Normal, e);
                posY += altoLinea;
            }
        }

        public override void Imprimir(object dto, string printerName, FirmaDto firmaDto)
        {
            parametros = (ImpConstanciaDeEntregaLaserDto)dto;
            firma = firmaDto;
            if (!String.IsNullOrEmpty(printerName))
            {
                PrinterSettings.PrinterName = printerName;
                Print();
            }
        }
    }
}
