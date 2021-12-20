using System.Collections.Generic;
using System.IO;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace Molinos.Scato.Web.EXCEL
{
    public class ExcelMovimientosDeTercerosEgresos
    {
        public void GenerarArchivo(ResultadoPrevisualizar resultado, IEnumerable<MovimientoDeTercerosDto> muestras)
        {
            var workbook = GenerarExcel(muestras);
            resultado.Archivo = workbook;
        }

        private static byte[] GenerarExcel(IEnumerable<MovimientoDeTercerosDto> muestras)
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
            celda.SetCellValue("ALMACEN_EMISOR");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(1);
            celda.SetCellValue("ALMACEN_RECEPTOR");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(2);
            celda.SetCellValue("CANTIDAD");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(3);
            celda.SetCellValue("CENTRO_EMISOR");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(4);
            celda.SetCellValue("CENTRO_RECEPTOR");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(5);
            celda.SetCellValue("CUIT_TRANSP");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(6);
            celda.SetCellValue("FECHA_CON");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(7);
            celda.SetCellValue("FECHA_DOC");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(8);
            celda.SetCellValue("MATERIAL");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(9);
            celda.SetCellValue("NOM_CHOFER");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(10);
            celda.SetCellValue("NOM_TRANSPORTIST");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(11);
            celda.SetCellValue("NUM_DOC");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(12);
            celda.SetCellValue("PATENTE");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(13);
            celda.SetCellValue("PAT_REMOLQUE");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(14);
            celda.SetCellValue("PRESINTO01");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(15);
            celda.SetCellValue("PRESINTO02");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(16);
            celda.SetCellValue("TIPO_DOCU");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(17);
            celda.SetCellValue("DOC_LEGAL");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(18);
            celda.SetCellValue("KM_RECOR");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(19);
            celda.SetCellValue("CLASE_EXPEDICION");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(20);
            celda.SetCellValue("UNI_MED_CANT");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(21);
            celda.SetCellValue("LOTE");
            celda.CellStyle = cellBorderStyleColumnTitles;

            //Valores
            var i = 1;
            foreach (var muestra in muestras)
            {
                row = sheet.CreateRow(i);

                celda = row.CreateCell(0);
                celda.SetCellValue(muestra.AlmEmisor);
                celda = row.CreateCell(1);
                celda.SetCellValue(muestra.AlmReceptor);
                celda = row.CreateCell(2);
                celda.SetCellValue(muestra.Cantidad);
                celda = row.CreateCell(3);
                celda.SetCellValue(muestra.CentroEmisor);
                celda = row.CreateCell(4);
                celda.SetCellValue(muestra.CentroReceptor);
                celda = row.CreateCell(5);
                celda.SetCellValue(muestra.CUITTransp);
                celda = row.CreateCell(6);
                celda.SetCellValue(muestra.FechaContab);

                celda = row.CreateCell(7);
                celda.SetCellValue(muestra.FechaDoc);
                celda = row.CreateCell(8);
                celda.SetCellValue(muestra.Material);
                celda = row.CreateCell(9);
                celda.SetCellValue(muestra.NombreChofer);
                celda = row.CreateCell(10);
                celda.SetCellValue(muestra.NombreTransportista);
                celda = row.CreateCell(11);
                celda.SetCellValue(muestra.DocChofer);
                celda = row.CreateCell(12);
                celda.SetCellValue(muestra.Patente1);
                celda = row.CreateCell(13);
                celda.SetCellValue(muestra.Patente2);
                celda = row.CreateCell(14);
                celda.SetCellValue(muestra.Precinto1);
                celda = row.CreateCell(15);
                celda.SetCellValue(muestra.Precinto2);
                celda = row.CreateCell(16);
                celda.SetCellValue(muestra.TipoDoc);
                celda = row.CreateCell(17);
                celda.SetCellValue(muestra.NroDocumento);
                celda = row.CreateCell(18);
                celda.SetCellValue(muestra.Kilometros);
                celda = row.CreateCell(19);
                celda.SetCellValue(muestra.ClaseExpedicion);
                celda = row.CreateCell(20);
                celda.SetCellValue(muestra.UniMed);
                celda = row.CreateCell(21);
                celda.SetCellValue(muestra.Lote);
                
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
