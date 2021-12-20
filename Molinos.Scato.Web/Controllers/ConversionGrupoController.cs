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
    public class ConversionGrupoController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public ConversionGrupoController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        [HttpGet]
        public ActionResult Index(int? camaraId, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListarConsultaGrupo(camaraId, pagina, ordenarPor, dirOrden);
            SetearVista(camaraId);
            return View(new ConversionGrupoDto{CamaraId = camaraId ?? 0});
        }

        [HttpGet]
        public ActionResult Indexa(int? camaraId, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListarConsultaGrupo(camaraId, pagina, ordenarPor, dirOrden);
            SetearVista(camaraId);
            return View("ListarConversionGrupos");
        }

        private void ListarConsultaGrupo(int? camaraId, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(
                ordenarPor,
                dirOrden,
                pagina,
                10);

            ViewBag.Items = servicio.ListarPaginadoConversionGrupo(paginacion, camaraId);
        }

        private void SetearVista(int? camId)
        {
            ViewBag.Camaras = servicio.ListarCamaras().ToSelectList(f => f.Id.ToString(CultureInfo.InvariantCulture), f => f.Descripcion);

            int camaraId = camId.HasValue ? camId.Value : 0;
            ViewBag.Materiales = servicio.ListarMaterialesPorCamara(camaraId).ToSelectList(f => f.Id.ToString(CultureInfo.InvariantCulture), f => f.Descripcion);
        }

        [HttpPost]
        public ActionResult Crear(ConversionGrupoDto tipo)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new CrearConversionGrupo { Dto = tipo });
                if (!resultado.HayErrores)
                {
                    return RedirectToAction("Index", new ConversionGrupoDto{CamaraId = tipo.CamaraId});
                }
                ModelState.AgregarErrores(resultado);
            }
            SetearVista(tipo.CamaraId);
            ListarConsultaGrupo(tipo.CamaraId, 1, "Id", DirOrden.Asc);
            return View("Index", tipo);
        }

        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarConversionGrupo { Id = id });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }
    }
}
