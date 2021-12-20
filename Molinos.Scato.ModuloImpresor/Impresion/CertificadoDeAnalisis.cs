using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.ModuloImpresor.Impresion
{
    public class CertificadoDeAnalisis : DocumentoImpresion
    {
        private FirmaDto firma;
        private ImpCertificadoDeAnalisisDto parametros;

        public CertificadoDeAnalisis(ImpCertificadoDeAnalisisDto parametros, string printerName, FirmaDto firma)
            : base(printerName)
        {
            this.parametros = parametros;
            this.firma = firma;
        }

        public CertificadoDeAnalisis()
        {
        }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            var xMargin = DefaultPageSettings.Margins.Left; //X
            var yMargin = DefaultPageSettings.Margins.Top; //Y
            Font = new Font("Arial", 9);
            var fontBold = new Font("Arial", 10, FontStyle.Bold);
            var fontUnderline = new Font("Arial", 10, FontStyle.Underline);
            var posY = 0;
            var altoLinea = Font.Height;
            var center = xMargin + 30; //centro de la hoja
            var resultados = DefaultPageSettings.PaperSize.Width - DefaultPageSettings.PaperSize.Width / 2;
            var izq = DefaultPageSettings.PaperSize.Width - DefaultPageSettings.PaperSize.Width / 4; //izq

            var sustentable = Textos.No_Sustentable;
            if (parametros.EsSustentable)
			{
                sustentable = Textos.Es_Sustentable;
            }

            Text = firma.RazonSocial;
            ImprimirTexto(Text, xMargin, yMargin + posY, fontBold, e);

            Text = "Fecha " + string.Format(CultureInfo.CurrentCulture, "{0:d/M/yyyy}", DateTime.Now);
            ImprimirTexto(Text, izq, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = parametros.Centro;
            ImprimirTexto(Text, xMargin, yMargin + posY, fontUnderline, e);
            posY += 2 * altoLinea;

            Text = "Analisis correspondiente al vehículo:";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);
            posY += altoLinea;
            Text = "Número de tarjeta Asignada: " + parametros.NumeroDeTarjetaAsignada;
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "Fecha y Hora de Ingreso: " + parametros.FechaYhoraDeIngreso;
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea;
            Text = "Material: " + parametros.MaterialDesc + " - " + sustentable;
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea;
            Text = "Número de Carta de Porte: " + parametros.NumeroDocumento;
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "ID Vehículo: " + parametros.NumeroIngreso;
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "Patente Chasis: " + parametros.Patente;
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "Patente Acoplado: " + parametros.PatenteAcoplado;
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea;
            Text = "CTG: " + parametros.CTG;
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "Titular de Carta de Porte: " + parametros.TitularDeCartaDePorteCuit + " - " +
                    parametros.TitularDeCartaDePorteRazon;
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "Intermediario: " + parametros.IntermediarioCuit + " - " + parametros.IntermediarioRazon;
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea;
            Text = "Remitente Comercial: " + parametros.RemitenteComercialCuit + " - " +
                    parametros.RemitenteComercialRazon;
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea;
            Text = "Corredor comprador: " + parametros.CorredorCuit + " - " + parametros.Corredor;
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "Corredor Vendedor: " + parametros.VendedorCuit + " - " + parametros.Vendedor;
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea;
            Text = "Entregador: " + parametros.EntregadorCuit + " - " + parametros.Entregador;
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea;
            Text = "Agente de Compras: " + parametros.AgenteDeComprasCuit + " - " + parametros.AgenteDeComprasRazon;
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea;
            Text = "Destinatario: " + parametros.DestinatarioCuit + " - " + parametros.DestinatarioRazon;
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea;
            Text = "Destino: " + parametros.DestinoCuit + " - " + parametros.DestinoRazon;
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea;
            Text = "Transportista: " + parametros.TransportistaCuit + " - " + parametros.TransportistaRazon;
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea;
            Text = "Chofer: " + parametros.ChoferCuit + " - " + parametros.ChoferNombre;
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea;
            Text = "Procedencia de la Mercadería: " + parametros.ProcedenciaDeLaMercanderiaCodAfip + " - " +
                    parametros.ProcedenciaDeLaMercanderiaDesc;
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea;
            Text = "Cupo: " + parametros.Cupo;
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea;

            //grilla
            Text = "--------------------------------------------------------------------------------------------";
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea;

            Text = "Rubro";
            ImprimirTexto(Text, center, yMargin + posY, Font, e);

            Text = "Resultado";
            ImprimirTexto(Text, resultados, yMargin + posY, Font, e);
            posY += altoLinea / 2;

            Text = "--------------------------------------------------------------------------------------------";
            ImprimirTexto(Text, center, yMargin + posY, Font, e);
            posY += altoLinea;

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

            var numeroDeAnalisis = 1;
            if (!string.IsNullOrEmpty(humedad))
            {
                Text = numeroDeAnalisis.ToString(CultureInfo.InvariantCulture) + "  " +
                        parametros.HumedadDescripcion;
                ImprimirTexto(Text, center, yMargin + posY, Font, e);

                Text = humedad;
                ImprimirTexto(Text, resultados, yMargin + posY, Font, e);
                posY += altoLinea;

                numeroDeAnalisis++;
            }

            foreach (var valorCaract in valores)
            {
                if (valorCaract.Key != parametros.HumedadDescripcion)
                {
                    Text = numeroDeAnalisis.ToString(CultureInfo.InvariantCulture) + "  " + valorCaract.Key;
                    ImprimirTexto(Text, center, yMargin + posY, Font, e);

                    Text = valorCaract.Value;
                    ImprimirTexto(Text, resultados, yMargin + posY, Font, e);
                    posY += altoLinea;

                    numeroDeAnalisis++;
                }
            }

            posY += 2 * altoLinea;
            //fin grilla

            Text = "          KGS SUJETOS A CONDICIONES COMERCIALES ACORDADAS";
            ImprimirTexto(Text, xMargin, yMargin + posY, fontBold, e);
            posY += 2 * altoLinea;

            Text = "Observaciones  " + parametros.Observaciones;
            ImprimirTexto(Text, xMargin, yMargin + posY, fontBold, e);
            posY += 2 * altoLinea;

            Text = "NOTA: la firma del presente por parte del Representante/Entregador implica la total aceptación";
            ImprimirTexto(Text, xMargin, yMargin + posY, fontBold, e);
            posY += altoLinea;
            Text = " de análisis y rebajas establecidas, y la correspondiente autorización para que el vehículo";
            ImprimirTexto(Text, xMargin, yMargin + posY, fontBold, e);
            posY += altoLinea;
            Text = " sea descargado.";
            ImprimirTexto(Text, xMargin, yMargin + posY, fontBold, e);

            posY += 5 * altoLinea;

            Text = "          __________________________";
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);

            Text = "__________________________";
            ImprimirTexto(Text, izq - xMargin, yMargin + posY, Font, e);
            posY += 2 * altoLinea;

            Text = "          " + firma.Descripcion;
            ImprimirTexto(Text, xMargin, yMargin + posY, Font, e);

            Text = "Representante/Entregador";
            ImprimirTexto(Text, izq - xMargin, yMargin + posY, Font, e);

        }

        public override void Imprimir(object dto, string printerName, FirmaDto firmaDto)
        {
            parametros = (ImpCertificadoDeAnalisisDto) dto;
            firma = firmaDto;
            if (!string.IsNullOrEmpty(printerName))
            {
                PrinterSettings.PrinterName = printerName;
                Print();
            }
        }
    }
}