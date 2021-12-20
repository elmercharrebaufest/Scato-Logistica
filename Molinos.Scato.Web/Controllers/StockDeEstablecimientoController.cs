using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AbmStockDeEstablecimiento)]
    public class StockDeEstablecimientoController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public StockDeEstablecimientoController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        public ActionResult Index(string filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(filtro, pagina, ordenarPor, dirOrden);
            return View((object)filtro);
        }

        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(string filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(filtro, pagina, ordenarPor, dirOrden);
            return View("Listar", (object)filtro);
        }

        private void ListQuery(string filtro, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);

            ViewBag.Items = servicio.ListarPaginadoStockDeEstablecimientos(filtro, paginacion);
        }

        public ActionResult Crear()
        {
            return View();
        }
        [HttpPost]
        [DatosUsuario]
        public ActionResult Crear(StockDeEstablecimientoDto model, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new CrearStockDeEstablecimiento { Dto = model, Usuario = datosUsuario.NombreUsuario });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            return View(model);
        }

        public ActionResult Modificar(int id)
        {
            var establecimientoAModificar = servicio.ObtenerStockDeEstablecimiento(id);
            return View(establecimientoAModificar);

        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Modificar(StockDeEstablecimientoDto model, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new ModificarStockDeEstablecimiento { Dto = model, Usuario = datosUsuario.NombreUsuario });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            return View(model);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Eliminar(int id, DatosUsuario datosUsuario)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarStockDeEstablecimiento { Id = id, Usuario = datosUsuario.NombreUsuario });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }
    }
}
