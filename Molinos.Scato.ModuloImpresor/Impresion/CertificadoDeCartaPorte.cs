using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.ModuloImpresor.Impresion
{
    public class CertificadoDeCartaPorte : DocumentoImpresion
    {
        private ImpCertificadoDeCartaPorteDto parametros;

        public CertificadoDeCartaPorte(ImpCertificadoDeCartaPorteDto parametros, string printerName)
            : base(printerName)
        {
            this.parametros = parametros;
        }

        public CertificadoDeCartaPorte() { }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            DefaultPageSettings.PaperSize = new PaperSize("PaperA4", 826, 1169);
            int xMargin = DefaultPageSettings.Margins.Left;  //X
            int yMargin = DefaultPageSettings.Margins.Top;  //Y
            

            var font = new Font("Arial", 13, FontStyle.Regular);
            var fontBold = new Font("Arial", 13, FontStyle.Bold);
            var posY = 0;
            var altoLinea = Font.Height + (Font.Height / 3);
            var center = DefaultPageSettings.PaperSize.Width / 3;//centro de la hoja
            var izq = DefaultPageSettings.PaperSize.Width - (DefaultPageSettings.PaperSize.Width / 4);//izq

            Text = "Planta   : " + parametros.Centro;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "Producto : " + parametros.Material.ToUpper();
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "Balanza  : " + parametros.Balanza.ToUpper();
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += (2 * altoLinea);

            Text = "FECHA";
            ImprimirTexto(Text, izq - (2 * xMargin), yMargin + posY, Font, e);
            
            Text = "HORA";
            ImprimirTexto(Text, izq - xMargin, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "Nro. de Chapa: " + parametros.Patente;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);

            Text = "ENTRADA: ";
            ImprimirTexto(Text, izq - (3 * xMargin), yMargin + posY, Font, e);

            Text = String.Format(CultureInfo.CurrentCulture, "{0:d/M/yy}", parametros.FechaEntrada);
            ImprimirTexto(Text, izq - (2 * xMargin), yMargin + posY, Font, e);

            Text = String.Format(CultureInfo.CurrentCulture, "{0:HH:mm}", parametros.FechaEntrada);
            ImprimirTexto(Text, izq - xMargin, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "Nro. de Orden: " + parametros.NumeroIngreso;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);

            Text = "SALIDA : ";
            ImprimirTexto(Text, izq - (3 * xMargin), yMargin + posY, Font, e);

            Text = String.Format(CultureInfo.CurrentCulture, "{0:d/M/yy}", parametros.FechaSalida);
            ImprimirTexto(Text, izq - (2 * xMargin), yMargin + posY, Font, e);

            Text = String.Format(CultureInfo.CurrentCulture, "{0:HH:mm}", parametros.FechaSalida);
            ImprimirTexto(Text, izq - xMargin, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "Nro. de Certificación: " + parametros.NumeroCertificacion;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "Doc. Ingreso: " + parametros.TipoDocumento + "   " + parametros.NumeroDocumento;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += (4 * altoLinea);

            Text = "BRUTO";
            ImprimirTexto(Text, izq - (5 * xMargin), yMargin + posY, Font, e);

            Text = "TARA";
            ImprimirTexto(Text, izq - (4 * xMargin), yMargin + posY, Font, e);

            Text = "NETO";
            ImprimirTexto(Text, izq - (3 * xMargin), yMargin + posY, Font, e);

            Text = "NETO ORIGEN";
            ImprimirTexto(Text, izq - (2 * xMargin), yMargin + posY, Font, e);

            Text = "DIFERENCIA";
            ImprimirTexto(Text, izq - xMargin + (5 * 13), yMargin + posY, Font, e);
            posY += altoLinea;

            Text = parametros.PesoBruto;
            ImprimirTexto(Text, izq - (5 * xMargin), yMargin + posY, Font, e);

            Text = parametros.PesoTara;
            ImprimirTexto(Text, izq - (4 * xMargin), yMargin + posY, Font, e);

            Text = parametros.PesoNeto;
            ImprimirTexto(Text, izq - (3 * xMargin), yMargin + posY, Font, e);

            Text = parametros.PesoNetoOrigen;
            ImprimirTexto(Text, izq - (2 * xMargin), yMargin + posY, Font, e);

            Text = parametros.Diferencia;
            ImprimirTexto(Text, izq - xMargin + (5 * 13), yMargin + posY, Font, e);
            posY += (5 * altoLinea);
            
            Text = "__________________________";
            ImprimirTexto(Text, izq - xMargin, yMargin + posY, Font, e);
            posY += (2 * altoLinea);

            Text = "Firma";
            ImprimirTexto(Text, izq - xMargin, yMargin + posY, Font, e);

            var altoLineaLetraGrande = font.Height + (font.Height / 3);
            var altoLineaLetraNegrita = fontBold.Height + (fontBold.Height / 3);
            posY += (4 * altoLineaLetraGrande);
            Text = "Ahora podrás consultar: \n \u2022 Ticket pesada \n \u2022 Carta porte \n \u2022 Ticket Tasa Municipal \n \t \t Págalo con Mercado Pago \n \n Ingresá a:"; 
            ImprimirTexto(Text, center, yMargin + posY, font, e);

            posY += Convert.ToInt16(5.5 * altoLineaLetraGrande);
            Text = "www.moaoperaciones.com.ar";
            ImprimirTexto(Text, center, yMargin + posY, fontBold, e);

            posY += altoLineaLetraGrande;
            Text = "y seleccioná 'Comprobante del transporte''";
            ImprimirTexto(Text, center, yMargin + posY, font, e);

            posY += altoLineaLetraGrande;
            Text = "ATENCIÓN: a partir de Febrero 2022 \n no se entregará esta información impresa, \n para visualizarla también se deberá \n ingresar a la web.";
            ImprimirTexto(Text, center, yMargin + posY, font, e);

        }

        public override void Imprimir(object dto, string printerName, FirmaDto firmaDto)
        {
            parametros = (ImpCertificadoDeCartaPorteDto)dto;
            if (!String.IsNullOrEmpty(printerName))
            {
                PrinterSettings.PrinterName = printerName;
                Print();
            }
        }
    }
}
