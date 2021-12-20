using System.Collections.Generic;
using System.IO;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace Molinos.Scato.Web.EXCEL
{
    public class ExcelMovimientosDeTercerosIngresos
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
            celda.SetCellValue("Pref");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(1);
            celda.SetCellValue("Carta de porte");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(2);
            celda.SetCellValue("T.Tra");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(3);
            celda.SetCellValue("N.Vagon");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(4);
            celda.SetCellValue("Fecha CP");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(5);
            celda.SetCellValue("Fecha Venc");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(6);
            celda.SetCellValue("C.E.E. Nro");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(7);
            celda.SetCellValue("C.T.G.");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(8);
            celda.SetCellValue("Fecha CTG");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(9);
            celda.SetCellValue("CUIT Titular");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(10);
            celda.SetCellValue("Titular");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(11);
            celda.SetCellValue("N. Planta");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(12);
            celda.SetCellValue("CUIT Inter.");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(13);
            celda.SetCellValue("Intermediario");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(14);
            celda.SetCellValue("CUIT Rem. Com.");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(15);
            celda.SetCellValue("Remitente Comercial");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(16);
            celda.SetCellValue("Cod");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(17);
            celda.SetCellValue("Mercadería");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(18);
            celda.SetCellValue("T.Grano");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(19);
            celda.SetCellValue("Cosecha");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(20);
            celda.SetCellValue("Cal");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(21);
            celda.SetCellValue("Cod.");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(22);
            celda.SetCellValue("Procedencia");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(23);
            celda.SetCellValue("Provincia");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(24);
            celda.SetCellValue("C.Postal");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(25);
            celda.SetCellValue("K.B.Pr");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(26);
            celda.SetCellValue("K.T.Pr");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(27);
            celda.SetCellValue("K.N.Pr");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(28);
            celda.SetCellValue("Observaciones");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(29);
            celda.SetCellValue("CUIT Corre.");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(30);
            celda.SetCellValue("Corredor");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(31);
            celda.SetCellValue("CUIT Entreg");///////////////////////////////////////////
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(32);
            celda.SetCellValue("Entregador");///////////////////////////////////////////////
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(33);
            celda.SetCellValue("CUIT Destinatario");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(34);
            celda.SetCellValue("Destinatario");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(35);
            celda.SetCellValue("CUIT Destino");//////////////////////////////////////////////////
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(36);
            celda.SetCellValue("Destino");///////////////////////////////////////////////////////
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(37);
            celda.SetCellValue("Planta");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(38);
            celda.SetCellValue("CUIT Trans.");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(39);
            celda.SetCellValue("Transportista");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(40);
            celda.SetCellValue("Patente");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(41);
            celda.SetCellValue("PatAcopl");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(42);
            celda.SetCellValue("Km a rec");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(43);
            celda.SetCellValue("TarxTon");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(44);
            celda.SetCellValue("Cuit Chofer");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(45);
            celda.SetCellValue("Nombre Chofer");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(46);
            celda.SetCellValue("Fecha Arr.");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(47);
            celda.SetCellValue("H. Ar");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(48);
            celda.SetCellValue("Fecha Des.");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(49);
            celda.SetCellValue("H.De");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(50);
            celda.SetCellValue("Bruto");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(51);
            celda.SetCellValue("Tara");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(52);
            celda.SetCellValue("Neto");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(53);
            celda.SetCellValue("Merma");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(54);
            celda.SetCellValue("Neto Apl.");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(55);
            celda.SetCellValue("%Humed.");
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(56);
            celda.SetCellValue("Analisis");
            celda.CellStyle = cellBorderStyleColumnTitles;

            //Valores
            var i = 1;
            foreach (var muestra in muestras)
            {
                row = sheet.CreateRow(i);

                row.CreateCell(0).SetCellValue(muestra.Pref);
                row.CreateCell(1).SetCellValue(muestra.NroDocumento);
                row.CreateCell(2).SetCellValue(muestra.TipoVehiculo);
                row.CreateCell(3).SetCellValue(muestra.NumeroDeSecuencia);
                row.CreateCell(4).SetCellValue("");
                row.CreateCell(5).SetCellValue("");
                row.CreateCell(6).SetCellValue("");
                row.CreateCell(7).SetCellValue(muestra.Ctg);
                row.CreateCell(8).SetCellValue(muestra.FechaCtg);
                row.CreateCell(9).SetCellValue(muestra.TitularCartaPorte);
                row.CreateCell(10).SetCellValue(muestra.TitularCartaPorteDesc);
                row.CreateCell(11).SetCellValue("");
                row.CreateCell(12).SetCellValue("");
                row.CreateCell(13).SetCellValue("");
                row.CreateCell(14).SetCellValue(muestra.RtteComercial);
                row.CreateCell(15).SetCellValue(muestra.RtteComercialDesc);
                row.CreateCell(16).SetCellValue(muestra.CodEspecie);
                row.CreateCell(17).SetCellValue(muestra.MaterialDesc);
                row.CreateCell(18).SetCellValue("");
                row.CreateCell(19).SetCellValue(muestra.Cosecha);
                row.CreateCell(20).SetCellValue(muestra.Calidad);
                row.CreateCell(21).SetCellValue(muestra.Procedencia);
                row.CreateCell(22).SetCellValue(muestra.ProcedenciaDesc);
                row.CreateCell(23).SetCellValue(muestra.Provincia);
                row.CreateCell(24).SetCellValue("");
                row.CreateCell(25).SetCellValue(muestra.BrutoOrigen);
                row.CreateCell(26).SetCellValue(muestra.TaraOrigen);
                row.CreateCell(27).SetCellValue(muestra.NetoOrigen);
                row.CreateCell(28).SetCellValue("");
                row.CreateCell(29).SetCellValue(muestra.Corredor);
                row.CreateCell(30).SetCellValue(muestra.CorredorDesc);
                row.CreateCell(31).SetCellValue(""); //entregador
                row.CreateCell(32).SetCellValue("");
                row.CreateCell(33).SetCellValue(muestra.Destinatario);
                row.CreateCell(34).SetCellValue(muestra.DestinatarioDesc);
                row.CreateCell(35).SetCellValue(muestra.Destinatario);  //destino
                row.CreateCell(36).SetCellValue(muestra.DestinatarioDesc);
                row.CreateCell(37).SetCellValue("");
                row.CreateCell(38).SetCellValue(muestra.Transportista);
                row.CreateCell(39).SetCellValue(muestra.TransportistaDesc);
                row.CreateCell(40).SetCellValue(muestra.Patente1);
                row.CreateCell(41).SetCellValue(muestra.Patente2);
                row.CreateCell(42).SetCellValue(muestra.Kilometros);
                row.CreateCell(43).SetCellValue("");
                row.CreateCell(44).SetCellValue(muestra.ChoferDoc);
                row.CreateCell(45).SetCellValue(muestra.ChoferNombre);
                row.CreateCell(46).SetCellValue(muestra.FechaIngreso);
                row.CreateCell(47).SetCellValue(muestra.HoraIngreso);
                row.CreateCell(48).SetCellValue(muestra.FechaEgreso);
                row.CreateCell(49).SetCellValue(muestra.HoraEgreso);
                row.CreateCell(50).SetCellValue(muestra.PesoBruto);
                row.CreateCell(51).SetCellValue(muestra.PesoTara);
                row.CreateCell(52).SetCellValue(muestra.PesoNeto);
                row.CreateCell(53).SetCellValue("");
                row.CreateCell(54).SetCellValue(muestra.PesoNetoDescontado);
                row.CreateCell(55).SetCellValue(muestra.Humedad);
                row.CreateCell(56);
                celda.SetCellValue("");
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
