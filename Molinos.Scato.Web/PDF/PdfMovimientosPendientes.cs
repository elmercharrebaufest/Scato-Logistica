using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Helpers;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using StackExchange.Profiling;

namespace Molinos.Scato.Web.PDF
{
    public class PdfMovimientosPendientes : ActionResult
    {
        private readonly string fileName;
        private readonly List<MovimientoDeTercerosListaDto> movimientos;
        private readonly IFirmaProvider firma;

        public PdfMovimientosPendientes(string fileName, List<MovimientoDeTercerosListaDto> movimientos, IFirmaProvider firma)
        {
            this.fileName = fileName;
            this.movimientos = movimientos;
            this.firma = firma;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            using (MiniProfiler.Current.Step("ExecutePDFResult"))
            {
                var viewContext = new ViewContext(
                    context,
                    new NullView(),
                    context.Controller.ViewData,
                    context.Controller.TempData,
                    context.HttpContext.Response.Output);

                context.HttpContext.Response.ContentType = "application/pdf";
                context.HttpContext.Response.AppendHeader("content-disposition",
                                                          string.Format("attachment; filename=" + fileName));

                Render(viewContext);

                var newToken = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                context.HttpContext.Response.AppendCookie(new HttpCookie("fileDownloadToken", newToken));
            }
        }

        private class NullView : IView
        {
            public void Render(ViewContext viewContext, TextWriter writer) { }
        }

        private void Render(ViewContext viewContext)
        {
            var memory = new MemoryStream();

            // Create a MigraDoc document
            Document document = CreateDocument();

            MigraDoc.DocumentObjectModel.IO.DdlWriter.WriteToFile(document, "MigraDoc.mdddl");

            var renderer = new PdfDocumentRenderer(true, PdfFontEmbedding.Always) { Document = document };
            using (MiniProfiler.Current.Step("Render Document"))
            {
                renderer.RenderDocument();
            }

            using (MiniProfiler.Current.Step("Saving Document"))
            {
                renderer.PdfDocument.Save(memory, false);
            }
            memory.CopyTo(viewContext.HttpContext.Response.OutputStream);
        }


        private Document CreateDocument()
        {
            // Create a new MigraDoc document
            var document = new Document();
            DefineStyles(document);
            DefineContentSection(document);
            DefineTables(document);
            return document;
        }

        private void DefineTables(Document document)
        {
            var table = new Table();
            table.Style = "Table";
            table.AddColumn(Unit.FromCentimeter(5.5)).Format.Alignment = ParagraphAlignment.Center;  //Producto
            table.AddColumn(Unit.FromCentimeter(6)).Format.Alignment = ParagraphAlignment.Center;  //Vendedor
            table.AddColumn(Unit.FromCentimeter(5)).Format.Alignment = ParagraphAlignment.Center;  //Corredor
            table.AddColumn(Unit.FromCentimeter(1.5)).Format.Alignment = ParagraphAlignment.Center;  //Descarga
            table.AddColumn(Unit.FromCentimeter(1)).Format.Alignment = ParagraphAlignment.Center;  //Kilos
            table.AddColumn(Unit.FromCentimeter(2.5)).Format.Alignment = ParagraphAlignment.Center;  //Muestra
            table.AddColumn(Unit.FromCentimeter(3.5)).Format.Alignment = ParagraphAlignment.Center;  //Localidad
            table.AddColumn(Unit.FromCentimeter(0.5)).Format.Alignment = ParagraphAlignment.Center;  //Blanco
            table.AddColumn(Unit.FromCentimeter(1.5)).Format.Alignment = ParagraphAlignment.Center;  //Patente

            var row = table.AddRow();

            row.HeadingFormat = true;

            row.Cells[0].AddParagraph(Textos.Lote_FechaDescarga);
            row.Cells[1].AddParagraph(Textos.TipoDocumentoIngreso);
            row.Cells[2].AddParagraph(Textos.NumeroDocumentoIngreso);
            row.Cells[3].AddParagraph(Textos.Lote_Patente);
            row.Cells[4].AddParagraph(Textos.Material);
            row.Cells[5].AddParagraph(Textos.PesoNeto);
            row.Cells[6].AddParagraph(Textos.Transportista);
            row.Cells[7].AddParagraph(Textos.Destino);
            row.Format = new ParagraphFormat { Alignment = ParagraphAlignment.Left };
            row.Borders.Bottom = new Border { Width = new Unit(0.75) };

            foreach (var muestra in movimientos)
            {
                row = table.AddRow();
                AddTextToCell(muestra.FechaDescarga.FormattedTime(), row.Cells[0], document);
                AddTextToCell(muestra.TipoDocumentoIngreso.DisplayText(), row.Cells[1], document);
                AddTextToCell(muestra.NroCartaPorteConFormato, row.Cells[2], document);
                AddTextToCell(muestra.Patente, row.Cells[3], document);
                AddTextToCell(muestra.Material, row.Cells[4], document);
                AddTextToCell((muestra.PesoNeto.HasValue ? muestra.PesoNeto.Value.ToString(CultureInfo.InvariantCulture) : ""), row.Cells[5], document);
                AddTextToCell(muestra.Transportista, row.Cells[6], document);
                AddTextToCell(muestra.CentroDestino, row.Cells[7], document);

            }
            document.LastSection.Add(table);
        }

