using System;
using System.Collections.Generic;
using System.IO;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace Molinos.Scato.Web.EXCEL
{
    public class ExcelHuellaDigital
    {
        private ExcelHuellaDigital() { }
        public static byte[] GenerarArchivoExcel(IList<HuellaDigitalOrdenDto> huellas)
        {
            var workbook = new HSSFWorkbook();
            var sheet = (HSSFSheet)workbook.CreateSheet("HuellasDigitales");

            // Crear estilos para el título y las celdas con bordes
            var estiloTitulo = CrearEstiloTitulo(workbook);
            var estiloFecha = CrearEstiloFecha(workbook);
            var estiloBordes = CrearEstiloConBordes(workbook);

            // Crear encabezados de columnas
            CrearEncabezados(sheet, estiloTitulo);

            // Llenar datos en las filas
            int fila = 1;
            foreach (var huella in huellas)
            {
                var row = sheet.CreateRow(fila++);
                LlenarFila(row, huella, estiloFecha, estiloBordes);
            }

            // Autoajustar el ancho de las columnas
            for (int i = 0; i <= 14; i++)
            {
                sheet.AutoSizeColumn(i);
            }

            // Escribir el archivo en un MemoryStream
            using (var fileData = new MemoryStream())
            {
                workbook.Write(fileData);
                return fileData.ToArray();
            }
        }

        private static ICellStyle CrearEstiloTitulo(HSSFWorkbook workbook)
        {
            var style = workbook.CreateCellStyle();
            var font = workbook.CreateFont();
            font.Boldweight = (short)FontBoldWeight.Bold;
            style.SetFont(font);
            style.BorderBottom = BorderStyle.Thin;
            style.BorderTop = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            return style;
        }

        private static ICellStyle CrearEstiloFecha(HSSFWorkbook workbook)
        {
            var style = workbook.CreateCellStyle();
            style.DataFormat = workbook.CreateDataFormat().GetFormat("dd/MM/yyyy HH:mm");
            return style;
        }

        private static ICellStyle CrearEstiloConBordes(HSSFWorkbook workbook)
        {
            var style = workbook.CreateCellStyle();
            style.BorderBottom = BorderStyle.Thin;
            style.BorderTop = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            return style;
        }

        private static void CrearEncabezados(ISheet sheet, ICellStyle estiloTitulo)
        {
            var row = sheet.CreateRow(0);
            string[] encabezados = { "ID", "TIPO", "PATENTE", "ACOPLADO", "CHOFER", "TRANSPORTISTA",
                                     "PESO_TARA", "LUGAR_PESAJE", "FECHA_HORA_PESAJE", "ESTADO" ,"BALANZA", "OBSERVACIONES" , "USUARIO"
                                   };

            for (int i = 0; i < encabezados.Length; i++)
            {
                var celda = row.CreateCell(i);
                celda.SetCellValue(encabezados[i]);
                celda.CellStyle = estiloTitulo;
            }
        }

        private static void LlenarFila(IRow row, HuellaDigitalOrdenDto muestra, ICellStyle estiloFecha, ICellStyle estiloBordes)
        {
            row.CreateCell(0).SetCellValue(muestra.Id);
            row.CreateCell(1).SetCellValue(muestra.DescripcionTipo);
            row.CreateCell(2).SetCellValue(muestra.Patente);
            row.CreateCell(3).SetCellValue(muestra.Acoplado);
            row.CreateCell(4).SetCellValue(muestra.Chofer);
            row.CreateCell(5).SetCellValue(muestra.Transportista);
            row.CreateCell(6).SetCellValue(muestra.PesoTara ?? 0);
            row.CreateCell(7).SetCellValue(muestra.LugarPesaje);

            var celdaFechaHoraPesaje = row.CreateCell(8);
            celdaFechaHoraPesaje.SetCellValue(muestra.FechaHoraPesaje ?? DateTime.Now);
            celdaFechaHoraPesaje.CellStyle = estiloFecha;

            row.CreateCell(9).SetCellValue(muestra.DescripcionEstado);
            row.CreateCell(10).SetCellValue(muestra.Balanza);
            row.CreateCell(11).SetCellValue(muestra.Observaciones);
            row.CreateCell(12).SetCellValue(muestra.Usuario);

            // Aplicar bordes a cada celda en la fila
            for (int i = 0; i <= 12; i++)
            {
                row.GetCell(i).CellStyle = estiloBordes;
            }
        }
    }
}
