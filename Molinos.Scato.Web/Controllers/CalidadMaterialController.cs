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
    [Autorizacion(PermisosScato.AbmCalidadMaterial)]
    public class CalidadMaterialController : BaseController
    {
        private readonly IServicioComandos servicioComandos;
        private ILogger log;

        public CalidadMaterialController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos) 
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, int? materialId, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListarConsulta(datosUsuario.CentroId, materialId, pagina, ordenarPor, dirOrden);
            return View();
        }

        [AjaxOnly]
        [ActionName("Index")]
        [DatosUsuario]
        public ActionResult ListarCalidades(DatosUsuario datosUsuario, int? materialId, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListarConsulta(datosUsuario.CentroId, materialId, pagina, ordenarPor, dirOrden);
            return View("ListarCalidades");
        }

        private void ListarConsulta(int centroId, int? materialId, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            materialId = materialId ?? 0;
            var paginacion = new Paginacion(
            ordenarPor,
            dirOrden,
            pagina,
            10);
            ViewBag.Items = servicio.ListarPaginadoCalidadMaterial(materialId.Value,centroId, paginacion);
        }

        [DatosUsuario]
        public ActionResult Crear(DatosUsuario datosUsuario, int? materialId)
        {
            var material = servicio.ObtenerMaterialPorCentro(datosUsuario.CentroId, materialId ?? 0);
            if (material != null)
            {
                return View(new CalidadMaterialDto
                    {
                        Material = material.MaterialDesc,
                        MaterialId = material.MaterialId
                    });
            }
            return View();
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Crear(DatosUsuario datosUsuario, CalidadMaterialDto model)
        {
            if (ModelState.IsValid)
            {
                model.CentroId = datosUsuario.CentroId;
                var resultado = servicioComandos.Ejecutar(new CrearCalidadMaterial { Dto = model });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarCalidadMaterial { Id = id });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }
    }
}
