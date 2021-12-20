using System;
using System.Drawing;
using System.Drawing.Printing;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.ModuloImpresor.Impresion
{
    public class ReciboMunicipalEtiqueta : DocumentoImpresion
    {
        private ImpReciboMunicipalDto parametros;
        private FirmaDto firma;

        public ReciboMunicipalEtiqueta(ImpReciboMunicipalDto parametros, string printerName, FirmaDto firma)
            : base(printerName)
        {
            this.parametros = parametros;
            this.firma = firma;
            this.EsZebra = true;
        }

        public ReciboMunicipalEtiqueta() { }

        public string GenerarZpl()
        {
            //return "^XA\r\n~TA000~JSN^LT0^MNW^MTT^PON^PMN^LH0,0^JMA^PR5,5^MD15^LRN^CI0\r\n^MMT\r\n^PW609\r\n^LL0406\r\n^LS0\r\n^FO8,12^GB593,384,4^FS\r\nFT66,272^A0N,28,28^FH\r\n^FT164,52^A0N,28,28^FH\\^FDTasa de sobrecarga^FS\r\n^FT21,106^A0N,28,28^FH\\^FDMunicipalidad de San Lorenzo^FS\r\n^FT21,149^A0N,28,28^FH\\^FDOrdenanza " + parametros.Ordenanza + "^FS\r\n^FT21,193^A0N,28,28^FH\\^FDValor $ " + parametros.Valor.ToString(CurrentUICulture) + "^FS\r\n^FT21,239^A0N,28,28^FH\\^FDTicket Nro: " + parametros.TicketNro + "^FS\r\n^FT21,292^A0N,28,28^FH\\^FD" + parametros.NroDocumentoLegal + "^FS\r\n^FT21,339^A0N,28,28^FH\\^FDAgente de percepcion segun ordenanza 2643/08^FS\r\n^FT21,384^A0N,28,28^FH\\^FD" + firma.Descripcion + " - Planta San Lorenzo^FS\r\n^XZ\r\n";
            string ticket;
            if (parametros.Valor == "0" )
            {
                ticket = $"^XA\r\n~TA000~JSN^LT0^MNW^MTT^PON^PMN^LH0,0^JMA^PR5,5^MD15^LRN^CI0\r\n^MMT\r\n^PW609\r\n^LL0406\r\n^LS0\r\n^FO8,12^GB593,384,4"
            + $"^FS\r\nFT66,272^A0N,28,28^FH\r\n^FT164,52^A0N,28,28^FH\\^FDTasa de sobrecarga"
            + $"^FS\r\n^FT21,106^A0N,28,28^FH\\^FDMunicipalidad de San Lorenzo"
            + $"^FS\r\n^FT21,202^A0N,45,45^FH\\^FDTasa abonada"
            + $"^FS\r\n^FT21,250^A0N,45,45^FH\\^FDdentro del dia"
            + $"^FS\r\n^FT21,292^A0N,28,28^FH\\^FD{ parametros.NroDocumentoLegal }"
            + $"^FS\r\n^FT21,384^A0N,28,28^FH\\^FD{ firma.Descripcion } - Planta San Lorenzo"
            + $"^FS\r\n^FO370,75^GB200,225,3^"
            + $"^FS\r\n^FT390,149^A0N,28,28^FH\\^FDVehiculo"
            + $"^FS\r\n^FT390,193^A0N,28,28^FH\\^FD{ parametros.Patente }"
            + $"^FS\r\n^FT390,239^A0N,40,40^FH\\^FD{ (parametros.TipoVehiculo.ToString()).Replace('ó', 'o') }"
            + $"^FS\r\n^XZ\r\n";
            }
            else
            {
                ticket = $"^XA\r\n~TA000~JSN^LT0^MNW^MTT^PON^PMN^LH0,0^JMA^PR5,5^MD15^LRN^CI0\r\n^MMT\r\n^PW609\r\n^LL0406\r\n^LS0\r\n^FO8,12^GB593,384,4"
            + $"^FS\r\nFT66,272^A0N,28,28^FH\r\n^FT164,52^A0N,28,28^FH\\^FDTasa de sobrecarga"
            + $"^FS\r\n^FT21,106^A0N,28,28^FH\\^FDMunicipalidad de San Lorenzo"
            + $"^FS\r\n^FT21,149^A0N,28,28^FH\\^FDOrdenanza {parametros.Ordenanza}"
            + $"^FS\r\n^FT21,202^A0N,45,45^FH\\^FDValor ${ parametros.Valor.ToString(CurrentUICulture).Replace('.', ',') }"
            + $"^FS\r\n^FT21,249^A0N,28,28^FH\\^FDTicket Nro: { parametros.TicketNro } "
            + $"^FS\r\n^FT21,292^A0N,28,28^FH\\^FD{ parametros.NroDocumentoLegal }"
            + $"^FS\r\n^FT21,339^A0N,28,28^FH\\^FDAgente de percepcion segun ordenanza { parametros.Ordenanza }"
            + $"^FS\r\n^FT21,384^A0N,28,28^FH\\^FD{ firma.Descripcion } - Planta San Lorenzo"
            + $"^FS\r\n^FO370,75^GB200,225,3^"
            + $"^FS\r\n^FT390,149^A0N,28,28^FH\\^FDVehiculo"
            + $"^FS\r\n^FT390,193^A0N,28,28^FH\\^FD{ parametros.Patente }"
            + $"^FS\r\n^FT390,239^A0N,40,40^FH\\^FD{ (parametros.TipoVehiculo.ToString()).Replace('ó', 'o') }"
            + $"^FS\r\n^XZ\r\n";
            }
            // return $"^XA\r\n~TA000~JSN^LT0^MNW^MTT^PON^PMN^LH0,0^JMA^PR5,5^MD15^LRN^CI0\r\n^MMT\r\n^PW609\r\n^LL0406\r\n^LS0\r\n^FO8,12^GB593,384,4"
            //+ $"^FS\r\nFT66,272^A0N,28,28^FH\r\n^FT164,52^A0N,28,28^FH\\^FDTasa de sobrecarga"
            //+ $"^FS\r\n^FT21,106^A0N,28,28^FH\\^FDMunicipalidad de San Lorenzo"
            //+ $"^FS\r\n^FT21,149^A0N,28,28^FH\\^FDOrdenanza {parametros.Ordenanza}"
            //+ $"^FS\r\n^FT21,202^A0N,45,45^FH\\^FDValor ${ parametros.Valor.ToString(CurrentUICulture).Replace('.', ',') }"
            //+ $"^FS\r\n^FT21,249^A0N,28,28^FH\\^FDTicket Nro: { parametros.TicketNro } "
            //+ $"^FS\r\n^FT21,292^A0N,28,28^FH\\^FD{ parametros.NroDocumentoLegal }"
            //+ $"^FS\r\n^FT21,339^A0N,28,28^FH\\^FDAgente de percepcion segun ordenanza { parametros.Ordenanza }"
            //+ $"^FS\r\n^FT21,384^A0N,28,28^FH\\^FD{ firma.Descripcion } - Planta San Lorenzo"
            //+ $"^FS\r\n^FO370,75^GB200,225,3^"
            //+ $"^FS\r\n^FT390,149^A0N,28,28^FH\\^FDVehiculo"
            //+ $"^FS\r\n^FT390,193^A0N,28,28^FH\\^FD{ parametros.Patente }"
            //+ $"^FS\r\n^FT390,239^A0N,40,40^FH\\^FD{ (parametros.TipoVehiculo.ToString()).Replace('ó', 'o') }"
            //+ $"^FS\r\n^XZ\r\n";
            return ticket;
        }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            var font = new Font("Arial", 10);
            ImprimirTextoTicket(GenerarZpl(), 0, 0, font, e);
        }

        public override void Imprimir(object dto, string printerName, FirmaDto firmaDto)
        {
            parametros = (ImpReciboMunicipalDto)dto;
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
