using System;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.HistoricoDeCamiones)]
    public class HistoricoDeFotosController : BaseController
    {
        private ILogger log;

        public HistoricoDeFotosController(ILogger log, IServicioRepositorio servicio, IConfiguracionProvider config) 
            : base(servicio)
        {
            this.log = log;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, string filtro)
        {
            ListQuery(datosUsuario, filtro);
            return View();
        }

        [DatosUsuario]
        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(DatosUsuario datosUsuario, string filtro)
        {
            ListQuery(datosUsuario, filtro);
            return View("_Listar");
        }

        private void ListQuery(DatosUsuario datosUsuario, string filtro)
        {
            ViewBag.Items = servicio.ListarHistoricoDeFotosPorPatente(datosUsuario.CentroId, filtro);
        }
    }
}
