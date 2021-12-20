using System.Collections.Generic;
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
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.TablaConversionMaterial)]
    public class ConversionCaracteristicaController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public ConversionCaracteristicaController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
        : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, int? camaraId, int? materialId, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListarConsultaCaracteristica(camaraId, materialId, pagina, ordenarPor, dirOrden);
            SetearVista(camaraId, materialId, datosUsuario.CentroId);
            return View(new ConversionCaracteristicaDto { CamaraId = camaraId ?? 0, MaterialId = materialId ?? 0 });
        }

        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(DatosUsuario datosUsuario, int? camaraId, int? materialId, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListarConsultaCaracteristica(camaraId, materialId, pagina, ordenarPor, dirOrden);
            SetearVista(camaraId, materialId, datosUsuario.CentroId);
            return View("ListarConversionCaracteristicas");
        }

        [HttpGet]
        [DatosUsuario]
        public ActionResult Indexa(DatosUsuario datosUsuario, int? camaraId, int? materialId, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListarConsultaCaracteristica(camaraId, materialId, pagina, ordenarPor, dirOrden);
            SetearVista(camaraId, materialId, datosUsuario.CentroId);
            return View("ListarConversionCaracteristicas");
        }

        private void ListarConsultaCaracteristica(int? camaraId, int? materialId, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(
                ordenarPor,
                dirOrden,
                pagina,
                10);

            ViewBag.Items = servicio.ListarPaginadoConversionCaracteristica(paginacion, camaraId, materialId);
        }

        [ActionName("SetearMaterialesPorCamara")]
        public JsonResult SetearMaterialesPorCamara(int? camaraId)
        {
            if (camaraId != 0 && camaraId != null)
            {
                var materiales =
                    servicio.ListarMaterialesPorCamara((int)camaraId)
                            .ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
                
                return Json(materiales, JsonRequestBehavior.AllowGet);
            }
            return Json(new List<SelectList>(), JsonRequestBehavior.AllowGet);
        }

        [ActionName("SetearCaracteristicasPorMaterial")]
        [DatosUsuario]
        public JsonResult SetearCaracteristicasPorMaterial(int? materialId, DatosUsuario datosUsuario)
        {
            if (materialId != 0 && materialId != null)
            {
                var caracteristicas =
                    servicio.ListarCaracteristicasDeCalidadPorMaterial((int)materialId, datosUsuario.CentroId)
                            .ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion).OrderBy(o=>o.Text);
                return Json(caracteristicas, JsonRequestBehavior.AllowGet);
            }
            return Json(new List<SelectList>(), JsonRequestBehavior.AllowGet);
        }

        private void SetearVista(int? camId, int? matId, int centroId, ConversionMaterialDto model = null)
        {
            ViewBag.Camaras = servicio.ListarCamaras().ToSelectList(f => f.Id.ToString(CultureInfo.InvariantCulture), f => f.Descripcion);

            int camaraId = camId.HasValue ? camId.Value : 0;
            int materialId = matId.HasValue ? matId.Value : 0;
            ViewBag.Materiales = servicio.ListarMaterialesPorCamara(camaraId).ToSelectList(f => f.Id.ToString(CultureInfo.InvariantCulture), f => f.Descripcion);
            ViewBag.Caracteristicas = servicio.ListarCaracteristicasDeCalidadPorMaterial(materialId, centroId).ToSelectList(f => f.Id.ToString(CultureInfo.InvariantCulture), f => f.Descripcion);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Crear(ConversionCaracteristicaDto tipo, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new CrearConversionCaracteristica() { Dto = tipo });
                if (!resultado.HayErrores)
                {
                    return RedirectToAction("Index", new ConversionCaracteristicaDto { CamaraId = tipo.CamaraId });
                }
                ModelState.AgregarErrores(resultado);
            }
            SetearVista(tipo.CamaraId, tipo.MaterialId, datosUsuario.CentroId);
            ListarConsultaCaracteristica(null, null, 1, "Id", DirOrden.Asc);
            return View("Index", tipo);
        }

        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarConversionCaracteristica() { Id = id });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }
    }
}
