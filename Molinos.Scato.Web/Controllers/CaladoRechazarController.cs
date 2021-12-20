using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadCaladoRechazar)]
    public class CaladoRechazarController : BaseController
    {
        private readonly IServicioActividadFactory<ICaladoService> factory;
        private readonly IServicioComandos servicioComandos;
        private readonly IConfiguracionProvider configuracion;
        private ILogger log;

        public CaladoRechazarController(ILogger log, IServicioActividadFactory<ICaladoService> factory, IServicioRepositorio servicio, IServicioComandos servicioComandos, IConfiguracionProvider configuracion)
            : base(servicio)
        {
            this.factory = factory;
            this.servicioComandos = servicioComandos;
            this.configuracion = configuracion;
            this.log = log;
        }

        [DatosUsuario]
        public ActionResult Index(Guid id, DatosUsuario datosUsuario, bool? caladoObligatorio = null)
        {
            log.Info("{0} - Index", id);



            var recorrido = SetearVista(id);
            

            return View(new CaladoPantallaDto
            {
                MuestraConjunto = null,
                NumeroOrden = recorrido.Centro.Descripcion.Substring(0, 3).Trim().ToUpper() + recorrido.Calado.Id.ToString(CultureInfo.InvariantCulture).PadLeft(8, '0'),
                WorkflowDefinicionId = recorrido.WorkflowDefinicionId,
                WorkflowInstanceId = recorrido.InstanciaWorkflow,
                CicloDeCalado = servicio.ObtenerCantidadCalados(id),
                NumeroDocumentoIngreso = recorrido.NumeroDocumentoIngreso,
                CentroId = recorrido.Centro.Id,
                Fecha = DateTime.Now,
                MaterialId = recorrido.Material.Id,
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                RecorridoId = recorrido.Id
            });
        }

        private RecorridoDto SetearVista(Guid id)
        {
            log.Info("{0} - SetearVista", id);
            var recorrido = servicio.ObtenerRecorridoPorGuid(id);
            ViewBag.TipoDocumentoIngreso = recorrido.TipoDocumentoIngreso;
            ViewBag.RecorridoId = recorrido.Id;
            ViewBag.Rechazado = recorrido.Rechazado;

            
            return recorrido;
        }

        [DatosUsuario]
        public ActionResult Rechazar(string codigoWf, int workflowDefinicionId, Guid instanceId, DatosUsuario datosUsuario)
        {
            log.Info("{0} - Calado Rechazar", instanceId);
            ViewBag.Motivos = servicio.ListarMotivos().ToSelectList(x => x.Descripcion, x => x.Descripcion);
            ViewBag.Workflow = codigoWf;
            ViewBag.WorkflowDefinicionId = workflowDefinicionId;
            var controlRecorrido = new ControlRecorridoDto
            {
                WorkflowInstanceId = instanceId,
                NombreUsuario = datosUsuario.NombreUsuario,
                Actividad = Textos.ActCalado + "/" + Textos.Rechazar,
                ActividadXaml = "Calado",
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId
            };

            return View("_TransportistaRechazado", controlRecorrido);
        }

        [DatosUsuario]
        public ActionResult TransportistaRechazado(string workflow, int workflowDefinicionId, ControlRecorridoDto controlRecorrido)
        {
            log.Info("{0} - Calado rechazado", workflowDefinicionId);
            controlRecorrido.Decision = true;
            var serivce = factory.CrearServicio(workflowDefinicionId);
            var listaCalaldosPorCaracteristicas = new CaladoPorCaracteristicaDto[1];
            var resultado = serivce.Calado(listaCalaldosPorCaracteristicas, 0, "", "", true, controlRecorrido.WorkflowInstanceId, controlRecorrido);
            if (!resultado.HayErrores)
            {
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            ModelState.AgregarErrores(resultado);
            return RedirectToAction("Index", new { id = controlRecorrido.WorkflowInstanceId });
        }


    }
}
