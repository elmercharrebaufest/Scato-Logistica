using System;
using System.Drawing;
using System.Drawing.Printing;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.ModuloImpresor.Impresion
{
    public class IdentificacionMuestraAuditoria : DocumentoImpresion
    {
        private ImpIdentificacionMuestraAuditoriaDto parametros;
        private int xPos;
        private int yPos;
        private FirmaDto firma;

        public IdentificacionMuestraAuditoria(ImpIdentificacionMuestraAuditoriaDto parametros, string printerName, int xPos, int yPos, FirmaDto firma)
            : base(printerName)
        {
            this.parametros = parametros;
            this.xPos = xPos;
            this.yPos = yPos;
            this.firma = firma;
            this.EsZebra = true;
        }

        public IdentificacionMuestraAuditoria() { }

        public string GenerarZpl()
        {
            return "^XA~TA000~JSN^LT0^MNW^MTT^PON^PMN^LH0,0^JMA^PR6,6~SD15^JUS^LRN^CI0^XZ\r\n^XA\r\n^MMT\r\n^PW609\r\n^LL0406\r\n^LS0\r\n^FT30,64^A0N,35,35^FH\\^FD" + firma.Descripcion + "^FS\r\n^FT30,100^A0N,35,35^FH\\^FD" +
                parametros.Centro + "^FS\r\n^FT30,139^A0N,25,24^FH\\^FDMuestra Auditoria:" + parametros.NroMuestra +
                "^FS\r\n^FT30,171^A0N,25,24^FH\\^FDCarta de Porte:" + parametros.NumeroDeOrden +
                "^FS\r\n^FT30,203^A0N,25,24^FH\\^FDFecha y Hora Calado:" + parametros.FechaHoraCalado +

                "^FS\r\n^FT30,236^A0N,25,24^FH\\^FDRecibidor:" + parametros.UsuarioCalado +
                "^FS\r\n^FT30,267^A0N,25,24^FH\\^FDTitular:" + parametros.ProveedorCuit + "-" + parametros.Proveedor +
                "^FS\r\n^FT30,299^A0N,25,24^FH\\^FDCorredor:" + parametros.CorredorCuit + "-" + parametros.Corredor +
                "^FS\r\n^FT30,330^A0N,25,24^FH\\^FDEntregador:" + parametros.EntregadorCuit + "-" + parametros.Entregador +
                " ^FS\r\n^FT30,362^A0N,25,24^FH\\^FDCamion:" + parametros.Patente + "- Acoplado: " +
                parametros.PatenteAcoplado + "^FS\r\n^FT30,394^A0N,25,24^FH\\^FDProcedencia:" + parametros.Procedencia +
                "^FS\r\n^PQ1,0,1,Y^XZ\r\n";
        }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            var font = new Font("Arial", 10);
            ImprimirTextoTicket(GenerarZpl(), 0, 0, font, e);
        }

        public override void Imprimir(object dto, string printerName, FirmaDto firmaDto)
        {
            parametros = (ImpIdentificacionMuestraAuditoriaDto)dto;
            firma = firmaDto;
            if (!String.IsNullOrEmpty(printerName))
            {
                PrinterSettings.PrinterName = printerName;
                Print();
            }
            else
            {
                ZplCode = GenerarZpl();
            }
        }
    }
}
