using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.ModuloImpresor.Impresion
{
    public class TicketPesadaAduana : DocumentoImpresion
    {
        private ImpTicketPesadaAduanaDto parametros;
        private FirmaDto firma;

        public TicketPesadaAduana(ImpTicketPesadaAduanaDto parametros, string printerName, FirmaDto firma)
            : base(printerName)
        {
            this.parametros = parametros;
            this.firma = firma;
        }

        public TicketPesadaAduana() { }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            int xMargin = DefaultPageSettings.Margins.Left;  //X
            int yMargin = DefaultPageSettings.Margins.Top;  //Y

            var fontBold = new Font("Arial", 10, FontStyle.Bold);
            var fontBoldSub = new Font("Arial", 10, FontStyle.Bold | FontStyle.Underline);
            var posY = 0;
            var altoLinea = Font.Height + (Font.Height/3);
            var center = DefaultPageSettings.PaperSize.Width / 2;//centro de la hoja

            Text = "TICKET DE BALANZA ADUANA";
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "Según Resol AFIP 3890/16";
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "Documento no valido como factura";
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += (2 * altoLinea);

            Text = "Fecha de Emisión: " + String.Format(CultureInfo.CurrentCulture, "{0:d/M/yyyy}", DateTime.Now);
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "Nro de Orden: " + parametros.NumeroIngreso;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "Nro de Comprobante Interno: " + parametros.NumeroDocumento;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += (2 * altoLinea);

            Text = "Código Aduana Planta: " + parametros.CodigoAduana;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += (2 * altoLinea);

            ///////////////////
            Text = "Datos de Balanza";
            ImprimirTexto(Text, xMargin, yMargin + posY, fontBoldSub, e);
            posY += altoLinea;

            Text = parametros.BalanzaNombre;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            Text = "Codigo LOT " + parametros.Lot;
            ImprimirTexto(Text, center, yMargin + posY, fontBold, e);
            posY += altoLinea;

            Text = "Certificado de habilitación: " + parametros.CertificadoDeHabilitacion;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            Text = "Longitud: " + parametros.Longitud;
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "Fecha de Vencimiento: " + (parametros.VencimientoDeCertificacion != null ? String.Format(CultureInfo.CurrentCulture, "{0:d/M/yyyy}", parametros.VencimientoDeCertificacion) : "");
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            Text = "Latitud: " + parametros.Latitud;
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += (2 * altoLinea);
            ///////////////////
            Text = "Datos de Exportador";
            ImprimirTexto(Text, xMargin, yMargin + posY, fontBoldSub, e);
            posY += altoLinea;

            Text = "CUIT: " + parametros.CuitExportador;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "Razon Social: " + parametros.RazonSocialExportador;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += (2 * altoLinea);
            ///////////////////
            Text = "Datos del Transportista ATA";
            ImprimirTexto(Text, xMargin, yMargin + posY, fontBoldSub, e);
            posY += altoLinea;

            Text = "CUIT: " + parametros.CuitTransportista;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "Razon Social: " + parametros.Transportista;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += (2 * altoLinea);
            ///////////////////

            Text = "Datos de Embarque";
            ImprimirTexto(Text, xMargin, yMargin + posY, fontBoldSub, e);
            posY += altoLinea;

            Text = "Permiso de Embarque: " + parametros.PermisoDeEmbarque;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "Producto: " + parametros.Material;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += (2 * altoLinea);
            ///////////////////

            Text = "Datos del Vehiculo";
            ImprimirTexto(Text, xMargin, yMargin + posY, fontBoldSub, e);
            posY += altoLinea;


            Text = "Patente Chasis: " + parametros.Patente;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            Text = "Peso Bruto (KG): " + parametros.PesoBruto;
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "Patente Acoplado: " + parametros.PatenteAcoplado;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            Text = "Peso Tara (KG): " + parametros.PesoTara;
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "Fecha y Hora Entrada: " + parametros.FechaInicio;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            Text = "Peso Neto (KG): " + parametros.PesoNeto;
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea;

            if (parametros.FechaHoraPesoTara == null)
            {
                Text = "Fecha y Hora Peso Tara: -";
                ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            }
            else
            {
                Text = "Fecha y Hora Peso Tara: " + String.Format(CultureInfo.CurrentCulture, "{0:d/M/yyyy HH:mm tt}", parametros.FechaHoraPesoTara);
                ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            }

            Text = "Identificador de Contenedor: -" + parametros.IdentificadorDeContenedor;
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea;
            if (parametros.FechaHoraPesoBruto == null)
            {
                Text = "Fecha y Hora Peso Bruto: ";
                ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            }
            else
            {
                Text = "Fecha y Hora Peso Bruto: " + String.Format(CultureInfo.CurrentCulture, "{0:d/M/yyyy HH:mm tt}", parametros.FechaHoraPesoBruto);
                ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            }            
            posY += (2 * altoLinea);
            ///////////////////
            Text = "Datos del Chofer";
            ImprimirTexto(Text, xMargin, yMargin + posY, fontBoldSub, e);
            posY += altoLinea;

            Text = "Nacionalidad: " + parametros.Nacionalidad;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;
            Text = "Tipo y N° de Documento: " + parametros.ChoferTipoDoc + " " + parametros.ChoferNumero;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;
            Text = "Apellido y Nombre: " + parametros.ChoferApellido + ", " +parametros.ChoferNombre;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += (2 * altoLinea);



            Text = "Observaciones: " + parametros.Observaciones;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += (4 * altoLinea);

            Text = "Firma Permisionario __________________________";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            Text = "Firma Funcionario Aduanero __________________________";
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
        }

        public override void Imprimir(object dto, string printerName, FirmaDto firmaDto)
        {
            parametros = (ImpTicketPesadaAduanaDto)dto;
            firma = firmaDto;
            if (!String.IsNullOrEmpty(printerName))
            {
                PrinterSettings.PrinterName = printerName;
                Print();
            }
        }
    }
}
