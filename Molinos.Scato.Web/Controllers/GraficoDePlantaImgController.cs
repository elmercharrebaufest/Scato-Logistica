using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Windows.Forms.DataVisualization.Charting;

namespace Molinos.Scato.Web.Controllers
{
    public class GraficoDePlantaImgController : Controller
    {
        private readonly IServicioComandos servicioComandos;
        private readonly IListaDeWorkflows listaDeWorkflows;
        private readonly IConfiguracionProvider configuracion;
        private readonly ILogger log;
        private readonly IServicioRepositorio servicio;

        public GraficoDePlantaImgController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos, IListaDeWorkflows listaDeWorkflows, IConfiguracionProvider configuracion)
        {
            this.servicioComandos = servicioComandos;
            this.listaDeWorkflows = listaDeWorkflows;
            this.configuracion = configuracion;
            this.log = log;
            this.servicio = servicio;
        }

        private List<EstadoMaterialDto> SetearCabezera(int centroId)
        {
            var materiales = servicio.ListarEstadoPlanta(centroId, true, true);
            var cupos = ListadoCupoController.ActualizarCuposOtorgados(servicio, servicioComandos, configuracion, centroId);

            var cupeados = cupos.Sum(x => x.Otorgados);
            ViewBag.Cupeados = cupeados;
            var arribados = cupos.Sum(x => x.ArribadosDia);
            ViewBag.Arribados = arribados;
            ViewBag.Descargados = cupos.Sum(x => x.Descargados);
            ViewBag.Cumplimiento = cupeados == 0 ? 0 : arribados * 100 / cupeados;
            return materiales;
        }

