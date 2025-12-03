using System.Net;
using System.Web.Mvc;
using System.Web.WebPages;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Comandos.Consultas;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AsignacionTicketMunicipal)]
    public class ExcepcionPagoTicketMunicipalController : BaseController
    {
        #region -- Fields --

        private readonly IServicioComandos servicioComandos;

        #endregion

        #region -- Constructors --

        public ExcepcionPagoTicketMunicipalController(IServicioRepositorio servicio, IServicioComandos comandos)
            : base(servicio)
        {
            this.servicioComandos = comandos;
        }

        #endregion

        #region -- Methods --

        #region -- Query methods --

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, FiltroExcepcionPagoTasaMunicipal filtro, int pagina = 1, string ordenarPor = "FechaCreacionExcepcion", DirOrden dirOrden = DirOrden.Asc)
        {
            this.ListQuery(datosUsuario, filtro, pagina, ordenarPor, dirOrden);

            return View(filtro);
        }

        [DatosUsuario]
        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(
            DatosUsuario datosUsuario, 
            FiltroExcepcionPagoTasaMunicipal filtro, 
            int pagina = 1, 
            string ordenarPor = "FechaCreacionExcepcion", 
            DirOrden dirOrden = DirOrden.Asc)
        {
            if (filtro.Patente != null)
                filtro.Patente = filtro.Patente.ToUpper();

            this.ListQuery(datosUsuario, filtro, pagina, ordenarPor, dirOrden);

            return View("Listar", filtro);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Filtrar(string filtroPatente, DatosUsuario datosUsuario)
        {
            return
                this.RefrescarResultados(
                    new ConsultarExcepcionPagoTicketMunicipal
                    {
                        Usuario = datosUsuario.NombreUsuario,
                        Patente = filtroPatente.IsEmpty() ? null : filtroPatente.ToUpper()
                    });
        }

        private void ListQuery(
            DatosUsuario datosUsuario, 
            FiltroExcepcionPagoTasaMunicipal filtro, 
            int pagina, 
            string ordenarPor, 
            DirOrden dirOrden)
        {
            var respuesta = this.BuscarExcepcionesPagoTasaMunicipal(datosUsuario.NombreUsuario, filtro, pagina, ordenarPor, dirOrden);

            if (!respuesta.HayErrores)
                ViewBag.Items = respuesta.ListaResultados;
        }

        private ResultadoConsultarExcepcionPagoTicketMunicipal BuscarExcepcionesPagoTasaMunicipal(
            string nombreUsuario, 
            FiltroExcepcionPagoTasaMunicipal filtro = default, 
            int pagina = 1, 
            string ordenarPor = "FechaCreacionExcepcion", 
            DirOrden dirOrden = DirOrden.Asc)
        {
            var respuesta = 
                servicioComandos.Ejecutar(new ConsultarExcepcionPagoTicketMunicipal
                {
                    Paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 25),
                    Patente = filtro?.Patente,
                    Usuario = nombreUsuario
                }) as ResultadoConsultarExcepcionPagoTicketMunicipal;

            return respuesta;
        }

        private ActionResult RefrescarResultados(Comando comando)
        {
            ActionResult actionResult = null;
            ResultadoConsultarExcepcionPagoTicketMunicipal respuesta;

            if (comando is ConsultarExcepcionPagoTicketMunicipal)
                respuesta = 
                    this.BuscarExcepcionesPagoTasaMunicipal(
                        comando.Usuario, 
                        new FiltroExcepcionPagoTasaMunicipal
                        {
                            Patente = (comando as ConsultarExcepcionPagoTicketMunicipal).Patente
                        });
            else
                respuesta = this.BuscarExcepcionesPagoTasaMunicipal(comando.Usuario);

            if (!respuesta.HayErrores)
            {
                ViewBag.Items = respuesta.ListaResultados;
                actionResult = PartialView("Listar", new FiltroExcepcionPagoTasaMunicipal());//exceptuados para la creación
            }
            else
            {
                ModelState.AgregarErrores(respuesta);
                actionResult = this.ErrorActionResult(HttpStatusCode.BadRequest, string.Join(",", respuesta.Errores.Values));
            }

            return actionResult;
        }

        private ActionResult ErrorActionResult(HttpStatusCode statusCode, string message = "Ocurrió un error en el servidor")
        {
            Response.StatusCode = (int)statusCode;
            Response.TrySkipIisCustomErrors = true;
            return Content(message);
        }

        #endregion

        #region -- Command methods -- 

        private ActionResult EjecutarComando(Comando comando)
        {
            ActionResult actionResult = null;

            if (ModelState.IsValid)
            {
                Resultado resultado = servicioComandos.Ejecutar(comando);

                if (!resultado.HayErrores)
                    actionResult = this.RefrescarResultados(comando);
                else
                {
                    ModelState.AgregarErrores(resultado);
                    actionResult = this.ErrorActionResult(HttpStatusCode.BadRequest, string.Join(",", resultado.Errores.Values));
                }
            }
            else
                actionResult = this.ErrorActionResult(HttpStatusCode.BadRequest);

            return actionResult;
        }

        #region -- Create --

        [HttpPost]
        [DatosUsuario]
        public ActionResult Crear(FiltroExcepcionPagoTasaMunicipal filtro, DatosUsuario datosUsuario)
        {
            ModelState.Remove("PatenteActual");
            ModelState.Remove("Id");

            return 
                this.EjecutarComando(
                    new CrearExcepcionPagoTasaMunicipal
                    {
                        ExceptuadosTicketMunicipalDto = new ExceptuadosTicketMunicipalDto
                        {
                            Patente = filtro.Patente,
                            NombreUsuario = datosUsuario.NombreUsuario
                        }, 
                        Usuario = datosUsuario.NombreUsuario
                    }
                );
        }

        #endregion

        #region -- Update --

        [HttpPost]
        [DatosUsuario]
        public ActionResult Modificar(FiltroExcepcionPagoTasaMunicipal filtro, DatosUsuario datosUsuario)
        {
            ModelState.Remove("Patente");

            return 
                this.EjecutarComando(
                    new ModificarExcepcionPagoTasaMunicipal
                    {
                        Id = filtro.FiltroEditar.Id,
                        Patente = filtro.FiltroEditar.PatenteActual,
                        Usuario = datosUsuario.NombreUsuario
                    });
        }

        #endregion

        #region -- Delete --

        [HttpPost]
        [DatosUsuario]
        public ActionResult Borrar(int idBorrar, DatosUsuario datosUsuario)
        {
            return 
                this.EjecutarComando(
                    new EliminarExcepcionPagoTasaMunicipal 
                    { 
                        Id = idBorrar, 
                        Usuario = datosUsuario.NombreUsuario 
                    });
        }

        #endregion

        #endregion

        #endregion
    }
}
