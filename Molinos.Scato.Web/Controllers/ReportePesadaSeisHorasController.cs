using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ninject.Extensions.Logging;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Dominio.Helpers;
using System.Globalization;

namespace Molinos.Scato.Web.Controllers
{
    public class ReportePesadaSeisHorasController : BaseController
    {

        private readonly ILogger log;
        public ReportePesadaSeisHorasController(ILogger log, IServicioRepositorio servicio)
            : base(servicio)
        {
            this.log = log;

        }
        public ActionResult Index(int pagina = 1, string ordenarPor = "Fecha", DirOrden dirOrden = DirOrden.Asc)
        {
            CargarExportadores();
            CargarMateriales();
            var filtros = new FiltroReportePesadaSeisHoras
            {
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                TodosExportador = true,
                TodosMaterial = true
            };

            Listar(filtros, pagina, ordenarPor, dirOrden);

            return View(filtros);
        }
        public ActionResult Listar(FiltroReportePesadaSeisHoras filtros, int pagina = 1, string ordenarPor = "Fecha", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(filtros, pagina, ordenarPor, dirOrden);

            return View();
        }

        private void ListQuery(FiltroReportePesadaSeisHoras filtros, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 25);
            var selectedStartDateTime = filtros.FechaDesde.InicioDelDia();
            var selectedEndDateTime = filtros.FechaHasta.Date.FinDelDia();

            ViewBag.Items = servicio.ListarReporteDePesadasPorTurno(selectedStartDateTime, selectedEndDateTime, paginacion, filtros.Exportador_Id, filtros.Material_Id);
        }

        private void CargarExportadores()
        {
            var exportadores = servicio.ListaExportadores();
            ViewBag.Exportadores = exportadores.ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Nombre);
        }
        private void CargarMateriales()
        {
            var materiales = servicio.ListaMaterialesPuerto();
            ViewBag.Materiales = materiales.ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
        }
    }
}
