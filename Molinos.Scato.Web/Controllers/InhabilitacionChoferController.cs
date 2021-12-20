using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AbmInhabilitacionChofer)]
    public class InhabilitacionChoferController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public InhabilitacionChoferController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, string filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(filtro, datosUsuario.CentroId, pagina, ordenarPor, dirOrden);
            return View((object)filtro);
        }

        [DatosUsuario]
        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult ListarDatos(DatosUsuario datosUsuario, string filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(filtro, datosUsuario.CentroId, pagina, ordenarPor, dirOrden);
            return View("Listar", (object)filtro);
        }

        private void ListQuery(string filtro,int centroId, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);
            ViewBag.Items = servicio.ListarInhabilitacionChoferes(filtro, centroId, paginacion);
        }

        public ActionResult Crear()
        {
            SetearTiposDocumentoIdentidadDropDownList();
            return View();
        }
        [DatosUsuario]
        [HttpPost]
        public ActionResult Crear(DatosUsuario datosUsuario,string archivosAdjuntos, InhabilitacionChoferDto model)
        {
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                var listaAdjuntos = archivosAdjuntos.FromJson<IList<AdjuntoDto>>();
                model.Adjuntos = listaAdjuntos;

                model.CentroId = datosUsuario.CentroId;
                model.NombreUsuarioResponsable = datosUsuario.NombreUsuario;
                var resultado = servicioComandos.Ejecutar(new CrearInhabilitacionChofer { Dto = model });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }

            var choferId = model.Id;
            ViewBag.Adjuntos = servicio.ListarAdjuntosChofer(choferId).ToList();
            SetearTiposDocumentoIdentidadDropDownList();
            return View(model);
        }

        public ActionResult Modificar(int id)
        {
            var choferInhabilitado = servicio.ObtenerInhabilitacionChofer(id);
            SetearTiposDocumentoIdentidadDropDownList();
            SetearTipoDocumentoString(choferInhabilitado.TipoDocumentoIdentidadId);
            var choferId = choferInhabilitado.Id;
            ViewBag.Adjuntos = servicio.ListarAdjuntosChofer(choferId).ToList();
            return View(choferInhabilitado);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Modificar(InhabilitacionChoferDto model, string archivosAdjuntos, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var listaAdjuntos = archivosAdjuntos.FromJson<IList<AdjuntoDto>>();
                model.Adjuntos = listaAdjuntos;

                model.NombreUsuarioResponsable = datosUsuario.NombreUsuario;
                var resultado = servicioComandos.Ejecutar(new ModificarInhabilitacionChofer() { Dto = model, Usuario = datosUsuario.NombreUsuario });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            SetearTiposDocumentoIdentidadDropDownList();
            SetearTipoDocumentoString(model.TipoDocumentoIdentidadId);
            var choferId = model.Id;
            ViewBag.Adjuntos = servicio.ListarAdjuntosChofer(choferId).ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarInhabilitacionChofer { Id = id });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        private void SetearTipoDocumentoString(int tipoDocumentoIdentidadId)
        {
            var tipo = ((IEnumerable<SelectListItem>)ViewBag.TiposDocumentoIdentidad).SingleOrDefault(x => x.Value == tipoDocumentoIdentidadId.ToString());
            ViewBag.TipoDocumentoIdentidad = tipo != null ? tipo.Text : "";
        }

        private void SetearTiposDocumentoIdentidadDropDownList()
        {
            ViewBag.TiposDocumentoIdentidad = servicio.ListarTiposDocumentoIdentidad().ToSelectList(x => x.Id.ToString(), x => x.DescripcionCorta);
        }

        public JsonResult ObtenerAdjuntos(int id)
        {
            var choferInhabilitado = servicio.ObtenerInhabilitacionChofer(id);
            
            if (choferInhabilitado != null)
            {
                var choferId = choferInhabilitado.Id;
                var adjuntos = servicio.ListarAdjuntosChofer(choferId).ToList();
                return Json(adjuntos, JsonRequestBehavior.AllowGet);
            }
            return Json(new List<AdjuntoDto>(), JsonRequestBehavior.AllowGet);
        }


        public FileResult DescargarArchivos(int id)
        {
            try
            {
                System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("RetornoArchivo", "ok"));


                var file = servicio.ObtenerAdjunto(id);
                var archivo = Convert.FromBase64String(file.Archivo.Split(new string[] { "base64," }, StringSplitOptions.None)[1]);

                return File(archivo, System.Net.Mime.MediaTypeNames.Application.Octet, file.Descripcion);
            }
            catch (Exception e)
            {
                System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("RetornoArchivo", "error"));
                return null;
            }
        }

        public ActionResult Listar(int id)
        {

            return View("Historico", new HistoricoInhabilitacionChoferDto
            {
                Historico = servicio.ListarHistoricoInhabilitacionChofer(id).OrderByDescending(x => x.Fecha).ToList(),
                Autorizacion = servicio.ListarAutorizacionChoferPorInhabilitacion(id).OrderByDescending(x => x.Fecha).ToList()

            });
        }
        
    } 
}
