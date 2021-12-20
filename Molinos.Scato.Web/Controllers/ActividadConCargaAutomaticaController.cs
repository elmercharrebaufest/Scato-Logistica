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
    [Autorizacion(PermisosScato.AbmActividadConCargaAutomatica)]
    public class ActividadConCargaAutomaticaController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioWorkflows servicioWorkflows;

        public ActividadConCargaAutomaticaController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos, IServicioWorkflows servicioWorkflows)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
            this.servicioWorkflows = servicioWorkflows;
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
            ViewBag.Items = servicio.ListarPaginadoActividadesConCargaAutomatica(centroId, filtro, paginacion);
        }

        [DatosUsuario]
        public ActionResult Crear(DatosUsuario datosUsuario)
        {
            ViewBag.Workflows = servicio.ListarWorkflowsPorCentro(datosUsuario.CentroId).OrderBy(f => f.Descripcion).ToSelectList(s => s.Id.ToString(CultureInfo.InvariantCulture), s => s.Descripcion);
            return View(new ActividadConCargaAutomaticaDto{CentroId =  datosUsuario.CentroId});
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Crear(ActividadConCargaAutomaticaDto model, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new CrearActividadConCargaAutomatica { Dto = model });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            SetearVista(model, datosUsuario.CentroId);
            return View(model);
        }

        [DatosUsuario]
        public ActionResult Modificar(int id, DatosUsuario datosUsuario)
        {
            var aModificar = servicio.ObtenerActividadConCargaAutomatica(id);
            SetearVista(aModificar, datosUsuario.CentroId);
            return View(aModificar);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Modificar(ActividadConCargaAutomaticaDto model, DatosUsuario datosUsuario)
        {
            model.CentroId = datosUsuario.CentroId;
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new ModificarActividadConCargaAutomatica {Dto = model});
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            SetearVista(model, datosUsuario.CentroId);
            return View(model);
        }

        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarActividadConCargaAutomatica { Id = id });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        [DatosUsuario]
        private void SetearVista(ActividadConCargaAutomaticaDto dto, int centroId)
        {
            ViewBag.Workflows = servicio.ListarWorkflowsPorCentro(centroId).OrderBy(f => f.Descripcion).ToSelectList(s => s.Id.ToString(CultureInfo.InvariantCulture), s => s.Descripcion, dto.WorkflowId.ToString(CultureInfo.InvariantCulture));
            if (dto.WorkflowId > 0)
            {
                var actividadesVisibles = servicio.ListarPermisosDeActividad();
                var actividades = servicioWorkflows.ListarActividadesPorDefinicionWorkflow(dto.WorkflowId).Where(actividadesVisibles.Contains);
                ViewBag.Actividades = actividades.OrderBy(x => x).ToSelectList(x => x, x => Textos.ResourceManager.GetString("Act" + x) ?? x, dto.Actividad);
            }
        }

        public ActionResult ObtenerActividades(int workflowId)
        {
            var actividadesVisibles = servicio.ListarPermisosDeActividad();
            var actividades = servicioWorkflows.ListarActividadesPorDefinicionWorkflow(workflowId).Where(actividadesVisibles.Contains);
            return Json(
                    actividades.Select(x => new { value = x, text = Textos.ResourceManager.GetString("Act" + x) ?? x }).OrderBy(x => x.text),
                    JsonRequestBehavior.AllowGet
                );
        }
    }
}
