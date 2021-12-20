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
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.TablaConversionMaterial)]
    public class ConversionMaterialController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public ConversionMaterialController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        [HttpGet]
        public ActionResult Index(int? camaraId, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListarConsultaMaterial(camaraId, pagina, ordenarPor, dirOrden);
            SetearVista(camaraId);
            return View(new ConversionMaterialDto{ CamaraId =  camaraId ?? 0});
        }

        [HttpGet]
        public ActionResult Indexa(int? camaraId, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListarConsultaMaterial(camaraId, pagina, ordenarPor, dirOrden);
            SetearVista(camaraId);
            return View("ListarConversionMateriales");
        }

        private void ListarConsultaMaterial(int? camaraId, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(
                ordenarPor,
                dirOrden,
                pagina,
                10);

            ViewBag.Items = servicio.ListarPaginadoConversionMaterial(camaraId, paginacion);
        }

        private void SetearVista(int? camId, ConversionMaterialDto model = null)
        {
            ViewBag.Camaras = servicio.ListarCamaras().ToSelectList(f => f.Id.ToString(CultureInfo.InvariantCulture), f => f.Descripcion);

            int camaraId = camId.HasValue ? camId.Value : 0;
            ViewBag.Materiales = servicio.ListarMaterialesPorCamara(camaraId).ToSelectList(f => f.Id.ToString(CultureInfo.InvariantCulture), f => f.Descripcion);
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

        [HttpPost] 
        public ActionResult Crear(ConversionMaterialDto tipo)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new CrearConversionMaterial() { Dto = tipo });
                if (!resultado.HayErrores)
                {
                    return RedirectToAction("Index", new ConversionMaterialDto { CamaraId = tipo.CamaraId });
                }
                ModelState.AgregarErrores(resultado);
            }
            SetearVista(tipo.CamaraId);
            ListarConsultaMaterial(tipo.CamaraId, 1, "Id", DirOrden.Asc);
            return View("Index", tipo);
        }

        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarConversionMaterial() { Id = id });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }
    }
}
