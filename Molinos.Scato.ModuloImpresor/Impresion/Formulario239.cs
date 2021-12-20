using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.ModuloImpresor.Impresion
{
    public class Formulario239 : DocumentoImpresion
    {
        private ImpFormulario239Dto parametros;
        private FirmaDto firma;

        public Formulario239(ImpFormulario239Dto parametros, string printerName, FirmaDto firma)
        {
            PrinterSettings.PrinterName = printerName;
            this.parametros = parametros;
            this.firma = firma;
        }

        public Formulario239() { }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            int xMargin = DefaultPageSettings.Margins.Left;  //X
            int yMargin = DefaultPageSettings.Margins.Top;  //Y
            var center = DefaultPageSettings.PaperSize.Width / 2;
            var der = DefaultPageSettings.PaperSize.Width - (DefaultPageSettings.PaperSize.Width / 3);

            var font12Bold = new Font("Arial", 12, FontStyle.Bold);
            var font11Bold = new Font("Arial", 11, FontStyle.Bold);
            var font9Bold = new Font("Arial", 9, FontStyle.Bold);
            var font8Bold = new Font("Arial", 8, FontStyle.Bold);
            var fontBold = new Font("Arial", 10, FontStyle.Bold);
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
            posY += altoLinea;

            Text = "----------------------------------------------------------------------------------------------------------------------------";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "AVISO\nPARA PORTERIA";
            ImprimirTexto(Text, xMargin, yMargin + posY, font12Bold, e, StringAlignment.Center);
            posY += font12Bold.Height + (font12Bold.Height / 3) * 3;

            ImprimirTexto("F.239", xMargin, yMargin + posY, font8Bold, e, StringAlignment.Center);
            posY += altoLinea;

            Text = "----------------------------------------------------------------------------------------------------------------------------";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "AUTORIZACION DE: -------------------- (1)";
            ImprimirTexto(Text, xMargin * 2, yMargin + posY, font11Bold, e);
            ImprimirTexto("ENTRADA\nSALIDA", xMargin * 2 + 160, yMargin + posY - font11Bold.Height / 2, font11Bold, e);
            ImprimirTexto("Nº " + parametros.NumeroDeOrden, der, yMargin + posY, fontBold, e);
            posY += altoLinea*2;

            Text = "EMPRESA (Denominación): " + parametros.EmpresaCuit + " - " + parametros.Empresa;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "REPRESENTANTE (Apellido y Nombre): " + parametros.Representante;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;

            ImprimirTexto("PATENTE CAMION: " + parametros.Patente, xMargin, yMargin + posY, Font, e);
            ImprimirTexto("ORDEN INGRESO Nº: " + parametros.NumeroDeFormulario, center-100, yMargin + posY, Font, e);
            
            posY += altoLinea;

            ImprimirTexto("PATENTE ACOPLADO: " + parametros.PatenteAcoplado, xMargin, yMargin + posY, Font, e);
            ImprimirTexto(parametros.TipoDocumentoEntrada.ToString().ToUpper() +" Nº: " + parametros.NumeroDocumentoEntrada +" CTG: " + parametros.CTG + "", center-100, yMargin + posY, Font, e);
            posY += altoLinea * 2;

            ImprimirTexto("ESTADO DEL VEHICULO: ------------------- (1)", xMargin, yMargin + posY, Font, e);
            ImprimirTexto("CARGADO\nVACIO", xMargin + 175, yMargin + posY - Font.Height / 2, fontBold, e);
            posY += altoLinea * 2;

            ImprimirTexto("MERCADERIA TRANSPORTADA: -------------------- (1)", xMargin, yMargin + posY, Font, e);
            ImprimirTexto("PROPIA\nDE TERC.", xMargin + 220, yMargin + posY - Font.Height / 2, fontBold, e);
            posY += altoLinea * 2;

            ImprimirTexto("DESCRIPCION DE LA MERCADERIA: " + parametros.MaterialCodigoSap + "-" + parametros.Material, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;
            ImprimirTexto("OBSERVACIONES: " + parametros.Observaciones, xMargin, yMargin + posY, Font, e);
            posY += altoLinea *4;

            Text = "----------------------------------------------------------------------------------------------------------------------------";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "AUTORIZACION";
            ImprimirTexto(Text, xMargin, yMargin + posY, fontBold, e, StringAlignment.Center);
            posY += altoLinea * 2;

            Text = "APELLIDO Y NOMBRE:_______________________________________________";
            ImprimirTexto(Text, xMargin * 2, yMargin + posY, fontBold, e);
            posY += altoLinea;
            Text = "CARGO:___________________________________________________________";
            ImprimirTexto(Text, xMargin * 2, yMargin + posY, fontBold, e);
            posY += altoLinea;
            Text = "Firma:___________________________________________________________";
            ImprimirTexto(Text, xMargin * 2, yMargin + posY, fontBold, e);
            posY += altoLinea * 2;
            ImprimirTexto("(1) Tachese lo que no corresponda.", xMargin, yMargin + posY, font8Bold, e);
        }

        public override void Imprimir(object dto, string printerName, FirmaDto firmaDto)
        {
            parametros = (ImpFormulario239Dto)dto;
            firma = firmaDto;
            if (!String.IsNullOrEmpty(printerName))
            {
                PrinterSettings.PrinterName = printerName;
                Print();
            }
        }
    }
}
