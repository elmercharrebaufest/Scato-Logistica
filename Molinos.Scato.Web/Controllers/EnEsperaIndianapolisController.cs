using System;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadEnEsperaIndianapolis)]
    public class EnEsperaIndianapolisController : BaseController
    {
        private readonly IServicioActividadFactory<IEjecutarService> factory;
        private readonly ILogger log;

        public EnEsperaIndianapolisController(ILogger log, IServicioActividadFactory<IEjecutarService> factory, IServicioRepositorio servicio)
            : base(servicio)
        {
            this.factory = factory;
            this.log = log;
        }

        [DatosUsuario]
        public ActionResult Index(Guid id, DatosUsuario datosUsuario)
        {
            var controlRecorrido = new ControlRecorridoDto
            {
                WorkflowInstanceId = id,
                NombreUsuario = datosUsuario.NombreUsuario,
                Actividad = Textos.ActSalidaDeCentro,
                ActividadXaml = "EnEsperaIndianapolis",
                Decision = true,
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId
            };

            var recorrido = servicio.ObtenerDatosDeInstanciaPorGuid(id);
            var serviciowf = factory.CrearServicio(recorrido.WorkflowDefinicionId);
            serviciowf.Ejecutar(id, controlRecorrido);
            return RedirectToAction("Index", "ListaDeCamiones");
        }

    }
}
