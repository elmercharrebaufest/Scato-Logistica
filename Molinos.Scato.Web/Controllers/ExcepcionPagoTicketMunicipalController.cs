using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Comandos.Consultas;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.WebPages;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AsignacionTicketMunicipal)]
    public class ExcepcionPagoTicketMunicipalController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public ExcepcionPagoTicketMunicipalController(IServicioRepositorio servicio, ILogger logger, IServicioComandos comandos)
            : base(servicio)
        {
            log = logger;
            servicioComandos = comandos;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, FiltroExcepcionPagoTasaMunicipal filtro, int pagina = 1, string ordenarPor = "FechaCreacion", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(datosUsuario, filtro, datosUsuario.CentroId, pagina, ordenarPor, dirOrden);
            return View(filtro);
        }

        [DatosUsuario]
        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(DatosUsuario datosUsuario, FiltroExcepcionPagoTasaMunicipal filtro, int pagina = 1, string ordenarPor = "FechaCreacion", DirOrden dirOrden = DirOrden.Asc)
        {
            if (filtro.Patente != null)
            {
                filtro.Patente = filtro.Patente.ToUpper();
            }
            ListQuery(datosUsuario, filtro, datosUsuario.CentroId, pagina, ordenarPor, dirOrden);
            return View("Listar", filtro);
        }

        private void ListQuery(DatosUsuario datosUsuario, FiltroExcepcionPagoTasaMunicipal filtro, int centroId, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var respuesta = servicioComandos.Ejecutar(new ConsultarExcepcionPagoTicketMunicipal
            {
                paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 25),
                Filtro = filtro,
                Usuario = datosUsuario.NombreUsuario
                
            }) as ResultadoConsultarExcepcionPagoTicketMunicipal;

            if (!respuesta.HayErrores)
            {
                ViewBag.Items = respuesta.ListaResultados;
            }

            ViewBag.TiposComerciales = servicio.ListarTiposComercialesPorCentro(datosUsuario.CentroId).ToSelectList(x => x.Id.ToString(), x => x.Descripcion);
            ViewBag.Workflows = servicio.ListarWorkflowsCodigoPorCentro(centroId).OrderBy(x => x.Descripcion).ToSelectList(x => x.Codigo.ToString(), x => x.Descripcion);
        }

        [HttpGet]
        public ActionResult Modificar(Guid id, bool pagaTicketMunicipal)
        {
            var logExceptuados = new LogExceptuadosTicketMunicipalDto { InstanceId = id, PagaTicketMunicipal = pagaTicketMunicipal };

            return View("_CrearModificar", new FiltroEditarExcepcionPagoTasaMunicipal());
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Modificar(FiltroExcepcionPagoTasaMunicipal filtro, DatosUsuario datosUsuario)
        {
            ModelState.Remove("Patente");
            ModelState.Remove("NumeroDocumentoIngreso");
            ModelState.Remove("WorkflowCodigo");
            if (ModelState.IsValid)
            {
                var resultado =
                   servicioComandos.Ejecutar(
                       new ModificarExcepcionPagoTasaMunicipal
                       {
                           NombreUsuario = datosUsuario.NombreUsuario,
                            Id = filtro.FiltroEditar.Id,
                            PatenteActual = filtro.FiltroEditar.PatenteActual,
                            WorkflowModal = filtro.FiltroEditar.WorkflowCodigoActual,
                            NumeroDocumentoIngresoActual = filtro.FiltroEditar.NumeroDocumentoIngresoActual,
                            WorkflowDescripcionModal = filtro.FiltroEditar.WorkflowDescripcionActual

                       });
                if (!resultado.HayErrores)
                {
                    var respuesta = InicializarGrid(datosUsuario.NombreUsuario);
                    if (!respuesta.HayErrores)
                    {
                        ViewBag.Items = respuesta.ListaResultados;
                        return PartialView("Listar", new FiltroExcepcionPagoTasaMunicipal());
                    }
                    ModelState.AgregarErrores(respuesta);
                }
            }
            Response.StatusCode = 400; 
            return Content("Ocurrió un error en el servidor: ");
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Crear(FiltroExcepcionPagoTasaMunicipal exceptuados, DatosUsuario datosUsuario)
        {
            ModelState.Remove("PatenteActual");
            ModelState.Remove("NumeroDocumentoIngresoActual");
            ModelState.Remove("WorkflowCodigoActual");
            ModelState.Remove("Id");
            var resultado = new Resultado();
            if (ModelState.IsValid)
            {
                    resultado =
                    servicioComandos.Ejecutar(new CrearExcepcionPagoTasaMunicipal
                    {
                        ExceptuadosTicketMunicipalDto = new ExceptuadosTicketMunicipalDto
                        {
                            NombreUsuario = datosUsuario.NombreUsuario,
                            NumeroDocumentoIngreso = exceptuados.NumeroDocumentoIngreso,
                            Patente = exceptuados.Patente,
                            WorkflowCodigo = exceptuados.WorkflowCodigo,
                            WorkflowDescripcion = exceptuados.WorkflowDescripcion
                        }
                    });
                if (!resultado.HayErrores)
                {
                    var respuesta = InicializarGrid(datosUsuario.NombreUsuario);
                    if (!respuesta.HayErrores)
                    {
                        ViewBag.Items = respuesta.ListaResultados;
                        return PartialView("Listar", exceptuados);
                    }

                    ModelState.AgregarErrores(respuesta);
                }
                ModelState.AgregarErrores(resultado);
            }
            Response.StatusCode = 400;
            return Content(string.Join("," , resultado.Errores.Values));
        }

      

        [HttpPost]
        [DatosUsuario]
        public ActionResult Borrar(int idBorrar, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var resultado =
                   servicioComandos.Ejecutar(
                       new EliminarExcepcionPagoTasaMunicipal
                       {
                           Id = idBorrar
                       });
                if (!resultado.HayErrores)
                {
                    var respuesta = InicializarGrid(datosUsuario.NombreUsuario);
                    if (!respuesta.HayErrores)
                    {
                        ViewBag.Items = respuesta.ListaResultados;
                        return PartialView("Listar", new FiltroExcepcionPagoTasaMunicipal());
                    }
                    ModelState.AgregarErrores(respuesta);
                }
            }
            Response.StatusCode = 500;
            return Content("Ocurrió un error en el servidor: ");
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Filtrar(string filtroPatente, string filtroWorkflow, string filtroNumeroDocumentoDeIngreso, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var filtro = new FiltroExcepcionPagoTasaMunicipal
                    {
                        Patente = filtroPatente.IsEmpty() ? null : filtroPatente.ToUpper(),
                        WorkflowCodigo = filtroWorkflow.IsEmpty() ? null : filtroWorkflow,
                        NumeroDocumentoIngreso = filtroNumeroDocumentoDeIngreso.IsEmpty() ? null : filtroNumeroDocumentoDeIngreso
                    };
                var respuesta = InicializarGrid(datosUsuario.NombreUsuario, filtro);
                    if (!respuesta.HayErrores)
                    {
                        ViewBag.Items = respuesta.ListaResultados;
                        return PartialView("Listar", new FiltroExcepcionPagoTasaMunicipal());
                    }
                    ModelState.AgregarErrores(respuesta);
                
            }
            Response.StatusCode = 500;
            return Content("Ocurrió un error en el servidor: ");
        }


        private ResultadoConsultarExcepcionPagoTicketMunicipal InicializarGrid(string nombreUsuario, FiltroExcepcionPagoTasaMunicipal filtro = default)
        {
            int pagina = 1;
            string ordenarPor = "FechaCreacion";
            DirOrden dirOrden = DirOrden.Asc;
            var respuesta = servicioComandos.Ejecutar(new ConsultarExcepcionPagoTicketMunicipal
            {
                paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 25),
                Filtro = filtro,
                Usuario = nombreUsuario

            }) as ResultadoConsultarExcepcionPagoTicketMunicipal;

            return respuesta;
        }
    }
}
