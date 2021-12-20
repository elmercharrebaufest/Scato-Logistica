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
    [Autorizacion(PermisosScato.AbmTipoComercial)]
    public class TipoComercialController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public TipoComercialController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        public ActionResult Index(string filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListarConsulta(filtro, pagina, ordenarPor, dirOrden);
            return View();
        }

        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(string filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListarConsulta(filtro, pagina, ordenarPor, dirOrden);
            return View("Listar");
        }

        private void ListarConsulta(string filtro, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(
                ordenarPor,
                dirOrden,
                pagina,
                10);

            ViewBag.Items = servicio.ListarPaginadoTiposComerciales(filtro, paginacion);
        }

        public ActionResult Modificar(int id)
        {
            var tipo = servicio.ObtenerTipoComercial(id);
            return View(tipo);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Modificar(TipoComercialDto tipo, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new ModificarTipoComercial { Dto = tipo, Usuario = datosUsuario.NombreUsuario});
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            return View(tipo);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Eliminar(int id, DatosUsuario datosUsuario)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarTipoComercial { Id = id, Usuario = datosUsuario.NombreUsuario});
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        public ActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Crear(TipoComercialDto tipo, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new CrearTipoComercial { Dto = tipo, Usuario = datosUsuario.NombreUsuario});
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            return View(tipo);
        }
    }
}
