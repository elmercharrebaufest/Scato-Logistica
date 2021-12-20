using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.ModuloImpresor.Impresion
{
    public class TicketPesada : DocumentoImpresion
    {
        private ImpTicketPesadaDto parametros;
        private FirmaDto firma;

        public TicketPesada(ImpTicketPesadaDto parametros, string printerName, FirmaDto firma)
            : base(printerName)
        {
            this.parametros = parametros;
            this.firma = firma;
        }

        public TicketPesada() { }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            int xMargin = DefaultPageSettings.Margins.Left;  //X
            int yMargin = DefaultPageSettings.Margins.Top;  //Y

            var fontBold = new Font("Arial", 10, FontStyle.Bold);
            var posY = 0;
            var altoLinea = Font.Height + (Font.Height/3);
            var center = DefaultPageSettings.PaperSize.Width / 2;//centro de la hoja

            Text = "TICKET DE BALANZA";
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "Segun Resol AFIP 271/98 y modif";
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "Documento no valido como factura";
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += (2 * altoLinea);

            Text = "Fecha de Emisión: " + String.Format(CultureInfo.CurrentCulture, "{0:d/M/yyyy}", DateTime.Now);
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "Nro: " + parametros.NumeroIngreso;
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += (2 * altoLinea);

            Text = firma.RazonSocial;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);

            Text = "CUIT " + firma.Cuit;
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = firma.Direccion;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);

            Text = "Ing. Brutos Conv Multilateral    " + firma.IngBrutosConvMultilateral;
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "Buenos Aires";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);

            Text = "Inicio de Actividades        " + firma.FechaDeInicio;
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += (3 * altoLinea);

            Text = "Emisor    ";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);

            Text = parametros.Emisor.ToUpper();
            ImprimirTexto(Text, xMargin + 100, yMargin + posY, fontBold, e);
            posY += (2 * altoLinea);

            Text = "Domicilio de Pesaje  " + parametros.DoimicilioCentro;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "Cbte de Traslado  " + parametros.TipoDocumento;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);

            Text = "Nro. Cbte  " + parametros.NumeroDocumento;
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += (2 * altoLinea);

            Text = "Producto Transportado  " + parametros.Material.ToUpper();
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += (2 * altoLinea);

            Text = "Peso Bruto          " + parametros.PesoBruto;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "Peso Tara           " + parametros.PesoTara;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "Peso Total Neto     " + parametros.PesoNeto;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += (2 * altoLinea);

            Text = "Remitente  ";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);

            Text = parametros.Remitente.ToUpper();
            ImprimirTexto(Text, xMargin + 100, yMargin + posY, fontBold, e);
            posY += altoLinea;

            Text = "CUIT " + firma.Cuit;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += (3 * altoLinea);

            Text = "Transportista   " + parametros.Transportista.ToUpper();
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "CUIT " + parametros.CuitTransportista;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += (2 * altoLinea);

            Text = "Patente Camión       " + parametros.Patente;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "Patente Acoplado     " + parametros.PatenteAcoplado;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += (2 * altoLinea);

            Text = "Observaciones  " + parametros.Observaciones;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += (4 * altoLinea);

            Text = "__________________________";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);

            Text = "__________________________";
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "CONFECCIONO";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);

            Text = "AUTORIZO";
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += (3 * altoLinea);

            Text = parametros.Identidad.ToString().ToUpper();
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
        }

        public override void Imprimir(object dto, string printerName, FirmaDto firmaDto)
        {
            parametros = (ImpTicketPesadaDto)dto;
            firma = firmaDto;
            if (!String.IsNullOrEmpty(printerName))
            {
                PrinterSettings.PrinterName = printerName;
                Print();
            }
        }
    }
}
