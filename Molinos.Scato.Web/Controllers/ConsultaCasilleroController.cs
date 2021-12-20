using System.Collections.Generic;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ConsultaCasillero)]
    public class ConsultaCasilleroController : BaseController
    {
        private readonly ILogger log;

        public ConsultaCasilleroController(ILogger log, IServicioRepositorio servicio)
            : base(servicio)
        {
            this.log = log;
        }

        public ActionResult Antiguedad()
        {
            ViewBag.Items = new ListaPaginada<ConsultaCasilleroAntiguedadDto>(new List<ConsultaCasilleroAntiguedadDto>(), 1, 1, 0);

            return View(new ConsultaCasilleroAntiguedadDto());
        }

        [AjaxOnly]
        public ActionResult ListarAntiguedad(ConsultaCasilleroAntiguedadDto filtro, int pagina = 1, string ordenarPor = "Centro", DirOrden dirOrden = DirOrden.Asc)
        {
            if (ModelState.IsValid)
            {
                ListAntiguedadQuery(filtro, pagina, ordenarPor, dirOrden);
            }
            else
            {
                ViewBag.Items = new ListaPaginada<ConsultaCasilleroAntiguedadDto>(new List<ConsultaCasilleroAntiguedadDto>(), 1, 1, 0);
            }
            return View("ListarAntiguedad", filtro);
        }

        private void ListAntiguedadQuery(ConsultaCasilleroAntiguedadDto filtro, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(
                ordenarPor,
                dirOrden,
                pagina,
                10);

            filtro.DiasDeAntiguedad = filtro.DiasDeAntiguedad.Replace("_", "");

            ViewBag.Items = servicio.ListarConsultaCasillerosPorAntiguedad(filtro, paginacion);
        }

        public ActionResult Casillero()
        {
            ViewBag.Items = new ListaPaginada<ConsultaCasilleroDto>(new List<ConsultaCasilleroDto>(), 1, 1, 0);

            return View(new ConsultaCasilleroDto());
        }

        [DatosUsuario]
        [AjaxOnly]
        public ActionResult ListarCasillero(DatosUsuario datosUsuario, ConsultaCasilleroDto filtro, int pagina = 1, string ordenarPor = "NDeCasillero", DirOrden dirOrden = DirOrden.Asc)
        {
            if (ModelState.IsValid)
            {
                ListCasilleroQuery(filtro, pagina, ordenarPor, dirOrden, datosUsuario.CentroId);
            }
            else
            {
                ViewBag.Items = new ListaPaginada<ConsultaCasilleroDto>(new List<ConsultaCasilleroDto>(), 1, 1, 0);
            }
            return View("ListarCasillero", filtro);
        }

        private void ListCasilleroQuery(ConsultaCasilleroDto filtro, int pagina, string ordenarPor, DirOrden dirOrden, int centroId)
        {
            var paginacion = new Paginacion(
                ordenarPor,
                dirOrden,
                pagina,
                10);

            filtro.CentroId = centroId;

            ViewBag.Items = servicio.ListarConsultaCasillerosPorCasillero(filtro, paginacion);
        }

        public ActionResult Muestra()
        {
            ViewBag.Items = new ListaPaginada<ConsultaCasilleroMuestraDto>(new List<ConsultaCasilleroMuestraDto>(), 1, 1, 0);

            return View(new ConsultaCasilleroMuestraDto());
        }

        [AjaxOnly]
        public ActionResult ListarMuestra(ConsultaCasilleroMuestraDto filtro, int pagina = 1, string ordenarPor = "NumeroDocumento", DirOrden dirOrden = DirOrden.Asc)
        {
            if (ModelState.IsValid)
            {
                ListMuestraQuery(filtro, pagina, ordenarPor, dirOrden);
            }
            else
            {
                ViewBag.Items = new ListaPaginada<ConsultaCasilleroMuestraDto>(new List<ConsultaCasilleroMuestraDto>(), 1, 1, 0);
            }
            return View("ListarMuestra", filtro);
        }

        private void ListMuestraQuery(ConsultaCasilleroMuestraDto filtro, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(
                ordenarPor,
                dirOrden,
                pagina,
                10);

            ViewBag.Items = servicio.ListarConsultaCasillerosPorMuestra(filtro, paginacion);
        }
    }
}
