using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.ModuloImpresor.Impresion
{
    public class EtiquetaRubrosAnalizar : DocumentoImpresion
    {
        private ImpEtiquetaRubrosAnalizarDto parametros;


        public EtiquetaRubrosAnalizar(ImpEtiquetaRubrosAnalizarDto parametros, string printerName)
        {
            PrinterSettings.PrinterName = printerName;
            this.parametros = parametros;
            this.EsZebra = true;

        }

        public EtiquetaRubrosAnalizar() { }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            var font = new Font("Arial", 8);
            
            Text = ArmarCodigoImpresion();

            ImprimirTextoTicket(Text, 0, 0, font, e);
        }

        private string ArmarCodigoImpresion()
        {
            string analisisSeleccionados = parametros.AnalisisSeleccionados;
            String[] analisis = analisisSeleccionados.Split(new [] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            int valor = 40;
            int segundoValor = 28;
            int tercerValor = 28;
            string patente = parametros.Patente;

            if (analisis.Length > 0)
            {
                //El texto inicial el cual comienza el codigo que se manda a la impresora, hasta donde dice FDPatente.
                string text = "^XA~TA000~JSN^LT0^MNW^MTT^PON^PMN^LH0,0^JMA^PR6,6~SD15^JUS^LRN^CI0^XZ\r\n^XA\r\n^MMT\r\n^PW609\r\n^LL0406\r\n^LS0\r\n^FO0,45^GB620,0,5^FS\r\n^FT30," + valor + "^A0N,28,28^FH\\^FDPatente " + patente + "^FS";
                valor += 50;
                //Aca concatenamos la linea que contiene el titulo Rubros a Analizar
                text += "\r\n^FT30," + valor.ToString() + "^A0N," + segundoValor.ToString() + "," + tercerValor.ToString() + "^FH\\^FDRUBROS A ANALIZAR: " + "^FS\r";
                segundoValor = 25;
                tercerValor = 24;

                for (int i = 0; i < analisis.Length; i++)
                {
                    valor += 25;
                    if (i <= 10)
                    {
                        //Siempre que no sea el primer analisis, entra en esta lina. Aca concatena todos los analisis, menos el primero.
                        text += "\r\n^FT30," + valor.ToString() + "^A0N," + segundoValor.ToString() + "," + tercerValor.ToString() + "^FH\\^FD" + analisis[i] + "^FS\r";
                    }
                    else if (i <= 20)
                    {
                        if (i == 11)
                        {
                            valor = 140;
                        }
                        //Siempre que no sea el primer analisis, entra en esta lina. Aca concatena todos los analisis, menos el primero.
                        text += "\r\n^FT230," + valor.ToString() + "^A0N," + segundoValor.ToString() + "," + tercerValor.ToString() + "^FH\\^FD" + analisis[i] + "^FS\r";
                    }
                    else if (i <= 30)
                    {
                        if (i == 21)
                        {
                            valor = 140;
                        }
                        //Siempre que no sea el primer analisis, entra en esta lina. Aca concatena todos los analisis, menos el primero.
                        text += "\r\n^FT430," + valor.ToString() + "^A0N," + segundoValor.ToString() + "," + tercerValor.ToString() + "^FH\\^FD" + analisis[i] + "^FS\r";
                    }
                }
                //El texto que va al final.
                text += "\n^PQ1,0,1,Y^XZ\r\n";
                return text;
            }
            return string.Empty;
        }

        public override void Imprimir(object dto, string printerName, FirmaDto firmaDto)
        {
            parametros = (ImpEtiquetaRubrosAnalizarDto)dto;
            if (!String.IsNullOrEmpty(printerName))
            {
                PrinterSettings.PrinterName = printerName;
                Print();
            }
            else
            {
                ZplCode = ArmarCodigoImpresion();
            }
        }

    }
}
