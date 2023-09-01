using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExcelDataReader;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Molinos.Scato.Web.Controllers
{

    [Autorizacion(PermisosScato.EtiquetaPuerto)]
    public class EtiquetaPuertoController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public EtiquetaPuertoController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        //[DatosUsuario]
        //[HttpGet]
        //public ActionResult Index(DatosUsuario datosUsuario)
        //{
        //    //var usuario = servicio.ObtenerUsuarioId(datosUsuario.NombreUsuario);
        //    //ViewBag.Impresoras = servicio.ListarImpresoras(datosUsuario.CentroId).Where(x => x.IsZebra).ToSelectList(x => x.Id.ToString(), x => x.Descripcion).OrderBy(x => x.Text);
        //    var paginacion = new Paginacion("Fecha", DirOrden.Asc, 1, 10);
        //    //var items = servicio.ListarEtiquetasPuerto(usuario.Id, paginacion);
        //    //ViewBag.Items = items;
        //    //ViewBag.HayItems = items.Count() > 0 ? "true" : "false";
        //    SetViewBag(datosUsuario, paginacion);
        //    return View();
        //}

        private void SetViewBag(DatosUsuario datosUsuario, Paginacion paginacion)
        {
            var usuario = servicio.ObtenerUsuarioId(datosUsuario.NombreUsuario);
            ViewBag.Impresoras = servicio.ListarImpresoras(datosUsuario.CentroId).Where(x => x.IsZebra).ToSelectList(x => x.Id.ToString(), x => x.Descripcion).OrderBy(x => x.Text);
            var items = servicio.ListarEtiquetasPuerto(usuario.Id, paginacion);
            ViewBag.Items = items;
            ViewBag.HayItems = items.Count() > 0 ? "true" : "false";
        }

        [DatosUsuario]
        [ActionName("Index")]
        public ActionResult Listar(DatosUsuario datosUsuario, int pagina = 1, string ordenarPor = "Fecha", DirOrden dirOrden = DirOrden.Desc)
        {
            //var usuario = servicio.ObtenerUsuarioId(datosUsuario.NombreUsuario);
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);

            //ViewBag.Impresoras = servicio.ListarImpresoras(datosUsuario.CentroId).ToSelectList(x => x.Id.ToString(), x => x.Descripcion).OrderBy(x => x.Text);
            //var items = servicio.ListarEtiquetasPuerto(usuario.Id, paginacion);
            //ViewBag.Items = items;
            //ViewBag.HayItems = items.Count() > 0 ? "true" : "false";
            SetViewBag(datosUsuario, paginacion);

            return View(Request.QueryString.Count > 0 ? "Listar" : "Index");
        }

        public FileResult DescargarTemplate()
        {
            try
            {
                var file = System.IO.File.ReadAllBytes(Server.MapPath("~/Content/templates/TemplateEtiquetaPuerto.xlsx"));

                return File(file, System.Net.Mime.MediaTypeNames.Application.Octet, "EtiquetaPuerto.xlsx");
            }
            catch (Exception e)
            {
                return null;
            }
        }

        [HttpPost]
        [DatosUsuario]
        [ActionName("Index")]
        public ActionResult SubirArchivo(DatosUsuario datosUsuario)
        {
            var usuario = servicio.ObtenerUsuarioId(datosUsuario.NombreUsuario);

            if (Request.Files?.Count > 0)
            {
                var resultado = ProcesarArchivoResultado(Request.Files[0].InputStream, usuario.Id);
                if (resultado.HayErrores)
                {
                    ModelState.AgregarErrores(resultado);
                }
                else
                {
                    ViewBag.success = "Se grabó correctamente";
                }
            }
            else
            {
                ModelState.AddModelError("", "Seleccione un archivo");
            }

            //ViewBag.Impresoras = servicio.ListarImpresoras(datosUsuario.CentroId).ToSelectList(x => x.Id.ToString(), x => x.Descripcion).OrderBy(x => x.Text);
            var paginacion = new Paginacion("Fecha", DirOrden.Asc, 1, 10);
            //var items = servicio.ListarEtiquetasPuerto(usuario.Id, paginacion);
            //ViewBag.Items = items;
            //ViewBag.HayItems = items.Count() > 0 ? "true" : "false";
            SetViewBag(datosUsuario, paginacion);
            return View("Index");
        }

        private Resultado ProcesarArchivoResultado(Stream stream, int usuarioId)
        {
            var resultado = new Resultado();
            try
            {
                List<string> errores = new List<string>();

                var etiquetas = CargarArchivo(stream, errores);

                foreach (var error in errores)
                {
                    resultado.Error(string.Empty, error);
                }

                if (errores.Count == 0)
                {
                    servicioComandos.Ejecutar(new EliminarEtiquetaPuerto() { UsuarioId = usuarioId });

                    foreach (var etiqueta in etiquetas)
                    {
                        etiqueta.Usuario_Id = usuarioId;

                        var result = servicioComandos.Ejecutar(new GuardarEtiquetaPuerto()
                        {
                            Etiqueta = etiqueta
                        });

                        if (result.HayErrores)
                        {
                            result.Errores.ToList().ForEach(x => resultado.Errores.Add("", x.Value));
                        }
                    }
                }                
            }
            catch (Exception ex)
            {
                log.Error(ex, "archivo etiquetas puerto ");
                resultado.Errores.Add("", ex.Message);
            }
            return resultado;
        }

        private static List<ImpEtiquetaPuertoDto> CargarArchivo(Stream stream, List<string> errores)
        {
            var doc = new XSSFWorkbook(stream);
            var result = new List<ImpEtiquetaPuertoDto>();

            ISheet sheet = doc.GetSheetAt(0);

            for (int row = 2; row <= sheet.LastRowNum; row++) //skip headers
            {
                var r = sheet.GetRow(row);

                if(r != null)
                {
                    var formatoError = "La fila {0} tiene los campos {1} incorrectos o incompletos.";
                    var columnasErrores = new List<string>();

                    var Vapor = r.GetCell(0) != null ? r.GetCell(0).ToString() : string.Empty;
                    var Cargador = r.GetCell(1) != null ? r.GetCell(1).ToString() : string.Empty;
                    var Mercaderia = r.GetCell(2) != null ? r.GetCell(2).ToString() : string.Empty;
                    var Destino = r.GetCell(3) != null ? r.GetCell(3).ToString() : string.Empty;
                    var Kg = r.GetCell(4) != null ? r.GetCell(4).ToString() : string.Empty;
                    var NumeroLote = r.GetCell(5) != null ? r.GetCell(5).ToString() : string.Empty;
                    var Bodega = r.GetCell(6) != null ? r.GetCell(6).ToString() : string.Empty;
                    var Control = r.GetCell(7) != null ? r.GetCell(7).ToString() : string.Empty;
                    var Fecha = r.GetCell(8) != null && r.GetCell(8).CellType == CellType.Numeric ? r.GetCell(8).DateCellValue : (DateTime?)null;
                    var FechaStr = r.GetCell(8) != null ? r.GetCell(8).ToString() : string.Empty;

                    if (string.IsNullOrEmpty(Vapor)) columnasErrores.Add("Vapor");
                    if (Fecha == null && !string.IsNullOrEmpty(FechaStr)) columnasErrores.Add("Fecha");
                    
                    int kgNum = 0;
                    int.TryParse(Kg, out kgNum);
                    if (kgNum <= 0 && !string.IsNullOrEmpty(Kg)) columnasErrores.Add("Kg");

                    var filaStr = Vapor?.Trim() + Cargador.Trim() + Mercaderia.Trim() + Destino.Trim() + Kg?.Trim() + NumeroLote.Trim() + Bodega.Trim() + Control.Trim() + FechaStr?.Trim();

                    if (columnasErrores.Count == 0)
                    {
                        NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
                        nfi.CurrencyDecimalSeparator = ",";
                        nfi.CurrencyGroupSeparator = ".";
                        nfi.CurrencySymbol = "";

                        result.Add(new ImpEtiquetaPuertoDto()
                        {
                            Vapor = Vapor,
                            Cargador = Cargador,
                            Mercaderia = Mercaderia,
                            Destino = Destino,
                            Kg = kgNum > 0 ? Convert.ToDecimal(kgNum).ToString("C0",nfi) : string.Empty,
                            NumeroLote = NumeroLote,
                            Bodega = Bodega,
                            Control = Control,
                            Fecha = Fecha
                        });
                    }
                    else if(!string.IsNullOrEmpty(filaStr))
                    {
                        errores.Add(string.Format(formatoError, row + 1, string.Join(", ", columnasErrores.ToArray())));
                    }
                }
            }

            return result;
        }

        [DatosUsuario]
        public ActionResult Imprimir(DatosUsuario datosUsuario, int impresora)
        {
            try
            {
                var usuario = servicio.ObtenerUsuarioId(datosUsuario.NombreUsuario);

                var resultado = servicioComandos.Ejecutar(new ImprimirEtiquetaPuerto
                {
                    UsuarioId = usuario.Id,
                    ImpresoraId = impresora,
                    CentroId = datosUsuario.CentroId,
                });

                return Content(resultado.HayErrores ? "true" : resultado.Errores.Values.FirstOrDefault());
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
                var usuario = servicio.ObtenerUsuarioId(datosUsuario.NombreUsuario);

                var resultado =
                    servicioComandos.Ejecutar(new ImprimirEtiquetaPuerto
                    {
                        UsuarioId = 0,
                        ImpresoraId = 0,
                        CentroId = datosUsuario.CentroId,
                        Id = id
                    });
                if (System.Web.HttpContext.Current != null)
                {
                    System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("RetornoExportacion", "ok"));
                }
                if (!resultado.HayErrores)
                {
                    byte[] file = ((ResultadoPrevisualizar)resultado).Archivo;
                    return File(file, "application/octet-stream", "vistaPrevia.pdf");
                }
                return Content(resultado.Errores.Values.FirstOrDefault());
            }
            catch (Exception e)
            {
                return Content(Textos.ReimpresionDeDocumento_ImprimirError + e.Message);
            }
        }
    }
}
