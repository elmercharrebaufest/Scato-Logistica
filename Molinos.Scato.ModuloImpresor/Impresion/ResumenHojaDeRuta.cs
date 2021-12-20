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
    public class ResumenHojaDeRuta : DocumentoImpresion
    {
        private ImpResumenHojaDeRutaDto parametro;
        private int Pagina { get; set; }

        public ResumenHojaDeRuta(ImpResumenHojaDeRutaDto parametro, string printerName)
        {
            PrinterSettings.PrinterName = printerName;
            this.parametro = parametro;
            Pagina = 0;
        }

        public ResumenHojaDeRuta() { }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = CurrentUICulture;
            //Formato de la Hoja
            var formatoDeImpresion = new FormatoDeImpresionDto
                {
                    Columnas = 14,
                    Filas = 42,
                    FormatoDePapelAlto = 280,
                    FormatoDePapelAncho = 210,
                    Posicion = Posicion.Vertical,
                };

            //División de elementos
            var listas = parametro.DatosDeWorkflows.OrderBy(o => o.FechaCalado).Split(23);
            var totalPaginas = listas.Count;

            if (Pagina < totalPaginas || totalPaginas == 0)
            {
                //Titulo
                ImprimirTexto("Camiones asignados - Puesto Comando", 2, 4, new Font("Arial", 10, FontStyle.Bold), formatoDeImpresion, e, StringAlignment.Far);
                //Cabeceras
                ImprimirTexto(Textos.Patente               , 4, 1, new Font("Arial", 10, FontStyle.Underline), formatoDeImpresion, e);
                ImprimirTexto("Acoplado"                   , 4, 2, new Font("Arial", 10, FontStyle.Underline), formatoDeImpresion, e);
                ImprimirTexto(Textos.Material              , 4, 3, new Font("Arial", 10, FontStyle.Underline), formatoDeImpresion, e);
                ImprimirTexto(Textos.NumeroDocumentoIngreso, 4, 6, new Font("Arial", 10, FontStyle.Underline), formatoDeImpresion, e);
                ImprimirTexto(Textos.Calidad               , 4, 8, new Font("Arial", 10, FontStyle.Underline), formatoDeImpresion, e);
                ImprimirTexto(Textos.Workflow_FechaCalado  , 4, 10, new Font("Arial", 10, FontStyle.Underline), formatoDeImpresion, e);
                ImprimirTexto(Textos.Workflow_Humedad      , 4, 12, new Font("Arial", 10, FontStyle.Underline), formatoDeImpresion, e);
                
                if (totalPaginas > 0)
                {
                    //Datos
                    var fila = 5;
                    var lista = listas[Pagina];
                    foreach (var item in lista)
                    {

                        ImprimirTexto(item.Patente               , fila, 1, new Font("Arial", 9), formatoDeImpresion, e);
                        ImprimirTexto(item.PatenteAcoplado       , fila, 2, new Font("Arial", 9), formatoDeImpresion, e);
                        ImprimirTexto(item.MaterialDescripcion   , fila, 3, new Font("Arial", 8), formatoDeImpresion, e);
                        ImprimirTexto(item.NumeroDocumentoIngreso, fila, 6, new Font("Arial", 9), formatoDeImpresion, e);
                        ImprimirTexto(item.Calidad               , fila, 8, new Font("Arial", 9), formatoDeImpresion, e);
                        ImprimirTexto(item.FechaCalado.ToString("dd/MM/yyyy HH:mm"), fila, 10, new Font("Arial", 9), formatoDeImpresion, e);
                        ImprimirTexto(item.Humedad               , fila, 12, new Font("Arial", 9), formatoDeImpresion, e);
                        fila++;
                    }
                }
            }
            Pagina++;
            e.HasMorePages = Pagina < totalPaginas;
        }

        public override void Imprimir(object dto, string printerName, FirmaDto firmaDto)
        {
            parametro = (ImpResumenHojaDeRutaDto)dto;

            if (!string.IsNullOrEmpty(printerName))
            {
                PrinterSettings.PrinterName = printerName;
                Print();
            }
        }
    }
}
