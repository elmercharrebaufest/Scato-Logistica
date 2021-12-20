using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AbmUsuario)]
    public class UsuarioController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public UsuarioController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        public ActionResult Index(string filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(filtro, pagina, ordenarPor, dirOrden);
            return View();
        }

        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(string filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(filtro, pagina, ordenarPor, dirOrden);
            return View("Listar");
        }

        private void ListQuery(string filtro, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);
            ViewBag.Items = servicio.ListarPaginadoUsuarios(filtro, paginacion);
        }

        public ActionResult Crear(int id = 0)
        {
            UsuarioDto usuarioACopiar = null;
            if (id != 0)
            {            
                usuarioACopiar = servicio.ObtenerUsuario(id);
                usuarioACopiar.Id = 0;
                usuarioACopiar.NombreUsuario = "";
                usuarioACopiar.Apellido = "";
                usuarioACopiar.Nombre = "";
                usuarioACopiar.Email = "";
               
            }
            ViewBag.Roles = servicio.ListarRoles().OrderBy(p => p.Descripcion).ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
            ViewBag.Centros = servicio.ListarCentros().OrderBy(p => p.Descripcion).ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
            return View(usuarioACopiar);
        }

        [DatosUsuario]
        [HttpPost]
        public ActionResult Crear(UsuarioDto model, string roles, string centros, DatosUsuario datosUsuario)
        {
            model.Id = 0;
            if (ModelState.IsValid && roles != "" && centros != "")
            {
                var listaRoles = roles.FromJson<List<RolDto>>();
                var listaCentros = centros.FromJson<List<CentroDto>>();

                model.RolesAsociados = listaRoles;
                model.CentrosAsociados = listaCentros;

                var resultado = servicioComandos.Ejecutar(new CrearUsuario { Dto = model, Usuario = datosUsuario.NombreUsuario});
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            if (roles == "")
            {
                ModelState.AddModelError("RolesAsociados", string.Format(Textos.Error_Requerido, "Rol"));
            }
            if (centros == "")
            {
                ModelState.AddModelError("CentrosAsociados", string.Format(Textos.Error_Requerido,"Centro"));
            }

            ViewBag.Roles = servicio.ListarRoles().OrderBy(p => p.Descripcion).ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
            ViewBag.Centros = servicio.ListarCentros().OrderBy(p => p.Descripcion).ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
            return View(model);
        }

        public ActionResult Modificar(int id)
        {
            var aModificar = servicio.ObtenerUsuario(id);
            ViewBag.Roles = servicio.ListarRoles().OrderBy(p => p.Descripcion).ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
            ViewBag.Centros = servicio.ListarCentros().OrderBy(p => p.Descripcion).ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
            return View(aModificar);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Modificar(UsuarioDto model, string roles, string centros, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid && roles != "" && centros != "")
            {
                var listaRoles = roles.FromJson<List<RolDto>>();
                var listaCentros = centros.FromJson<List<CentroDto>>();

                model.RolesAsociados = listaRoles;
                model.CentrosAsociados = listaCentros;

                var resultado = servicioComandos.Ejecutar(new ModificarUsuario { Dto = model, Usuario = datosUsuario.NombreUsuario });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
            }
            if (roles == "")
            {
                ModelState.AddModelError("RolesAsociados", Textos.Error_Requerido);
            }
            if (centros == "")
            {
                ModelState.AddModelError("CentrosAsociados", Textos.Error_Requerido);
            }

            ViewBag.Roles = servicio.ListarRoles().OrderBy(p => p.Descripcion).ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
            ViewBag.Centros = servicio.ListarCentros().OrderBy(p => p.Descripcion).ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
            return View(model);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Eliminar(int id, DatosUsuario datosUsuario)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarUsuario { Id = id, Usuario = datosUsuario.NombreUsuario });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        public JsonResult ObtenerRoles(int id)
        {
            var usuario = servicio.ObtenerUsuario(id);
            var roles = usuario != null ? usuario.RolesAsociados.ToList() : null;
            return Json(roles, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerCentros(int id)
        {
            var usuario = servicio.ObtenerUsuario(id);
            var centros = usuario != null ? usuario.CentrosAsociados.ToList() : null;
            return Json(centros, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult GetExcel()
        //{
        //    var headers = new string[4];

        //    int index = 0;

        //    headers[index++] = Textos.Usuario_NombreUsuario;
        //    headers[index++] = Textos.Chofer_Apellido;
        //    headers[index++] = Textos.Chofer_Nombre;
        //    headers[index++] = Textos.Rol;
        //    headers[index++] = Textos.Permiso;

        //    return new ExcelResult(headers, this.gestorLotesService.ExportarConfigLotes(), Textos.ExcelPermisosUsuarios + ".xlsx", Textos.ExcelPermisosUsuarios);
        //}
    }
}
