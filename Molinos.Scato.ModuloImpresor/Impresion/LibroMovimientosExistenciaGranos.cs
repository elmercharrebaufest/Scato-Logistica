using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.ModuloImpresor.Impresion
{
    public class LibroMovimientosExistenciaGranos : DocumentoImpresion
    {
        private ImpLibroMovimientosExistenciaGranosDto parametros;
        private FormatoDeImpresionDto formatoDeImpresion;
        private int Pagina { get; set; }

        public LibroMovimientosExistenciaGranos(ImpLibroMovimientosExistenciaGranosDto parametros, FormatoDeImpresionDto formatoDeImpresion)
        {
            if (!parametros.EsPdf)
            {
                PrinterSettings.PrinterName = parametros.Impresora;
            }
            DefaultPageSettings.PaperSize = PrinterSettings.PaperSizes.Cast<PaperSize>().AsQueryable().FirstOrDefault(x => x.RawKind == formatoDeImpresion.FormatoDePapelCodigoTipoPapel);
            this.parametros = parametros;
            this.formatoDeImpresion = formatoDeImpresion;
            Pagina = 0;
        }

        public LibroMovimientosExistenciaGranos() { }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            var fila = formatoDeImpresion.FormatosDeCampo.First(w => w.EsColumna).Fila;
            var filasTabla = formatoDeImpresion.Filas - fila - 1;

            //División de elementos
            var listas = parametros.Dtos.OrderBy(x => x.FechaEmision).ThenBy(o => o.Id).Split(filasTabla);
            
            var totalPaginas = listas.Count;

            if (Pagina < totalPaginas && totalPaginas > 0)
            {
                var lista = listas[Pagina];

                if (parametros.EsPdf)
                {
                    foreach (var f in formatoDeImpresion.FormatosDeCampo.Where(f => f.EsColumna))
                    {
                        var campo = !String.IsNullOrEmpty(f.TituloOncca) ? f.TituloOncca : f.CampoDescripcion;

                        if (f.CampoDireccion == "PesoNeto" && f.TipoDeCampo == TipoDeCampo.Ingreso)
                        {
                            campo = "Kilos Brutos Ing";
                        }
                        if (f.CampoDireccion == "PesoNeto" && f.TipoDeCampo == TipoDeCampo.Egreso)
                        {
                            campo = "Kilos Netos Egr";
                        }           
                        if (f.CampoDireccion == "CTG" )
                        {
                            campo = "CTG AFIP";
                        }
                        
                        ImprimirTexto(campo, fila, f.Columna, new Font("Arial", 7, FontStyle.Bold | FontStyle.Underline), formatoDeImpresion, e, StringAlignment.Center);
                    }
                    fila++;
                }

                foreach (var dto in lista)
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

                        if (f.EsColumna)
                        {
                            if (!dto.EsTotalDia && !dto.EsTotalMes)
                            {
                                if ((dto.TipoDeWorkflow == TipoDeWorkflow.Egreso && f.TipoDeCampo == TipoDeCampo.Egreso) ||
                                    (dto.TipoDeWorkflow == TipoDeWorkflow.Ingreso && f.TipoDeCampo == TipoDeCampo.Ingreso) ||
                                    f.TipoDeCampo == TipoDeCampo.Siempre)
                                {
                                    if (f.CampoDireccion == "TitularCP" || f.CampoDireccion == "Destinatario")
                                    {
                                        ImprimirTexto(
                                          string.IsNullOrEmpty(f.Texto) ? dto.GetValueOrDefault(f.CampoDireccion) : f.Texto,
                                          fila, f.Columna, font,
                                          formatoDeImpresion, e,
                                          f.Alineacion == Alineacion.Derecha
                                              ? StringAlignment.Far
                                              : StringAlignment.Near, null, 21);
                                    }
                                    else
                                    {
                                        ImprimirTexto(
                                            string.IsNullOrEmpty(f.Texto) ? dto.GetValueOrDefault(f.CampoDireccion) : f.Texto,
                                            fila, f.Columna, font,
                                            formatoDeImpresion, e,
                                            f.Alineacion == Alineacion.Derecha
                                                ? StringAlignment.Far
                                                : StringAlignment.Near);
                                    }
                                }
                            }
                            else if (dto.EsTotalDia)
                            {
                                    ImprimirTexto("Total Dia:", fila, 5, new Font("Arial", 9, FontStyle.Bold), formatoDeImpresion, e);
                                    ImprimirTexto(dto.GetValueOrDefault("FechaEmision"), fila, 9, new Font("Arial", 9), formatoDeImpresion, e);

                                    if (f.CampoDireccion == "PesoNeto" && f.TipoDeCampo == TipoDeCampo.Ingreso)
                                    {
                                        ImprimirTexto(dto.PesoNetoIngreso, fila, f.Columna, font, formatoDeImpresion, e, f.Alineacion == Alineacion.Derecha ? StringAlignment.Far : StringAlignment.Near);
                                    }
                                    if (f.CampoDireccion == "PesoNeto" && f.TipoDeCampo == TipoDeCampo.Egreso)
                                    {
                                        ImprimirTexto(dto.PesoNetoEgreso, fila, f.Columna, font, formatoDeImpresion, e, f.Alineacion == Alineacion.Derecha ? StringAlignment.Far : StringAlignment.Near);
                                    }
                                    if (f.CampoDireccion == "PesoNetoSinHumedad" && f.TipoDeCampo == TipoDeCampo.Ingreso)
                                    {
                                        ImprimirTexto(dto.PesoNetoSinHumedad, fila, f.Columna, font, formatoDeImpresion, e, f.Alineacion == Alineacion.Derecha ? StringAlignment.Far : StringAlignment.Near);
                                    }
                                    if (f.CampoDireccion == "SaldosSTOCK")
                                    {
                                        ImprimirTexto(dto.SaldosSTOCK, fila, f.Columna, font, formatoDeImpresion, e, f.Alineacion == Alineacion.Derecha ? StringAlignment.Far : StringAlignment.Near);
                                    }
                            }
                            else if (dto.EsTotalMes)
                            {
                                    ImprimirTexto("Total Mes:", fila, 5, new Font("Arial", 9, FontStyle.Bold), formatoDeImpresion, e);
                                    ImprimirTexto(dto.GetTotalMesOrDefault("FechaEmision"), fila, 9, new Font("Arial", 9, FontStyle.Bold), formatoDeImpresion, e);

                                    if (f.CampoDireccion == "PesoNeto" && f.TipoDeCampo == TipoDeCampo.Ingreso)
                                    {
                                        ImprimirTexto(dto.PesoNetoIngreso, fila, f.Columna, font, formatoDeImpresion, e, f.Alineacion == Alineacion.Derecha ? StringAlignment.Far : StringAlignment.Near);
                                    }
                                    if (f.CampoDireccion == "PesoNeto" && f.TipoDeCampo == TipoDeCampo.Egreso)
                                    {
                                        ImprimirTexto(dto.PesoNetoEgreso, fila, f.Columna, font, formatoDeImpresion, e, f.Alineacion == Alineacion.Derecha ? StringAlignment.Far : StringAlignment.Near);
                                    }
                                    if (f.CampoDireccion == "PesoNetoSinHumedad" && f.TipoDeCampo == TipoDeCampo.Ingreso)
                                    {
                                        ImprimirTexto(dto.PesoNetoSinHumedad, fila, f.Columna, font, formatoDeImpresion, e, f.Alineacion == Alineacion.Derecha ? StringAlignment.Far : StringAlignment.Near);
                                    }
                                    if (f.CampoDireccion == "SaldosSTOCK")
                                    {
                                        ImprimirTexto(dto.SaldosSTOCK, fila, f.Columna, font, formatoDeImpresion, e, f.Alineacion == Alineacion.Derecha ? StringAlignment.Far : StringAlignment.Near);
                                    }
                            }
                        }
                        else
                        {
                            ImprimirTexto(string.IsNullOrEmpty(f.Texto) ? parametros.Dtos.FirstOrDefault().GetValueOrDefault(f.CampoDireccion) : f.Texto, f.Fila, f.Columna, font, formatoDeImpresion, e, f.Alineacion == Alineacion.Derecha ? StringAlignment.Far : StringAlignment.Near);
                        }
                    }
                    fila++;
                }
                Pagina++;
            }
            e.HasMorePages = Pagina < totalPaginas;
        }

        public override void Imprimir(object dto, string printerName, FormatoDeImpresionDto formato)
        {
            PrinterSettings.PrinterName = printerName;
            DefaultPageSettings.PaperSize = PrinterSettings.PaperSizes.Cast<PaperSize>().AsQueryable().FirstOrDefault(x => x.RawKind == formato.FormatoDePapelCodigoTipoPapel);
            parametros = (ImpLibroMovimientosExistenciaGranosDto)dto;
            formatoDeImpresion = formato;
            Print();
        }
    }
}
