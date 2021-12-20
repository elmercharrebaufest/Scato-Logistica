using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AbmControlDeTiempo)]
    public class ControlDeTiempoController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioWorkflows servicioWorkflows;

        public ControlDeTiempoController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos, IServicioWorkflows servicioWorkflows)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
            this.servicioWorkflows = servicioWorkflows;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, int pagina = 1, string ordenarPor = "Workflow.Descripcion", DirOrden dirOrden = DirOrden.Asc)
        {
            ListarConsulta(datosUsuario.CentroId, pagina, ordenarPor, dirOrden);
            return View();
        }

        [AjaxOnly]
        [ActionName("Index")]
        [DatosUsuario]
        public ActionResult Listar(DatosUsuario datosUsuario, int pagina = 1, string ordenarPor = "Workflow.Descripcion", DirOrden dirOrden = DirOrden.Asc)
        {
            ListarConsulta(datosUsuario.CentroId, pagina, ordenarPor, dirOrden);
            return View("Listar");
        }

        private void ListarConsulta(int centroId, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(
                ordenarPor,
                dirOrden,
                pagina,
                10);

            ViewBag.Items = servicio.ListarPaginadoControlesDeTiempo(paginacion, centroId);
        }

        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarControlDeTiempo() { Id = id });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        [DatosUsuario]
        public ActionResult Crear(DatosUsuario datosUsuario)
        {
            ViewBag.Workflows = servicio.ListarWorkflowsPorCentro(datosUsuario.CentroId).OrderBy(f => f.Descripcion).ToSelectList(s => s.Id.ToString(CultureInfo.InvariantCulture), s => s.Descripcion);
            return View();
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Crear(ControlDeTiempoDto tipo, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new CrearControlDeTiempo { Dto = tipo });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            SetearVista(datosUsuario.CentroId, tipo);
            return View(tipo);
        }
        [DatosUsuario]
        public ActionResult Modificar(DatosUsuario datosUsuario, int id)
        {
            var control = servicio.ObtenerControlDeTiempo(id);
            SetearVista(datosUsuario.CentroId, control); 
            return View(control);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Modificar(ControlDeTiempoDto tipo, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new ModificarControlDeTiempo { Dto = tipo });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            SetearVista(datosUsuario.CentroId, tipo);
            return View(tipo);
        }

        private void SetearVista(int centroId, ControlDeTiempoDto control)
        {
            ViewBag.Workflows = servicio.ListarWorkflowsPorCentro(centroId).OrderBy(f => f.Descripcion).ToSelectList(s => s.Id.ToString(CultureInfo.InvariantCulture), s => s.Descripcion, control.WorkflowId.ToString(CultureInfo.InvariantCulture));
            if (control.WorkflowId > 0)
            {
                var actividades = servicioWorkflows.ListarActividadesPorDefinicionWorkflow(control.WorkflowId);
                ViewBag.ActividadesDesde = actividades.ToSelectList(x => x, x => Textos.ResourceManager.GetString("Act" + x) ?? x, control.ActividadDesde);
                ViewBag.ActividadesHasta = actividades.ToSelectList(x => x, x => Textos.ResourceManager.GetString("Act" + x) ?? x, control.ActividadHasta);
            } 
        }

        public ActionResult ObtenerActividades(int workflowId)
        {
            var actividades = servicioWorkflows.ListarActividadesPorDefinicionWorkflow(workflowId);

            return Json(
                    actividades.Select(x => new { value = x, text = Textos.ResourceManager.GetString("Act" + x) ?? x }).OrderBy(x => x.text),
                    JsonRequestBehavior.AllowGet
                );
        }
    }
}
