using System;
using System.Collections.Generic;
using System.IO;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace Molinos.Scato.Web.EXCEL
{
    public class ExcelDetalleDeMovimientos
    {
        public void GenerarArchivo(ResultadoPrevisualizar resultado, ReporteDetalleMovimientoDto detalleMovimiento, DateTime fecha)
        {
            var workbook = GenerarExcel(detalleMovimiento,fecha);
            resultado.Archivo = workbook;
        
        }

        private static byte[] GenerarExcel(ReporteDetalleMovimientoDto detalleMovimiento, DateTime fecha)
        {
            var workbook = new HSSFWorkbook();
            var sheet = (HSSFSheet) workbook.CreateSheet("Sheet1");
            var stylebold = workbook.CreateCellStyle();
            var fontBold = workbook.CreateFont();
            fontBold.Boldweight = (short)FontBoldWeight.Bold;
            stylebold.SetFont(fontBold);
            //Imagen
            //var row = sheet.CreateRow(0);
            //var celda = row.CreateCell(0);
            //var patriarch = celda.Sheet.CreateDrawingPatriarch();
            //patriarch.CreatePicture()
            //var cellMergeImg = new CellRangeAddress(0, 1, 0, 0);
            //sheet.AddMergedRegion(cellMergeImg);

            //Titulos Columnas Tabla
            var cellBorderStyleColumnTitles = workbook.CreateCellStyle();
            cellBorderStyleColumnTitles.BorderBottom = BorderStyle.Thin;
            cellBorderStyleColumnTitles.BorderTop = BorderStyle.Thin;
            cellBorderStyleColumnTitles.BorderLeft = BorderStyle.Thin;
            cellBorderStyleColumnTitles.BorderRight = BorderStyle.Thin;
            cellBorderStyleColumnTitles.SetFont(fontBold);   

            var row = sheet.CreateRow(3);
            var celda = row.CreateCell(0);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda.SetCellValue("Detalle de Movimiento de Ingresos:");
            CellRangeAddress regionTitulo = new CellRangeAddress(4,4,0,3);
            sheet.AddMergedRegion(regionTitulo);

            celda = row.CreateCell(3);
            celda.SetCellValue(fecha.ToString("dd/MM/yyyy"));

            row = sheet.CreateRow(5);
            celda = row.CreateCell(0);
            celda.SetCellValue("Ingreso");
            CellRangeAddress regionIngreso = new CellRangeAddress(5,6, 0, 1);
            sheet.AddMergedRegion(regionIngreso);
            celda.CellStyle = cellBorderStyleColumnTitles;
            
            celda = row.CreateCell(2);
            celda.SetCellValue("Dia");
            celda.CellStyle = cellBorderStyleColumnTitles;
            CellRangeAddress region = new CellRangeAddress(5, 5, 2, 3);
            sheet.AddMergedRegion(region);
            
            celda = row.CreateCell(4);
            celda.SetCellValue("Mes");
            celda.CellStyle = cellBorderStyleColumnTitles;
            CellRangeAddress region2 = new CellRangeAddress(5, 5, 4, 5);
            sheet.AddMergedRegion(region2);

            celda = row.CreateCell(6);
            celda.SetCellValue("Año");
            celda.CellStyle = cellBorderStyleColumnTitles;
            CellRangeAddress region3 = new CellRangeAddress(5, 5, 6, 7);
            sheet.AddMergedRegion(region3);

            row = sheet.CreateRow(6);

            celda = row.CreateCell(2);
            celda.SetCellValue("Cantidad");
            celda.CellStyle = cellBorderStyleColumnTitles;

            celda = row.CreateCell(3);
            celda.SetCellValue("Tons");
            celda.CellStyle = cellBorderStyleColumnTitles;

            celda = row.CreateCell(4);
            celda.SetCellValue("Cantidad");
            celda.CellStyle = cellBorderStyleColumnTitles;

            celda = row.CreateCell(5);
            celda.SetCellValue("Tons");
            celda.CellStyle = cellBorderStyleColumnTitles;
            
            celda = row.CreateCell(6);
            celda.SetCellValue("Cantidad");
            celda.CellStyle = cellBorderStyleColumnTitles;

            celda = row.CreateCell(7);
            celda.SetCellValue("Tons");
            celda.CellStyle = cellBorderStyleColumnTitles;

            //Valores
            int i = 7;
            int totalCantidadxD = 0;
            decimal totalTonsxD = 0;
            int totalCantidadxM = 0;
            decimal totalTonsxM = 0;
            int totalCantidadxA = 0;
            decimal totalTonsxA = 0;
            foreach (var detalle in detalleMovimiento.DsIngresoDto)
            {
               
                row = sheet.CreateRow(i);

                celda = row.CreateCell(0);
                celda.SetCellValue(detalle.IngresoxD);
                
                celda = row.CreateCell(2);
                celda.SetCellValue(detalle.CantidadxD);
                totalCantidadxD += detalle.CantidadxD;

                celda = row.CreateCell(3);
                celda.SetCellValue((double)detalle.TonsxD);
                totalTonsxD += detalle.TonsxD;

                celda = row.CreateCell(4);
                celda.SetCellValue(detalle.CantidadxM);
                totalCantidadxM += detalle.CantidadxM;

                celda = row.CreateCell(5);
                celda.SetCellValue((double)detalle.TonsxM);
                totalTonsxM += detalle.TonsxM;

                celda = row.CreateCell(6);
                celda.SetCellValue(detalle.CantidadxA);
                totalCantidadxA += detalle.CantidadxA;

                celda = row.CreateCell(7);
                celda.SetCellValue((double)detalle.TonsxA);
                totalTonsxA += detalle.CantidadxA;

                i++;
                
            }
            row = sheet.CreateRow(i);
            celda = row.CreateCell(0);
            celda.SetCellValue("Total");
            CellRangeAddress regionTotal = new CellRangeAddress(i, i, 0, 1);
            sheet.AddMergedRegion(regionTotal);

            celda = row.CreateCell(2);
            celda.SetCellValue(totalCantidadxD);
            celda = row.CreateCell(3);
            celda.SetCellValue((double)totalTonsxD);
            celda = row.CreateCell(4);
            celda.SetCellValue(totalCantidadxM);
            celda = row.CreateCell(5);
            celda.SetCellValue((double)totalTonsxM);
            celda = row.CreateCell(6);
            celda.SetCellValue(totalCantidadxA);
            celda = row.CreateCell(7);
            celda.SetCellValue((double)totalTonsxA);

            using (var fileData = new MemoryStream())
            {
                workbook.Write(fileData);
                return fileData.ToArray();
            }
        }

    }
}
