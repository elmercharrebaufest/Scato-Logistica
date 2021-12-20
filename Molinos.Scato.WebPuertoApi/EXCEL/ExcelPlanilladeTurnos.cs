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
    public class ExcelLiquido
    {
        public void GenerarArchivo(ResultadoPrevisualizar resultado, ModuloDeCargaPlanillaDeTurnosDto turnos, int IdModuloDeCarga)
        {
            var workbook = GenerarExcel(turnos, IdModuloDeCarga);
            resultado.Archivo = workbook;
        }

        private static byte[] GenerarExcel(ModuloDeCargaPlanillaDeTurnosDto turnos, int IdModuloDeCarga)
        {
            var workbook = new HSSFWorkbook();
            var sheet = (HSSFSheet)workbook.CreateSheet("Planilla de Turnos");
            //var stylebold = workbook.CreateCellStyle();   
            //stylebold.SetFont(fontBold);

            ICellStyle estiloHeaderTabla = EstiloHeaderTabla(workbook);
            ICellStyle estiloNombreBuque = EstiloNombreBuque(workbook);
            ICellStyle estiloDate = EstiloDate(workbook);
            ICellStyle estiloContenido = EstiloContenido(workbook);
            ICellStyle estiloContenidoBold = EstiloContenidoBold(workbook);
            ICellStyle estiloCabeceraCorte = EstiloCabeceraCorte(workbook);
            ICellStyle estiloRegion = EstiloRegion(workbook);
            ICellStyle estiloRegionTurnos = EstiloRegionTurnos(workbook);
            //Valores
            var encabezado = false;
            var i = 1;
          
             var row = sheet.CreateRow(i);
            InsertTableHeaderTurnos(sheet, estiloHeaderTabla, i);
            InsertRowsTurnos(sheet, ref i, turnos, estiloContenido, estiloContenidoBold, estiloRegion, estiloRegionTurnos, estiloHeaderTabla, estiloCabeceraCorte);
            

            sheet.SetColumnWidth(0, (int)Math.Floor(30m * 256)); 
            sheet.SetColumnWidth(1, (int)Math.Floor(15m * 256)); 
            sheet.SetColumnWidth(2, (int)Math.Floor(15m * 256)); 
            sheet.SetColumnWidth(3, (int)Math.Floor(15m * 256)); 
            sheet.SetColumnWidth(4, (int)Math.Floor(40m * 256)); 
            sheet.SetColumnWidth(5, (int)Math.Floor(10m * 256)); 
            sheet.SetColumnWidth(6, (int)Math.Floor(18m * 256)); 
            sheet.SetColumnWidth(7, (int)Math.Floor(40m * 256));
            sheet.SetColumnWidth(8, (int)Math.Floor(40m * 256));
            sheet.SetColumnWidth(9, (int)Math.Floor(40m * 256));
            sheet.SetColumnWidth(10, (int)Math.Floor(40m * 256));
            sheet.SetColumnWidth(11, (int)Math.Floor(40m * 256));

            using (var fileData = new MemoryStream())
            {
                workbook.Write(fileData);
                return fileData.ToArray();
            }
        }

        private static ICellStyle EstiloRegion(HSSFWorkbook workbook)
        {
            var fontBold = workbook.CreateFont();
            fontBold.FontHeightInPoints = 12;
            fontBold.Boldweight = (short)FontBoldWeight.Bold;

            var cellBorderStyleColumnTitles = workbook.CreateCellStyle();
            /*cellBorderStyleColumnTitles.BorderBottom = BorderStyle.Medium;
            cellBorderStyleColumnTitles.BorderTop = BorderStyle.Medium;
            cellBorderStyleColumnTitles.BorderLeft = BorderStyle.Medium;
            cellBorderStyleColumnTitles.BorderRight = BorderStyle.Medium;*/
            cellBorderStyleColumnTitles.SetFont(fontBold);
            cellBorderStyleColumnTitles.FillForegroundColor = IndexedColors.LightOrange.Index;
            cellBorderStyleColumnTitles.FillPattern = FillPattern.SolidForeground;
            cellBorderStyleColumnTitles.Alignment = HorizontalAlignment.Center;
            cellBorderStyleColumnTitles.WrapText = true;
            return cellBorderStyleColumnTitles;
        }

        private static ICellStyle EstiloRegionTurnos(HSSFWorkbook workbook)
        {
            var fontBold = workbook.CreateFont();
            fontBold.FontHeightInPoints = 12;
            fontBold.Boldweight = (short)FontBoldWeight.Bold;

            var cellBorderStyleColumnTitles = workbook.CreateCellStyle();
            cellBorderStyleColumnTitles.SetFont(fontBold);
            cellBorderStyleColumnTitles.FillForegroundColor = IndexedColors.Yellow.Index;
            cellBorderStyleColumnTitles.FillPattern = FillPattern.SolidForeground;
            cellBorderStyleColumnTitles.Alignment = HorizontalAlignment.Center;
            cellBorderStyleColumnTitles.WrapText = true;
            return cellBorderStyleColumnTitles;
        }

        private static ICellStyle EstiloCabeceraCorte(HSSFWorkbook workbook)
        {
            var fontBold = workbook.CreateFont();
            fontBold.FontHeightInPoints = 13;
            fontBold.Boldweight = (short)FontBoldWeight.Bold;
            

            var cellBorderStyleColumnTitles = workbook.CreateCellStyle();
          
            cellBorderStyleColumnTitles.SetFont(fontBold);
            cellBorderStyleColumnTitles.FillForegroundColor = IndexedColors.Red.Index;
            
            cellBorderStyleColumnTitles.FillPattern = FillPattern.SolidForeground;
            cellBorderStyleColumnTitles.Alignment = HorizontalAlignment.Center;
            cellBorderStyleColumnTitles.WrapText = true;
            return cellBorderStyleColumnTitles;
        }
        

        private static ICellStyle EstiloHeaderTabla(HSSFWorkbook workbook)
        {
            var fontBold = workbook.CreateFont();
            fontBold.FontHeightInPoints = 11;
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

        private static ICellStyle EstiloNombreBuque(HSSFWorkbook workbook)
        {
            var fontBold = workbook.CreateFont();
            fontBold.FontHeightInPoints = 18;
            fontBold.Boldweight = (short)FontBoldWeight.Bold;

            var cellBorderStyleColumnTitles = workbook.CreateCellStyle();
            cellBorderStyleColumnTitles.SetFont(fontBold);
            cellBorderStyleColumnTitles.Alignment = HorizontalAlignment.Center;
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

        private static void InsertRowsTurnos(HSSFSheet sheet, ref int i,  ModuloDeCargaPlanillaDeTurnosDto turnosPlanilla, ICellStyle font, ICellStyle fontBold, ICellStyle estiloRegion, ICellStyle estiloRegionTurnos, ICellStyle estiloHeaderTabla, ICellStyle estiloCabeceraCorte)
        {
            
            bool crearfecha = true;
            bool crearTurno = true;

            foreach (var planilla in turnosPlanilla.ModuloDeCargaPlanillaDeTurnosTurnos)
            {
             
                foreach (var turnoPlanilla in planilla.ModuloDeCargaPlanillaDeTurnosTurnosDetalles)
                {
                    var turno = planilla.TurnoPuerto.Nombre;
                    var fecha = turnosPlanilla.Fecha;
                    
                    var row = sheet.CreateRow(i);
                    var celda = row.CreateCell(0);
                    
                    if (crearfecha)
                    {
                        celda.SetCellValue(fecha.ToString());
                        crearfecha = false;
                        
                    }
                    celda.CellStyle = estiloRegionTurnos;
                    celda = row.CreateCell(1);
                    
                    
                    if (crearTurno)
                    {
                        celda.SetCellValue(planilla.TurnoPuerto.Nombre);
                        crearTurno = false;
                        
                    }
                    celda.CellStyle = estiloRegion;
                    celda = row.CreateCell(2);
                    celda.SetCellValue(turnoPlanilla.Exportador.Nombre);
                    celda.CellStyle = font;
                    celda = row.CreateCell(3);
                    celda.SetCellValue(turnoPlanilla.Linea);
                    celda.CellStyle = font;
                    celda = row.CreateCell(4);
                    celda.SetCellValue(turnoPlanilla.BodegaParcel);
                    celda.CellStyle = font;
                    celda = row.CreateCell(5);
                    celda.SetCellValue(turnoPlanilla.MaterialPuerto.Descripcion);
                    celda.CellStyle = font;
                    celda = row.CreateCell(6);
                    celda.SetCellValue(turnoPlanilla.Tk);
                    celda.CellStyle = font;
                    celda = row.CreateCell(7);
                    celda.SetCellValue(turnoPlanilla.Temperatura);
                    celda.CellStyle = font;
                    celda = row.CreateCell(8);
                    celda.SetCellValue(turnoPlanilla.MedidaInicialCM);
                    celda.CellStyle = font;
                    celda = row.CreateCell(9);
                    celda.SetCellValue(turnoPlanilla.MedidaFinalCM);
                    celda.CellStyle =  font;
                    celda = row.CreateCell(10);
                    celda.SetCellValue(turnoPlanilla.Destino.Nombre);
                    celda.CellStyle = font;
                    celda = row.CreateCell(11);
                    celda.SetCellValue(turnoPlanilla.Cantidad);
                    celda.CellStyle = font;
                    i++;
                }

                // sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, i, 0, 0));

                //sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, i, 1, 1));
                int x = i + 4;
                InsertTableHeaderCortes(sheet, estiloHeaderTabla,estiloRegionTurnos, estiloCabeceraCorte, x);
                foreach (var cortesPlanilla in planilla.ModuloDeCargaPlanillaDeTurnosTurnosCortes)
                {
                    var turno = planilla.TurnoPuerto.Nombre;
                    var fecha = turnosPlanilla.Fecha;

                    var row = sheet.CreateRow(x+1);
                    
                    var celda = row.CreateCell(0);
                    celda.SetCellValue(cortesPlanilla.MotivosDeCorte.Nombre);                    
                    celda.CellStyle = font;
                    
                    celda = row.CreateCell(1);
                    celda.SetCellValue(cortesPlanilla.HoraInicio);
                    celda.CellStyle = font;
                    celda = row.CreateCell(2);
                    celda.SetCellValue(cortesPlanilla.HoraFin);
                    celda.CellStyle = font;
                    celda = row.CreateCell(3);
                    celda.SetCellValue(cortesPlanilla.TiempoTotal);
                    celda.CellStyle = font;
                    celda = row.CreateCell(4);
                    celda.SetCellValue(cortesPlanilla.Observaciones);
                    celda.CellStyle = font;
                    x++;
                }
            }
        }
         
        private static int InsertTableHeaderTurnos(HSSFSheet sheet, ICellStyle cellBorderStyleColumnTitles, int i)
        {
            var row = sheet.CreateRow(0);
            var celda =  row.CreateCell(0);
            
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda.SetCellValue("Dia");
            celda = row.CreateCell(1);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda.SetCellValue("Turno");
            celda = row.CreateCell(2);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda.SetCellValue("Exportador");
            celda = row.CreateCell(3);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda.SetCellValue("Linea");
            celda = row.CreateCell(4);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda.SetCellValue("Partida");
            celda = row.CreateCell(5);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda.SetCellValue("Producto");
            celda = row.CreateCell(6);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda.SetCellValue("Tk");
            celda = row.CreateCell(7);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda.SetCellValue("°C");
            celda = row.CreateCell(8);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda.SetCellValue("Med.Ini.");
            celda = row.CreateCell(9);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda.SetCellValue("Med.Fin.");
            celda = row.CreateCell(10);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda.SetCellValue("Destino");
            celda = row.CreateCell(11);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda.SetCellValue("Cant.");
            return i;
        }

        private static int InsertTableHeaderCortes(HSSFSheet sheet, ICellStyle cellBorderStyleColumnTitles,ICellStyle estiloRegionTurnos,ICellStyle estiloCabeceraCorte, int i)
        {
            var row1 = sheet.CreateRow(i-1);
            var celda1 = row1.CreateCell(0);
            celda1.CellStyle = estiloCabeceraCorte;
           
            celda1 = row1.CreateCell(1);
            celda1.CellStyle = estiloCabeceraCorte;
            celda1 = row1.CreateCell(2);
            celda1.SetCellValue("CORTES");
            celda1.CellStyle = estiloCabeceraCorte;            
            celda1 = row1.CreateCell(3);
            celda1.CellStyle = estiloCabeceraCorte;
            celda1 = row1.CreateCell(4);
            celda1.CellStyle = estiloCabeceraCorte;
            //sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(i - 1, i - 1, 0, 4));

            var row = sheet.CreateRow(i);
            var celda = row.CreateCell(0);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda.SetCellValue("Motivo");
            celda = row.CreateCell(1);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda.SetCellValue("Inicio");
            celda = row.CreateCell(2);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda.SetCellValue("Fin");
            celda = row.CreateCell(3);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda.SetCellValue("Tiempo.Total");
            celda = row.CreateCell(4);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda.SetCellValue("Observaciones");
            return i;
        }
    }

  

}