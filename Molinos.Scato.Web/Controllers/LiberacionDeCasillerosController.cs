using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.LiberacionDeCasilleros)]
    public class LiberacionDeCasillerosController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public LiberacionDeCasillerosController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario)
        {
            ViewBag.Items = new ListaPaginada<LiberacionDeCasillerosDto>(new List<LiberacionDeCasillerosDto>(), 1, 1, 0);
            return View(new LiberacionDeCasillerosDto { CentroId = datosUsuario.CentroId, Centro = datosUsuario.CentroDescripcion });
        }

        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(LiberacionDeCasillerosDto filtro, int pagina = 1, string ordenarPor = "NDeCasillero", DirOrden dirOrden = DirOrden.Asc)
        {
            if (ModelState.IsValid)
            {
                ListQuery(filtro, pagina, ordenarPor, dirOrden);
            }
            else
            {
                ViewBag.Items = new ListaPaginada<LiberacionDeCasillerosDto>(new List<LiberacionDeCasillerosDto>(), 1, 1, 0);
            }
            return View("Listar", filtro);
        }

        private void ListQuery(LiberacionDeCasillerosDto filtro, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(
                ordenarPor,
                dirOrden,
                pagina,
                10);

            filtro.DiasDeAntiguedad = string.IsNullOrEmpty(filtro.DiasDeAntiguedad) ? "" : filtro.DiasDeAntiguedad.Replace("_", "");

            ViewBag.Items = servicio.ListarLiberacionDeCasilleros(filtro, paginacion);
        }

        [HttpPost]
        public ActionResult LiberarCasilleros(string muestras)
        {
            var ids = muestras.Split('|');
            var resultado = new Resultado();

            foreach (var id in ids)
            {
                resultado = servicioComandos.Ejecutar(new EliminarMicroMuestrasPorCasillero() { Id = Convert.ToInt32(id) });
            }

            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

    }
}
