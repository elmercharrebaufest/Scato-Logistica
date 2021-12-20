using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AbmImpresionPorCentro)]
    public class DocumentoDeImpresionPorCentroController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public DocumentoDeImpresionPorCentroController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        [DatosUsuario]
        public ActionResult Index(string filtro, DatosUsuario datosUsuario, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListarConsulta(pagina,datosUsuario, ordenarPor, dirOrden, filtro);
            return View();
        }

        [AjaxOnly]
        [ActionName("Index")]
        [DatosUsuario]
        public ActionResult Listar(string filtro, DatosUsuario datosUsuario, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListarConsulta(pagina,datosUsuario, ordenarPor, dirOrden, filtro);
            return View("Listar");
        }

        private void ListarConsulta(int pagina, DatosUsuario datosUsuario, string ordenarPor, DirOrden dirOrden, string filtro)
        {
            var paginacion = new Paginacion(
                ordenarPor,
                dirOrden,
                pagina,
                10);

            ViewBag.Items = servicio.ListarPaginadoDocumentoDeImpresionPorCentro(paginacion, filtro, datosUsuario.CentroId);
        }

        [DatosUsuario]
        public ActionResult Modificar(int id, DatosUsuario datosUsuario)
        {
            SetearVista(datosUsuario);
            var tipo = servicio.ObtenerDocumentoDeImpresionPorCentro(id);
            return View(tipo);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Modificar(DocumentoDeImpresionPorCentroDto tipo, DatosUsuario datosUsuario)
        {
            ModelState["PuestoDeTrabajoId"].Errors.Clear();
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new ModificarDocumentoDeImpresionPorCentro { Dto = tipo, Usuario = datosUsuario.NombreUsuario });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            SetearVista(datosUsuario);
            return View(tipo);
        }

        [DatosUsuario]
        public ActionResult Crear(DatosUsuario datosUsuario)
        {
            SetearVista(datosUsuario);
            return View(new DocumentoDeImpresionPorCentroDto { CentroId = datosUsuario.CentroId});
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Crear(DocumentoDeImpresionPorCentroDto tipo, DatosUsuario datosUsuario)
        {
            ModelState["PuestoDeTrabajoId"].Errors.Clear();
            tipo.CentroId = datosUsuario.CentroId;
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new CrearDocumentoDeImpresionPorCentro { Dto = tipo, Usuario = datosUsuario.NombreUsuario});
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            SetearVista(datosUsuario);
            return View(tipo);
        }

        [HttpPost]
        [DatosUsuario]
        [Autorizacion(PermisosScato.BorrarImpresionPorCentro)]
        public ActionResult Eliminar(int id, DatosUsuario datosUsuario)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarDocumentoDeImpresionPorCentro { Id = id, Usuario = datosUsuario.NombreUsuario});
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        private void SetearVista(DatosUsuario datosUsuario)
        {
            ViewBag.DocumentosDeImpresion = servicio.ListarDocumentosDeImpresion().ToSelectList(f => f.Id.ToString(CultureInfo.InvariantCulture), f => f.Descripcion).OrderBy(x => x.Text);
            ViewBag.FormatosDeImpresion = servicio.ListarFormatosDeImpresion().ToSelectList(f => f.Id.ToString(CultureInfo.InvariantCulture), f => f.Descripcion).OrderBy(x => x.Text);
            ViewBag.Impresoras = servicio.ListarImpresoras(datosUsuario.CentroId).ToSelectList(f => f.Id.ToString(CultureInfo.InvariantCulture), f => f.Descripcion).OrderBy(x => x.Text);
            ViewBag.Puestos = servicio.ListarPuestosDeTrabajoPorCentro(datosUsuario.CentroId).ToSelectList(f => f.Id.ToString(CultureInfo.InvariantCulture), f => f.NombrePuesto).OrderBy(x => x.Text);
        }
    }
}
