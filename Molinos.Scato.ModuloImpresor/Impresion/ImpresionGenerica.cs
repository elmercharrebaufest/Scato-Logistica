using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.ModuloImpresor.Impresion
{
    public class ImpresionGenerica : DocumentoImpresion
    {
        private ImpImpresionGenericaDto parametros;
        private FormatoDeImpresionDto formatoDeImpresion;
        public ImpresionGenerica(ImpImpresionGenericaDto parametros, FormatoDeImpresionDto formatoDeImpresion)
        {
            PrinterSettings.PrinterName = parametros.Impresora;
            DefaultPageSettings.PaperSize = PrinterSettings.PaperSizes.Cast<PaperSize>().AsQueryable().FirstOrDefault(x => x.RawKind == formatoDeImpresion.FormatoDePapelCodigoTipoPapel);
            this.parametros = parametros;
            this.formatoDeImpresion = formatoDeImpresion;
        }

        public ImpresionGenerica() { }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            foreach (var f in formatoDeImpresion.FormatosDeCampo)
            {
                var style = new FontStyle();
                if (f.Negrita)
                {
                    style = style | FontStyle.Bold;
                }
                if (f.Cursiva)
                {
                    style = style | FontStyle.Italic;
                }
                if (f.Subrayado)
                {
                    style = style | FontStyle.Underline;
                }
                var font = new Font(f.LetraDescripcion, f.Tamaño, style);

                if ((parametros.TipoDeWorkflow == TipoDeWorkflow.Egreso && f.TipoDeCampo == TipoDeCampo.Egreso) || 
                    (parametros.TipoDeWorkflow == TipoDeWorkflow.Ingreso && f.TipoDeCampo == TipoDeCampo.Ingreso) || 
                    (parametros.TipoDeWorkflow == TipoDeWorkflow.Ingreso && parametros.EsSustentable && f.TipoDeCampo == TipoDeCampo.IngresoSustentable)||
                    (parametros.TipoDeWorkflow == TipoDeWorkflow.Egreso && parametros.EsSustentable && f.TipoDeCampo == TipoDeCampo.EgresoSustentable)||
                    f.TipoDeCampo == TipoDeCampo.Siempre)
                {
                    ImprimirTexto(string.IsNullOrEmpty(f.Texto) ? parametros.GetValueOrDefault(f.CampoDireccion) : f.Texto, f.Fila, f.Columna, font, formatoDeImpresion, e, f.Alineacion == Alineacion.Derecha ? StringAlignment.Far : StringAlignment.Near);
                }
            }
        }

        public override void Imprimir(object dto, string printerName, FormatoDeImpresionDto formato)
        {
            DefaultPageSettings.PaperSize = PrinterSettings.PaperSizes.Cast<PaperSize>().AsQueryable().FirstOrDefault(x => x.RawKind == formato.FormatoDePapelCodigoTipoPapel);
            parametros = (ImpImpresionGenericaDto)dto;
            this.formatoDeImpresion = formato;
            if (!String.IsNullOrEmpty(printerName))
            {
                PrinterSettings.PrinterName = printerName;
                Print();
            }
        }
    }
}