        public FileResult DescargarGraficoCamionesPorHora(int MaterialId=4, int centroId=5)
        {
            log.Debug("Obteniendo Datos Grafico Camiones Por Hora");
            GraficoCamionesHoraDto model = new GraficoCamionesHoraDto { MaterialId = MaterialId, FechaVieja = DateTime.Now.AddYears(-1) };
            var result = servicio.ObtenerGraficoDeCamionesPorHora(model, centroId);
            

            var materiales = SetearCabezera(centroId);

            string htmlstring = RenderViewToString("Cabecera", materiales);
            Image imagecabecerea = TheArtOfDev.HtmlRenderer.WinForms.HtmlRender.RenderToImage(htmlstring);


            Chart chart = new Chart();
            chart.BackColor = System.Drawing.ColorTranslator.FromHtml("#f7f7f7");
            chart.Width = 1920;
            chart.Height = 1080- imagecabecerea.Height;
            Title titulo = new Title
            {
                Font = new Font("Verdana", 20.25f, FontStyle.Bold),
                Text = "Camiones Por Hora"
            };
            chart.Titles.Add(titulo);


            Series series1 = new Series("Series1");
            series1.ChartArea = "ca1";
            series1.ChartType = SeriesChartType.Line;
            series1.Font = new Font("Verdana", 8.25f, FontStyle.Regular);
            series1.Legend = "Default";
            series1.LegendText = "Camiones Actuales";
            series1.IsVisibleInLegend = true;
            series1.Color = System.Drawing.ColorTranslator.FromHtml("#0070ff");

            Series series2 = new Series("Series2");
            series2.ChartArea = "ca1";
            series2.ChartType = SeriesChartType.Line;
            series2.Font = new Font("Verdana", 8.25f, FontStyle.Regular);
            series2.Legend = "Default";
            series2.LegendText = "Camiones Historico";
            series2.IsVisibleInLegend = true;
            series2.Color = System.Drawing.ColorTranslator.FromHtml("#ff5722");


            chart.Series.Add(series1);
            for (int i = 0; i < result.CamionesActuales.Length; i++)
            {
                chart.Series["Series1"].Points.AddY(double.Parse(result.CamionesActuales[i].ToString()));
            }
            chart.Series.Add(series2);
            for (int i = 0; i < result.CamionesHistorico.Length; i++)
            {
                chart.Series["Series2"].Points.AddY(double.Parse(result.CamionesHistorico[i].ToString()));
            }

            chart.Series["Series1"].ChartType = SeriesChartType.Line;
            chart.Series["Series1"].IsValueShownAsLabel = false;
            chart.Series["Series1"].BorderWidth = 3;
            chart.Series["Series1"].MarkerStyle = MarkerStyle.Square;


            chart.Series["Series2"].ChartType = SeriesChartType.Line;
            chart.Series["Series2"].IsValueShownAsLabel = false;
            chart.Series["Series2"].BorderWidth = 3;
            chart.Series["Series2"].MarkerStyle = MarkerStyle.Circle;
            Legend legend = new Legend("Default");
            chart.Legends.Add(legend);
            chart.Legends["Default"].BackColor = Color.Transparent;
            chart.Legends["Default"].BorderColor = Color.Black;
            chart.Legends["Default"].BorderWidth = 1;
            chart.Legends["Default"].BorderDashStyle = ChartDashStyle.Solid;
            chart.Legends["Default"].ShadowOffset = 1;
            chart.Legends["Default"].Docking = Docking.Top;
            chart.Legends["Default"].Font = new Font("Verdana", 14.25f, FontStyle.Regular);



            ChartArea ca1 = new ChartArea("ca1");
            ca1.BackColor = System.Drawing.ColorTranslator.FromHtml("#f7f7f7");
            chart.ChartAreas.Add(ca1);
            ca1.AxisX.Title = "Hora del día";
            ca1.AxisX.TitleFont = new Font("Verdana", 14.25f, FontStyle.Regular);
            ca1.AxisY.Title = "Cantidad";
            ca1.AxisY.TitleFont = new Font("Verdana", 14.25f, FontStyle.Regular);
            ca1.AxisX.MajorGrid.Interval = 1;
            ca1.AxisX.Interval = 1;
            ca1.AxisX.MajorGrid.LineColor = Color.LightGray;
            ca1.AxisY.MajorGrid.LineColor = Color.LightGray;

           
            Image imagenGrafico;

            using (var ms = new MemoryStream())
            {
                chart.SaveImage(ms, ChartImageFormat.Png);
                ms.Seek(0, SeekOrigin.Begin);
                imagenGrafico = Image.FromStream(ms);
            }
            Bitmap bitmap = new Bitmap(1920,imagecabecerea.Height+ imagenGrafico.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(imagecabecerea, 0, 0);
                g.DrawImage(imagenGrafico,  0,imagecabecerea.Height );
            }
            
            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Seek(0, SeekOrigin.Begin);

                return File(ms.ToArray(), "image/png", "GraficoCamionesPorHora.png");
            }

        }

        
        public string RenderViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
        [DatosUsuario]
        public FileResult DescargarGraficoCamionesPorDia(DatosUsuario datosUsuario, int MaterialId=4)
        {
            log.Debug("Obteniendo Datos Grafico Camiones Por Dia, usuario: {0}", datosUsuario.NombreUsuario);
            GraficoCamionesDiaDto model = new GraficoCamionesDiaDto { MaterialId = MaterialId };
            var result = servicio.ObtenerGraficoDeCamionesPorDia(model, datosUsuario.CentroId);
            
            Chart chart = new Chart();
            chart.BackColor = System.Drawing.ColorTranslator.FromHtml("#f7f7f7");
            chart.Width = 1920;
            chart.Height = 1080;
            Title titulo = new Title
            {
                Font = new Font("Verdana", 20.25f, FontStyle.Bold),
                Text = "Camiones Por Día"
            };
            chart.Titles.Add(titulo);


            Series series1 = new Series("Series1");
            series1.ChartArea = "ca1";
            series1.ChartType = SeriesChartType.Line;
            series1.Font = new Font("Verdana", 8.25f, FontStyle.Regular);
            series1.Legend = "Default";
            series1.LegendText = "Camiones Por Día";
            series1.IsVisibleInLegend = true;
            series1.Color = System.Drawing.ColorTranslator.FromHtml("#0070ff");

            chart.Series.Add(series1);
            for (int i = 0; i < result.CamionesDia.Length; i++)
            {
                chart.Series["Series1"].Points.AddY(double.Parse(result.CamionesDia[i].ToString()));
            }
            chart.Series["Series1"].ChartType = SeriesChartType.Line;
            chart.Series["Series1"].IsValueShownAsLabel = false;
            chart.Series["Series1"].BorderWidth = 3;
            chart.Series["Series1"].MarkerStyle = MarkerStyle.None;

            Legend legend = new Legend("Default");
            chart.Legends.Add(legend);
            chart.Legends["Default"].BackColor = Color.Transparent;
            chart.Legends["Default"].BorderColor = Color.Black;
            chart.Legends["Default"].BorderWidth = 1;
            chart.Legends["Default"].BorderDashStyle = ChartDashStyle.Solid;
            chart.Legends["Default"].ShadowOffset = 1;
            chart.Legends["Default"].Docking = Docking.Top;
            chart.Legends["Default"].Font = new Font("Verdana", 14.25f, FontStyle.Regular);



            ChartArea ca1 = new ChartArea("ca1");
            ca1.BackColor = System.Drawing.ColorTranslator.FromHtml("#f7f7f7");
            chart.ChartAreas.Add(ca1);
            ca1.AxisX.Title = "Días Atrás";
            ca1.AxisX.TitleFont = new Font("Verdana", 14.25f, FontStyle.Regular);
            ca1.AxisY.Title = "Cantidad";
            ca1.AxisY.TitleFont = new Font("Verdana", 14.25f, FontStyle.Regular);
            ca1.AxisX.MajorGrid.Interval = 1;
            ca1.AxisX.Interval = 1;
            ca1.AxisX.MajorGrid.LineColor = Color.LightGray;
            ca1.AxisY.MajorGrid.LineColor = Color.LightGray;

            using (var ms = new MemoryStream())
            {
                chart.SaveImage(ms, ChartImageFormat.Png);
                ms.Seek(0, SeekOrigin.Begin);

                return File(ms.ToArray(), "image/png", "GraficoCamionesPorDia.png");
            }
        }

        [DatosUsuario]
        public FileResult DescargarGraficoEficienciaHidraulicasPorCamiones(DatosUsuario datosUsuario, int MaterialId=4)
        {
            log.Debug("Obteniendo Datos Grafico Eficiencia Hidraulicas, usuario: {0}", datosUsuario.NombreUsuario);
            var model = new GraficoEficienciaHidraulicasDto { MaterialId = MaterialId, Fecha = DateTime.Now };
            var result = servicio.ObtenerGraficoDeEficienciaHidraulicas(model, datosUsuario.CentroId, configuracion.AppSettings["CodigoHidraulicaVagon"]);
            
            Chart chart = new Chart();
            chart.BackColor = System.Drawing.ColorTranslator.FromHtml("#f7f7f7");
            chart.Width = 1920;
            chart.Height = 1080;
            Title titulo = new Title
            {
                Font = new Font("Verdana", 20.25f, FontStyle.Bold),
                Text = "Eficiencia de las hidráulicas"
            };
            chart.Titles.Add(titulo);


            Series series1 = new Series("Series1");
            series1.ChartArea = "ca1";
            series1.ChartType = SeriesChartType.Column;
            series1.Font = new Font("Verdana", 8.25f, FontStyle.Regular);
            series1.Legend = "Default";
            series1.LegendText = "Cantidad Camiones Por Hidráulica";
            series1.IsVisibleInLegend = true;
            series1.Color = System.Drawing.ColorTranslator.FromHtml("#9be334");
            series1.IsValueShownAsLabel = false;
            series1.BorderWidth = 3;
            series1.MarkerStyle = MarkerStyle.None;
            series1.Font = new Font("Verdana", 14.25f, FontStyle.Regular);
            chart.Series.Add(series1);

            chart.Series["Series1"].Points.DataBindXY(result.CamionesPorHidraulicaMaterial.Select(x => x.Clave).ToArray(), result.CamionesPorHidraulicaMaterial.Select(x => x.Valor).ToArray());


            Legend legend = new Legend("Default");
            chart.Legends.Add(legend);
            chart.Legends["Default"].BackColor = Color.Transparent;
            chart.Legends["Default"].BorderColor = Color.Black;
            chart.Legends["Default"].BorderWidth = 1;
            chart.Legends["Default"].BorderDashStyle = ChartDashStyle.Solid;
            chart.Legends["Default"].ShadowOffset = 1;
            chart.Legends["Default"].Docking = Docking.Top;
            chart.Legends["Default"].Font = new Font("Verdana", 14.25f, FontStyle.Regular);



            ChartArea ca1 = new ChartArea("ca1");
            ca1.BackColor = System.Drawing.ColorTranslator.FromHtml("#f7f7f7");
            chart.ChartAreas.Add(ca1);
            ca1.AxisX.Title = "Hidráulica";
            ca1.AxisX.TitleFont = new Font("Verdana", 14.25f, FontStyle.Regular);
            ca1.AxisY.Title = "Cantidad";
            ca1.AxisY.TitleFont = new Font("Verdana", 14.25f, FontStyle.Regular);
            ca1.AxisX.MajorGrid.Interval = 1;
            ca1.AxisX.Interval = 1;
            ca1.AxisX.MajorGrid.LineColor = Color.LightGray;
            ca1.AxisY.MajorGrid.LineColor = Color.LightGray;

            using (var ms = new MemoryStream())
            {
                chart.SaveImage(ms, ChartImageFormat.Png);
                ms.Seek(0, SeekOrigin.Begin);

                return File(ms.ToArray(), "image/png", "GraficoEficienciaHidraulicasPorCamiones.png");
            }
        }

        [DatosUsuario]
        public FileResult DescargarGraficoEficienciaHidraulicasPorToneladas(DatosUsuario datosUsuario, int MaterialId=4)
        {
            log.Debug("Obteniendo Datos Grafico Eficiencia Hidraulicas, usuario: {0}", datosUsuario.NombreUsuario);
            var model = new GraficoEficienciaHidraulicasDto { MaterialId = MaterialId, Fecha = DateTime.Now };
            var result = servicio.ObtenerGraficoDeEficienciaHidraulicas(model, datosUsuario.CentroId, configuracion.AppSettings["CodigoHidraulicaVagon"]);
            
            Chart chart = new Chart();
            chart.BackColor = System.Drawing.ColorTranslator.FromHtml("#f7f7f7");
            chart.Width = 1920;
            chart.Height = 1080;
            Title titulo = new Title
            {
                Font = new Font("Verdana", 20.25f, FontStyle.Bold),
                Text = "Eficiencia de las hidráulicas"
            };
            chart.Titles.Add(titulo);


            Series series2 = new Series("Series2");
            series2.ChartArea = "ca1";
            series2.ChartType = SeriesChartType.Column;
            series2.Font = new Font("Verdana", 8.25f, FontStyle.Regular);
            series2.Legend = "Default";
            series2.LegendText = "Toneladas Por Hidráulica";
            series2.IsVisibleInLegend = true;
            series2.Color = System.Drawing.ColorTranslator.FromHtml("#ff5722");
            series2.IsValueShownAsLabel = false;
            series2.BorderWidth = 3;
            series2.MarkerStyle = MarkerStyle.None;
            series2.Font = new Font("Verdana", 14.25f, FontStyle.Regular);
            chart.Series.Add(series2);

            chart.Series["Series2"].Points.DataBindXY(result.CamionesPorHidraulicaMaterial.Select(x => x.Clave).ToArray(), result.CamionesPorHidraulicaMaterial.Select(x => x.Toneladas).ToArray());

            Legend legend = new Legend("Default");
            chart.Legends.Add(legend);
            chart.Legends["Default"].BackColor = Color.Transparent;
            chart.Legends["Default"].BorderColor = Color.Black;
            chart.Legends["Default"].BorderWidth = 1;
            chart.Legends["Default"].BorderDashStyle = ChartDashStyle.Solid;
            chart.Legends["Default"].ShadowOffset = 1;
            chart.Legends["Default"].Docking = Docking.Top;
            chart.Legends["Default"].Font = new Font("Verdana", 14.25f, FontStyle.Regular);



            ChartArea ca1 = new ChartArea("ca1");
            ca1.BackColor = System.Drawing.ColorTranslator.FromHtml("#f7f7f7");
            chart.ChartAreas.Add(ca1);
            ca1.AxisX.Title = "Hidráulica";
            ca1.AxisX.TitleFont = new Font("Verdana", 14.25f, FontStyle.Regular);
            ca1.AxisY.Title = "Cantidad";
            ca1.AxisY.TitleFont = new Font("Verdana", 14.25f, FontStyle.Regular);
            ca1.AxisX.MajorGrid.Interval = 1;
            ca1.AxisX.Interval = 1;
            ca1.AxisX.MajorGrid.LineColor = Color.LightGray;
            ca1.AxisY.MajorGrid.LineColor = Color.LightGray;

            using (var ms = new MemoryStream())
            {
                chart.SaveImage(ms, ChartImageFormat.Png);
                ms.Seek(0, SeekOrigin.Begin);

                return File(ms.ToArray(), "image/png", "GraficoEficienciaHidraulicasPorToneladas.png");
            }
        }

        [DatosUsuario]
        public FileResult DescargarGraficoToneladasPorRangoDeDia(DatosUsuario datosUsuario, int MaterialId = 4, int cantidadDeDias = 1)
        {
            log.Debug("Obteniendo Datos Grafico Toneladas por Rango de Dias, usuario: {0}", datosUsuario.NombreUsuario);
            if (cantidadDeDias == 0) cantidadDeDias = 1;
            var model = new GraficoToneladasRangoDeDiasDto
            {
                MaterialId = MaterialId,
                FechaDesde = DateTime.Now.AddDays(cantidadDeDias * -1),
                FechaHasta = DateTime.Now
            };
            var result = servicio.ObtenerGraficoToneladasPorRangoDeDias(model, datosUsuario.CentroId);

            Chart chart = new Chart();
            chart.BackColor = System.Drawing.ColorTranslator.FromHtml("#f7f7f7");
            chart.Width = 1920;
            chart.Height = 1080;
            Title titulo = new Title
            {
                Font = new Font("Verdana", 20.25f, FontStyle.Bold),
                Text = "Toneladas Por Dia"
            };
            chart.Titles.Add(titulo);

            List<string> listaHidraulicas = result.Toneladas.Select(a => a.Codigo).Distinct().ToList();

            var colores = new string[] { "#ff0000", "#8500ff", "#0400ff", "#1bff00", "#ccff00", "#ff0081", "#ff5e00", "#00ffff", "#000000", "#40bf96" };
            int i = 0;
            foreach (string codigo in listaHidraulicas)
            {
                var pesadasPorHidraulica = result.Toneladas.Where(a => a.Codigo == codigo).OrderBy(a => a.Orden).ToList();
                Series series = new Series("Series" + codigo);
                series.ChartArea = "ca1";
                series.ChartType = SeriesChartType.Line;
                series.Font = new Font("Verdana", 8.25f, FontStyle.Regular);
                series.Legend = "Default";
                series.LegendText = codigo;
                series.IsVisibleInLegend = true;
                series.Color = System.Drawing.ColorTranslator.FromHtml(colores[i]);

                chart.Series.Add(series);

                series.Points.DataBindXY(pesadasPorHidraulica.Select(x => x.Fecha).ToArray(), pesadasPorHidraulica.Select(x => x.Toneladas).ToArray());

                series.ChartType = SeriesChartType.Line;
                series.IsValueShownAsLabel = false;
                series.BorderWidth = 3;
                series.MarkerStyle = MarkerStyle.Circle;
                i++;
            }


            Legend legend = new Legend("Default");
            chart.Legends.Add(legend);
            chart.Legends["Default"].BackColor = Color.Transparent;
            chart.Legends["Default"].BorderColor = Color.Black;
            chart.Legends["Default"].BorderWidth = 1;
            chart.Legends["Default"].BorderDashStyle = ChartDashStyle.Solid;
            chart.Legends["Default"].ShadowOffset = 1;
            chart.Legends["Default"].Docking = Docking.Top;
            chart.Legends["Default"].Font = new Font("Verdana", 14.25f, FontStyle.Regular);



            ChartArea ca1 = new ChartArea("ca1");
            ca1.BackColor = System.Drawing.ColorTranslator.FromHtml("#f7f7f7");
            chart.ChartAreas.Add(ca1);
            ca1.AxisX.Title = "Hidráulica";
            ca1.AxisX.TitleFont = new Font("Verdana", 14.25f, FontStyle.Regular);
            ca1.AxisY.Title = "Toneladas Por Dia";
            ca1.AxisY.TitleFont = new Font("Verdana", 14.25f, FontStyle.Regular);
            ca1.AxisX.MajorGrid.Interval = 1;
            ca1.AxisX.Interval = 1;
            ca1.AxisX.MajorGrid.LineColor = Color.LightGray;
            ca1.AxisY.MajorGrid.LineColor = Color.LightGray;

            using (var ms = new MemoryStream())
            {
                chart.SaveImage(ms, ChartImageFormat.Png);
                ms.Seek(0, SeekOrigin.Begin);

                return File(ms.ToArray(), "image/png", "GraficoCamionesPorHora.png");
            }
        }
    }
}