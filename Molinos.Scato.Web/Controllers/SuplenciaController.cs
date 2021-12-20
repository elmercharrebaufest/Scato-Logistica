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
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AbmSuplencia)]
    public class SuplenciaController : BaseController
    {
        private readonly IServicioComandos servicioComandos;
        private ILogger log;

        public SuplenciaController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        [DatosUsuario]
        public ActionResult Index(string filtro, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc, int pagina = 1)
        {
            ListarConsulta(filtro, pagina, ordenarPor, dirOrden);
            return View();
        }

        [DatosUsuario]
        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(string filtro, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc, int pagina = 1)
        {
            ListarConsulta(filtro, pagina, ordenarPor, dirOrden);
            return View("Listar");
        }

        private void ListarConsulta(string filtro, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, itemsPorPagina: 10);

            ViewBag.Items = servicio.ListarPaginadoSuplencias(filtro, paginacion);
        }

        public ActionResult Crear()
        {
            var suplencia = new SuplenciaDto();

            ViewBag.Usuarios = servicio.ListarUsuarios().OrderBy(c => c.NombreUsuario).ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.NombreUsuario);

            return View(suplencia);
        }

        [HttpPost]
        public ActionResult Crear(SuplenciaDto model)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new CrearSuplencia { Dto = model });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }

            ViewBag.Usuarios = servicio.ListarUsuarios().OrderBy(c => c.NombreUsuario).ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.NombreUsuario);
            return View(model);
        }

        public ActionResult Modificar(int id)
        {
            var suplencia = servicio.ObtenerSuplencia(id);

            ViewBag.Usuarios = servicio.ListarUsuarios().OrderBy(c => c.NombreUsuario).ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.NombreUsuario);

            return View(suplencia);
        }

        [HttpPost]
        public ActionResult Modificar(SuplenciaDto suplencia)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new ModificarSuplencia { Dto = suplencia });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }

            ViewBag.Usuarios = servicio.ListarUsuarios().OrderBy(c => c.NombreUsuario).ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.NombreUsuario);
            return View(suplencia);
        }

        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarSuplencia { Id = id });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }
    }
}
