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
    [Autorizacion(PermisosScato.AbmProveedorExcluidoIntacta)]
    public class ProveedorExcluidoIntactaController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public ProveedorExcluidoIntactaController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        [DatosUsuario]
        public ActionResult Index(string filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListarConsulta(filtro, pagina, ordenarPor, dirOrden);
            return View();
        }

        [AjaxOnly]
        [ActionName("Index")]
        [DatosUsuario]
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

            ViewBag.Items = servicio.ListarPaginadoProveedorExcluidoIntacta(filtro, paginacion);
        }

        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarProveedorExcluidoIntacta {Id = id});
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        public ActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Crear(ProveedorExcluidoIntactaDto dto)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new CrearProveedorExcluidoIntacta { ProveedorId = dto.ProveedorId });
                if (!resultado.HayErrores)
                {
                    log.Debug("Proveedor excluido creado con exito");
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            return View(dto);
        }
    }
}
