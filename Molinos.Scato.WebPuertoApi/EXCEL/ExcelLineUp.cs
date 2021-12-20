using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Molinos.Scato.WebPuertoApi.EXCEL
{
    public class ExcelLineUp
    {
        public void GenerarArchivo(ResultadoPrevisualizar resultado, IEnumerable<InstanciaWorkflowPuertoDto> embarques, EstadoPuertoDto estadoPuerto)
        {
            var workbook = GenerarExcel(embarques, estadoPuerto);
            resultado.Archivo = workbook;
        }

        private static byte[] GenerarExcel(IEnumerable<InstanciaWorkflowPuertoDto> muestras, EstadoPuertoDto estadoPuerto)
        {
            var workbook = new HSSFWorkbook();
            var sheet = (HSSFSheet)workbook.CreateSheet("Posicion");
            //var stylebold = workbook.CreateCellStyle();
            //stylebold.SetFont(fontBold);

            ICellStyle estiloHeaderTabla = EstiloHeaderTabla(workbook);
            ICellStyle estiloNombrePuerto = EstiloNombrePuerto(workbook);
            ICellStyle estiloDate = EstiloDate(workbook);
            ICellStyle estiloContenido = EstiloContenido(workbook);
            ICellStyle estiloContenidoBold = EstiloContenidoBold(workbook);
            ICellStyle estiloMuelle = EstiloMuelle(workbook);

            var fechaCalado = estadoPuerto.FechaCalado.HasValue? estadoPuerto.FechaCalado.Value.ToString("dd-MMM HH:mm") : "";
            //Valores
            var encabezado = false;
            var i = 1;
            var row = sheet.CreateRow(i++);
            var celda = row.CreateCell(0);
            celda.SetCellValue("Date:");
            celda.CellStyle = estiloDate;
            celda = row.CreateCell(1);
            celda.SetCellValue(DateTime.Now.ToString("dd-MMM"));
            celda.CellStyle = estiloDate;
            celda = row.CreateCell(4);
            celda.SetCellValue($"Calado ({fechaCalado}):");
            celda.CellStyle = estiloDate;
            celda = row.CreateCell(5);
            celda.SetCellValue(estadoPuerto.Calado);
            celda.CellStyle = estiloDate;
            celda = row.CreateCell(6);
            celda.SetCellValue("Paso Crítico:");
            celda.CellStyle = estiloDate;
            celda = row.CreateCell(7);
            celda.SetCellValue(estadoPuerto.Ubicacion);
            celda.CellStyle = estiloDate;

            row = sheet.CreateRow(i++);
            foreach (var muestrasPorPuerto in muestras.GroupBy(x => new { x.Embarque.SanBenito, x.Embarque.Vicentin, x.Embarque.Noryon, x.Embarque.OtrosMuelles})
                    .OrderBy(x => x.Key.OtrosMuelles).ThenBy(x => x.Key.Noryon).ThenBy(x => x.Key.Vicentin).ThenBy(x => x.Key.SanBenito))
            {
                row = sheet.CreateRow(i++);
                row.Height = 550;
                celda = row.CreateCell(1);
                celda.CellStyle = estiloMuelle;
                celda.SetCellValue($"Disponibilidad del muelle: {(muestrasPorPuerto.Any(x => x.Embarque.Ubicacion == 2) ? "" : "No")} Operando"); /**Muelle de Carga**/
                celda = row.CreateCell(4);
                celda.CellStyle = estiloNombrePuerto;
                if (muestrasPorPuerto.Key.SanBenito)
                {
                    celda.SetCellValue("SAN BENITO");
                    i = InsertTableHeader(sheet, estiloHeaderTabla, i);
                    encabezado = true;
                }
                if (muestrasPorPuerto.Key.Vicentin)
                {
                    celda.SetCellValue("VICENTIN");
                    if (!encabezado)
                    { 
                        i = InsertTableHeader(sheet, estiloHeaderTabla, i);
                        encabezado = true;
                    }
                    else
                        row = sheet.CreateRow(i++);
                }
                else if(muestrasPorPuerto.Key.Noryon)
                {
                    celda.SetCellValue("NOURYON");
                    if (!encabezado)
                    {
                        i = InsertTableHeader(sheet, estiloHeaderTabla, i);
                        encabezado = true;
                    }
                    else
                        row = sheet.CreateRow(i++);
                }
                else if (muestrasPorPuerto.Key.OtrosMuelles)
                {
                    celda.SetCellValue("OTROS MUELLES");
                    if (!encabezado)
                    {
                        i = InsertTableHeader(sheet, estiloHeaderTabla, i);
                        encabezado = true;
                    }
                    else
                        row = sheet.CreateRow(i++);
                }
                InsertRows(sheet, ref i, muestrasPorPuerto, estiloContenido, estiloContenidoBold);
            }

            sheet.SetColumnWidth(0, (int)Math.Floor(30m * 256)); //Buque
            sheet.SetColumnWidth(1, (int)Math.Floor(15m * 256)); //Coordinador
            sheet.SetColumnWidth(2, (int)Math.Floor(15m * 256)); //Agencia
            sheet.SetColumnWidth(3, (int)Math.Floor(15m * 256)); //Ata
            sheet.SetColumnWidth(4, (int)Math.Floor(40m * 256)); //Cantidades
            sheet.SetColumnWidth(5, (int)Math.Floor(10m * 256)); //Oblig. Carga
            sheet.SetColumnWidth(6, (int)Math.Floor(18m * 256)); //Recala
            sheet.SetColumnWidth(7, (int)Math.Floor(40m * 256)); //Observaciones

            using (var fileData = new MemoryStream())
            {
                workbook.Write(fileData);
                return fileData.ToArray();
            }
        }

        private static ICellStyle EstiloHeaderTabla(HSSFWorkbook workbook)
        {
            var fontBold = workbook.CreateFont();
            fontBold.FontHeightInPoints = 9;
            fontBold.Boldweight = (short)FontBoldWeight.Bold;

            var cellBorderStyleColumnTitles = workbook.CreateCellStyle();
            cellBorderStyleColumnTitles.BorderBottom = BorderStyle.Medium;
            cellBorderStyleColumnTitles.BorderTop = BorderStyle.Medium;
            cellBorderStyleColumnTitles.BorderLeft = BorderStyle.Medium;
            cellBorderStyleColumnTitles.BorderRight = BorderStyle.Medium;
            cellBorderStyleColumnTitles.SetFont(fontBold);
            cellBorderStyleColumnTitles.FillForegroundColor = IndexedColors.LightGreen.Index;
            cellBorderStyleColumnTitles.FillPattern = FillPattern.SolidForeground;
            cellBorderStyleColumnTitles.Alignment = HorizontalAlignment.Center;
            cellBorderStyleColumnTitles.WrapText = true;
            return cellBorderStyleColumnTitles;
        }

        private static ICellStyle EstiloDate(HSSFWorkbook workbook)
        {
            var fontBold = workbook.CreateFont();
            fontBold.FontHeightInPoints = 12;
            fontBold.Boldweight = (short)FontBoldWeight.Bold;

            var cellBorderStyleColumnTitles = workbook.CreateCellStyle();
            cellBorderStyleColumnTitles.BorderBottom = BorderStyle.Medium;
            cellBorderStyleColumnTitles.BorderTop = BorderStyle.Medium;
            cellBorderStyleColumnTitles.BorderLeft = BorderStyle.Medium;
            cellBorderStyleColumnTitles.BorderRight = BorderStyle.Medium;
            cellBorderStyleColumnTitles.SetFont(fontBold);
            cellBorderStyleColumnTitles.FillForegroundColor = IndexedColors.Grey25Percent.Index;
            cellBorderStyleColumnTitles.FillPattern = FillPattern.SolidForeground;
            cellBorderStyleColumnTitles.Alignment = HorizontalAlignment.Center;
            return cellBorderStyleColumnTitles;
        }

        private static ICellStyle EstiloMuelle(HSSFWorkbook workbook)
        {
            var fontBold = workbook.CreateFont();
            fontBold.FontHeightInPoints = 12;
            fontBold.Boldweight = (short)FontBoldWeight.Bold;

            var cellBorderStyleColumnTitles = workbook.CreateCellStyle();
            cellBorderStyleColumnTitles.SetFont(fontBold);
            cellBorderStyleColumnTitles.Alignment = HorizontalAlignment.Center;
            return cellBorderStyleColumnTitles;
        }

        private static ICellStyle EstiloNombrePuerto(HSSFWorkbook workbook)
        {
            var fontBold = workbook.CreateFont();
            fontBold.FontHeightInPoints = 18;
            fontBold.Boldweight = (short)FontBoldWeight.Bold;

            var cellBorderStyleColumnTitles = workbook.CreateCellStyle();
            cellBorderStyleColumnTitles.SetFont(fontBold);
            cellBorderStyleColumnTitles.Alignment = HorizontalAlignment.Center;
            return cellBorderStyleColumnTitles;
        }

        private static ICellStyle EstiloContenidoBold(HSSFWorkbook workbook)
        {
            var fontBold = workbook.CreateFont();
            fontBold.FontHeightInPoints = 9;
            fontBold.Boldweight = (short)FontBoldWeight.Bold;

            var cellBorderStyleColumnTitles = workbook.CreateCellStyle();
            cellBorderStyleColumnTitles.BorderBottom = BorderStyle.Thin;
            cellBorderStyleColumnTitles.BorderTop = BorderStyle.Thin;
            cellBorderStyleColumnTitles.BorderLeft = BorderStyle.Thin;
            cellBorderStyleColumnTitles.BorderRight = BorderStyle.Thin;
            cellBorderStyleColumnTitles.SetFont(fontBold);
            cellBorderStyleColumnTitles.WrapText = true;

            return cellBorderStyleColumnTitles;
        }

        private static ICellStyle EstiloContenido(HSSFWorkbook workbook)
        {
            var fontBold = workbook.CreateFont();
            fontBold.FontHeightInPoints = 9;

            var cellBorderStyleColumnTitles = workbook.CreateCellStyle();
            cellBorderStyleColumnTitles.BorderBottom = BorderStyle.Thin;
            cellBorderStyleColumnTitles.BorderTop = BorderStyle.Thin;
            cellBorderStyleColumnTitles.BorderLeft = BorderStyle.Thin;
            cellBorderStyleColumnTitles.BorderRight = BorderStyle.Thin;
            cellBorderStyleColumnTitles.SetFont(fontBold);
            cellBorderStyleColumnTitles.WrapText = true;

            return cellBorderStyleColumnTitles;
        }

        private static void InsertRows(HSSFSheet sheet, ref int i, IGrouping<dynamic, InstanciaWorkflowPuertoDto> muestrasPorPuerto, ICellStyle font, ICellStyle fontBold)
        {
            foreach (var muestra in muestrasPorPuerto)
            {
                var row = sheet.CreateRow(i);
                var celda = row.CreateCell(0);
                celda.SetCellValue(muestra.Embarque.NombreBuque);
                celda.CellStyle = fontBold;
                celda = row.CreateCell(1);
                celda.SetCellValue(muestra.Embarque.Coordinadores != null ? muestra.Embarque.Coordinadores.Nombre : "");
                celda.CellStyle = font;
                celda = row.CreateCell(2);
                celda.SetCellValue(muestra.Embarque.Agencias != null ? muestra.Embarque.Agencias.Nombre : "");
                celda.CellStyle = font;
                celda = row.CreateCell(3);
                celda.SetCellValue(muestra.Embarque.ATA != null ? muestra.Embarque.ATA.Nombre : "");
                celda.CellStyle = font;
                celda = row.CreateCell(4);
                celda.SetCellValue(string.Join(" / ", muestra.Embarque.MaterialesPuertoCantidad));
                celda.CellStyle = font;
                celda = row.CreateCell(5);
                celda.SetCellValue(muestra.Embarque.ObligacionCarga.HasValue ? muestra.Embarque.ObligacionCarga.Value.ToString("dd-MMM") : "");
                celda.CellStyle = font;
                celda = row.CreateCell(6);
                celda.SetCellValue(muestra.Embarque.FechaRecalada.HasValue ? 
                    muestra.Embarque.FechaRecalada.Value.ToString("dd/MM/yyyy ") + 
                        (String.IsNullOrEmpty(muestra.Embarque.HoraRecalada) ? "" : 
                            muestra.Embarque.HoraRecalada.ToString() + (muestra.Embarque.HoraRecalada.Length > 2 ? " Hrs": "")) : "");
                celda.CellStyle = font;
                celda = row.CreateCell(7);
                //celda.SetCellValue(muestra.Embarque.Ubicacion.ToString());
                //celda.CellStyle = font;
                //celda = row.CreateCell(8);
                celda.SetCellValue(muestra.Embarque.Observaciones);
                celda.CellStyle = font;
                i++;
            }
        }

        private static int InsertTableHeader(HSSFSheet sheet, ICellStyle cellBorderStyleColumnTitles, int i)
        {
            var row = sheet.CreateRow(i++);
            row = sheet.CreateRow(i++);
            var celda =  row.CreateCell(0);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda.SetCellValue("Buque");
            celda = row.CreateCell(1);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda.SetCellValue("Coordinador");
            celda = row.CreateCell(2);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda.SetCellValue("Agencia");
            celda = row.CreateCell(3);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda.SetCellValue("ATA");
            celda = row.CreateCell(4);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda.SetCellValue("Cantidades / Productos");
            celda = row.CreateCell(5);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda.SetCellValue("Oblig Carga");
            celda = row.CreateCell(6);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda.SetCellValue("Recalada");
            celda = row.CreateCell(7);
            //celda.CellStyle = cellBorderStyleColumnTitles;
            //celda.SetCellValue("Posición de buque");
            //celda = row.CreateCell(8);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda.SetCellValue("Observaciones");
            return i;
        }
    }
}