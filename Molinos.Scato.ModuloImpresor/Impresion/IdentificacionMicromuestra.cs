using System;
using System.Drawing;
using System.Drawing.Printing;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.ModuloImpresor.Impresion
{
    public class IdentificacionMicromuestra : DocumentoImpresion
    {
        private ImpIdentificacionMicromuestraDto parametros;
        private TipoMicromuestra tipoMicromuestra;
        private int xPos;
        private int yPos;

        public IdentificacionMicromuestra(ImpIdentificacionMicromuestraDto parametros, TipoMicromuestra tipoMicromuestra, string printerName, int xPos, int yPos)
            : base(printerName)
        {
            this.parametros = parametros;
            this.tipoMicromuestra = tipoMicromuestra;
            this.xPos = xPos;
            this.yPos = yPos;
            this.EsZebra = true;
        }

        public IdentificacionMicromuestra() { }

        public string GenerarZpl()
        {
            if (tipoMicromuestra == TipoMicromuestra.AnalisisInterno)
            {
               return "^XA~TA000~JSN^LT0^MNW^MTT^PON^PMN^LH0,0^JMA^PR6,6~SD15\r\n^JUS^LRN^CI0^XZ\r\n^XA\r\n^MMT\r\n^PW609\r\n^LL0406\r\n^LS0\r\n^FO45,395^GB595,0,5^FS\r\n^FO45,133^GB595,0,5^FS\r\n^FO45,10^GB595,0,5^FS\r\n^FO45,10^GB0,390,5^FS\r\n^FO605,10^GB0,390,5^FS\r\n^FT90,100^A0N,28,28^FH\\^FD" + parametros.Centro + "^FS\r\n^FT90,136^A0N,28,28^FH\\^FDMaterial: " + parametros.Material.ToUpper() + "^FS\r\n^BY3,3,83^FT202,244^BCN,,N,N\r\n^FD>;" + parametros.NroMuestra + "^FS\r\n^FT66,392^A0N,28,28^FH\\^FDINTERNO - Humedad: " + parametros.Humedad + "^FS\r\n^FT66,353^A0N,28,28^FH\\^FDOrden/Chapa: " + parametros.NumeroDeOrden + " - " + parametros.Patente + "^FS\r\n^FT66,310^A0N,56,55^FH\\^FD" + parametros.NroMuestra + "^FS\r\n^FT14,39^A0R,28,28^FH\\^FDCasillero: ^FS\r\n^PQ1,0,1,Y^XZ\r\n";
            }
            if (tipoMicromuestra == TipoMicromuestra.Entregador)
            {
                return "^XA~TA000~JSN^LT0^MNW^MTT^PON^PMN^LH0,0^JMA^PR6,6~SD15\r\n^JUS^LRN^CI0^XZ\r\n^XA\r\n^MMT\r\n^PW609\r\n^LL0406\r\n^LS0\r\n^FO45,395^GB595,0,5^FS\r\n^FO45,133^GB595,0,5^FS\r\n^FO45,10^GB595,0,5^FS\r\n^FO45,10^GB0,390,5^FS\r\n^FO605,10^GB0,390,5^FS\r\n^FT90,100^A0N,28,28^FH\\^FD" + parametros.Centro + "^FS\r\n^BY3,3,83^FT202,244^BCN,,N,N\r\n^FD>;" + parametros.NroMuestra + "^FS\r\n^FT66,392^A0N,25,24^FH\\^FD" + parametros.Proveedor + "^FS\r\n^FT66,353^A0N,28,28^FH\\^FDOrden/Chapa: " + parametros.NumeroDeOrden + " - " + parametros.Patente + "^FS\r\n^FT66,310^A0N,56,55^FH\\^FD" + parametros.NroMuestra + "^FS\r\n^PQ1,0,1,Y^XZ\r\n";
            }
            if (tipoMicromuestra == TipoMicromuestra.Camara)
            {
                return "^XA~TA000~JSN^LT0^MNW^MTT^PON^PMN^LH0,0^JMA^PR6,6~SD15\r\n^JUS^LRN^CI0^XZ\r\n^XA\r\n^MMT\r\n^PW609\r\n^LL0406\r\n^LS0\r\n^FO45,395^GB595,0,5^FS\r\n^FO45,133^GB595,0,5^FS\r\n^FO45,10^GB595,0,5^FS\r\n^FO45,10^GB0,390,5^FS\r\n^FO605,10^GB0,390,5^FS\r\n^FT90,100^A0N,28,28^FH\\^FD" + parametros.Centro + "^FS\r\n^BY3,3,83^FT202,244^BCN,,N,N\r\n^FD>;" + parametros.NroMuestra + "^FS\r\n^FT66,392^A0N,25,24^FH\\^FD" + parametros.Proveedor + "^FS\r\n^FT66,353^A0N,28,28^FH\\^FDOrden/Chapa: " + parametros.NumeroDeOrden + " - " + parametros.Patente + "^FS\r\n^FT66,310^A0N,56,55^FH\\^FD" + parametros.NroMuestra + "^FS\r\n^FT14,39^A0R,28,28^FH\\^FDCAMARA^FS\r\n^PQ1,0,1,Y^XZ\r\n";
            }
            if (tipoMicromuestra == TipoMicromuestra.Casillero)
            {
                return "^XA~TA000~JSN^LT0^MNW^MTT^PON^PMN^LH0,0^JMA^PR6,6~SD15\r\n^JUS^LRN^CI0^XZ\r\n^XA\r\n^MMT\r\n^PW609\r\n^LL0406\r\n^LS0\r\n^FO45,395^GB595,0,5^FS\r\n^FO45,133^GB595,0,5^FS\r\n^FO45,10^GB595,0,5^FS\r\n^FO45,10^GB0,390,5^FS\r\n^FO605,10^GB0,390,5^FS\r\n^FT90,100^A0N,28,28^FH\\^FD" + parametros.Centro + "^FS\r\n^BY3,3,83^FT202,244^BCN,,N,N\r\n^FD>;" + parametros.NroMuestra + "^FS\r\n^FT66,353^A0N,28,28^FH\\^FDOrden/Chapa: " + parametros.NumeroDeOrden + " - " + parametros.Patente + "^FS\r\n^FT66,310^A0N,56,55^FH\\^FD" + parametros.NroMuestra + "^FS\r\n^FT14,39^A0R,28,28^FH\\^FDCasillero: " + parametros.NroCasillero + "^FS\r\n^PQ1,0,1,Y^XZ\r\n";
            }
            return "";
        }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            var font = new Font("Arial", 10);
            ImprimirTextoTicket(GenerarZpl(), 0, 0, font, e);
        }

        public override void Imprimir(object dto, string printerName, FirmaDto firmaDto)
        {
            parametros = (ImpIdentificacionMicromuestraDto)dto;
            tipoMicromuestra = ((ImpIdentificacionMicromuestraDto) dto).TipoMicromuestra;
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
