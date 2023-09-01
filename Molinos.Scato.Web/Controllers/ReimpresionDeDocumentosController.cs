using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
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
                try
                {
                    documentos = servicio.ListarImpresiones(model.TipoDocumentoIngreso, model.NumeroDocumentoIngreso, model.Patente, model.Tipo, paginacion);
                    if (!documentos.Any())
                    {
                        ModelState.AddModelError("NumeroDocumentoIngreso", Textos.Reimpresion_DocumentosNoEncontrados);
                    }
                }
                catch (Exception e)
                {
                    log.Error(e, "Error ListarImpresiones");
                    ModelState.AddModelError("NumeroDocumentoIngreso", "Ocurrio un error al realizar la consulta.");
                }
            }

            ViewBag.Items = documentos;
            ViewBag.Impresoras = servicio.ListarImpresoras(datosUsuario.CentroId).ToSelectList(x => x.Id.ToString(), x => x.Descripcion);
            return View("Listar", model);
        }

        [DatosUsuario]
        public ActionResult Imprimir(DatosUsuario datosUsuario, int id, int impresora, int cantCopias, string ctg = null, string ctgdg = null)
        {
            try
            {
                if(id == default(int) && !string.IsNullOrEmpty(ctg))
                {
                    var impresoraModel = servicio.ObtenerImpresora(impresora);
                    var cartaPorte = servicioComandos.Ejecutar(new ConsultarPDFCpe { NroCtg = Convert.ToInt64(ctg) }) as ResultadoConsultarPDFCpe;
                    if (cartaPorte != null && !cartaPorte.HayErrores)
                    {
                        var resultado = 
                        servicioComandos.Ejecutar(new ImprimirFileGenerico {
                            CantidadCopias = cantCopias,
                            File = cartaPorte.Pdf,
                            Impresora = impresoraModel?.Direccion,
                            CodigoDocumentoImpresion = Enum.GetName(typeof(TipoImpresion), TipoImpresion.CartaDePorteElectronica)
                        });

                        return Content("true");
                    }
                    else
                    {
                        return Content(cartaPorte.Errores.Values.FirstOrDefault());
                    }
                } else if (id == default(int) && !string.IsNullOrEmpty(ctgdg))
                {
                    var impresoraModel = servicio.ObtenerImpresora(impresora);
                    var cartaPorteDerivadoGranario = servicio.ObtenerCartaPorteDerivadoGranarioPorCTG(ctgdg);
                    if (cartaPorteDerivadoGranario != null && !string.IsNullOrEmpty(cartaPorteDerivadoGranario.RutaFotoCPEDG) && System.IO.File.Exists(cartaPorteDerivadoGranario.RutaFotoCPEDG))
                    {
                        byte[] pdf = null;
                        var document = new Document();
                        using (var stream = new MemoryStream())
                        {
                            PdfWriter.GetInstance(document, stream);
                            document.Open();
                            Image image = Image.GetInstance(cartaPorteDerivadoGranario.RutaFotoCPEDG);
                            image.ScaleAbsolute(document.PageSize.Width, document.PageSize.Height);
                            image.SetAbsolutePosition(0, 0);
                            document.Add(image);
                            document.Close();
                            pdf = stream.ToArray();
                        }
                        var resultado =
                        servicioComandos.Ejecutar(new ImprimirFileGenerico
                        {
                            CantidadCopias = cantCopias,
                            File = pdf,
                            Impresora = impresoraModel?.Direccion,
                            CodigoDocumentoImpresion = Enum.GetName(typeof(TipoImpresion), TipoImpresion.CartaDePorteElectronica)
                        });
                        return Content("true");
                    }
                    return Content("No existe la Carta de Porte Derivado Granario");
                }
                else
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
            }
            catch (Exception e)
            {
                return Content(Textos.ReimpresionDeDocumento_ImprimirError + e.Message);
            }            
        }

        [DatosUsuario]
        public ActionResult Previsualizar(DatosUsuario datosUsuario, int id, string ctg = null, string ctgdg = null)
        {
            try
            {
                if(id == default(int) && !string.IsNullOrEmpty(ctg))
                {
                    var cartaPorte = servicioComandos.Ejecutar(new ConsultarPDFCpe { NroCtg = Convert.ToInt64(ctg) }) as ResultadoConsultarPDFCpe;

                    if (System.Web.HttpContext.Current != null)
                    {
                        System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("RetornoExportacion", "ok"));
                    }

                    if (cartaPorte != null && !cartaPorte.HayErrores)
                        return File(cartaPorte.Pdf, "application/octet-stream", $"{ctg}.pdf");

                    return Content(cartaPorte?.Errores.Values.FirstOrDefault());
                } else if (id == default(int) && !string.IsNullOrEmpty(ctgdg))
                {
                    var cartaPorteDerivadoGranario = servicio.ObtenerCartaPorteDerivadoGranarioPorCTG(ctgdg);

                    if (System.Web.HttpContext.Current != null)
                    {
                        System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("RetornoExportacion", "ok"));
                    }

                    if (cartaPorteDerivadoGranario != null && !string.IsNullOrEmpty(cartaPorteDerivadoGranario.RutaFotoCPEDG) && System.IO.File.Exists(cartaPorteDerivadoGranario.RutaFotoCPEDG))
                    {
                        byte[] pdf = null;
                        var document = new Document();
                        using (var stream = new MemoryStream())
                        {
                            PdfWriter.GetInstance(document, stream);
                            document.Open();
                            Image image = Image.GetInstance(cartaPorteDerivadoGranario.RutaFotoCPEDG);
                            image.ScaleAbsolute(document.PageSize.Width, document.PageSize.Height);
                            image.SetAbsolutePosition(0, 0);
                            document.Add(image);
                            document.Close();
                            pdf = stream.ToArray();
                        }
                        return File(pdf, "application/octet-stream", $"{ctgdg}.pdf");
                    }
                    return Content("No existe la Carta de Porte Derivado Granario");
                } 
                else
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
                        byte[] file = ((ResultadoPrevisualizar)resultado).Archivo;
                        return File(file, "application/octet-stream", "vistaPrevia.pdf");
                    }
                    return Content(resultado.Errores.Values.FirstOrDefault());
                }
            }
            catch (Exception e)
            {
                return Content(Textos.ReimpresionDeDocumento_ImprimirError + e.Message);
            }
        }

        public ActionResult Eliminar(int id, string ctg = null, string ctgdg = null)
        {
            try
            {
                if(id == default(int) && (!string.IsNullOrEmpty(ctg) || !string.IsNullOrEmpty(ctgdg)))
                {
                    return Content(Textos.Reimpresion_Documentos_CPE_Eliminar);
                } else
                {
                    var resultado = servicioComandos.Ejecutar(new EliminarDocumento { Id = id });
                    return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.FirstOrDefault());
                }
            }
            catch (Exception e)
            {
                return Content(e.Message);
            } 
        }
    }
}
