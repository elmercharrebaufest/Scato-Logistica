using System;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Filtros;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadAutorizarTransportistaInhabilitado)]
    public class AutorizarTransportistaInhabilitadoController : BaseController
    {
        private readonly IServicioActividadFactory<IAutorizarTransportistaInhabilitadoService> factory;
        private readonly ILogger log;

        public AutorizarTransportistaInhabilitadoController(ILogger log, IServicioActividadFactory<IAutorizarTransportistaInhabilitadoService> factory, IServicioRepositorio servicio)
            : base(servicio)
        {
            this.factory = factory;
            this.log = log;
        }

        public ActionResult Index(Guid id, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            var recorrido = servicio.ObtenerRecorridoPorGuid(id);
            ListarInhabilitacionCamion(recorrido, pagina, ordenarPor, dirOrden);
            ListarInhabilitacionChofer(recorrido, pagina, ordenarPor, dirOrden);
            return View(recorrido);
        }
        private void ListarInhabilitacionCamion(RecorridoDto recorrido, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var patenteAcoplado = recorrido.Vehiculo != null ? recorrido.Vehiculo.PatenteAcoplado : "";
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);
            ViewBag.InhabilitacionCamion = servicio.ListarInhabilitacionCamionPaginada(recorrido.Patente, patenteAcoplado, recorrido.Centro.Id, paginacion);
        }
        private void ListarInhabilitacionChofer(RecorridoDto recorrido, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);
            ViewBag.InhabilitacionChofer = servicio.ListarInhabilitacionChoferPaginada(recorrido.Chofer.Id, recorrido.Centro.Id, paginacion);
        }

        [DatosUsuario]
        [HttpPost]
        [HttpParamAction]
        public ActionResult TerminarInhabilitaciones(string codigoWf, int workflowDefinicionId, Guid instanceId, DatosUsuario datosUsuario, string motivoAutorizacion)
        {
            var controlRecorrido = new ControlRecorridoDto
            {
                WorkflowInstanceId = instanceId,
                NombreUsuario = datosUsuario.NombreUsuario,
                Actividad = Textos.ActAutorizarTransportistaHabilitado,
                ActividadXaml = "AutorizarTransportistaInhabilitado",
                Decision = true,
                Comentario = motivoAutorizacion,
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId
            };

            var serviciowf = factory.CrearServicio(workflowDefinicionId);
            serviciowf.AutorizarTransportistaInhabilitado(instanceId, true, true, controlRecorrido);
            return RedirectToAction("Index", "ListaDeCamiones");
        }

        [DatosUsuario]
        [HttpPost]
        [HttpParamAction]
        public ActionResult Autorizar(string codigoWf, int workflowDefinicionId, Guid instanceId, DatosUsuario datosUsuario, string motivoAutorizacion)
        {
            var controlRecorrido = new ControlRecorridoDto
            {
                WorkflowInstanceId = instanceId,
                NombreUsuario = datosUsuario.NombreUsuario,
                Actividad = Textos.ActAutorizarTransportistaHabilitado,
                ActividadXaml = "AutorizarTransportistaInhabilitado",
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                Decision = true,
                Comentario = motivoAutorizacion
            };

            var serviciowf = factory.CrearServicio(workflowDefinicionId);
            serviciowf.AutorizarTransportistaInhabilitado(instanceId, true, false, controlRecorrido);
            return RedirectToAction("Index", "ListaDeCamiones");
        }

        [DatosUsuario]
        [HttpPost]
        [HttpParamAction]
        public ActionResult Rechazar(string codigoWf, int workflowDefinicionId, Guid instanceId, DatosUsuario datosUsuario)
        {
            var controlRecorrido = new ControlRecorridoDto
            {
                WorkflowInstanceId = instanceId,
                NombreUsuario = datosUsuario.NombreUsuario,
                Actividad = Textos.ActAutorizarTransportistaHabilitado,
                ActividadXaml = "AutorizarTransportistaInhabilitado",
                Mensaje = Textos.TransportistaInhabilitado,
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                Decision = false
            };

            var serviciowf = factory.CrearServicio(workflowDefinicionId);
            serviciowf.AutorizarTransportistaInhabilitado(instanceId, false, false, controlRecorrido);
            return RedirectToAction("Index", "ListaDeCamiones");
        }
    }
}
