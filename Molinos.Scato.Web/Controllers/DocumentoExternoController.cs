using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Newtonsoft.Json;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.DocumentoExterno)]
    public class DocumentoExternoController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public DocumentoExternoController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }
        public ActionResult Index(string filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Desc) {
            ListQuery(filtro, pagina, ordenarPor, dirOrden);
            return View((object)filtro);
        }

        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(string filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Desc)
        {
            ListQuery(filtro, pagina, ordenarPor, dirOrden);
            return View("Listar", (object)filtro);
        }

        private void ListQuery(string filtro, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);

            ViewBag.Items = servicio.ListarDocumentos(filtro, paginacion);
        }

        public ActionResult ObtenerNrosCP()
        {
            var cpsEncontrados = new List<dynamic>();

            if (Request.Files.Count > 0)
            {
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var nombreArchivo = Path.GetFileName(Request.Files[i].FileName);
                    var nroCp = ObtenerNroCP(Request.Files[i]);

                    cpsEncontrados.Add(new { nombreArchivo, nroCp });
                }
            }

            return new JsonResult() { Data = new { CartasDePorte = cpsEncontrados } };
        }

        public ActionResult GuardarCartasDePorte(DatosUsuario datosUsuario, string cartasDePorte)
        {
            var cps = JsonConvert.DeserializeObject<DocumentoExternoDto[]>(cartasDePorte);
            var cpsError = new List<string>();

            if (Request.Files.Count > 0)
            {
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var cp = cps.First(x => x.NombreArchivoOriginal == Path.GetFileName(Request.Files[i].FileName));
                    if (!string.IsNullOrEmpty(cp.NumeroDeDocumento))
                    {
                        System.IO.Stream fs = Request.Files[i].InputStream;
                        System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
                        Byte[] bytes = br.ReadBytes((Int32)fs.Length);
                        cp.ArchivoExtension = Path.GetFileName(Request.Files[i].FileName).Split('.').Last();
                        cp.ArchivoRutaDestino = Convert.ToBase64String(bytes, 0, bytes.Length);
                        
                        cpsError.AddRange(GuardarCP(cp, datosUsuario));
                    }
                }
            }

            return new JsonResult() { Data = new { Errores = cpsError } };
        }

        private IList<string> GuardarCP(DocumentoExternoDto dto, DatosUsuario datosUsuario)
        {
            dto.ArchivoRutaDestino = servicio.GuardarArchivoFotoDocumentoExterno(dto.ArchivoRutaDestino, dto.ArchivoExtension, dto.NumeroDeDocumento);
            var resultado = servicioComandos.Ejecutar(new CrearDocumentoExterno { Dto = dto, Usuario = datosUsuario.NombreUsuario });
            var ret = new List<string>();

            if (resultado.HayErrores)
            {
                ret.AddRange(resultado.Errores.Values);
            }

            return ret;
        }

        private string ObtenerNroCP(HttpPostedFileBase file)
        {
            System.IO.Stream fs = file.InputStream;
            System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
            Byte[] bytes = br.ReadBytes((Int32)fs.Length);

            var leerNumeroCartaPorteComando = new LeerNumeroCartaPorte
            {
                CodigoBarrasCartaPorte = bytes,
                NombreArchivo = Path.GetFileName(file.FileName),
                CalcularRecorte = true
            };

            var resultado = (ResultadoLeerNumeroCartaPorte)servicioComandos.Ejecutar(leerNumeroCartaPorteComando);
            return resultado.NumeroCartaPorte;
        }

        public FileResult DescargarArchivos(int id)
        {
            try
            {
                var file = servicio.ObtenerAdjuntoDocumentoExterno(id);
                System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("RetornoArchivo", "ok"));

                return File(file.Archivo, System.Net.Mime.MediaTypeNames.Application.Octet, file.NumeroDeDocumento + "." + file.ArchivoExtension);
            }
            catch (Exception e)
            {
                System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("RetornoArchivo", "error"));
                return null;
            }
        }

        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarDocumentoExterno { Id = id });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        public ActionResult Modificar(int id)
        {
            var tipo = servicio.ObtenerDocumentoExterno(id);
            return View(tipo);
        }

        [HttpPost]
        public ActionResult Modificar(DocumentoExternoDto tipo)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new ModificarDocumentoExterno { Dto = tipo });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            return View(tipo);
        }
    }
}