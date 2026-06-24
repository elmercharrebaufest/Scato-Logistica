using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ReporteMoaStop)]
    public class ReporteMoaStopController : BaseController
    {
        private readonly ILogger log;

        public ReporteMoaStopController(ILogger log, IServicioRepositorio servicio)
            : base(servicio)
        {
            this.log = log;
        }

        public ActionResult Index()
        {
            var mensajes = servicio.MoaStopListarMensajes();
            ViewBag.Mensajes = mensajes;
            return View();
        }

        //[AjaxOnly]
        //[ActionName("Index")]
        public ActionResult Listar(FechaModel fechaModel, String patente, String mensaje, int page = 1)
        {
            
            if (fechaModel.FechaHasta < fechaModel.FechaDesde)
            {
                ModelState.AddModelError("", Textos.TarjetaRango_ErrorFechas);
            }

            int pageSize = 15;

            var items = new List<LogValidacionAccesoStopBandasHorariasDto>();

            if (ModelState.IsValid)
            {
                items = servicio.ListarLogValidacionAccesoStopBandasHorarias(
                    fechaModel.FechaDesde,
                    fechaModel.FechaHasta.AddDays(1).AddTicks(-1),
                    patente,
                    mensaje
                ).ToList();
            }

            // ✅ aunque falle ModelState
            var ordered = items
                .OrderByDescending(x => x.FechaIngreso)
                .ToList();

            int totalRegistros = ordered.Count;
            ViewBag.ItemsTotales = ordered;

            var itemsPaginados = ordered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Items = itemsPaginados;
            ViewBag.Page = page;
            ViewBag.MensajeSeleccionado = mensaje;
            ViewBag.Mensajes = servicio.MoaStopListarMensajes();
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRegistros / pageSize);
            
            return View("Listar");
        }

        public ActionResult ExportarExcelMoaStop(DateTime? fechaDesde, DateTime? fechaHasta, string patente, string mensaje)
        {
            // Ajuste de fecha hasta (mismo criterio que Listar)
            DateTime? hasta = null;

            if (fechaHasta.HasValue)
            {
                hasta = fechaHasta.Value.AddDays(1).AddTicks(-1);
            }

            var items = servicio.ListarLogValidacionAccesoStopBandasHorarias(
                fechaDesde,
                hasta,
                patente,
                mensaje
            );

            var sb = new System.Text.StringBuilder();

            // Cabecera
            sb.AppendLine("CTG;Patente;Fecha Ingreso;Mensaje");

            // Datos
            foreach (var item in items)
            {
                sb.AppendLine(string.Format("{0};{1};{2};{3}",
                    item.CTG,
                    item.Patente,
                    item.FechaIngreso.ToString("dd/MM/yyyy HH:mm"),
                    item.Mensaje.Replace(";", ",") // evitar romper columnas
                ));
            }

            return File(
                System.Text.Encoding.UTF8.GetBytes(sb.ToString()),
                "application/vnd.ms-excel",
                "ReporteMoaStop.csv"
            );
        }
    }
}