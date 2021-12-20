using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AbmReimpresionDeDocumentos)]
    public class ReimpresionDeDocumentosController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public ReimpresionDeDocumentosController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, int pagina = 1)
        {
            ViewBag.Items = new ListaPaginada<ImpresionDto>(new List<ImpresionDto>(), pagina, 10, 0);
            ViewBag.Patente = "";
            ViewBag.Error = false;
            ViewBag.Impresoras = servicio.ListarImpresoras(datosUsuario.CentroId).ToSelectList(x => x.Id.ToString(), x => x.Descripcion);
            ViewBag.CantCopias = new List<int> {1, 2, 3, 4, 5, 6, 7, 8, 9, 10}.ToSelectList(x => x.ToString(),
                                                                                            x => x.ToString());
            return View();
        }

        [AjaxOnly]
        //[HttpPost]
        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, ReimpresionDeDocumentosDto model, int pagina = 1, string ordenarPor = "FechaImpresion", DirOrden dirOrden = DirOrden.Desc)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);
            var documentos = new ListaPaginada<ImpresionDto>(new List<ImpresionDto>(), 1, 1, 0);
            
            if (ModelState.IsValid)
            {
                documentos = servicio.ListarImpresiones(model.TipoDocumentoIngreso, model.NumeroDocumentoIngreso,model.Patente, model.Tipo, paginacion);
                if (!documentos.Any())
                {
                    ModelState.AddModelError("NumeroDocumentoIngreso", Textos.Reimpresion_DocumentosNoEncontrados);
                }
            }

            ViewBag.Items = documentos;
            ViewBag.Impresoras = servicio.ListarImpresoras(datosUsuario.CentroId).ToSelectList(x => x.Id.ToString(), x => x.Descripcion);
            return View("Listar", model);
        }

        [DatosUsuario]
        public ActionResult Imprimir(DatosUsuario datosUsuario, int id, int impresora, int cantCopias)
        {
            try
            {
                var resultado =
                    servicioComandos.Ejecutar(new ImprimirDocumento
                        {
                            Id = id,
                            Impresora = impresora,
                            CentroId = datosUsuario.CentroId,
                            CantCopias = cantCopias
                        });
                return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.FirstOrDefault());
            }
            catch (Exception e)
            {
                return Content(Textos.ReimpresionDeDocumento_ImprimirError + e.Message);
            }            
        }

        [DatosUsuario]
        public ActionResult Previsualizar(DatosUsuario datosUsuario, int id)
        {
            try
            {
                var resultado =
                    servicioComandos.Ejecutar(new ImprimirDocumento
                    {
                        Id = id,
                        Impresora = 0,
                        CentroId = datosUsuario.CentroId,
                        CantCopias = 1
                    });
                if (System.Web.HttpContext.Current != null)
                {
                    System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("RetornoExportacion", "ok"));
                }
                if (!resultado.HayErrores)
                {   
                    byte[] file = ((ResultadoPrevisualizar) resultado).Archivo;
                    return File(file, "application/octet-stream", "vistaPrevia.pdf");
                }
                return Content(resultado.Errores.Values.FirstOrDefault());
            }
            catch (Exception e)
            {
                return Content(Textos.ReimpresionDeDocumento_ImprimirError + e.Message);
            }
        }

        public ActionResult Eliminar(int id)
        {
            try
            {
                var resultado = servicioComandos.Ejecutar(new EliminarDocumento { Id = id });
                return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.FirstOrDefault());
            }
            catch (Exception e)
            {
                return Content(e.Message);
            } 
        }
    }
}
