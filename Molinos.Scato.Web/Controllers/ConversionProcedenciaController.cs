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
    [Autorizacion(PermisosScato.TablaConversionProcedencia)]
    public class ConversionProcedenciaController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public ConversionProcedenciaController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        public ActionResult Index(int? camaraId, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListarConsultaProcedencia(camaraId, pagina, ordenarPor, dirOrden);
            SetearVista();
            return View(new ConversionProcedenciaDto { CamaraId = camaraId ?? 0 });
        }

        [HttpGet]
        public ActionResult Indexa(int? camaraId, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListarConsultaProcedencia(camaraId, pagina, ordenarPor, dirOrden);
            SetearVista();
            return View("ListarConversionProcedencias");
        }

        private void ListarConsultaProcedencia(int? camaraId, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(
                ordenarPor,
                dirOrden,
                pagina,
                10);

            ViewBag.Items = servicio.ListarPaginadoConversionProcedencia(paginacion, camaraId);
        }

        private void SetearVista()
        {
            ViewBag.Camaras = servicio.ListarCamaras().ToSelectList(f => f.Id.ToString(CultureInfo.InvariantCulture), f => f.Descripcion);
            ViewBag.Localidades = servicio.ListarLocalidades().ToSelectList(f => f.Id.ToString(CultureInfo.InvariantCulture), f => f.Descripcion);
        }

        [HttpPost]
        public ActionResult Crear(ConversionProcedenciaDto tipo)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new CrearConversionProcedencia { Dto = tipo });
                if (resultado.HayErrores)
                {
                    ModelState.AgregarErrores(resultado);
                }
            }
            SetearVista();
            ListarConsultaProcedencia(tipo.CamaraId, 1, "Id", DirOrden.Asc);
            return View("Index", tipo);
        }

        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarConversionProcedencia { Id = id });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }
    }
}
