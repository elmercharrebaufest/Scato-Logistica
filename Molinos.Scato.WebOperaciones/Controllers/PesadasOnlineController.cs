using System;
using System.Web.Mvc;
using Molinos.Scato.Servicios;
using Ninject.Extensions.Logging;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.WebOperaciones.Atributos;
using Molinos.Scato.WebOperaciones.ViewModel;
using System.Globalization;
using Molinos.Scato.Dominio.Seguridad;

namespace Molinos.Scato.WebOperaciones.Controllers
{
    [Autorizacion(PermisosScato.ScatoPuerto)]
    public class PesadasOnlineController : Controller
    {

        private readonly IServicioRepositorio servicio;
        private readonly ILogger log;

        public PesadasOnlineController(ILogger log, IServicioRepositorio servicio)
        {
            this.log = log;
            this.servicio = servicio;
        }
        public ActionResult Index(int pagina = 1, string ordenarPor = "Fecha", DirOrden dirOrden = DirOrden.Asc)
        {
            var filtros = new PesadaOnlineViewModel();
            filtros.DateNow = DateTime.Now;
            filtros.BeginTime = TimeSpan.Parse("00:00");
            filtros.EndTime = TimeSpan.Parse($"{filtros.DateNow.Hour.ToString("D2")}:{filtros.DateNow.Minute.ToString("D2")}");

            Listar(filtros, pagina, ordenarPor, dirOrden);

            return View(filtros);
        }

        public ActionResult Seleccionar(string idCarga, string numeroBalanza, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 50);
            ViewBag.Items = servicio.ListarPaginadoBalanzadas(int.Parse(idCarga), null, numeroBalanza, null, paginacion);

            return View("DetalleCarga");
        }

        [AjaxOnly]
        [ActionName("Seleccionar")]
        public ActionResult DetalleCargaListar(string IdCarga, string NumeroBalanza, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 50);
            ViewBag.Items = servicio.ListarPaginadoBalanzadas(int.Parse(IdCarga), null, NumeroBalanza, null, paginacion);

            return View("DetalleCargaListar");
        }

        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(PesadaOnlineViewModel filtros, int pagina = 1, string ordenarPor = "Fecha", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(filtros, pagina, ordenarPor, dirOrden);

            return View("Listar");
        }

        private void ListQuery(PesadaOnlineViewModel filtros, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 50);
            var selectedStartDateTime = filtros.DateNow.Date + filtros.BeginTime;
            var selectedEndDateTime = filtros.DateNow.Date + filtros.EndTime;

            ViewBag.Items = servicio.ListarCargasOnline(selectedStartDateTime, selectedEndDateTime, paginacion);
        }
    }
}
