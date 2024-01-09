using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.WebMobile.Controllers
{
    public class ConsultasController : Controller
    {
        private readonly ILogger log;
        protected readonly IConfiguracionProvider Configuracion;
        private readonly IServicioRepositorio servicio;

        public ConsultasController(ILogger log, IServicioRepositorio servicio, IConfiguracionProvider configuracion)
        {
            this.servicio = servicio;
            this.log = log;
            Configuracion = configuracion;
        }

        public ActionResult BuscarMaterial(string term)
        {
            log.Debug("Obteniendo material por: ", term);
            var material = servicio.BuscarMaterial(5, term);
            return material != null ? Json(new { label = material.Descripcion, material.Id, material.Descripcion }, JsonRequestBehavior.AllowGet) : Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscarMateriales(string term)
        {
            log.Debug("Obteniendo materiales por: ", term);
            var materiales = servicio.BuscarMaterialesPorCentro(5, term);
            return Json(materiales.Select(s => new { label = s.MaterialDesc, Id = s.MaterialId, s.MaterialDesc }), JsonRequestBehavior.AllowGet);
        }

        public MaterialIdYDescripcionDto ObtenerMaterialIdYDescripcion()
        {
            var codigoSapSoja = Configuracion.AppSettings["CodigoSapSemillaSoja"];
            var datos = servicio.ObtenerMaterialIdYDescripcionPorCodigoSap(codigoSapSoja);
            return datos;
        }

        public ActionResult BuscarCaracteristicaDeCalidadPorId(int id)
        {
            var caracteristicaDeCalidad = servicio.ObtenerCaracteristicaDeCalidad(id);
            return caracteristicaDeCalidad != null
                ? Json(new { id = caracteristicaDeCalidad.Id, nombre = caracteristicaDeCalidad.DescripcionCorta, caladoMaximo = caracteristicaDeCalidad.CaladoMaximo, caladoMinimo = caracteristicaDeCalidad.CaladoMinimo }, JsonRequestBehavior.AllowGet)
                : Json("", JsonRequestBehavior.AllowGet);
        }
    }
}
