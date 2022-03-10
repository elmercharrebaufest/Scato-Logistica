using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AbmCaracteristicaDeCalidad)]
    public class CaracteristicaDeCalidadController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public CaracteristicaDeCalidadController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, string filtro = "", int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(filtro, pagina, ordenarPor, dirOrden, datosUsuario.CentroId);
            return View();
        }

        [AjaxOnly]
        [ActionName("Index")]
        [DatosUsuario]
        public ActionResult Listar(DatosUsuario datosUsuario, string filtro = "", int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(filtro, pagina, ordenarPor, dirOrden, datosUsuario.CentroId);
            return View("Listar");
        }

        private void ListQuery(string filtro, int pagina, string ordenarPor, DirOrden dirOrden, int centroId)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);
            ViewBag.Items = servicio.ListarCaracteristicasDeCalidadPaginado(paginacion, filtro, centroId);
        }

        [DatosUsuario]
        public ActionResult Crear()
        {
            var caracteristicaDeCalidad = new CaracteristicaDeCalidadDto { TipoCaracteristica = CaracteristicasCalidad.Ninguno, EnviaASap = true };
            return View(caracteristicaDeCalidad);
        }

        [DatosUsuario]
        [HttpPost]
        public ActionResult Crear(DatosUsuario datosUsuario, string descuentos, CaracteristicaDeCalidadDto model)
        {
            if (ModelState.IsValid)
            {
                if (descuentos != "")
                {
                    var listadescuentos = descuentos.FromJson<DescuentoDto[]>();
                    model.DescuentosDto = listadescuentos.Where(x => x._destroy == false).ToList();
                }
                if(model.SituacionEnvioACamara == EnvioACamara.SiempreSiSuperaValorCamara && !model.SiSuperaValorCamara.HasValue)
                {
                    model.SiSuperaValorCamara = 0;
                }
                model.CentroId = datosUsuario.CentroId;

                var resultado = (ResultadoCrear)servicioComandos.Ejecutar(new CrearCaracteristicaDeCalidad { Dto = model, Usuario = datosUsuario.NombreUsuario });

                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            return View(model);
        }

        [DatosUsuario]
        public ActionResult Modificar(int id)
        {
            var aModificar = servicio.ObtenerCaracteristicaDeCalidad(id);
            return View(aModificar);
        }

        [DatosUsuario]
        [HttpPost]
        public ActionResult Modificar(DatosUsuario datosUsuario, string descuentos, CaracteristicaDeCalidadDto model)
        {
            if (ModelState.IsValid)
            {
                var descuentosBorrados = new List<int>();
                if (descuentos != "")
                {
                    var listadescuentos = descuentos.FromJson<DescuentoDto[]>();

                    if (listadescuentos != null)
                    {
                        descuentosBorrados = listadescuentos.Where(d => d._destroy && !d.EsNuevo).Select(x => x.Id).ToList();
                        model.DescuentosDto = listadescuentos.Where(x => x._destroy == false && x.EsNuevo).ToList();
                    }
                }
                if (model.SituacionEnvioACamara == EnvioACamara.SiempreSiSuperaValorCamara && !model.SiSuperaValorCamara.HasValue)
                {
                    model.SiSuperaValorCamara = 0;
                }
                model.CentroId = datosUsuario.CentroId;

                var resultado = servicioComandos.Ejecutar(new ModificarCaracteristicaDeCalidad
                    {
                        Dto = model,
                        DescuentosBorrados = descuentosBorrados,
                        Usuario = datosUsuario.NombreUsuario
                    });

                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            ViewBag.descuentos = descuentos;
            return View(model);
        }

        [DatosUsuario]
        [HttpPost]
        public ActionResult Eliminar(int id, DatosUsuario datosUsuario)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarCaracteristicaDeCalidad { Id = id, Usuario = datosUsuario.NombreUsuario });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        public JsonResult ObtenerDescuentos(int caracteristicaId)
        {
            var descuentos = servicio.ListarDescuentos(caracteristicaId).ToList();
            return Json(descuentos, JsonRequestBehavior.AllowGet);
        }
    }
}
