using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.ModuloImpresor.Impresion
{
    public class TicketPesadaBodega : DocumentoImpresion
    {
        private ImpTicketPesadaBodegaDto parametros;
        private FirmaDto firma;

        public TicketPesadaBodega(ImpTicketPesadaBodegaDto parametros, string printerName, FirmaDto firma)
            : base(printerName)
        {
            this.parametros = parametros;
            this.firma = firma;
        }

        public TicketPesadaBodega() { }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            var alto = 1100;
            var ancho = 600;
            int xMargin = DefaultPageSettings.Margins.Left;  //X
            int yMargin = DefaultPageSettings.Margins.Top;  //Y
            int buttomMargin = DefaultPageSettings.Margins.Bottom;

            var fontBold = new Font("Arial", 10, FontStyle.Bold);
            var fontBoldSubrayado = new Font("Arial", 10, FontStyle.Bold | FontStyle.Underline);
            var posY = 0;
            var altoLinea = Font.Height + (Font.Height/3);
            var center = (xMargin + ancho) / 2;//centro de la hoja

            Text = firma.RazonSocial;
            ImprimirTexto(Text, xMargin, yMargin + posY, fontBold, e);
            Text = "Fecha       " + String.Format(CultureInfo.CurrentCulture, "{0:dd/MM/yyyy}", parametros.FechaUltimaPesada);
            ImprimirTexto(Text, xMargin, yMargin + posY,ancho,alto, Font, e, StringAlignment.Far);          
            posY += altoLinea;

            Text = "Planta " + parametros.CentroDescripcion;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            Text = "Hora               " + String.Format(CultureInfo.CurrentCulture, "{0:hh:mm}", parametros.FechaUltimaPesada);
            ImprimirTexto(Text, xMargin, yMargin + posY, ancho, alto, Font, e, StringAlignment.Far);
            posY += altoLinea;

            Text = parametros.CentroDireccion + " - " + parametros.CentroLocalidad;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            Text = "Nro.Pesada  " + parametros.ComprobanteInterno;
            ImprimirTexto(Text, xMargin, yMargin + posY, ancho, alto, Font, e, StringAlignment.Far);
            posY += (altoLinea);

            Text = "___________________________________________________________________________";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += (2 * altoLinea);

            Text = "TICKET PESADA";
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += (altoLinea);

            Text = "___________________________________________________________________________";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += (2 * altoLinea);

            Text = "Producto    " + parametros.Material;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "Proveedor   " + parametros.Proveedor;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "Remito   " + parametros.Remito;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += (2 * altoLinea);

            Text = "Fecha de descarga: " + String.Format(CultureInfo.CurrentCulture, "{0:dd/MM/yyyy}", parametros.FechaUltimaPesada);
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += (2 * altoLinea);

            Text = "Peso Bruto:   " + parametros.PesoBrutoBodega;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            Text = "Balanza Bruto: " + parametros.BalanzaBruto;
            ImprimirTexto(Text, xMargin, yMargin + posY, ancho, alto, Font, e, StringAlignment.Far);
            posY += altoLinea;

            Text = "Peso Tara:   " + parametros.PesoTaraBodega;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            Text = "Balanza Tara: " + parametros.BalanzaTara;
            ImprimirTexto(Text, xMargin, yMargin + posY, ancho, alto, Font, e, StringAlignment.Far);
            posY += (2 * altoLinea);

            Text = "Transportista: " + parametros.Transportista;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            Text = "Fecha Ingreso: " + String.Format(CultureInfo.CurrentCulture, "{0:dd/MM/yyyy hh:mm}", parametros.FechaIngreso);
            ImprimirTexto(Text, xMargin, yMargin + posY, ancho, alto, Font, e, StringAlignment.Far);
            posY += altoLinea;

            Text = "Chofer: " + parametros.Chofer;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            Text = "Fecha Egreso: " + String.Format(CultureInfo.CurrentCulture, "{0:dd/MM/yyyy hh:mm}", parametros.FechaEgreso);
            ImprimirTexto(Text, xMargin, yMargin + posY, ancho, alto, Font, e, StringAlignment.Far);
            posY += altoLinea;

            Text = "Patente Camión: " + parametros.Patente;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;
            Text = "Patente Acoplado: " + parametros.PatenteAcoplado;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += (2 * altoLinea);

            Text = "Observaciones";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;
            Text = parametros.Observaciones;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += (2 * altoLinea);

            Text = "KILOS NETOS SUJETOS A LAS CONDICIONES COMERCIALES ACORDADAS";
            ImprimirTexto(Text, xMargin, yMargin + posY, fontBoldSubrayado, e);
            posY += (altoLinea);

            Text = "___________________________________________________________________________";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += (2 * altoLinea);

            Text = "Número de CIU: " + parametros.CIU;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            Text = "Número de Pedido: " + parametros.Pedido;
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += (altoLinea);

            if (parametros.RubrosCalados.Any())
            {
                Text = "Rubro";
                ImprimirTexto(Text, xMargin, yMargin + posY, fontBold, e);
                Text = "Valor";
                ImprimirTexto(Text, center, yMargin + posY, fontBold, e);
                posY += altoLinea;
            }
            foreach (var rubro in parametros.RubrosCalados)
            {
                var valor = rubro.ValorCalado.HasValue ? ((int)rubro.ValorCalado.Value).ToString(CultureInfo.InvariantCulture) : "";
                Text = rubro.Caracteristica;
                ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
                Text = valor;
                ImprimirTexto(Text, center, yMargin + posY, Font, e);
                posY += altoLinea;
            }

            Text = "___________________________________________________________________________";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += (2 * altoLinea);

            Text = "_______________________";
            ImprimirTexto(Text, xMargin + center,alto - buttomMargin, Font, e);
            Text = "FIRMA";
            ImprimirTexto(Text, xMargin + center, alto - buttomMargin + altoLinea, Font, e);
            
        }

        public override void Imprimir(object dto, string printerName, FirmaDto firmaDto)
        {
            parametros = (ImpTicketPesadaBodegaDto)dto;
            firma = firmaDto;
            if (!String.IsNullOrEmpty(printerName))
            {
                PrinterSettings.PrinterName = printerName;
                Print();
            }
        }
    }
}
