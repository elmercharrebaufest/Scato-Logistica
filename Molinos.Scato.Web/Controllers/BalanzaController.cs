using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
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
    [Autorizacion(PermisosScato.AbmBalanza)]
    public class BalanzaController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioOrquestador orquestador;
        private readonly IServicioNotificarUsuario notificador;
        public BalanzaController(ILogger log, IServicioNotificarUsuario notificador, IServicioRepositorio servicio, IServicioComandos servicioComandos, IServicioOrquestador orquestador)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
            this.orquestador = orquestador;
            this.notificador = notificador;
        }
        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, string filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListarConsulta(datosUsuario, filtro, pagina, ordenarPor, dirOrden);
            return View();
        }
        [DatosUsuario]
        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(DatosUsuario datosUsuario, string filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListarConsulta(datosUsuario, filtro, pagina, ordenarPor, dirOrden);
            return View("Listar");
        }

        private void ListarConsulta(DatosUsuario datosUsuario, string filtro, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(
                ordenarPor,
                dirOrden,
                pagina,
                10);

            ViewBag.Items = servicio.ListarPaginadoBalanza(filtro, datosUsuario.CentroId, paginacion);
        }

        public ActionResult Modificar(int id)
        {
            var tipo = servicio.ObtenerBalanza(id);
            ViewBag.Balanzas = orquestador.ListarBalanzas().ToSelectList(x => x.Codigo, x => x.Descripcion);
            return View(tipo);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Modificar(DatosUsuario datosUsuario, BalanzaDto model)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new ModificarBalanza { Dto = model, Usuario = datosUsuario.NombreUsuario});
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            ViewBag.Balanzas = orquestador.ListarBalanzas().ToSelectList(x => x.Codigo, x => x.Descripcion);
            return View(model);
        }
        
        [HttpPost]
        [DatosUsuario]
        public ActionResult Eliminar(int id, DatosUsuario datosUsuario)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarBalanza { Id = id, Usuario = datosUsuario.NombreUsuario});
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        
        public ActionResult Modalidad(DatosUsuario datosUsuario, int balanzaId, Modalidad modalidad)
        {
            var nombre = servicio.ObtenerBalanzaNombre(balanzaId);
            var model = new BalanzaModificacionModalidadDto { BalanzaId = balanzaId, BalanzaNombre = nombre, Modalidad = modalidad };
            return View(model);
        }

        [DatosUsuario]
        [HttpPost]
        [ActionName("Modalidad")]
        public ActionResult ModificarModalidad(DatosUsuario datosUsuario, BalanzaModificacionModalidadDto model)
        {
            if (ModelState.IsValid)
            {
                model.Fecha = DateTime.Now;
                model.NombreUsuarioResponsable = datosUsuario.NombreUsuario;
                
                var resultado = servicioComandos.Ejecutar(new CrearBalanzaModificarModalidad { Dto = model });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                
                ModelState.AgregarErrores(resultado);
            }
            return View(model);
        }

        [DatosUsuario]
        public ActionResult Crear(DatosUsuario datosUsuario)
        {
            ViewBag.Id = 0;
            ViewBag.CentroId = datosUsuario.CentroId;
            ViewBag.Balanzas = orquestador.ListarBalanzas().ToSelectList(x => x.Codigo, x => x.Descripcion);
            return View();
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Crear(BalanzaDto model, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new CrearBalanza { Dto = model, Usuario = datosUsuario.NombreUsuario});
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            ViewBag.Balanzas = orquestador.ListarBalanzas().ToSelectList(x => x.Codigo, x => x.Descripcion);
            return View(model);
        }

        private void SetearITCDropDownList()
        {
           //ViewBag.Itc = 
        }

        [DatosUsuario]
        public ActionResult CerearBalanza(int balanzaId, DatosUsuario datosUsuario)
        {
            var balanza = servicio.ObtenerBalanza(balanzaId);
            if (balanza.Modalidad != Dominio.Enums.Modalidad.Manual)
            {
                var resultado = orquestador.Ejecutar(new EjecutarCereoCabezal { CodigoDispositivo = balanza.CodigoCabezal });
                if (resultado.Mensaje.Codigo != 0)
                {
                    var htmlIconoColor = new StringBuilder();
                    htmlIconoColor.AppendLine("<span class=\"iconoColor\" style=\"background-color:");
                    htmlIconoColor.AppendLine(balanza.Color);
                    htmlIconoColor.AppendLine(";\">&nbsp;&nbsp;&nbsp;&nbsp;</span>");

                    notificador.Notificar(new NotificacionDto
                    {
                        Grupo = datosUsuario.CentroId + "|" + PermisosScato.Administradores.ToString(), //TODO : rever  grupo!
                        Mensaje = String.Format(Textos.Notificacion_PendienteCereo, balanza.Nombre, htmlIconoColor),
                        TipoAlerta = TipoAlerta.Sobre
                    });
                    return Json(new { data = resultado.Mensaje.Descripcion }, JsonRequestBehavior.AllowGet);
                }
            }
            var resultadoRepositorio = ActualizarEstadoBalanzaCereada(balanzaId);
            return Json(new { data = !resultadoRepositorio.HayErrores ? "true" : resultadoRepositorio.Errores.Values.FirstOrDefault() }, JsonRequestBehavior.AllowGet);
        }

        private Resultado ActualizarEstadoBalanzaCereada(int balanzaId)
        {
            var resultado = servicioComandos.Ejecutar(new ModificarBalanzaEstaEnCero
            {
                BalanzaId = balanzaId,
                EstaEnCero = true
            });
            if (resultado.HayErrores)
            {
                return resultado;
            }
            var cookie = new CookieUsuario();
            cookie.ActualizarValor("BalanzaId", balanzaId.ToString(CultureInfo.InvariantCulture));

            return resultado;
        }
    }
}
