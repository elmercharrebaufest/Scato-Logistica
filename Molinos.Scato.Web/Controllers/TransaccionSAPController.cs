using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AbmTransaccionSAP)]
    public class TransaccionSAPController : BaseController
    {
        private readonly IServicioComandos servicioComandos;
        private ILogger log;

        public TransaccionSAPController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, string filtro, string ordenarPor = "DescripcionCorta", DirOrden dirOrden = DirOrden.Asc, int pagina = 1)
        {
            ListarConsulta(datosUsuario.CentroId, filtro, pagina, ordenarPor, dirOrden);
            return View();
        }

        [AjaxOnly]
        [DatosUsuario]
        [ActionName("Index")]
        public ActionResult Listar(DatosUsuario datosUsuario, string filtro, string ordenarPor = "DescripcionCorta", DirOrden dirOrden = DirOrden.Asc, int pagina = 1)
        {
            ListarConsulta(datosUsuario.CentroId, filtro, pagina, ordenarPor, dirOrden);
            return View("Listar");
        }

        private void ListarConsulta(int centroId,string filtro, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, itemsPorPagina: 10);

            ViewBag.Items = servicio.ListarPaginadoTransaccionesSAPPorCentro(filtro, paginacion, centroId);
        }

        public ActionResult Crear()
        {
            SetearDropDownLists();
            return View();
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Crear(TransaccionSAPDto model, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new CrearTransaccionSAP { Dto = model, Usuario = datosUsuario.NombreUsuario });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            SetearDropDownLists();
            return View(model);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Eliminar(int id, DatosUsuario datosUsuario)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarTransaccionSAP { Id = id, Usuario = datosUsuario.NombreUsuario });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        private void SetearDropDownLists()
        {
            var centrosOrigen = servicio.ListarCentros().OrderBy(c => c.Descripcion).ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
            var centrosDestino = servicio.ListarCentros().OrderBy(c => c.Descripcion).ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
            ViewBag.Centros = centrosOrigen;
            centrosDestino.Insert(0, new SelectListItem { Value = null, Text = null });
            ViewBag.CentrosDestino = centrosDestino;
            ViewBag.TiposComerciales = servicio.ListarTiposComerciales().OrderBy(c => c.Descripcion).ToSelectList(x => x.Id.ToString(), x => x.Descripcion);
            ViewBag.FuncionesSAP = Enum.GetValues(typeof(FuncionSAP)).Cast<FuncionSAP>().ToSelectList(x => ((int)x).ToString(CultureInfo.InvariantCulture), x => x.ToString()).OrderBy(x => x.Text);
        }
    }
}
