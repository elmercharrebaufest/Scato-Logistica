using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.ModuloImpresor.Impresion
{
    public class InformeDeRecepcion : DocumentoImpresion
    {
        private List<ImpInformeDeRecepcionDto> parametro;
        private FirmaDto firma;
        private int Pagina { get; set; }

        public InformeDeRecepcion(List<ImpInformeDeRecepcionDto> parametro, string printerName, FirmaDto firma)
        {
            PrinterSettings.PrinterName = printerName;
            this.parametro = parametro;
            this.firma = firma;
        }

        public InformeDeRecepcion()
        {
        }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            var formatoDeImpresion = new FormatoDeImpresionDto
                {
                    Columnas = 42,
                    Filas = 56,
                    FormatoDePapelAlto = 280,
                    FormatoDePapelAncho = 210,
                    Posicion = Posicion.Vertical,
                };

            foreach (var orden in parametro)
            {
                if (Pagina < 1)
                {
                    orden.ImpInformeDeRecepcionItems.GroupBy(g => g.MaterialDescripcion).ToList()
                             .ForEach(f => orden.ImpInformeDeRecepcionItems.Add(new ImpInformeDeRecepcionItemDto
                             {
                                 MaterialDescripcion = f.Key,
                                 CantidadDescargada = f.Sum(s => decimal.Parse(s.CantidadDescargada)).ToString(),
                                 ItemNro = 99999
                             }));
                }

                //División de elementos
                var listas = orden.ImpInformeDeRecepcionItems.OrderBy(o => o.MaterialDescripcion).ThenBy(o => o.ItemNro).Split(32);
                var totalPaginas = listas.Count;

                if (Pagina < totalPaginas)
                {
                    var lista = listas[Pagina];
                    //Titulo
                    ImprimirTexto(new String('_', 146), 4, 2, new Font("Arial", 9), formatoDeImpresion, e);
                    ImprimirTexto(firma.Descripcion, 5, 2, new Font("Arial", 10, FontStyle.Bold), formatoDeImpresion,
                                  e, StringAlignment.Far);
                    ImprimirTexto(Textos.InformeDeRecepcion_Titulo, 5, 15, new Font("Arial", 10, FontStyle.Bold),
                                  formatoDeImpresion,
                                  e, StringAlignment.Far);
                    ImprimirTexto(Textos.Fecha + ": " + orden.Fecha.ToShortDateString(), 5, 35, new Font("Arial", 8),
                                  formatoDeImpresion, e, StringAlignment.Far);
                    ImprimirTexto(new String('═', 146), 7, 2, new Font("Arial", 7), formatoDeImpresion, e);

                    //Subtitulos
                    ImprimirTexto(orden.NumeroInforme, 6, 18, new Font("Arial", 9), formatoDeImpresion, e,
                                  StringAlignment.Far);
                    ImprimirTexto(Textos.Proveedor + ": ", 8, 2, new Font("Arial", 9, FontStyle.Bold),
                                  formatoDeImpresion, e,
                                  StringAlignment.Far);
                    ImprimirTexto(orden.Proveedor, 8, 7, new Font("Arial", 9), formatoDeImpresion, e,
                                  StringAlignment.Far);
                    ImprimirTexto(Textos.InformeDeRecepcion_Pedido, 9, 2, new Font("Arial", 9, FontStyle.Bold),
                                  formatoDeImpresion, e,
                                  StringAlignment.Far);
                    ImprimirTexto(orden.NumeroPedido, 9, 7, new Font("Arial", 9), formatoDeImpresion, e,
                                  StringAlignment.Far);
                    ImprimirTexto(new String('_', 146), 10, 2, new Font("Arial", 9), formatoDeImpresion, e);

                    //Cabeceras
                    ImprimirTexto(Textos.Item, 11, 2, new Font("Arial", 9, FontStyle.Bold), formatoDeImpresion, e,
                                  StringAlignment.Far);
                    ImprimirTexto(Textos.InformeDeRecepcion_Codigo, 11, 4, new Font("Arial", 9, FontStyle.Bold),
                                  formatoDeImpresion, e,
                                  StringAlignment.Far);
                    ImprimirTexto(Textos.InformeDeRecepcion_Material, 12, 4, new Font("Arial", 9, FontStyle.Bold),
                                  formatoDeImpresion, e,
                                  StringAlignment.Far);
                    ImprimirTexto(Textos.Descripcion, 11, 8, new Font("Arial", 9, FontStyle.Bold), formatoDeImpresion, e,
                                  StringAlignment.Far);
                    ImprimirTexto(Textos.InformeDeRecepcion_Unidad, 11, 22, new Font("Arial", 9, FontStyle.Bold),
                                  formatoDeImpresion, e,
                                  StringAlignment.Far);
                    ImprimirTexto(Textos.InformeDeRecepcion_Medida, 12, 22, new Font("Arial", 9, FontStyle.Bold),
                                  formatoDeImpresion, e,
                                  StringAlignment.Far);
                    ImprimirTexto(Textos.InformeDeRecepcion_Cantidad, 11, 26, new Font("Arial", 9, FontStyle.Bold),
                                  formatoDeImpresion, e,
                                  StringAlignment.Far);
                    ImprimirTexto(Textos.InformeDeRecepcion_Descargada, 12, 26, new Font("Arial", 9, FontStyle.Bold),
                                  formatoDeImpresion, e,
                                  StringAlignment.Far);
                    ImprimirTexto(Textos.Remito_NroRemito, 11, 31, new Font("Arial", 9, FontStyle.Bold),
                                  formatoDeImpresion, e,
                                  StringAlignment.Far);
                    ImprimirTexto(new String('_', 146), 13, 2, new Font("Arial", 9), formatoDeImpresion, e);

                    //Datos
                    var fila = 14;
                    foreach (var item in lista)
                    {
                        if (item.ItemNro != 99999)
                        {
                            ImprimirTexto((fila - 13).ToString(CultureInfo.InvariantCulture), fila, 2,
                                          new Font("Arial", 9),
                                          formatoDeImpresion, e, StringAlignment.Far);
                            ImprimirTexto(item.MaterialCodigo, fila, 4, new Font("Arial", 8), formatoDeImpresion, e,
                                          StringAlignment.Far, float.Parse("14,7"));
                            ImprimirTexto(item.MaterialDescripcion, fila, 8, new Font("Arial", 8), formatoDeImpresion, e);
                            ImprimirTexto(item.UniMed, fila, 20, new Font("Arial", 8), formatoDeImpresion, e,
                                          StringAlignment.Far, float.Parse("14,7"));
                            ImprimirTexto(item.CantidadDescargada, fila, 25, new Font("Arial", 8), formatoDeImpresion, e,
                                          StringAlignment.Far, float.Parse("14,7"));
                            ImprimirTexto(item.Remito, fila, 31, new Font("Arial", 8), formatoDeImpresion, e);
                        }
                        else //Subtotal
                        {
                            ImprimirTexto(Textos.ResumenDeRecepcion_TotalMaterial, fila, 18, new Font("Arial", 9, FontStyle.Bold), formatoDeImpresion, e);
                            ImprimirTexto(item.CantidadDescargada, fila, 24, new Font("Arial", 9), formatoDeImpresion, e, StringAlignment.Far, float.Parse("14,7"));
                        }
                        fila++;
                    }
                    ImprimirTexto(new String('_', 146), fila + 1, 2, new Font("Arial", 9), formatoDeImpresion, e);
                    ImprimirTexto(Textos.Observaciones, fila + 2, 2, new Font("Arial", 9, FontStyle.Bold),
                                  formatoDeImpresion, e);
                    ImprimirTexto(Textos.Estado + ": ", fila + 5, 2, new Font("Arial", 8, FontStyle.Bold),
                                  formatoDeImpresion, e);
                    ImprimirTexto(orden.Estado, fila + 5, 5, new Font("Arial", 8), formatoDeImpresion, e);
                    ImprimirTexto("│", fila + 2, 24, new Font("Arial", 9, FontStyle.Bold), formatoDeImpresion, e);
                    ImprimirTexto("│", fila + 3, 24, new Font("Arial", 9, FontStyle.Bold), formatoDeImpresion, e);
                    ImprimirTexto("│", fila + 4, 24, new Font("Arial", 9, FontStyle.Bold), formatoDeImpresion, e);
                    ImprimirTexto("│", fila + 5, 24, new Font("Arial", 9, FontStyle.Bold), formatoDeImpresion, e);
                    ImprimirTexto(Textos.InformeDeRecepcion_Recepciono, fila + 2, 25,
                                  new Font("Arial", 9, FontStyle.Bold), formatoDeImpresion, e);
                    ImprimirTexto("│", fila + 2, 29, new Font("Arial", 9, FontStyle.Bold), formatoDeImpresion, e);
                    ImprimirTexto("│", fila + 3, 29, new Font("Arial", 9, FontStyle.Bold), formatoDeImpresion, e);
                    ImprimirTexto("│", fila + 4, 29, new Font("Arial", 9, FontStyle.Bold), formatoDeImpresion, e);
                    ImprimirTexto("│", fila + 5, 29, new Font("Arial", 9, FontStyle.Bold), formatoDeImpresion, e);
                    ImprimirTexto(Textos.InformeDeRecepcion_Controlo, fila + 2, 30, new Font("Arial", 9, FontStyle.Bold),
                                  formatoDeImpresion, e);
                    ImprimirTexto("│", fila + 2, 34, new Font("Arial", 9, FontStyle.Bold), formatoDeImpresion, e);
                    ImprimirTexto("│", fila + 3, 34, new Font("Arial", 9, FontStyle.Bold), formatoDeImpresion, e);
                    ImprimirTexto("│", fila + 4, 34, new Font("Arial", 9, FontStyle.Bold), formatoDeImpresion, e);
                    ImprimirTexto("│", fila + 5, 34, new Font("Arial", 9, FontStyle.Bold), formatoDeImpresion, e);
                    ImprimirTexto(Textos.InformeDeRecepcion_CCalidad, fila + 2, 35, new Font("Arial", 9, FontStyle.Bold),
                                  formatoDeImpresion, e);
                    ImprimirTexto(new String('_', 146), fila + 5, 2, new Font("Arial", 9), formatoDeImpresion, e);

                    ImprimirTexto(Textos.InformeDeRecepcion_Pagina + ++Pagina + " / " + totalPaginas, 53, 35,
                                  new Font("Arial", 9),
                                  formatoDeImpresion, e);
                }

                e.HasMorePages = Pagina < totalPaginas;
            }
        }

        public override void Imprimir(object dto, string printerName, FirmaDto firmaDto)
        {
            parametro = (List<ImpInformeDeRecepcionDto>)dto;
            firma = firmaDto;
            if (!String.IsNullOrEmpty(printerName))
            {
                PrinterSettings.PrinterName = printerName;
                Print();
            }
        }
    }
}