        private void AddTextToCell(string instring, Cell cell, Document document)
        {
            PdfDocument pdfd = new PdfDocument();
            PdfPage pg = pdfd.AddPage();
            XGraphics oGfx = XGraphics.FromPdfPage(pg);
            Unit maxWidth = cell.Column.Width - (cell.Column.LeftPadding + cell.Column.RightPadding);
            Paragraph par;
            var font = new XFont(document.Styles["Table"].Font.Name, document.Styles["Table"].Font.Size);


            if (string.IsNullOrEmpty(instring))
            {
                par = cell.AddParagraph(string.Empty);
            }
            else if (oGfx.MeasureString(instring, font).Width < maxWidth.Value || instring.IndexOf(" ", StringComparison.Ordinal) != -1)
            {
                par = cell.AddParagraph(instring);
            }
            else // String does not fit - start the truncation process...
            {
                int stringlength = instring.Length;
                for (int i = 0; i < 3; i++)
                {
                    if (oGfx.MeasureString(instring.Substring(0, stringlength), font).Width > maxWidth.Value)
                    {
                        stringlength -= (int)Math.Ceiling(instring.Length * Math.Pow(0.5f, i));
                    }
                    else if (i < 2)
                    {
                        stringlength += (int)Math.Ceiling(instring.Length * Math.Pow(0.5f, i));
                    }
                }
                par = cell.AddParagraph(Regex.Replace(instring, ".{" + stringlength + "}", "$0 "));
            }
            par.Format.Font.Size = document.Styles["Table"].Font.Size;
            par.Format.Alignment = ParagraphAlignment.Left;
        }

        private void DefineStyles(Document document)
        {
            // Get the predefined style Normal.
            Style style = document.Styles["Normal"];
            style.Font.Name = "Times New Roman";
            style.Font.Size = 12;

            style = document.Styles["Heading1"];
            style.Font.Size = 16;
            style.Font.Bold = true;
            style.ParagraphFormat.PageBreakBefore = false;

            style = document.Styles["Heading2"];
            style.Font.Size = 14;
            style.Font.Bold = true;
            style.ParagraphFormat.PageBreakBefore = false;


            style = document.AddStyle("FirstColumn", "Normal");
            style.ParagraphFormat.Alignment = ParagraphAlignment.Left;
            style.Font.Bold = true;

            style = document.AddStyle("HeaderColumn", "Normal");
            style.ParagraphFormat.Alignment = ParagraphAlignment.Center;
            style.Font.Bold = true;

            style = document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("17cm", TabAlignment.Left);
            style.ParagraphFormat.Font.Size = 10;
            style.ParagraphFormat.SpaceAfter = "-1.1cm";

            style = document.AddStyle("Subtitle", "Heading2");
            style.Font.Size = 12;
            style.ParagraphFormat.LeftIndent = "1.5cm";
            style.Font.Bold = false;

            style = document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

            style = document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

            // Create a new style called Table based on style Normal
            style = document.Styles.AddStyle("Table", "Normal");
            style.Font.Size = 8;
        }

        private void DefineContentSection(Document document)
        {
            var section = document.AddSection();
            section.PageSetup.StartingNumber = 1;
            section.PageSetup.LeftMargin = "1.5cm";
            section.PageSetup.RightMargin = "1.5cm";
            section.PageSetup.Orientation = Orientation.Landscape;

            var paragraph = section.Headers.Primary.AddParagraph();
            paragraph.AddText("Fecha:");
            paragraph.AddDateField("dd/MM/yyyy");
            paragraph.AddLineBreak();
            paragraph.AddText("Hoja:");
            paragraph.AddPageField();
            paragraph.Format.LeftIndent = "23cm";
            section.Headers.EvenPage.Add(paragraph.Clone());

            TextFrame frame = section.Headers.Primary.AddTextFrame();
            frame.Width = "12cm";
            frame.Height = "0.8cm";
            frame.Left = "8.1cm";
            frame.Top = "-0.87cm";
            frame.MarginTop = "-0.87cm";
            paragraph = frame.AddParagraph(string.Format(Textos.MovimientosPendientes));
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.Format.Shading = new Shading { Color = Colors.LightGray };
            paragraph.Format.SpaceAfter = "0.5cm";
            paragraph.Style = "Heading1";
            section.Headers.EvenPage.Add(frame.Clone());


            
            var image = section.Headers.Primary.AddImage("base64:" + Convert.ToBase64String(firma.ObtenerLogo()));
            image.Height = "0.9cm";
            image.Width = "5.6cm";
            image.Top = "-1.3cm";
            image.LockAspectRatio = true;
            image.RelativeVertical = RelativeVertical.Margin;
            image.RelativeHorizontal = RelativeHorizontal.Margin;
            image.WrapFormat.Style = WrapStyle.Through;
            section.Headers.EvenPage.Add(image.Clone());
        }




















    }
}
