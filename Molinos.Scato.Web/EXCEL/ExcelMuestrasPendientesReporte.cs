using System;
using System.Collections.Generic;
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
    public class ExcelMuestrasPendientesReporte
    {
        public void GenerarArchivo(ResultadoPrevisualizar resultado, List<MuestraEnvioACamaraDto> muestras)
        {
            var workbook = GenerarExcel(resultado, muestras);
            resultado.Archivo = workbook;
        }

        private static byte[] GenerarExcel(ResultadoPrevisualizar resultado, List<MuestraEnvioACamaraDto> muestras)
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

            //TITULOREPORTE
            var titleFont = workbook.CreateFont();
            titleFont.FontName = "Arial Black";
            var cellTitleStyle = workbook.CreateCellStyle();
            cellTitleStyle.BorderBottom = BorderStyle.None;
            cellTitleStyle.BorderLeft = BorderStyle.None;
            cellTitleStyle.BorderRight = BorderStyle.None;
            cellTitleStyle.BorderTop = BorderStyle.None;
            cellTitleStyle.SetFont(titleFont);
            var row = sheet.CreateRow(1);
            var celda = row.CreateCell(2);
            celda.CellStyle = cellTitleStyle;
            celda.SetCellValue("Cartas de Porte Sin Lote Asignado");
            var merge = new CellRangeAddress(1,1,2,4);
            sheet.AddMergedRegion(merge);

            //Fecha y Hora del reporte
            celda = row.CreateCell(6);
            celda.SetCellValue(DateTime.Now.TimeOfDay.Formatted());
            celda.CellStyle = stylebold;
            row = sheet.CreateRow(0);
            celda = row.CreateCell(6);
            celda.SetCellValue(DateTime.Now.Formatted());
            celda.CellStyle = stylebold;

            //Titulos Columnas Tabla
            var cellBorderStyleColumnTitles = workbook.CreateCellStyle();
            cellBorderStyleColumnTitles.BorderBottom = BorderStyle.Thin;
            cellBorderStyleColumnTitles.BorderTop = BorderStyle.Thin;
            cellBorderStyleColumnTitles.BorderLeft = BorderStyle.Thin;
            cellBorderStyleColumnTitles.BorderRight = BorderStyle.Thin;
            cellBorderStyleColumnTitles.SetFont(fontBold);
            row = sheet.CreateRow(2);
            celda = row.CreateCell(0);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda.SetCellValue(Textos.CartaPorte);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(1);
            celda.SetCellValue(Textos.Fecha);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(2);
            celda.SetCellValue(Textos.Lote_FechaDescarga);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(3);
            celda.SetCellValue(Textos.Lote_NetoPlanta);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(4);
            celda.SetCellValue(Textos.Proveedor);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(5);
            celda.SetCellValue(Textos.Lote_NombreProveedor);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(6);
            celda.SetCellValue(Textos.Lote_NumeroMuestra);
            celda.CellStyle = cellBorderStyleColumnTitles;

            //Valores
            var i = 3;
            foreach (var muestra in muestras)
            {
                row = sheet.CreateRow(i);

                celda = row.CreateCell(0);
                celda.SetCellValue(muestra.NroCartaPorte);
                celda = row.CreateCell(1);
                celda.SetCellValue(muestra.FechaCartaPorte.Formatted());
                celda = row.CreateCell(2);
                celda.SetCellValue(muestra.FechaDescarga.HasValue ? muestra.FechaDescarga.Value.Formatted() : string.Empty);
                celda = row.CreateCell(3);
                celda.SetCellValue(muestra.PesoNeto.ToString());
                celda = row.CreateCell(4);
                celda.SetCellValue(muestra.ProveedorCodigoSap);
                celda = row.CreateCell(5);
                celda.SetCellValue(muestra.Proveedor);
                celda = row.CreateCell(6);
                celda.SetCellValue(muestra.NroMuestra);
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
