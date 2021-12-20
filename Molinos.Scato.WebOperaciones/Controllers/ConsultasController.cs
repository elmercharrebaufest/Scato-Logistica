using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.WebOperaciones.Atributos;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.WebOperaciones.Controllers
{
    [Autorizacion(PermisosScato.ScatoPuerto)]
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
    }
}
