using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.ModuloImpresor.Impresion
{
    public class ResumenDeRecepcion : DocumentoImpresion
    {
        private ImpResumenDeRecepcionDto parametro;
        private int Pagina { get; set; }

        public ResumenDeRecepcion(ImpResumenDeRecepcionDto parametro, string printerName)
        {
            PrinterSettings.PrinterName = printerName;
            this.parametro = parametro;
            Pagina = 0;
        }

        public ResumenDeRecepcion() { }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = CurrentUICulture;
            //Formato de la Hoja
            var formatoDeImpresion = new FormatoDeImpresionDto
                {
                    Columnas = 56,
                    Filas = 42,
                    FormatoDePapelAlto = 280,
                    FormatoDePapelAncho = 210,
                    Posicion = Posicion.Horizontal,
                };
            //Agrego subtotales (solo hacerlo una vez)
            if (Pagina < 1)
            {
                parametro.ImpResumenDeRecepcionItems.GroupBy(g => g.MaterialDescripcion)
                         .ToList()
                         .ForEach(f => parametro.ImpResumenDeRecepcionItems.Add(new ImpResumenDeRecepcionItemDto()
                         {
                             MaterialDescripcion = f.Key,
                             PesoBruto = f.Sum(s => s.PesoBruto),
                             PesoNeto = f.Sum(s => s.PesoNeto),
                             PesoTara = f.Sum(s => s.PesoTara),
                             ItemNro = 99999
                         }));
            }
            //División de elementos
            var listas = parametro.ImpResumenDeRecepcionItems.OrderBy(o => o.MaterialDescripcion).ThenBy(o => o.ItemNro).Split(23);
            var totalPaginas = listas.Count;

            if (Pagina < totalPaginas || totalPaginas == 0)
            {
                //Titulo
                ImprimirTexto(Textos.ResumenDeRecepcion_TituloNumero + parametro.NumeroRomaneo, 5, 21, new Font("Arial", 10, FontStyle.Bold), formatoDeImpresion, e, StringAlignment.Far);
                //Subtitulos
                ImprimirTexto(Textos.ResumenDeRecepcion_FechaRomaneo, 7, 2, new Font("Arial", 9, FontStyle.Bold | FontStyle.Underline), formatoDeImpresion, e, StringAlignment.Near, float.Parse("30"));
                ImprimirTexto(Textos.ResumenDeRecepcion_NumeroPedido, 7, 10, new Font("Arial", 9, FontStyle.Bold | FontStyle.Underline), formatoDeImpresion, e, StringAlignment.Far);
                ImprimirTexto(Textos.ResumenDeRecepcion_KgsNeto, 7, 22, new Font("Arial", 9, FontStyle.Bold | FontStyle.Underline), formatoDeImpresion, e, StringAlignment.Far);
                ImprimirTexto(Textos.ResumenDeRecepcion_Bultos, 7, 28, new Font("Arial", 9, FontStyle.Bold | FontStyle.Underline), formatoDeImpresion, e, StringAlignment.Far);
                ImprimirTexto(Textos.Proveedor, 7, 32, new Font("Arial", 9, FontStyle.Bold | FontStyle.Underline), formatoDeImpresion, e, StringAlignment.Far);
                ImprimirTexto(Textos.Estado, 7, 50, new Font("Arial", 9, FontStyle.Bold | FontStyle.Underline), formatoDeImpresion, e, StringAlignment.Far);

                ImprimirTexto(parametro.FechaRomaneo.ToShortDateString(), 8, 2, new Font("Arial", 9), formatoDeImpresion, e, StringAlignment.Far);
                ImprimirTexto(parametro.NumeroPedido, 8, 10, new Font("Arial", 9), formatoDeImpresion, e, StringAlignment.Far);
                ImprimirTexto(parametro.CantidadComprobada,8, 22, new Font("Arial", 9), formatoDeImpresion, e, StringAlignment.Far, float.Parse("20"));
                ImprimirTexto(parametro.Bultos, 8, 28, new Font("Arial", 9), formatoDeImpresion, e, StringAlignment.Far, float.Parse("12"));
                ImprimirTexto(parametro.Proveedor, 8, 32, new Font("Arial", 8), formatoDeImpresion, e, StringAlignment.Far);
                ImprimirTexto(parametro.Estado, 8, 50, new Font("Arial", 9), formatoDeImpresion, e, StringAlignment.Far);


                //Cabeceras
                ImprimirTexto(Textos.ResumenDeRecepcion_ItemN, 10, 2, new Font("Arial", 9, FontStyle.Underline), formatoDeImpresion, e, StringAlignment.Near, float.Parse("28,2"));
                ImprimirTexto(Textos.Material, 10, 5, new Font("Arial", 9, FontStyle.Underline), formatoDeImpresion, e);
                ImprimirTexto(Textos.Descripcion, 10, 9, new Font("Arial", 9, FontStyle.Underline), formatoDeImpresion, e);
                ImprimirTexto(Textos.Remito, 10, 22, new Font("Arial", 9, FontStyle.Underline), formatoDeImpresion, e, StringAlignment.Near, float.Parse("38"));
                ImprimirTexto(Textos.ResumenDeRecepcion_Lote, 10, 28, new Font("Arial", 9, FontStyle.Underline), formatoDeImpresion, e, StringAlignment.Near, float.Parse("32"));
                ImprimirTexto(Textos.ResumenDeRecepcion_DocMaterial, 10, 32, new Font("Arial", 9, FontStyle.Underline), formatoDeImpresion, e, StringAlignment.Near, float.Parse("36"));
                ImprimirTexto(Textos.ResumenDeRecepcion_DescAlmacen, 10, 37, new Font("Arial", 9, FontStyle.Underline), formatoDeImpresion, e);
                ImprimirTexto(Textos.Bruto, 10, 43, new Font("Arial", 9, FontStyle.Underline), formatoDeImpresion, e, StringAlignment.Center, float.Parse("12"));
                ImprimirTexto(Textos.ResumenDeRecepcion_Kgs, 11, 43, new Font("Arial", 9, FontStyle.Underline), formatoDeImpresion, e, StringAlignment.Center, float.Parse("12"));
                ImprimirTexto(Textos.Tara, 10, 46, new Font("Arial", 9, FontStyle.Underline), formatoDeImpresion, e, StringAlignment.Center, float.Parse("12"));
                ImprimirTexto(Textos.ResumenDeRecepcion_Kgs, 11, 46, new Font("Arial", 9, FontStyle.Underline), formatoDeImpresion, e, StringAlignment.Center, float.Parse("12"));
                ImprimirTexto(Textos.Neto, 10, 49, new Font("Arial", 9, FontStyle.Underline), formatoDeImpresion, e, StringAlignment.Center, float.Parse("12"));
                ImprimirTexto(Textos.ResumenDeRecepcion_Kgs, 11, 49, new Font("Arial", 9, FontStyle.Underline), formatoDeImpresion, e, StringAlignment.Center, float.Parse("12"));
                ImprimirTexto(Textos.ResumenDeRecepcion_Tipo, 10, 52, new Font("Arial", 9, FontStyle.Underline), formatoDeImpresion, e);
                ImprimirTexto(Textos.ResumenDeRecepcion_Pesada, 11, 52, new Font("Arial", 9, FontStyle.Underline), formatoDeImpresion, e);
                if (totalPaginas > 0)
                {
                    //Datos
                    var fila = 12;
                    var lista = listas[Pagina];
                    foreach (var item in lista)
                    {

                        if (item.ItemNro != 99999)
                        {
                            ImprimirTexto(item.ItemNro.ToString(CultureInfo.InvariantCulture), fila, 2,
                                          new Font("Arial", 9), formatoDeImpresion, e, StringAlignment.Far);
                            ImprimirTexto(item.MaterialCodigo, fila, 5, new Font("Arial", 9), formatoDeImpresion, e,
                                          StringAlignment.Near, float.Parse("28,2"));
                            ImprimirTexto(item.MaterialDescripcion, fila, 9, new Font("Arial", 8), formatoDeImpresion, e,
                                          StringAlignment.Near, float.Parse("62"));
                            ImprimirTexto(item.RemitoNumero, fila, 22, new Font("Arial", 9), formatoDeImpresion, e);
                            ImprimirTexto(item.Lote, fila, 28, new Font("Arial", 9), formatoDeImpresion, e);
                            ImprimirTexto(item.DocMaterial, fila, 32, new Font("Arial", 9), formatoDeImpresion, e);
                            ImprimirTexto(item.AlmacenDescripcion, fila, 37, new Font("Arial", 8), formatoDeImpresion, e,
                                          StringAlignment.Near, float.Parse("32"));
                            ImprimirTexto(item.PesoBruto.HasValue ? item.PesoBruto.ToString() : string.Empty, fila, 43,
                                          new Font("Arial", 9), formatoDeImpresion, e, StringAlignment.Far,
                                          float.Parse("14,7"));
                            ImprimirTexto(item.PesoTara.HasValue ? item.PesoTara.ToString() : string.Empty, fila, 46,
                                          new Font("Arial", 9), formatoDeImpresion, e, StringAlignment.Far,
                                          float.Parse("14,7"));
                            ImprimirTexto(item.PesoNeto.HasValue ? item.PesoNeto.ToString() : string.Empty, fila, 49,
                                          new Font("Arial", 9), formatoDeImpresion, e, StringAlignment.Far,
                                          float.Parse("14,7"));
                            ImprimirTexto(item.ModalidadBalanza, fila, 52, new Font("Arial", 9), formatoDeImpresion, e,
                                          StringAlignment.Center, float.Parse("15"));
                        }
                        else //Subtotal
                        {
                            ImprimirTexto(Textos.ResumenDeRecepcion_TotalMaterial, fila, 37, new Font("Arial", 9, FontStyle.Bold), formatoDeImpresion, e);
                            ImprimirTexto(item.PesoBruto.ToString(), fila, 43, new Font("Arial", 9), formatoDeImpresion, e, StringAlignment.Far, float.Parse("14,7"));
                            ImprimirTexto(item.PesoTara.ToString(), fila, 46, new Font("Arial", 9), formatoDeImpresion, e, StringAlignment.Far, float.Parse("14,7"));
                            ImprimirTexto(item.PesoNeto.ToString(), fila, 49, new Font("Arial", 9), formatoDeImpresion, e, StringAlignment.Far, float.Parse("14,7"));
                        }
                        fila++;
                    }
                    if (Pagina == totalPaginas - 1)
                    {
                        fila++; //Final de Página
                        ImprimirTexto(Textos.ResumenDeRecepcion_Total, fila, 37, new Font("Arial", 9, FontStyle.Bold), formatoDeImpresion, e, StringAlignment.Far, float.Parse("14,7"));
                        ImprimirTexto(parametro.ImpResumenDeRecepcionItems.Sum(s => s.PesoBruto).ToString(), fila, 43, new Font("Arial", 9), formatoDeImpresion, e, StringAlignment.Far, float.Parse("14,7"));
                        ImprimirTexto(parametro.ImpResumenDeRecepcionItems.Sum(s => s.PesoTara).ToString(), fila, 46, new Font("Arial", 9), formatoDeImpresion, e, StringAlignment.Far, float.Parse("14,7"));
                        ImprimirTexto(parametro.ImpResumenDeRecepcionItems.Sum(s => s.PesoNeto).ToString(), fila, 49, new Font("Arial", 9), formatoDeImpresion, e, StringAlignment.Far, float.Parse("14,7"));
                    }
                    ImprimirTexto(new String('_', 146), 37, 1, new Font("Arial", 9), formatoDeImpresion, e);
                    ImprimirTexto(Textos.ResumenDeRecepcion_Página + "  " + ++Pagina + "/" + totalPaginas, 38, 49, new Font("Arial", 9), formatoDeImpresion, e);
                }
            }
            e.HasMorePages = Pagina < totalPaginas;
        }

        public override void Imprimir(object dto, string printerName, FirmaDto firmaDto)
        {
            PrinterSettings.PrinterName = printerName;
            parametro = (ImpResumenDeRecepcionDto)dto;
            Print();
        }
    }
}
