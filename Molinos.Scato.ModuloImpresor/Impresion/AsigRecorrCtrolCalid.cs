using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.ModuloImpresor.Impresion
{
    public class AsigRecorrCtrolCalid : DocumentoImpresion
    {
        private FirmaDto firma;
        private ImpAsigRecorrCtrolCalidDto parametros;

        public AsigRecorrCtrolCalid(ImpAsigRecorrCtrolCalidDto parametros, string printerName, FirmaDto firma)
            : base(printerName)
        {
            PrinterSettings.PrinterName = printerName;
            this.parametros = parametros;
            this.firma = firma;
        }

        public AsigRecorrCtrolCalid()
        {
        }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            var xMargin = DefaultPageSettings.Margins.Left; //X
            var yMargin = DefaultPageSettings.Margins.Top; //Y
            var Font = new Font("Arial", 9);
            var font = new Font("Arial", 11);
            var fontSub = new Font("Arial", 9, FontStyle.Underline);
            var fontBold = new Font("Arial", 10, FontStyle.Bold);
            var fontBoldmaterial = new Font("Arial", 20, FontStyle.Bold);
            var fontBoldSub = new Font("Arial", 10, FontStyle.Bold | FontStyle.Underline);
            var fontUnderline = new Font("Arial", 10, FontStyle.Underline);
            var fontGrande = new Font("Arial", 35, FontStyle.Bold);
            var posY = 0;
           
            var altoLinea = Font.Height;
            var center = xMargin + 30; //centro de la hoja
            var resultados = DefaultPageSettings.PaperSize.Width - DefaultPageSettings.PaperSize.Width / 2;
            var izq = DefaultPageSettings.PaperSize.Width - DefaultPageSettings.PaperSize.Width / 4; //izq

            var sustentable = string.Empty;
            if (parametros.EsSustentable)
            {
                sustentable = " - " + Textos.Es_Sustentable;
            }
            //Text = "Nº Orden:" + parametros.NumeroDeOrden + "  ASIGNACION DE RECORRIDO CONTROL CALIDAD\n";
            //ImprimirTexto(Text, xMargin, yMargin + posY, Font, e)
            Text = "Fecha " + string.Format(CultureInfo.CurrentCulture, "{0:d/M/yyyy}", DateTime.Now) +
                    "\nHora: " + String.Format(CultureInfo.CurrentCulture, "{0:HH:mm:ss}", parametros.FechaImpresion);
            ImprimirTexto(Text, izq, yMargin + posY, Font, e); 
            posY += altoLinea * 2;
            Text = firma.RazonSocial;
            ImprimirTexto(Text, xMargin, yMargin + posY, fontBold, e);
            posY += altoLinea * 2;
            Text = parametros.Centro;
            ImprimirTexto(Text, xMargin, yMargin + posY, fontUnderline, e);
            posY += 2 * altoLinea;
            posY += altoLinea * 2;
            Text = "Patente: ";
            ImprimirTexto(Text, xMargin, yMargin + posY, fontBold, e);
            Text = parametros.Patente.ToUpper();          
            ImprimirTexto(Text, xMargin + 80, yMargin + posY, fontGrande, e);
            Text = " Acoplado: ";           
            ImprimirTexto(Text, xMargin * 4, yMargin + posY, fontBold, e);
            Text = parametros.PatenteAcoplado;
            ImprimirTexto(Text, xMargin * 5, yMargin + posY, fontGrande, e);

    
            posY += 4* altoLinea;          
            

            //Text = "Analisis correspondiente al vehículo:";
            //ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            //posY += altoLinea;
            Text = "Número de tarjeta Asignada: " + parametros.NumeroDeTarjetaAsignada;
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea * 2;

            //Text = "Fecha y Hora de Ingreso: " + parametros.FechaYhoraDeIngreso;
            //ImprimirTexto(Text, center, yMargin + posY, Font, e);
            //posY += altoLinea;
            Text = "Material: " + parametros.MaterialDesc + sustentable;
            ImprimirTexto(Text, center, yMargin + posY, fontBoldmaterial, e);
            posY += altoLinea * 3;
            Text = "Número de Carta de Porte: " + parametros.NumeroDocumento;
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea;

            //Text = "ID Vehículo: " + parametros.NumeroIngreso;
            //ImprimirTexto(Text, center, yMargin + posY, Font, e);
            //posY += altoLinea;

            //Text = "CTG: " + parametros.CTG;
            //ImprimirTexto(Text, center, yMargin + posY, Font, e);
            //posY += altoLinea;

            //Text = "Titular de Carta de Porte: " + parametros.TitularDeCartaDePorteCuit + " - " +
            //        parametros.TitularDeCartaDePorteRazon;
            //ImprimirTexto(Text, center, yMargin + posY, Font, e);
            //posY += altoLinea;

            //Text = "Intermediario: " + parametros.IntermediarioCuit + " - " + parametros.IntermediarioRazon;
            //ImprimirTexto(Text, center, yMargin + posY, Font, e);
            //posY += altoLinea;
            //Text = "Remitente Comercial: " + parametros.RemitenteComercialCuit + " - " +
            //        parametros.RemitenteComercialRazon;
            //ImprimirTexto(Text, center, yMargin + posY, Font, e);
            //posY += altoLinea;
            //Text = "Corredor comprador: " + parametros.CorredorCuit + " - " + parametros.Corredor;
            //ImprimirTexto(Text, center, yMargin + posY, Font, e);
            //posY += altoLinea;

            //Text = "Corredor Vendedor: " + parametros.VendedorCuit + " - " + parametros.Vendedor;
            //ImprimirTexto(Text, center, yMargin + posY, Font, e);
            //posY += altoLinea;
            //Text = "Entregador: " + parametros.EntregadorCuit + " - " + parametros.Entregador;
            //ImprimirTexto(Text, center, yMargin + posY, Font, e);
            //posY += altoLinea;
            //Text = "Agente de Compras: " + parametros.AgenteDeComprasCuit + " - " + parametros.AgenteDeComprasRazon;
            //ImprimirTexto(Text, center, yMargin + posY, Font, e);
            //posY += altoLinea;
            //Text = "Destinatario: " + parametros.DestinatarioCuit + " - " + parametros.DestinatarioRazon;
            //ImprimirTexto(Text, center, yMargin + posY, Font, e);
            //posY += altoLinea;
            //Text = "Destino: " + parametros.DestinoCuit + " - " + parametros.DestinoRazon;
            //ImprimirTexto(Text, center, yMargin + posY, Font, e);
            //posY += altoLinea;
            //Text = "Transportista: " + parametros.TransportistaCuit + " - " + parametros.TransportistaRazon;
            //ImprimirTexto(Text, center, yMargin + posY, Font, e);
            //posY += altoLinea;
            //Text = "Chofer: " + parametros.ChoferCuit + " - " + parametros.ChoferNombre;
            //ImprimirTexto(Text, center, yMargin + posY, Font, e);
            //posY += altoLinea;
            //Text = "Procedencia de la Mercadería: " + parametros.ProcedenciaDeLaMercanderiaCodAfip + " - " +
            //        parametros.ProcedenciaDeLaMercanderiaDesc;
            //ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea;
            //Text = "Cupo: " + parametros.Cupo;
            //ImprimirTexto(Text, center, yMargin + posY, Font, e);
            Text = "Calle: ";
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            Text = parametros.Calle;
            ImprimirTexto(Text, 165, yMargin + posY, fontBold, e);
            posY += altoLinea * 2;
            Text = "Carga/Descarga: ";            
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            foreach (var h in parametros.Hidraulicas)
            {
                Text =  h.ToUpper() + "/ ";
            }
            ImprimirTexto(Text, 225, yMargin + posY, fontBold, e);
            posY += altoLinea * 2;
            Text = "Almacen: " + parametros.Almacen;
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea * 2;
            //Text = "Balanza tara: " + parametros.BalanzaTara;
            //ImprimirTexto(Text, center, yMargin + posY, Font, e);
            //posY += altoLinea;
            //Text = "Balanza bruto: " + parametros.BalanzaBruto;
            //ImprimirTexto(Text, center, yMargin + posY, Font, e);
            //posY += altoLinea;
            Text = "Calidad: "; 
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            if (parametros.Calidad != "")
            {
                Text = parametros.Calidad;
                ImprimirTexto(Text, 180, yMargin + posY, font, e);

            }
            posY += altoLinea * 2;
            Text = "Fecha Calado: " + (parametros.FechaCalado != null ? String.Format(CultureInfo.CurrentCulture, "{0:d/M/yyyy HH:mm tt}", parametros.FechaCalado) : "");
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea * 2;
            var caladoPorCaract = parametros.CaladoPorCaracteristicas ?? new List<CaladoPorCaracteristicaDto>();
            var analisisPorCaract =
                parametros.AnalisisPorCaracteristicas ?? new List<AnalisisPorCaracteristicaDto>();
            var valores = new Dictionary<string, string>();
            foreach (var calado in caladoPorCaract)
            {
                valores[calado.Caracteristica] = calado.ValorCalado + " " +
                                                    (string.IsNullOrEmpty(calado.Unidad) ? "%" : calado.Unidad);
            }
            foreach (var analisis in analisisPorCaract)
            {
                valores[analisis.Caracteristica] = analisis.ValorAnalisis + " " +
                                                    (string.IsNullOrEmpty(analisis.Unidad) ? "%" : analisis.Unidad);
            }

            string humedad;
            valores.TryGetValue(parametros.HumedadDescripcion, out humedad);
            
            if (!string.IsNullOrEmpty(humedad))
            {
                Text = "Humedad: " + humedad;
                ImprimirTexto(Text, center, yMargin + posY, Font, e);
                posY += altoLinea * 2;
            }

            if (!string.IsNullOrEmpty(parametros.ProteinaAlta))
            {
                Text = "Proteína: ";
                ImprimirTexto(Text, center, yMargin + posY, Font, e);
                Text = "ALTA";
                ImprimirTexto(Text, 185, yMargin + posY, font, e);
                posY += altoLinea * 2;
            }
           
            if (!string.IsNullOrEmpty(parametros.ProteinaBaja))
            {
                Text = "Proteína: ";
                ImprimirTexto(Text, center, yMargin + posY, Font, e);
                Text = "BAJA";
                ImprimirTexto(Text, 185, yMargin + posY, font, e);
                posY += altoLinea * 2;
            }
           
            if (!string.IsNullOrEmpty(parametros.MateriaGrasa))
            {
                Text = "Materia Grasa: " + parametros.MateriaGrasa +" %";
                ImprimirTexto(Text, center, yMargin + posY, Font, e);
                posY += altoLinea * 2;
            }
            posY += 6 * altoLinea;

            //fin grilla

            Rectangle border = new Rectangle(xMargin, yMargin + posY, 700, 45);

            e.Graphics.DrawRectangle(Pens.Black, border);
            Text = "“LA SEGURIDAD LA HACEMOS ENTRE TODOS”";
            ImprimirTexto(Text, xMargin * 3, yMargin + posY, fontBold, e);
            posY += altoLinea * 2;
            Text = "TU FAMILIA TE ESPERA";            
            ImprimirTexto(Text, xMargin * 4, yMargin + posY, fontBold, e);
            posY += altoLinea * 3;
            Text = "- PROHIBIDO FUMAR O HACER FUEGO “RIESGO DE INCENDIO Y EXPLOSIONES”.";
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea * 2;
            Text = "- PROHIBIDO DESCENDER DEL CAMION “RIESGO DE ATROPELLO”.";
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea * 2;
            Text = "- CONTROLÁ EL ESTADO DE TU VEHÍCULO (CUBIERTAS, ENGANCHE, LANZA, ELASTICOS, FRENOS).";
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea * 2;
        }

        public override void Imprimir(object dto, string printerName, FirmaDto firmaDto)
        {
            parametros = (ImpAsigRecorrCtrolCalidDto)dto;

            firma = firmaDto;
            if (!string.IsNullOrEmpty(printerName))
            {
                PrinterSettings.PrinterName = printerName;
                Print();
            }
        }
    }
}