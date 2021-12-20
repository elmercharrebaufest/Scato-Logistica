using System;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
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
    [Autorizacion(PermisosScato.AsignacionTicketMunicipal)]
    public class AsignarTicketMunicipalController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public AsignarTicketMunicipalController(IServicioRepositorio servicio, ILogger logger, IServicioComandos comandos)
            : base(servicio)
        {
            log = logger;
            servicioComandos = comandos;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, FiltroListaDeWorkflowsDto filtro, int pagina = 1, string ordenarPor = "FechaCreacion", DirOrden dirOrden = DirOrden.Asc)
        {
            filtro.CantidadDeResultados = CantidadDeResultados.Veinticinco;
            ListQuery(datosUsuario, filtro, pagina, ordenarPor, dirOrden);
            return View(filtro);
        }

        [DatosUsuario]
        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(DatosUsuario datosUsuario, FiltroListaDeWorkflowsDto filtro, int pagina = 1, string ordenarPor = "FechaCreacion", DirOrden dirOrden = DirOrden.Asc)
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
            var datosWorkflow = servicio.ListarRecorridosEnPlayaExternaPorCentro(filtro, paginacion);

            ViewBag.TiposComerciales = servicio.ListarTiposComercialesPorCentro(datosUsuario.CentroId).ToSelectList(x => x.Id.ToString(), x => x.Descripcion);
            ViewBag.Items = datosWorkflow.Workflows;
            ViewBag.Workflows = datosWorkflow.WorkflowsCentro.OrderBy(x => x.Descripcion).ToSelectList(x => x.Descripcion, x => x.Descripcion);
        }

        [HttpGet]
        public ActionResult Modificar(Guid id, bool pagaTicketMunicipal)
        {
            var logExceptuados = new LogExceptuadosTicketMunicipalDto { InstanceId = id, PagaTicketMunicipal = pagaTicketMunicipal };

            return View("_CrearModificar", logExceptuados);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Modificar(LogExceptuadosTicketMunicipalDto logExceptuados, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                logExceptuados.NombreUsuario = datosUsuario.NombreUsuario;
                var resultado =
                    servicioComandos.Ejecutar(new ModificarRecorridoPagaTicketMunicipal { LogExceptuadosTicketMunicipalDto = logExceptuados });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            return View(logExceptuados);
        }

    }
}
