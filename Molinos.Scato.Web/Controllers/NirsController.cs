using System;
using System.Linq;
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
    [Autorizacion(PermisosScato.AbmNirs)]
    public class NirsController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioOrquestador orquestador;

        public NirsController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos, IServicioOrquestador orquestador)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
            this.orquestador = orquestador;
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

            ViewBag.Items = servicio.ListarPaginadoNirs(filtro, datosUsuario.CentroId, paginacion);
        }

        [DatosUsuario]
        public ActionResult Crear()
        {
            ViewBag.Nirs = orquestador.ListarNirs().ToSelectList(x => x.Codigo, x => x.Descripcion);
            return View();
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Crear(NirsDto model, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                model.CentroId = datosUsuario.CentroId;
                if (model.Modalidad == Dominio.Enums.Modalidad.Automática && model.Codigo == null)
                {
                    ModelState.AddModelError("Codigo", String.Format(Textos.Error_Requerido, Textos.Codigo));
                    ViewBag.Nirs = orquestador.ListarNirs().ToSelectList(x => x.Codigo, x => x.Descripcion);
                    return View(model);
                }
                if (model.Modalidad == Dominio.Enums.Modalidad.Manual && model.Codigo == null)
                {
                    model.Codigo = String.Empty;
                }
                var resultado = servicioComandos.Ejecutar(new CrearNirs { Dto = model, Usuario = datosUsuario.NombreUsuario});
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            ViewBag.Nirs = orquestador.ListarNirs().ToSelectList(x => x.Codigo, x => x.Descripcion);
            return View(model);
        }

        public ActionResult Modificar(int id)
        {
            var aModificar = servicio.ObtenerNirs(id);
            ViewBag.Nirs = orquestador.ListarNirs().ToSelectList(x => x.Codigo, x => x.Descripcion);
            return View(aModificar);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Modificar(NirsDto model, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                model.CentroId = datosUsuario.CentroId;
                if (model.Modalidad == Dominio.Enums.Modalidad.Automática && model.Codigo == null)
                {
                    ModelState.AddModelError("Codigo", String.Format(Textos.Error_Requerido,Textos.Codigo));
                    ViewBag.Nirs = orquestador.ListarNirs().ToSelectList(x => x.Codigo, x => x.Descripcion);
                    return View(model);
                }
                if (model.Modalidad == Dominio.Enums.Modalidad.Manual && model.Codigo == null)
                {
                    model.Codigo = String.Empty;
                }
                var resultado = servicioComandos.Ejecutar(new ModificarNirs { Dto = model, Usuario = datosUsuario.NombreUsuario});
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            ViewBag.Nirs = orquestador.ListarNirs().ToSelectList(x => x.Codigo, x => x.Descripcion);
            return View(model);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Eliminar(int id, DatosUsuario datosUsuario)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarNirs { Id = id, Usuario = datosUsuario.NombreUsuario});
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        public ActionResult Modalidad(int nirsId, Modalidad modalidad)
        {
            var nirs = servicio.ObtenerNirs(nirsId);
            var model = new NirsModificacionModalidadDto { NirsId = nirsId, NirsDescripcion = nirs.Descripcion, Modalidad = modalidad };
            return View(model);
        }

        [DatosUsuario]
        [HttpPost]
        [ActionName("Modalidad")]
        public ActionResult ModificarModalidad(DatosUsuario datosUsuario, NirsModificacionModalidadDto model)
        {
            if (ModelState.IsValid)
            {
                model.Fecha = DateTime.Now;
                model.NombreUsuarioResponsable = datosUsuario.NombreUsuario;

                var resultado = servicioComandos.Ejecutar(new CrearNirsModificarModalidad { Dto = model });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }

                ModelState.AgregarErrores(resultado);
            }
            return View(model);
        }
    }
}
