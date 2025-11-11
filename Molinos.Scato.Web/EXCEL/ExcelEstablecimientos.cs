using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Web.Helpers;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace Molinos.Scato.Web.EXCEL
{
    public class ExcelEstablecimientos
    {
        public void GenerarArchivo(ResultadoPrevisualizar resultado, IEnumerable<EstablecimientoDto> muestras)
        {
            var workbook = GenerarExcel(muestras);
            resultado.Archivo = workbook;
        }

        private static byte[] GenerarExcel(IEnumerable<EstablecimientoDto> items)
        {
            //Crear Libro de Trabajo Excel
            var workbook = new HSSFWorkbook();
            //workbook.Write();
            //Crear Hoja
            var sheet = (HSSFSheet)workbook.CreateSheet("Sheet1");

            //Crear Fuente
            var fontBold = workbook.CreateFont();
            fontBold.Boldweight = (short)FontBoldWeight.Bold;
            //Crear Estilo de Celda
            var stylebold = workbook.CreateCellStyle();
            stylebold.SetFont(fontBold);

            //Crear TITULO Excel
            //Crear Funete del Titulo
            var titleFont = workbook.CreateFont();
            titleFont.FontName = "Arial Black";
            //Crear Estilo de Celdas del Titulo
            var cellTitleStyle = workbook.CreateCellStyle();
            cellTitleStyle.BorderBottom = BorderStyle.None;
            cellTitleStyle.BorderLeft = BorderStyle.None;
            cellTitleStyle.BorderRight = BorderStyle.None;
            cellTitleStyle.BorderTop = BorderStyle.None;
            cellTitleStyle.SetFont(titleFont);
            //Crear Filas y Celdas para Titulo
            var row = sheet.CreateRow(1);
            var celda = row.CreateCell(2);
            //Establecer Estilo de Celda a la Celda del Titulo
            celda.CellStyle = cellTitleStyle;
            //Establecer Valor(Texto) a la Celda del Titulo
            celda.SetCellValue("Establecimientos Libro");
            //Crear Nuevas Celdas Combinadas CellRangeAddress(int firstRow, int lastRow, int firstCol, int lastCol)
            var merge = new CellRangeAddress(1, 1, 2, 4);
            sheet.AddMergedRegion(merge);
            //Crear Fecha y Hora del Libro
            celda = row.CreateCell(6);
            celda.SetCellValue(DateTime.Now.TimeOfDay.Formatted());
            celda.CellStyle = stylebold;
            row = sheet.CreateRow(0);
            celda = row.CreateCell(6);
            celda.SetCellValue(DateTime.Now.Formatted());
            celda.CellStyle = stylebold;
            //Titulos de Columnas de la Tabla
            //Crear Estilos de Encabezado de Columnas de la Tabla
            var cellBorderStyleColumnTitles = workbook.CreateCellStyle();
            cellBorderStyleColumnTitles.BorderBottom = BorderStyle.Thin;
            cellBorderStyleColumnTitles.BorderTop = BorderStyle.Thin;
            cellBorderStyleColumnTitles.BorderLeft = BorderStyle.Thin;
            cellBorderStyleColumnTitles.BorderRight = BorderStyle.Thin;
            cellBorderStyleColumnTitles.SetFont(fontBold);
            //Establecer Encabezados de Columnas de Tabla
            row = sheet.CreateRow(2);
            celda = row.CreateCell(0);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda.SetCellValue(Textos.Establecimiento_Codigo);
            celda = row.CreateCell(1);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda.SetCellValue(Textos.Establecimiento_Nombre);
            celda = row.CreateCell(2);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda.SetCellValue(Textos.Proveedor);
            celda = row.CreateCell(3);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda.SetCellValue(Textos.Comercial);
            celda = row.CreateCell(4);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda.SetCellValue(Textos.Corredor);
            celda = row.CreateCell(5);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda.SetCellValue(Textos.Observaciones);
            celda = row.CreateCell(6);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda.SetCellValue(Textos.Transportista_Localidad);
            celda = row.CreateCell(7);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda.SetCellValue(Textos.Transportista_Provincia); 
            celda = row.CreateCell(8);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda.SetCellValue("Renspa");

            //Establecer Relleno de Celdas de la Tabla
            var i = 3; //Fila Actual
            foreach (var item in items)
            {
                //Crear Nueva Fila
                row = sheet.CreateRow(i);
                //Rellenar Fila
                celda = row.CreateCell(0);
                celda.SetCellValue(item.CodigoDeEstablecimiento);
                celda = row.CreateCell(1);
                celda.SetCellValue(item.NombreDeEstablecimiento);
                celda = row.CreateCell(2);
                celda.SetCellValue(item.Proveedor);
                celda = row.CreateCell(3);
                celda.SetCellValue(item.Comercial);
                celda = row.CreateCell(4);
                celda.SetCellValue(item.ListaCorredoresAsociados);
                celda = row.CreateCell(5);
                celda.SetCellValue(item.Observaciones);
                celda = row.CreateCell(6);
                celda.SetCellValue(item.Localidad);
                celda = row.CreateCell(7);
                celda.SetCellValue(item.Provincia);
                celda = row.CreateCell(8);
                celda.SetCellValue(item.CodigoRENSPA);
                i++;
            }

            using (var fileData = new MemoryStream())
            {
                workbook.Write(fileData);
                return fileData.ToArray();
            }
        }

    }
}
