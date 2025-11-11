using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.PanelDeControlTransaccionesVisec)]
    public class PanelDeControlTransaccionesVisecController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioHangfireQueue hangfireService;

        public PanelDeControlTransaccionesVisecController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos, IServicioHangfireQueue hangfireService)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
            this.hangfireService = hangfireService;
        }

        public ActionResult Index(int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Desc)
        {
            var filtro = new FiltroPanelDeTransaccionesVisecDto
            {
                FechaDesde = DateTime.Now.Date.AddDays(-7),
                FechaHasta = DateTime.Now.Date
            };

            ListQuery(filtro, pagina, ordenarPor, dirOrden);
            return View(filtro);
        }

        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(FiltroPanelDeTransaccionesVisecDto filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Desc)
        {
            ListQuery(filtro, pagina, ordenarPor, dirOrden);
            return View("_Listar", filtro);
        }

        [HttpPost]
        public ActionResult Transmitir(List<int> transmisiones)
        {
            foreach (var id in transmisiones)
            {
                var transmisionAVisec = servicio.ObtenerTransmisionAVisec(id);
                transmisionAVisec.Estado = Dominio.Enums.EstadoTransmisionAVisec.Pendiente;
                servicioComandos.Ejecutar(new CrearActualizarVisecTransmision { Dto = transmisionAVisec });
                hangfireService.EncolarImportarCartaPorteVisec(id);
            }
            return new ContentResult { Content = "OK" };
        }

        public ActionResult Modificar(int id)
        {
            var model = servicio.ObtenerTransmisionAVisec(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Modificar(VisecTransmisionDto model)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new CrearActualizarVisecTransmision { Dto = model });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            return View(model);
        }

        private void ListQuery(FiltroPanelDeTransaccionesVisecDto filtro, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(
                       ordenarPor,
                       dirOrden,
                       pagina,
                   10);

            ViewBag.Items = servicio.ListarTransmisionesAVisec(filtro, paginacion);
        }
    }
}