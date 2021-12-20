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
    [Autorizacion(PermisosScato.AbmCalle)]
    public class CalleController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public CalleController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, string filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListarConsulta(filtro, pagina, ordenarPor, dirOrden, datosUsuario.CentroId);
            return View();
        }

        [AjaxOnly]
        [ActionName("Index")]
        [DatosUsuario]
        public ActionResult Listar(DatosUsuario datosUsuario, string filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListarConsulta(filtro, pagina, ordenarPor, dirOrden, datosUsuario.CentroId);
            return View("Listar");
        }

        private void ListarConsulta(string filtro, int pagina, string ordenarPor, DirOrden dirOrden, int centroId)
        {
            var paginacion = new Paginacion(
                ordenarPor,
                dirOrden,
                pagina,
                10);

            ViewBag.Items = servicio.ListarPaginadoCalle(centroId, paginacion);
        }

        [DatosUsuario]
        public ActionResult Modificar(int id , DatosUsuario datosUsuario)
        {
         
            var tipo = servicio.ObtenerCalle(id);
            tipo.CaracteristicaDeCalidadDesc = tipo.CaracteristicaDeCalidadId.ToString();
            ViewBag.CaracteristicasCalidad = servicio.ListarCaracteristicasDeCalidadPorMaterial(tipo.MaterialId, datosUsuario.CentroId).Select(x=> new SelectListItem { Selected = x.Id == tipo.CaracteristicaDeCalidadId ,Text = x.DescripcionCorta, Value = x.Id.ToString()});
            return View(tipo);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Modificar(CalleDto tipo, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                tipo.CentroId = datosUsuario.CentroId;
                tipo.CaracteristicaDeCalidadDesc = tipo.CaracteristicaDeCalidadId.ToString();
                ViewBag.CaracteristicasCalidad = servicio.ListarCaracteristicasDeCalidadPorMaterial(tipo.MaterialId, datosUsuario.CentroId).Select(x => new SelectListItem { Selected = x.Id == tipo.CaracteristicaDeCalidadId, Text = x.DescripcionCorta, Value = x.Id.ToString() });
                var resultado = servicioComandos.Ejecutar(new ModificarCalle() { Dto = tipo });
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
            var resultado = servicioComandos.Ejecutar(new EliminarCalle() {Id = id});
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        public ActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Crear(CalleDto tipo, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                tipo.CentroId = datosUsuario.CentroId;
                var resultado = servicioComandos.Ejecutar(new CrearCalle() { Dto = tipo });
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
