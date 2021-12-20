using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using Ninject.Extensions.Logging;
using Molinos.Scato.Servicios.ServicioImpresion;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorLibroMovimientosExistenciaDeGranosExcel : ProcesadorImprimirLibroMovimientosExistenciaGranos, IProcesadorComando<LibroMovimientosExistenciaGranosExcel>
    {
        public ProcesadorLibroMovimientosExistenciaDeGranosExcel(IRepositorio repositorio, IConversor conversor, ILogger log, IFirmaProvider firmaProvider, IServicioImpresion servicioImpresion)
            : base(repositorio, conversor, log, firmaProvider, servicioImpresion)
        {
        }

        protected override void GenerarArchivo(ResultadoPrevisualizar resultado, List<ImpImpresionGenericaDto> dtos, FormatoDeImpresionDto formatoDeImpresion, string direccionImpresora)
        {
            var workbook = GenerarExcel(dtos, formatoDeImpresion);
            resultado.Archivo = workbook;
        }

        private static byte[] GenerarExcel(List<ImpImpresionGenericaDto> dtos, FormatoDeImpresionDto formatoDeImpresion)
        {
            var workbook = new HSSFWorkbook();
            var sheet = (HSSFSheet)workbook.CreateSheet("Sheet1");

            var listas = dtos.Where(x => !x.EsTotalDia && !x.EsTotalMes).OrderBy(x => x.FechaEmision).ThenBy(o => o.Id);

            var ultimos = listas.GroupBy(x => x.FechaEmision.FinDelDia()).Select(s => s.Last()).ToList();
            
            var styleBold = workbook.CreateCellStyle();
            var fontBold = workbook.CreateFont();

            fontBold.Boldweight = (short)FontBoldWeight.Bold;
            styleBold.SetFont(fontBold);

            //header
            var cellBorderStyle = workbook.CreateCellStyle();
            cellBorderStyle.BorderBottom = BorderStyle.Thin;
            cellBorderStyle.BorderLeft = BorderStyle.Thin;
            cellBorderStyle.BorderTop = BorderStyle.Thin;
            cellBorderStyle.BorderRight = BorderStyle.Thin;

            var row = sheet.CreateRow(0);
            var celda = row.CreateCell(1);
            celda.SetCellValue("Grano");
            celda.CellStyle = styleBold;

            celda = row.CreateCell(2);
            celda.SetCellValue("Cod.Nro");
            celda.CellStyle = styleBold;

            row = sheet.CreateRow(3);
            celda = row.CreateCell(1);
            celda.SetCellValue("Comprobante");
            celda.CellStyle = cellBorderStyle;
            celda = row.CreateCell(2);
            celda.CellStyle = cellBorderStyle;
            CellRangeAddress merge = new CellRangeAddress(3,3,1,2);
            sheet.AddMergedRegion(merge);

            celda = row.CreateCell(7);
            celda.SetCellValue("Ingresos");
            celda.CellStyle = cellBorderStyle;
            celda = row.CreateCell(8);
            celda.CellStyle = cellBorderStyle;
            merge = new CellRangeAddress(3,3,7,8);
            sheet.AddMergedRegion(merge);

            celda = row.CreateCell(9);
            celda.SetCellValue("Egresos");
            celda.CellStyle = cellBorderStyle;

            celda = row.CreateCell(10);
            celda.SetCellValue("Saldos");
            celda.CellStyle = cellBorderStyle;

            row = sheet.CreateRow(1);
            var cellNumber = 1;
            foreach (var f in formatoDeImpresion.FormatosDeCampo.Where(f => !(f.EsColumna)))
            {
                var valor = string.IsNullOrEmpty(f.Texto) ? dtos.First().GetValueOrDefault(f.CampoDireccion) : f.Texto;
                celda = row.CreateCell(cellNumber);
                celda.SetCellValue(!String.IsNullOrEmpty(valor) ? valor : "");
                celda.CellStyle = styleBold;
                cellNumber++;
            }

            

            //titulos
            row = sheet.CreateRow(4);
            cellNumber = 0;
            bool imprimioCuitDestinatario = false;
            bool imprimioDestinatario = false;
            foreach (var f in formatoDeImpresion.FormatosDeCampo.Where(f => f.EsColumna).OrderBy(f => f.Columna))
            {
                var campo = f.TituloOncca;

                if (((f.CampoDireccion == "TitularCP" || f.CampoDireccion == "Destinatario") && imprimioDestinatario) || ((f.CampoDireccion == "CuitTitularCP" || f.CampoDireccion == "CuitDestinatario") && imprimioCuitDestinatario))
                {
                    continue;
                }

                if (f.CampoDireccion == "PesoNeto" && f.TipoDeCampo == TipoDeCampo.Ingreso)
                {
                    campo = "Kilos Brutos";
                }
                if (f.CampoDireccion == "PesoNeto" && f.TipoDeCampo == TipoDeCampo.Egreso)
                {
                    campo = "Kilos Netos";
                }               
                if (f.CampoDireccion == "CTG" )
                {
                    campo = "CTG AFIP";
                }
                if (f.CampoDireccion == "CTGSap")
                {
                    campo = "CTG SAP";
                }

                var cell = row.CreateCell(cellNumber);
                cell.SetCellValue(!String.IsNullOrEmpty(campo) ? campo : "");
                cell.CellStyle = cellBorderStyle;
                cellNumber++;

                if ((f.CampoDireccion == "TitularCP" || f.CampoDireccion == "Destinatario"))
                {
                    imprimioDestinatario = true;
                }
                if ((f.CampoDireccion == "CuitTitularCP" || f.CampoDireccion == "CuitDestinatario"))
                {
                    imprimioCuitDestinatario = true;
                }
            }

            //valores
            var rowNumber = 5;
            foreach (var dto in listas)
            {
                row = sheet.CreateRow(rowNumber);
                cellNumber = 0;
                imprimioCuitDestinatario = false;
                imprimioDestinatario = false;
                foreach (var f in formatoDeImpresion.FormatosDeCampo.Where(f => f.EsColumna).OrderBy(f => f.Columna))
                {
                    if ((dto.TipoDeWorkflow == TipoDeWorkflow.Egreso && f.TipoDeCampo == TipoDeCampo.Egreso) || 
                            (dto.TipoDeWorkflow == TipoDeWorkflow.Ingreso && f.TipoDeCampo == TipoDeCampo.Ingreso) ||
                            f.TipoDeCampo == TipoDeCampo.Siempre)
                    {
                        var valor = string.IsNullOrEmpty(f.Texto) ? dto.GetValueOrDefault(f.CampoDireccion) : f.Texto;
                        var cell = row.CreateCell(cellNumber);
                        cell.SetCellValue(!String.IsNullOrEmpty(valor) ? valor : "");
                        if (ultimos.Any(x => x == dto))
                        {
                            cell.CellStyle = styleBold;
                        }
                    }

                    if (((f.CampoDireccion == "TitularCP" || f.CampoDireccion == "Destinatario") && imprimioDestinatario) || ((f.CampoDireccion == "CuitTitularCP" || f.CampoDireccion == "CuitDestinatario") && imprimioCuitDestinatario))
                    {
                        continue;
                    }

                    if (f.CampoDireccion != "TitularCP" && f.CampoDireccion != "Destinatario" &&
                        f.CampoDireccion != "CuitTitularCP" && f.CampoDireccion != "CuitDestinatario")
                    {
                        cellNumber++;
                    }
                    
                    if ((f.CampoDireccion == "TitularCP" || f.CampoDireccion == "Destinatario") && ((dto.TipoDeWorkflow == TipoDeWorkflow.Egreso && f.TipoDeCampo == TipoDeCampo.Egreso) ||
                            (dto.TipoDeWorkflow == TipoDeWorkflow.Ingreso && f.TipoDeCampo == TipoDeCampo.Ingreso) ||
                            f.TipoDeCampo == TipoDeCampo.Siempre))
                    {
                        cellNumber++;
                        imprimioDestinatario = true;
                    }
                    if ((f.CampoDireccion == "CuitTitularCP" || f.CampoDireccion == "CuitDestinatario") && ((dto.TipoDeWorkflow == TipoDeWorkflow.Egreso && f.TipoDeCampo == TipoDeCampo.Egreso) ||
                            (dto.TipoDeWorkflow == TipoDeWorkflow.Ingreso && f.TipoDeCampo == TipoDeCampo.Ingreso) ||
                            f.TipoDeCampo == TipoDeCampo.Siempre))
                    {
                        imprimioCuitDestinatario = true;
                        cellNumber++;
                    }
                }
                rowNumber++;
            }
            using (var fileData = new MemoryStream())
            {
                workbook.Write(fileData);
                return fileData.ToArray();
            }
        }

        public Resultado Ejecutar(LibroMovimientosExistenciaGranosExcel comando)
        {
            return base.Ejecutar(comando);
        }
    }
}
