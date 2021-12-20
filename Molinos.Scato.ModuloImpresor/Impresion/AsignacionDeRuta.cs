using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.ModuloImpresor.Impresion
{
    public class AsignacionDeRuta : DocumentoImpresion
    {
        private ImpAsignacionDeRutaDto parametros;

        public AsignacionDeRuta(ImpAsignacionDeRutaDto parametros, string printerName)
        {
            PrinterSettings.PrinterName = printerName;
            this.parametros = parametros;
        }

        public AsignacionDeRuta(){}

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            int xMargin = DefaultPageSettings.Margins.Left;  //X
            int yMargin = DefaultPageSettings.Margins.Top;  //Y
            var center = DefaultPageSettings.PaperSize.Width / 2;
            var der = DefaultPageSettings.PaperSize.Width - (DefaultPageSettings.PaperSize.Width / 3);

            var fontNormal = new Font("Arial", 10, FontStyle.Regular);
            var fontGrande = new Font("Arial", 35, FontStyle.Bold);
            var fontSub = new Font("Arial", 10, FontStyle.Underline);
            var fontBold = new Font("Arial", 10, FontStyle.Bold);
            var fontBoldSub = new Font("Arial", 10, FontStyle.Bold | FontStyle.Underline);
            var posY = 0;
            var altoLinea = fontNormal.Height + (fontNormal.Height / 3);

            Text = "Nº Orden:" + parametros.NumeroDeOrden + "  ASIGNACION DE RECORRIDO\n";
            ImprimirTexto(Text, xMargin, yMargin + posY, fontNormal, e);

            Text = "Fecha: " + String.Format(CultureInfo.CurrentCulture, "{0:d/M/yyyy}", parametros.FechaImpresion) +
                    "\nHora: " + String.Format(CultureInfo.CurrentCulture, "{0:HH:mm:ss}", parametros.FechaImpresion);
            ImprimirTexto(Text, der, yMargin + posY, fontNormal, e);
            posY += altoLinea * 3;

            Text = "Patente: ";
            ImprimirTexto(Text, xMargin, yMargin + posY, fontBold, e);
            Text = parametros.Patente.ToUpper();
            ImprimirTexto(Text, xMargin + 45, yMargin + posY / 2, fontGrande, e);
            Text = " Acoplado: ";
            ImprimirTexto(Text, xMargin * 4, yMargin + posY, fontBold, e);
            Text = parametros.PatenteAcoplado;
            ImprimirTexto(Text, xMargin * 5, yMargin + posY/2, fontGrande, e);
            posY += altoLinea * 2;

            ImprimirTexto("Tipo Vehiculo: ", xMargin, yMargin + posY, fontNormal, e);
            ImprimirTexto(parametros.TipoVehiculo, xMargin * 2, yMargin + posY, fontBold, e);

            posY += altoLinea * 2;

            Text = "Material: " + parametros.MaterialCodigoSap + " " + parametros.Material;
            ImprimirTexto(Text, xMargin, yMargin + posY, fontNormal, e);
            posY += altoLinea * 2;

            Text = "Balanza bruto: " + parametros.BalanzaBruto;
            ImprimirTexto(Text, xMargin, yMargin + posY, fontNormal, e);
            posY += altoLinea * 2;

            Text = "Calle: " + parametros.Calle;
            ImprimirTexto(Text, xMargin, yMargin + posY, fontNormal, e);
            posY += altoLinea * 2;

            Text = "Carga/Descarga: ";
            foreach (var h in parametros.Hidraulicas)
            {
                Text += h.ToUpper() + "/ ";
            }
            ImprimirTexto(Text, xMargin, yMargin + posY, fontSub, e);
            posY += altoLinea * 3;

            Text = "Almacen: " + parametros.Almacen;
            ImprimirTexto(Text, xMargin, yMargin + posY, fontNormal, e);
            posY += altoLinea * 2;

            Text = "Balanza tara: " + parametros.BalanzaTara;
            ImprimirTexto(Text, xMargin, yMargin + posY, fontNormal, e);
            posY += altoLinea * 2;

            Text = "Humedad: " + (parametros.Humedad ?? "") + " %";
            ImprimirTexto(Text, xMargin, yMargin + posY, fontBold, e);
            posY += altoLinea * 2;
            
            Text = "Calidad: " + parametros.Calidad.ToUpper();
            ImprimirTexto(Text, xMargin, yMargin + posY, fontBold, e);
            posY += altoLinea * 1;

            Text = "Fecha Calado: " + (parametros.FechaCalado != null ? String.Format(CultureInfo.CurrentCulture, "{0:d/M/yyyy HH:mm tt}", parametros.FechaCalado) : "");
            ImprimirTexto(Text, xMargin, yMargin + posY, fontBoldSub, e);

            if (!string.IsNullOrEmpty(parametros.Observacion))
            {
                posY += altoLinea * 2;
                ImprimirTexto(parametros.Observacion, xMargin + 45, yMargin + posY, fontGrande, e);
            }
            posY += 20 * altoLinea;

            Text = "          __________________________";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea * 1;

            Text = "Firma del operador de Carga/Descarga";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);

        }

        public override void Imprimir(object dto, string printerName, FirmaDto firmaDto)
        {
            parametros = (ImpAsignacionDeRutaDto)dto;
            if (!String.IsNullOrEmpty(printerName))
            {
                PrinterSettings.PrinterName = printerName;
                Print();
            }
        }
    }
}
