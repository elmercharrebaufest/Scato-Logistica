using System;
using System.Drawing.Printing;
using System.Globalization;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.ModuloImpresor.Impresion
{
    public class ControlDeCarga : DocumentoImpresion
    {
        private ImpControlDeCargaDto parametros;

        public ControlDeCarga(ImpControlDeCargaDto parametros, string printerName)
        {
            PrinterSettings.PrinterName = printerName;
            this.parametros = parametros;
        }

        public ControlDeCarga() { }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            int xMargin = DefaultPageSettings.Margins.Left;  //X
            int yMargin = DefaultPageSettings.Margins.Top;  //Y
            var der = DefaultPageSettings.PaperSize.Width - (DefaultPageSettings.PaperSize.Width / 3);
            var espacio = DefaultPageSettings.PaperSize.Width / 10;

            var posY = 0;
            var altoLinea = Font.Height + (Font.Height / 3);

            Text = "Control Nro: " + parametros.NumeroControl;
            ImprimirTexto(Text, der, yMargin + posY, Font, e);
            posY += altoLinea;
            Text = "Emisión: " + String.Format(CurrentUICulture, "{0:d/M/yyyy}", parametros.FechaImpresion);
            ImprimirTexto(Text, der, yMargin + posY, Font, e);
            posY += altoLinea*2;
            Text = "Planta: " + parametros.Centro;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea*3;
            Text = "Vehículo: " + parametros.Patente;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea*3;
            Text = "Entrada: " + String.Format(CultureInfo.CurrentCulture, "{0:d/M/yyyy}", parametros.FechaDocumentoDeIngreso) + " " + String.Format(CurrentUICulture, "{0:HH:mm:ss tt}", parametros.FechaDocumentoDeIngreso);
            ImprimirTexto(Text, der, yMargin + posY, Font, e);
            posY += altoLinea * 2;

            Text = "Bruto";
            ImprimirTexto(Text, xMargin + espacio, yMargin + posY, Font, e);
            Text = "Tara";
            ImprimirTexto(Text, xMargin + espacio * 2, yMargin + posY, Font, e);
            Text = "Neto";
            ImprimirTexto(Text, xMargin + espacio * 3, yMargin + posY, Font, e);
            Text = "Total Descargado";
            ImprimirTexto(Text, xMargin + espacio * 4, yMargin + posY, Font, e);
            Text = "Diferencia";
            ImprimirTexto(Text, xMargin + espacio * 6, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = parametros.PesoBruto != null ? String.Format(CultureInfo.CurrentCulture, "{0:0.00}", parametros.PesoBruto) : "";
            ImprimirTexto(Text, xMargin + espacio, yMargin + posY, Font, e);
            Text = parametros.PesoTara != null ? String.Format(CultureInfo.CurrentCulture, "{0:0.00}", parametros.PesoTara) : "";
            ImprimirTexto(Text, xMargin + espacio*2, yMargin + posY, Font, e);
            Text = parametros.PesoNeto != null ? String.Format(CultureInfo.CurrentCulture, "{0:0.00}", parametros.PesoNeto) : "";
            ImprimirTexto(Text, xMargin + espacio * 3, yMargin + posY, Font, e);
            Text = parametros.TotalDescargado != null ? String.Format(CultureInfo.CurrentCulture, "{0:0.00}", parametros.TotalDescargado) : "";
            ImprimirTexto(Text, xMargin + espacio * 4, yMargin + posY , Font, e);
            Text = parametros.Diferencia != null ? String.Format(CultureInfo.CurrentCulture, "{0:0.00}", parametros.Diferencia) : "";
            ImprimirTexto(Text, xMargin + espacio * 6, yMargin + posY , Font, e);

            posY += altoLinea * 3;

            Text = "Motivos:";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea * 2;
            Text = "[  ] Carga de pallets más";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;
            Text = "[  ] Carga de pallets menos";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;
            Text = "[  ] Carga de pallets equivocados";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;
            Text = "[  ] Diferencia de peso en mercadería";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;
            Text = "[  ] Faltantes de bultos en preparación";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;
            Text = "[  ] Habilitar salida con diferencia de kg";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;
            Text = "[  ] Pallets fuera de STD";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;
            Text = "[  ] Pallets mal armados";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;
            Text = "[  ] Pallets mal identificados";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;
            Text = "[  ] PRUEBA GDS";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;
            Text = "[  ] Sobrantes de bultos en preparación";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);

            posY += altoLinea * 10;
            Text = "_ _ _ _ _ _ _ _ _ __ _ _ _ _ _ _ _ _";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            Text = "_ _ _ _ _ _ _ _ _ __ _ _ _ _ _ _ _ _";
            ImprimirTexto(Text, der - 40, yMargin + posY, Font, e);
            posY += altoLinea;
            Text = "             Firma Autorizante";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            Text = "            Aclaración y Legajo";
            ImprimirTexto(Text, der - 40, yMargin + posY, Font, e);
        }

        public override void Imprimir(object dto, string printerName, FirmaDto firmaDto)
        {
            PrinterSettings.PrinterName = printerName;
            parametros = (ImpControlDeCargaDto)dto;
            Print();
        }
    }
}
