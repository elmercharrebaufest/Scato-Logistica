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
    [Autorizacion(PermisosScato.AbmInhabilitacionCamion)]
    public class InhabilitacionCamionController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public InhabilitacionCamionController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
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

        private void ListQuery(string filtro, int centroId, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);
            ViewBag.Items = servicio.ListarInhabilitacionCamiones(filtro, centroId, paginacion);
        }
        
        public ActionResult Crear()
        {
            return View();
        }

        [DatosUsuario]
        [HttpPost]
        public ActionResult Crear(DatosUsuario datosUsuario, string archivosAdjuntos, InhabilitacionCamionDto model)
        {
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                var listaAdjuntos = archivosAdjuntos.FromJson<IList<AdjuntoDto>>();
                model.Adjuntos = listaAdjuntos;

                model.CentroId = datosUsuario.CentroId;
                model.NombreUsuarioResponsable = datosUsuario.NombreUsuario;
                var resultado = servicioComandos.Ejecutar(new CrearInhabilitacionCamion { Dto = model });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            var camionId = model.Id;
            ViewBag.Adjuntos = servicio.ListarAdjuntosCamion(camionId).ToList();
            return View(model);
        }

        public ActionResult Modificar(int id)
        {
            var camionInhabilitado = servicio.ObtenerInhabilitacionCamion(id);
            var camionId = camionInhabilitado.Id;
            ViewBag.Adjuntos = servicio.ListarAdjuntosCamion(camionId).ToList();
            return View(camionInhabilitado);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Modificar(InhabilitacionCamionDto model, string archivosAdjuntos, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var listaAdjuntos = archivosAdjuntos.FromJson<IList<AdjuntoDto>>();
                model.Adjuntos = listaAdjuntos;

                model.NombreUsuarioResponsable = datosUsuario.NombreUsuario;
                var resultado = servicioComandos.Ejecutar(new ModificarInhabilitacionCamion() { Dto = model, Usuario = datosUsuario.NombreUsuario });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            var camionId = model.Id;
            ViewBag.Adjuntos = servicio.ListarAdjuntosCamion(camionId).ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarInhabilitacionCamion { Id = id });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        public JsonResult ObtenerAdjuntos(int id)
        {
            var camionInhabilitado = servicio.ObtenerInhabilitacionCamion(id);
            if(camionInhabilitado != null)
            {
                var camionId = camionInhabilitado.Id;
                var adjuntos = servicio.ListarAdjuntosCamion(camionId).ToList();
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

            return View("Historico", new HistoricoInhabilitacionCamionDto
            {
                Historico = servicio.ListarHistoricoInhabilitacionCamion(id).OrderByDescending(x => x.Fecha).ToList(),
                Autorizacion = servicio.ListarAutorizacionCamionPorInhabilitacion(id).OrderByDescending(x => x.Fecha).ToList()
        });
        }
    }
}
