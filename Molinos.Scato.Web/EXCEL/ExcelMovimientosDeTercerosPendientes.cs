using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Web.Helpers;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace Molinos.Scato.Web.EXCEL
{
    public class ExcelMovimientosDeTercerosPendientes
    {
        public void GenerarArchivo(ResultadoPrevisualizar resultado, IEnumerable<MovimientoDeTercerosListaDto> muestras)
        {
            var workbook = GenerarExcel(muestras);
            resultado.Archivo = workbook;
        }

        private static byte[] GenerarExcel(IEnumerable<MovimientoDeTercerosListaDto> muestras)
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
            var row = sheet.CreateRow(0);
            var celda = row.CreateCell(0);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda.SetCellValue(Textos.Lote_FechaDescarga);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(1);
            celda.SetCellValue(Textos.TipoDocumentoIngreso);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(2);
            celda.SetCellValue(Textos.NumeroDocumentoIngreso);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(3);
            celda.SetCellValue(Textos.Lote_Patente);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(4);
            celda.SetCellValue(Textos.Material);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(5);
            celda.SetCellValue(Textos.PesoNeto);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(6);
            celda.SetCellValue(Textos.Transportista);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(7);
            celda.SetCellValue(Textos.Destino);
            celda.CellStyle = cellBorderStyleColumnTitles;
            
            //Valores
            var i = 1;
            foreach (var muestra in muestras)
            {
                row = sheet.CreateRow(i);

                celda = row.CreateCell(0);
                celda.SetCellValue(muestra.FechaDescarga.FormattedTime());
                celda = row.CreateCell(1);
                celda.SetCellValue(muestra.TipoDocumentoIngreso.DisplayText());
                celda = row.CreateCell(2);
                celda.SetCellValue(muestra.NroCartaPorteConFormato);
                celda = row.CreateCell(3);
                celda.SetCellValue(muestra.Patente);
                celda = row.CreateCell(4);
                celda.SetCellValue(muestra.Material);
                celda = row.CreateCell(5);
                celda.SetCellValue((muestra.PesoNeto.HasValue ? muestra.PesoNeto.Value.ToString(CultureInfo.InvariantCulture) : ""));
                celda = row.CreateCell(6);
                celda.SetCellValue(muestra.Transportista);

                celda = row.CreateCell(7);
                celda.SetCellValue(muestra.CentroDestino);
                
                
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
