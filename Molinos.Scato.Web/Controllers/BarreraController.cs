using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AbmBarrera)]
    public class BarreraController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioOrquestador servicioOrquestador;

        public BarreraController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos, IServicioOrquestador servicioOrquestador)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
            this.servicioOrquestador = servicioOrquestador;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(pagina, ordenarPor, dirOrden, datosUsuario.CentroId);
            return View();
        }


        [AjaxOnly]
        [ActionName("Index")]
        [DatosUsuario]
        public ActionResult Listar(DatosUsuario datosUsuario, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(pagina, ordenarPor, dirOrden, datosUsuario.CentroId);
            return View("Listar");
        }

        [DatosUsuario]
        public ActionResult Crear()
        {
            ViewBag.Roles = servicio.ListarRoles().OrderBy(p => p.Descripcion).ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
            ViewBag.Sensores = servicioOrquestador.ListarSensores().OrderBy(p => p.Descripcion).ToSelectList(x => x.Codigo, x => x.Descripcion);
            ViewBag.sensoresBarrera = "[]";
            ViewBag.Barreras = servicioOrquestador.ListarBarrerasSemaforos().OrderBy(p => p.Descripcion).ToSelectList(x => x.Codigo, x => x.Descripcion);
            return View();
        }

        [DatosUsuario]
        public ActionResult Modificar(int id)
        {
            var aModificar = servicio.ObtenerVisualizacionBarrera(id);
            ViewBag.Roles = servicio.ListarRoles().OrderBy(p => p.Descripcion).ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
            ViewBag.Sensores = servicioOrquestador.ListarSensores().OrderBy(p => p.Descripcion).ToSelectList(x => x.Codigo, x => x.Descripcion);
            ViewBag.Barreras = servicioOrquestador.ListarBarrerasSemaforos().OrderBy(p => p.Descripcion).ToSelectList(x => x.Codigo, x => x.Descripcion);
            return View(aModificar);
        }

        [DatosUsuario]
        [HttpPost]
        public ActionResult Modificar(DatosUsuario datosUsuario, string sensoresBarrera, VisualizacionBarreraDto model)
        {
            if (ModelState.IsValid)
            {
                var sensoresBorrados = new List<int>();
                if (sensoresBarrera != "")
                {
                    var listadescuentos = sensoresBarrera.FromJson<SensorBarreraDto[]>();

                    if (listadescuentos != null)
                    {
                        sensoresBorrados = listadescuentos.Where(d => d._destroy && !d.EsNuevo).Select(x => x.Id).ToList();
                        model.SensoresBarreras = listadescuentos.Where(x => x._destroy == false && x.EsNuevo).ToList();
                    }
                }
                
                model.CentroId = datosUsuario.CentroId;

                var resultado = servicioComandos.Ejecutar(new ModificarVisualizacionBarrera
                {
                    Dto = model,
                    SensoresBorrados = sensoresBorrados,
                    Usuario = datosUsuario.NombreUsuario
                });

                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }

            ViewBag.sensoresBarrera = sensoresBarrera;
            return View(model);
        }

        [DatosUsuario]
        [HttpPost]
        public ActionResult Crear(DatosUsuario datosUsuario, string sensoresBarrera, VisualizacionBarreraDto model)
        {
            if (ModelState.IsValid)
            {
                if (sensoresBarrera != "")
                {
                    var listadescuentos = sensoresBarrera.FromJson<SensorBarreraDto[]>();
                    model.SensoresBarreras = listadescuentos.Where(x => x._destroy == false).ToList();
                }

                model.CentroId = datosUsuario.CentroId;

                var resultado = (ResultadoCrear)servicioComandos.Ejecutar(new CrearVisualizacionBarrera { Dto = model, Usuario = datosUsuario.NombreUsuario });

                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            return View(model);
        }

        [DatosUsuario]
        [HttpPost]
        public ActionResult Eliminar(int id, DatosUsuario datosUsuario)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarVisualizacionBarrera { Id = id, Usuario = datosUsuario.NombreUsuario });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        private void ListQuery(int pagina, string ordenarPor, DirOrden dirOrden, int centroId)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);
            ViewBag.Items = servicio.ListarPaginadoVisualizacionBarrera(centroId, paginacion);
        }

        public JsonResult ObtenerSensoresBarrera(int grupoId)
        {
            var descuentos = servicio.ListarSensoresBarreras(grupoId).ToList();
            return Json(descuentos, JsonRequestBehavior.AllowGet);
        }
    }
}
