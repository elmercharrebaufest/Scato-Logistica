using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;
using System;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.CamionesPendientesMesa)]
    public class CamionesPendientesNoGranosController : BaseController
    {
        private readonly ILogger log;
        private readonly IFirmaProvider configuracion;

        public CamionesPendientesNoGranosController(ILogger log, IServicioRepositorio servicio, IFirmaProvider configuracion)
            : base(servicio)
        {
            this.log = log;
            this.configuracion = configuracion;
        }

        [DatosUsuario]
        public ActionResult Index(int id, DatosUsuario datosUsuario)
        {
            var cargaDeCupo = servicio.ObtenerCupoPorId(id);
            
            ViewBag.Workflows = servicio.ListarWorkFlowsPendientesNoGrano(datosUsuario.CentroId);
            return View(cargaDeCupo);
        }
    }
}
