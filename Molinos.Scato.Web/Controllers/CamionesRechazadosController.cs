using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.CamionesRechazados)]
    public class CamionesRechazadosController : BaseController
    {
        private readonly ILogger log;
        private readonly IFirmaProvider config;

        public CamionesRechazadosController(IServicioRepositorio servicio, ILogger logger, IFirmaProvider configuracion)
            : base(servicio)
        {
            log = logger;
            config = configuracion;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, FiltroListaDeWorkflowsDto filtro, int pagina = 1, string ordenarPor = "FechaCreacion", DirOrden dirOrden = DirOrden.Desc)
        {
            filtro.CantidadDeResultados = CantidadDeResultados.Veinticinco;
            ListQuery(datosUsuario, filtro, pagina, ordenarPor, dirOrden);
            return View(filtro);
        }

        [DatosUsuario]
        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(DatosUsuario datosUsuario, FiltroListaDeWorkflowsDto filtro, int pagina = 1, string ordenarPor = "FechaCreacion", DirOrden dirOrden = DirOrden.Desc)
        {
            if (filtro.Patente != null)
            {
                filtro.Patente = filtro.Patente.ToUpper();
            }
            ListQuery(datosUsuario, filtro, pagina, ordenarPor, dirOrden);
            return View("Listar", filtro);
        }

        private void ListQuery(DatosUsuario datosUsuario, FiltroListaDeWorkflowsDto filtro, int pagina, string ordenarPor, DirOrden dirOrden)
        {

            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 25);
            filtro.CentroId = datosUsuario.CentroId;
            filtro.NombreUsuario = datosUsuario.NombreUsuario;
            filtro.CodigoSapMolinos = config.ObtenerFirmaSinLogo().CodigoSAP;

            ViewBag.CantidadCamiones = servicio.ObtenerCantidadCamionesRechazados(datosUsuario.CentroId);
            ViewBag.Items = servicio.ListarRecorridosRechazados(filtro, paginacion);
        }
    }
}
