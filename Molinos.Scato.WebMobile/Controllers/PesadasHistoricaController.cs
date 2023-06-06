using System;
using System.Web.Mvc;
using Molinos.Scato.Servicios;
using Ninject.Extensions.Logging;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.WebMobile.Atributos;
using Molinos.Scato.WebMobile.ViewModel;
using System.Globalization;
using Molinos.Scato.Dominio.Seguridad;

namespace Molinos.Scato.WebMobile.Controllers
{
    [Autorizacion(PermisosScato.ScatoPuerto)]
    public class PesadasHistoricaController : Controller
    {
        //
        // GET: /PesadasHistorica/
        private readonly IServicioRepositorio servicio;
        private readonly ILogger log;

        public PesadasHistoricaController(ILogger log, IServicioRepositorio servicio)
        {
            this.log = log;
            this.servicio = servicio;
        }
        public ActionResult Index(int pagina = 1, string ordenarPor = "Fecha", DirOrden dirOrden = DirOrden.Asc)
        {
            var model = new PesadaHistoricaViewModel();

            Listar(model, pagina, ordenarPor, dirOrden);

            return View(model);
        }

        public ActionResult Seleccionar(string IdCarga, string NumeroBalanza)
        {
            log.Info("Obteniendo Listado de Balanzadas de la carga con Id {0}  y  numero de balanza {1} ", IdCarga, NumeroBalanza);

            var paginacion = new Paginacion("Id", DirOrden.Asc, 1, 10);
            ViewBag.Items = servicio.ListarPaginadoBalanzadas(int.Parse(IdCarga), null, NumeroBalanza, null, paginacion);

            return View("DetalleCarga");
        }

        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(PesadaHistoricaViewModel filtros, int pagina = 1, string ordenarPor = "Fecha", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(filtros, pagina, ordenarPor, dirOrden);
            return View("Listar");
        }

        private void ListQuery(PesadaHistoricaViewModel filtros, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);

            var selectedStartDateTime = filtros.DateStart.Date + filtros.BeginTime;
            var selectedEndDateTime = filtros.DateFin.Date + filtros.EndTime;

            ViewBag.Items = servicio.ListarCargasHistoricas(selectedStartDateTime, selectedEndDateTime, paginacion);
        }
    }
}