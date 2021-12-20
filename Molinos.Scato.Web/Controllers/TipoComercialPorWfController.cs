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
    [Autorizacion(PermisosScato.AbmTipoComercialPorWf)]
    public class TipoComercialPorWfController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public TipoComercialPorWfController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, string filtro, int pagina = 1, string ordenarPor = "WorkflowDescripcion", DirOrden dirOrden = DirOrden.Asc)
        {
            ListarConsulta(filtro, datosUsuario.CentroId, pagina, ordenarPor, dirOrden);
            return View();
        }

        [AjaxOnly]
        [ActionName("Index")]
        [DatosUsuario]
        public ActionResult Listar(DatosUsuario datosUsuario, string filtro, int pagina = 1, string ordenarPor = "WorkflowDescripcion", DirOrden dirOrden = DirOrden.Asc)
        {
            ListarConsulta(filtro, datosUsuario.CentroId, pagina, ordenarPor, dirOrden);
            return View("Listar");
        }

        private void ListarConsulta(string filtro,int centroId, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(
                ordenarPor,
                dirOrden,
                pagina,
                10);

            ViewBag.Items = servicio.ListarTiposComercialesPorWf(filtro, centroId, paginacion);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Eliminar(int workflowId, int tipoComercialId, DatosUsuario datosUsuario)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarTipoComercialPorWf { WorkflowId = workflowId , TipoComercialId = tipoComercialId, Usuario = datosUsuario.NombreUsuario});
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        [DatosUsuario]
        public ActionResult Crear(DatosUsuario datosUsuario)
        {
            var workflows = servicio.ListarWorkflowsPorCentro(datosUsuario.CentroId);
            ViewBag.Worflows = workflows.OrderBy(f => f.Descripcion).Where(w => w.Activo).ToSelectList(s => s.Id.ToString(CultureInfo.InvariantCulture), s => s.Descripcion);
            return View();
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Crear(TipoComercialPorWfDto tipo, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new CrearTipoComercialPorWf { Dto = tipo, Usuario = datosUsuario.NombreUsuario});
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            return View(tipo);
        }

        public ActionResult ObtenerTiposComerciales(int workflowId)
        {
            var tiposAsociados = servicio.ObtenerWorkflow(workflowId).TiposComercialesAsociados;
            var tiposComerciales = servicio.ListarTiposComerciales();

            return Json(
                    tiposComerciales.OrderBy(o => o.Descripcion).Select(x => new { value = x.Id, text = x.Descripcion, disable = tiposAsociados.Select(s => s.Id).Contains(x.Id.Value) }),
                    JsonRequestBehavior.AllowGet
                );
        }
    }
}
