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
    [Autorizacion(PermisosScato.AbmTipoDocumentoIdentidad)]
    public class TipoDocumentoIdentidadController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public TipoDocumentoIdentidadController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        public ActionResult Index(int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListarConsulta(pagina, ordenarPor, dirOrden);
            return View();
        }

        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListarConsulta(pagina, ordenarPor, dirOrden);
            return View("Listar");
        }

        private void ListarConsulta(int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(
                ordenarPor,
                dirOrden,
                pagina,
                10);
            
            ViewBag.Items = servicio.ListarPaginadoTiposDocumentoIdentidad(paginacion);
        }

        public ActionResult Modificar(int id)
        {
            var tipo = servicio.ObtenerTipoDocumentoIdentidad(id);
            return View(tipo);
        }

        [HttpPost]
        public ActionResult Modificar(TipoDocumentoIdentidadDto tipo)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new ModificarTipoDocumentoIdentidad { Dto = tipo });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            return View(tipo);
        }

        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarTipoDocumentoIdentidad { Id = id });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        public ActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Crear(TipoDocumentoIdentidadDto tipo)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new CrearTipoDocumentoIdentidad{ Dto = tipo});
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
