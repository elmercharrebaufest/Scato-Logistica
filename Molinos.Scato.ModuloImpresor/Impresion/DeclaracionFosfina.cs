using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.ModuloImpresor.Impresion
{
    public class DeclaracionFosfina : DocumentoImpresion
    {
        private ImpDeclaracionFosfinaDto parametros;

        public DeclaracionFosfina(ImpDeclaracionFosfinaDto parametros, string printerName)
            : base(printerName)
        {
            this.parametros = parametros;
        }

        public DeclaracionFosfina() { }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            int xMargin = DefaultPageSettings.Margins.Left;  //X
            int yMargin = 50;  //Y

            var fontBold = new Font("Arial", 10, FontStyle.Bold);
            var posY = 0;
            Font = new Font("Arial", 8);
            
            var altoLinea = Font.Height + (Font.Height / 3);
            var center = (DefaultPageSettings.PaperSize.Width / 3) + 50;
            var izq = DefaultPageSettings.PaperSize.Width - (DefaultPageSettings.PaperSize.Width / 4);//izq
            Text = "ANEXO I";
            ImprimirTexto(Text, center + 54, yMargin + posY, fontBold, e);
            posY += altoLinea;

            Text = "Ley 27262 - Plaguicidas Fumigantes. Prohibición";
            var posicion = (DefaultPageSettings.PaperSize.Width/2);
            ImprimirTexto(Text, posicion-154, yMargin + posY, Font, e);
            posY += altoLinea;

            //cuadro 1
            e.Graphics.DrawRectangle(new Pen(Color.Black, 2), xMargin, yMargin + posY, izq + 20, altoLinea);

            Text = "DECLARACIÓN JURADA";
            ImprimirTexto(Text, center, yMargin + posY, fontBold, e);
            posY += (2 * altoLinea);

            //cuadro 2
            e.Graphics.DrawRectangle(new Pen(Color.Black, 2), xMargin, yMargin + posY, izq + 20, altoLinea);

            Text = "Tipo y Nro. Doc: " + parametros.TipoDocumento + " " + (parametros.NumeroDocumento != null ? parametros.NumeroDocumento.Replace("-", "") : parametros.NumeroDocumento);
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);

            Text = "CTG: " + parametros.Ctg;
            ImprimirTexto(Text, center + 70, yMargin + posY, Font, e);

            Text = "Fecha de Carga: " + String.Format(CultureInfo.CurrentCulture, "{0:dd/MM/yyyy}", parametros.FechaDeCarga);
            ImprimirTexto(Text, izq + 35 - xMargin, yMargin + posY, Font, e);
            posY += (2 * altoLinea);

            //cuadro 3
            e.Graphics.DrawRectangle(new Pen(Color.Black, 2), xMargin, yMargin + posY, izq + 20, 6 * altoLinea);

            Text = "Datos del Remitente";
            ImprimirTexto(Text, center, yMargin + posY, fontBold, e);
            posY += altoLinea;

            Text = "Nombre ó Razón Social: " + parametros.Nombre;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);

            Text = "CUIT: " + parametros.Cuit;
            ImprimirTexto(Text, izq - xMargin - 24, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "Establecimiento: " + parametros.Establecimiento;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);

            Text = "Domicilio Real: " + parametros.DomicilioReal;
            ImprimirTexto(Text, center + 34, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "Localidad: " + parametros.LocalidadReal;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);

            Text = "Provincia: " + parametros.ProvinciaReal;
            ImprimirTexto(Text, center + 34, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "Domicilio Legal: " + parametros.DomicilioLegal;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "Teléfono: " + parametros.Telefono;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);

            Text = "Localidad: " + parametros.LocalidadLegal;
            ImprimirTexto(Text, center - 44, yMargin + posY, Font, e);

            Text = "Provincia: " + parametros.ProvinciaLegal;
            ImprimirTexto(Text, izq + 20 - xMargin, yMargin + posY, Font, e);
            posY += (2 * altoLinea);

            //cuadro 4
            e.Graphics.DrawRectangle(new Pen(Color.Black, 2), xMargin, yMargin + posY, izq + 20, 3 * altoLinea);

            Text = "Datos de los Granos Transportados";
            ImprimirTexto(Text, center - 40, yMargin + posY, fontBold, e);
            posY += altoLinea;

            Text = "Grano/Tipo: " + parametros.Material;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);

            Text = "Peso: " + parametros.Peso;
            ImprimirTexto(Text, izq + 35 - xMargin, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "Observaciones: " + parametros.Observaciones;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += (2 * altoLinea);

            //cuadro 5
            e.Graphics.DrawRectangle(new Pen(Color.Black, 2), xMargin, yMargin + posY, izq + 20, 3 * altoLinea);

            Text = "Procedencia de la Carga";
            ImprimirTexto(Text, center - 10, yMargin + posY, fontBold, e);
            posY += altoLinea;
            Text = "Domicilio: " + parametros.DomicilioCarga;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;
            Text = "Localidad: " + parametros.LocalidadCarga;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            Text = "Provincia: " + parametros.ProvinciaCarga;
            ImprimirTexto(Text, izq + 35 - xMargin, yMargin + posY, Font, e);
            posY += (2 * altoLinea);

            //cuadro 6
            e.Graphics.DrawRectangle(new Pen(Color.Black, 2), xMargin, yMargin + posY, izq + 20, 5 * altoLinea);

            Text = "Datos del Transporte";
            ImprimirTexto(Text, center, yMargin + posY, fontBold, e);
            posY += altoLinea;

            Text = "Nombre ó Razón Social: " + parametros.NombreTransporte;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);

            Text = "CUIT: " + parametros.CuitTransporte;
            ImprimirTexto(Text, izq + 15 - xMargin, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "Patente Camión: " + parametros.Patente;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);

            Text = "Patente Acoplado: " + parametros.PatenteAcoplado;
            ImprimirTexto(Text, izq + 15 - xMargin, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "Domicilio Fiscal: " + parametros.DomicilioTransporte;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            
            Text = "Km a recorrer: " + parametros.KmsARecorrer;
            ImprimirTexto(Text, izq + 15 - xMargin, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "Localidad: " + parametros.LocalidadTrasnporte;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);

            Text = "Provincia: " + parametros.ProvinciaTransporte;
            ImprimirTexto(Text, izq - 80 - xMargin, yMargin + posY, Font, e);
            posY += (2 * altoLinea);

            //cuadro 7
            e.Graphics.DrawRectangle(new Pen(Color.Black, 2), xMargin, yMargin + posY, izq + 20, 2 * altoLinea);

            Text = "Datos del Chofer";
            ImprimirTexto(Text, center + 10, yMargin + posY, fontBold, e);
            posY += altoLinea;

            Text = "Nombre y Apellido: " + parametros.NombreChofer;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);

            Text = "CUIT/CUIL: " + parametros.CuitChofer;
            ImprimirTexto(Text, izq + 35 - xMargin, yMargin + posY, Font, e);
            posY += (2 * altoLinea);

            //cuadrio 8
            e.Graphics.DrawRectangle(new Pen(Color.Black, 2), xMargin, yMargin + posY, izq + 20, 4* altoLinea);

            Text = "Datos del Destinatario";
            ImprimirTexto(Text, center - 10, yMargin + posY, fontBold, e);
            posY += altoLinea;

            Text = "Nombre ó Razón Social: " + parametros.NombreDestinatario;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);

            Text = "CUIT: " + parametros.CuitDestinatario;
            ImprimirTexto(Text, izq + 35 - xMargin, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "Domicilio Fiscal: " + parametros.DomicilioDestinatario;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;
            Text = "Localidad: " + parametros.LocalidadDestinatario;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            Text = "Provincia: " + parametros.ProvinciaDestinatario;
            ImprimirTexto(Text, izq + 35 - xMargin, yMargin + posY, Font, e);
            posY += (2 * altoLinea);

            //cuadro 9
            e.Graphics.DrawRectangle(new Pen(Color.Black, 2), xMargin, yMargin + posY, izq + 20, 3 * altoLinea);

            Text = "Lugar de Destino de la Carga";
            ImprimirTexto(Text, center - 20, yMargin + posY, fontBold, e);
            posY += altoLinea;

            Text = "Domicilio: " + parametros.DomicilioDestino;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;
            Text = "Localidad: " + parametros.LocalidadDestino;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            Text = "Provincia: " + parametros.ProvinciaDestino;
            ImprimirTexto(Text, izq + 35 - xMargin, yMargin + posY, Font, e);

            posY += (2 * altoLinea - 12);
            Text = "Declaro bajo juramento que la presente carga no ha sido tratada con ningún plaguicida fumigante durante ";
            ImprimirTexto(Text, xMargin, yMargin + posY, new Font("Arial", 9, FontStyle.Bold), e);
            posY += altoLinea;
            Text = "su carga en camión o vagón, no autorizando dicho tratamiento durante su tránsito hasta destino";
            ImprimirTexto(Text, xMargin, yMargin + posY, new Font("Arial", 9, FontStyle.Bold), e);
            posY += (1 * altoLinea);

            Text = "USUARIO RESPONSABLE";
            ImprimirTexto(Text, xMargin, yMargin + posY, fontBold, e);
            posY += (2 * altoLinea);
            Text = "Firma:";
            ImprimirTexto(Text, xMargin, yMargin + posY, fontBold, e);
            posY += altoLinea;
            Text = "Aclaración:";
            ImprimirTexto(Text, xMargin, yMargin + posY, fontBold, e);
            posY += altoLinea;
            Text = "DNI:";
            ImprimirTexto(Text, xMargin, yMargin + posY, fontBold, e);
            
        }

        public override void Imprimir(object dto, string printerName, FirmaDto firmaDto)
        {
            parametros = (ImpDeclaracionFosfinaDto)dto;
            if (!String.IsNullOrEmpty(printerName))
            {
                PrinterSettings.PrinterName = printerName;
                Print();
            }
        }
    }
}
