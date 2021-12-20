using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.ModuloImpresor.Impresion
{
    public class EtiquetaAuditoria : DocumentoImpresion
    {
        private ImpEtiquetaAuditoriaDto parametros;
        private string printerName;
        private FirmaDto firma;

        public EtiquetaAuditoria(ImpEtiquetaAuditoriaDto parametros, string printerName, int xPos, int yPos, FirmaDto firma)
            :base(printerName)
        {
            this.parametros = parametros;
            this.printerName = printerName;
            this.firma = firma;
            this.EsZebra = true;
        }

        public EtiquetaAuditoria() { }

        public string GenerarZpl()
        {
            return "^XA~TA000~JSN^LT0^MNW^MTT^PON^PMN^LH0,0^JMA^PR6,6~SD15^JUS^LRN^CI0^XZ\r\n^XA\r\n^MMT\r\n^PW609\r\n^LL0406\r\n^LS0\r\n^FT30,64^A0N,28,28^FH\\^FD" + firma.Descripcion + "^FS\r\n^FT30,100^A0N,28,28^FH\\^FD" + parametros.Centro + "^FS\r\n^FT30,137^A0N,28,28^FH\\^FD" + parametros.CentroDomicilio + "^FS\r\n^FT30,167^A0N,25,24^FH\\^FDCarta de Porte: " + parametros.NumeroCartaPorte + "^FS\r\n^FT30,197^A0N,25,24^FH\\^FDFecha y Hora Descarga: " + String.Format(CultureInfo.CurrentCulture, "{0:d/M/yyyy HH:mm}", parametros.FechaDescarga.HasValue ? parametros.FechaDescarga.Value : (DateTime?)null) + "^FS\r\n^FT30,227^A0N,25,24^FH\\^FDPeso Bruto: " + parametros.PesoBruto + " - " + parametros.BalanzaBruto + " - " + parametros.UsuarioBruto + "^FS\r\n^FT30,257^A0N,25,24^FH\\^FDPeso Tara: " + parametros.PesoTara + " - " + parametros.BalanzaTara + " - " + parametros.UsuarioTara + "^FS\r\n^FT30,287^A0N,25,24^FH\\^FDPeso Neto: " + parametros.PesoNeto + " - " + "Peso Neto Desc: " + parametros.PesoNetoConDescuento + "^FS\r\n^FT30,317^A0N,25,24^FH\\^FDCamion: " + parametros.Patente + " - Acoplado: " + parametros.PatenteAcoplado + "^FS\r\n^FT30,347^A0N,25,24^FH\\^FDEntregador: " + parametros.EntregadorCuit + " - " + parametros.EntregadorRazonSocial + "^FS\r\n^FT30,377^A0N,25,24^FH\\^FDRecibidor: " + parametros.RecibidorNombre + " " + parametros.RecibidorApellido + "( MAT: " + parametros.RecibidorMatricula + " )^FS\r\n" + (string.IsNullOrEmpty(parametros.RecibidorFirma) ? "" : String.Format(parametros.RecibidorFirma, 300, 40)) + "\r\n^PQ1,0,1,Y^XZ\r\n";
        }
        protected override void OnBeginPrint(PrintEventArgs e)
        {
            RawPrinterHelper.SendStringToPrinter(printerName, GenerarZpl());
        }

        public override void Imprimir(object dto, string printerName, FirmaDto firmaDto)
        {
            parametros = (ImpEtiquetaAuditoriaDto)dto;
            firma = firmaDto;
            if (!String.IsNullOrEmpty(printerName))
            {
                this.printerName = printerName;
                Print();
            }
            else
            {
                ZplCode = GenerarZpl();
            }
        }
    }
}
