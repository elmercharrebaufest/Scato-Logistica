using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.EXCEL;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Molinos.Scato.Web.Models.ArchivosTxt;
using Molinos.Scato.Web.Models.ArchivosXml;
using Molinos.Scato.Web.PDF;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace Molinos.Scato.Web.Controllers
{
    public class LoteController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;
        private readonly IConfiguracionProvider configuracion;
        private readonly IFirmaProvider firmaProvider;

        public LoteController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos, IConfiguracionProvider configuracion, IFirmaProvider firmaProvider)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
            this.configuracion = configuracion;
            this.firmaProvider = firmaProvider;
        }

        [DatosUsuario]
        [Autorizacion(PermisosScato.ArmarLote)]
        public ActionResult Index(DatosUsuario datosUsuario)
        {
            SetearVista();
            var nroLote = datosUsuario.CentroDescripcion.Replace(" ", "").Substring(0, 4).ToUpper() + servicio.ObtenerNumeroDocumentoGenerado().ToString(CultureInfo.InvariantCulture).PadLeft(6, '0');
            return View(new LoteDto { NumeroDeLote = nroLote });
        }

        private void SetearVista()
        {
            var camaras =
                servicio.ListarCamaras()
                        .OrderBy(c => c.Descripcion)
                        .ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
            // ReSharper disable LocalizableElement
            camaras.Insert(0, new SelectListItem { Value = "0", Text = "---" });
            // ReSharper restore LocalizableElement
            ViewBag.Camaras = camaras;
        }

        [Autorizacion(PermisosScato.ArmarLote)]
        [DatosUsuario]
        public ActionResult ArmarLote(LoteDto lote, string muestras, bool? soloImprimir, DatosUsuario datosUsuario)
        {
            var serializer = new JavaScriptSerializer();
            var muestrasElegidas = serializer.Deserialize<MuestraEnvioACamaraDto[]>(muestras);
            if (muestrasElegidas.Any())
            {
                lote.Muestras = muestrasElegidas;
                lote.Fecha = DateTime.Now;

                if (soloImprimir.HasValue && soloImprimir.Value)
                {
                    return new PdfLoteReporte(String.Format("Lote_{0}.pdf", lote.NumeroDeLote), lote, firmaProvider);
                }
                var resultado = servicioComandos.Ejecutar(new CrearLote { Dto = lote }) as ResultadoCrear;
                if (resultado != null && resultado.HayErrores)
                {
                    ModelState.AgregarErrores(resultado);
                    SetearVista();
                    return View("Index", lote);
                }
                var envioEmail = lote.EnviarArchivoAutomaticamente;
                if (resultado != null)
                {
                    lote = servicio.ObtenerLoteParaArchivo(resultado.Id);
                    foreach (var muestra in lote.Muestras)
                    {
                        if (muestra.EsPreLote == true)
                        {
                            var documento = servicio.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo("LoteACamara", muestra.CentroId, datosUsuario.PuestoDeTrabajoId);
                            //if (documento == null) { throw new Exception(string.Format(Textos.Error_DocumentoDeImpresionNoEncontrado, "LoteACamara")); }

                            if (documento != null)
                            {
                                var calado = servicio.ObtenerCaladoPorId(muestra.CaladoId);
                                var precintos = servicio.ListarPrecintos(muestra.WorkflowInstanceId);
                                var dto = new ImpIdentificacionEnvioLoteACamaraDto
                                {
                                    Impresora = documento.ImpresoraDireccion ?? "",
                                    Centro = documento.CentroDescripcion,
                                    Codigo = "LoteACamara",
                                    NumeroDeOrden = muestra.NroDocumento,
                                    Patente = muestra.Patente,
                                    FechaCalado = calado.FechaCreacion.HasValue ? calado.FechaCreacion.Value : DateTime.MinValue,
                                    NumeroDeMuestra = muestra.NroMuestra,
                                    Precinto = precintos.Count == 0 ? " – S/N" : (String.Join(";", precintos.Select(x => x.Detalle))),
                                    WorkflowId = muestra.WorkflowInstanceId,
                                };

                                var resultadoComando = servicioComandos.Ejecutar(new ImprimirEnvioLoteACamara { Dto = dto, CantidadCopias = 2 });
                            }
                        }
                    }
                }
                TempData["Alerta"] = Textos.Lote_OK;
                TempData["TipoAlerta"] = TipoAlerta.Exito;
                if (envioEmail)
                {
                    try
                    {
                        if (lote.CamaraFormatoDeArchivo == CamaraFormatoDeArchivo.Rosario)
                        {
                            EnviarArchivoRosarioPorEmail(lote);
                        }
                        else if (lote.CamaraFormatoDeArchivo == CamaraFormatoDeArchivo.BuenosAires)
                        {
                            EnviarArchivoBuenosAiresPorEmail(lote, datosUsuario.CentroId);
                        }
                        else if (lote.CamaraFormatoDeArchivo == CamaraFormatoDeArchivo.BahiaBlanca)
                        {
                            EnviarArchivoBahiaBlancaPorEmail(lote);
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex.Message);
                        TempData["Alerta"] = Textos.Lote_ErrorEmail;
                        TempData["TipoAlerta"] = TipoAlerta.Advertencia;
                    }
                }
                return RedirectToAction("LoteCreado", "Lote", new { loteId = lote.Id, numeroLote = lote.NumeroDeLote });
            }
            ModelState.AddModelError("numeroDeMuestra", Textos.Lote_NoHayMuestras);
            SetearVista();
            return View("Index", lote);
        }

        public ActionResult LoteCreado(int loteId, string numeroLote)
        {
            ViewBag.numeroLote = numeroLote;
            return View(loteId);
        }

        [DatosUsuario]
        public ActionResult ObtenerMuestra(string numeroDeMuestra, int camaraId, DatosUsuario datosUsuario)
        {
            var muestra = servicio.ObtenerMuestraEnvioACamaraYRecorridoPorNumero(numeroDeMuestra, datosUsuario.CentroId,true);
            if (muestra != null && muestra.MuestraEnvioACamara != null)
            {
                if (muestra.MuestraEnvioACamara.EstadoMuestra == EstadoMuestra.Enviada)
                {
                    return Json(new { MuestraId = 0 }, JsonRequestBehavior.AllowGet);
                }
                if (!muestra.Terminado)
                {
                    return Json(new { MuestraId = -1 }, JsonRequestBehavior.AllowGet);
                }
                if (muestra.Rechazado)
                {
                    return Json(new { MuestraId = -3 }, JsonRequestBehavior.AllowGet);
                }
                if (muestra.MuestraEnvioACamara.CamaraId != camaraId && muestra.MuestraEnvioACamara.EsPreLote != true)
                {
                    return Json(new { MuestraId = -4 }, JsonRequestBehavior.AllowGet);
                }
                return
                    Json(
                        new
                        {
                            muestra.MuestraEnvioACamara.Material,
                            MuestraId = muestra.MuestraEnvioACamara.Id,
                            muestra.MuestraEnvioACamara.Vendedor,
                            muestra.MuestraEnvioACamara.Corredor,
                            muestra.MuestraEnvioACamara.FechaDescarga,
                            NetoPlanta = muestra.MuestraEnvioACamara.PesoNeto,
                            muestra.MuestraEnvioACamara.Localidad,
                            muestra.Patente,
                            NumeroMuestra = muestra.MuestraEnvioACamara.NroMuestra,
                            muestra.MuestraEnvioACamara.CamaraId
                        }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { MuestraId = -2 }, JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult ListarPendientes(DatosUsuario datosUsuario)
        {
            var pendientes = servicio.ListarMuestraEnvioACamaraSinLote(datosUsuario.CentroId, true) ?? new List<MuestraEnvioACamaraDto>();
            var generadorExcel = new ExcelMuestrasPendientesReporte();
            var resultado = new ResultadoPrevisualizar();
            generadorExcel.GenerarArchivo(resultado, pendientes.ToList());

            if (!resultado.HayErrores)
            {
                byte[] file = resultado.Archivo;

                if (System.Web.HttpContext.Current != null)
                {
                    System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("RetornoExportacion", "ok"));
                }
                return File(file, "application/octet-stream", "Lote_MuestrasPendientes.xls");
            }
            return View("Index");
        }

        [DatosUsuario]
        [Autorizacion(PermisosScato.BuscarLote)]
        public ActionResult BuscarLote(DatosUsuario datosUsuario, BuscarLoteDto filtro, int pagina = 1, string ordenarPor = "Fecha", DirOrden dirOrden = DirOrden.Desc)
        {
            filtro.CentroId = datosUsuario.CentroId;
            ListarConsulta(filtro, pagina, ordenarPor, dirOrden);
            return View();
        }

        [DatosUsuario]
        [AjaxOnly]
        [ActionName("BuscarLote")]
        [Autorizacion(PermisosScato.BuscarLote)]
        public ActionResult Listar(DatosUsuario datosUsuario, BuscarLoteDto filtro, int pagina = 1, string ordenarPor = "Fecha", DirOrden dirOrden = DirOrden.Desc)
        {
            filtro.CentroId = datosUsuario.CentroId;
            ListarConsulta(filtro, pagina, ordenarPor, dirOrden);
            return View("Listar");
        }

        private void ListarConsulta(BuscarLoteDto filtro, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(
                ordenarPor,
                dirOrden,
                pagina,
                10);

            if (filtro.FechaHasta.HasValue)
            {
                filtro.FechaHasta = filtro.FechaHasta.Value.AddHours(23);
                filtro.FechaHasta = filtro.FechaHasta.Value.AddMinutes(59);
                filtro.FechaHasta = filtro.FechaHasta.Value.AddSeconds(59);
            }
            ViewBag.Items = servicio.ListarPaginadoLote(filtro, paginacion);
            var camaras =
                servicio.ListarCamaras()
                        .OrderBy(c => c.Descripcion)
                        .ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
            ViewBag.Camaras = camaras;
        }

        private void EnviarArchivoRosarioPorEmail(LoteDto loteDto)
        {
            var smtpClient = new SmtpClient();
            ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;
            var message = new MailMessage();
            message.To.Add(new MailAddress(loteDto.CamaraEmail));
            message.Subject = "Generación de archivos";
            message.Body = "Archivos adjuntados";
            log.Debug("Generando email de archivos de cámara para enviar a " + message.To.First().Address);
            string str01, str02, str03;
            GenerarRosario(loteDto, out str01, out str02, out str03);
            if (str01.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    using (var writer = new StreamWriter(memoryStream, Encoding.GetEncoding(1252)))
                    {
                        writer.Write(str01);
                        writer.Flush();
                    }
                    var archive = new MemoryStream(memoryStream.ToArray());
                    archive.Seek(0, SeekOrigin.Begin);
                    var ct = new ContentType(MediaTypeNames.Text.Plain);
                    var attach = new Attachment(archive, ct);
                    attach.ContentDisposition.FileName = "Solici01.txt";
                    message.Attachments.Add(attach);
                }
            }
            if (str02.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    using (var writer = new StreamWriter(memoryStream, Encoding.GetEncoding(1252)))
                    {
                        writer.Write(str02);
                        writer.Flush();
                    }
                    var archive = new MemoryStream(memoryStream.ToArray());
                    archive.Seek(0, SeekOrigin.Begin);
                    var ct = new ContentType(MediaTypeNames.Text.Plain);
                    var attach = new Attachment(archive, ct);
                    attach.ContentDisposition.FileName = "Solici02.txt";
                    message.Attachments.Add(attach);
                }
            }
            if (str03.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    using (var writer = new StreamWriter(memoryStream, Encoding.GetEncoding(1252)))
                    {
                        writer.Write(str03);
                        writer.Flush();
                    }
                    var archive = new MemoryStream(memoryStream.ToArray());
                    archive.Seek(0, SeekOrigin.Begin);
                    var ct = new ContentType(MediaTypeNames.Text.Plain);
                    var attach = new Attachment(archive, ct);
                    attach.ContentDisposition.FileName = "Solici03.txt";
                    message.Attachments.Add(attach);
                }
            }
            log.Debug("Enviando email a " + message.To.First().Address);
            smtpClient.Send(message);
            log.Debug("Mail enviado a " + message.To.First().Address);
        }

        private void EnviarArchivoBuenosAiresPorEmail(LoteDto loteDto, int centroId)
        {
            var smtpClient = new SmtpClient();
            ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;
            var message = new MailMessage();
            message.To.Add(new MailAddress(loteDto.CamaraEmail));
            var centro = servicio.ObtenerCentro(centroId);

            message.Subject = " Generación de archivos para " + centro.Descripcion + " - " + DateTime.Now.Formatted();
            message.Body = "Archivos adjuntados";
            log.Debug("Generando email de archivos de cámara para enviar a " + message.To.First().Address);
            var presentacion = GenerarBuenosAires(loteDto, centro);
            var nombreArchivo = string.Format("AS-CABC-001-165-{0}-{1}.xml", centro.NumeroOrigenCamaraBsAs, presentacion.Resumen.NroSecArchivoSolicitudes);
            using (var memoryStream = new MemoryStream())
            {
                var namespaces = new XmlSerializerNamespaces();
                namespaces.Add(string.Empty, string.Empty);
                var serializer = new XmlSerializer(typeof(Presentacion));
                serializer.Serialize(memoryStream, presentacion, namespaces);
                var fileStream = new MemoryStream(memoryStream.ToArray());
                fileStream.Seek(0, SeekOrigin.Begin);
                var ct = new ContentType(MediaTypeNames.Text.Plain);
                var attach = new Attachment(fileStream, ct);
                attach.ContentDisposition.FileName = nombreArchivo;
                message.Attachments.Add(attach);
            }
            log.Debug("Enviando email a " + message.To.First().Address);
            smtpClient.Send(message);
            log.Debug("Mail enviado a " + message.To.First().Address);
        }

        private void EnviarArchivoBahiaBlancaPorEmail(LoteDto loteDto)
        {
            var smtpClient = new SmtpClient();
            ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;
            var message = new MailMessage();
            message.To.Add(new MailAddress(loteDto.CamaraEmail));
            message.Subject = "Generación de archivos";
            message.Body = "Archivos adjuntados";
            log.Debug("Generando email de archivos de cámara para enviar a " + message.To.First().Address);
            string archivo;
            GenerarBahiaBlanca(loteDto, out archivo);
            if (archivo.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    using (var writer = new StreamWriter(memoryStream, Encoding.GetEncoding(1252)))
                    {
                        writer.Write(archivo);
                        writer.Flush();
                    }
                    var archive = new MemoryStream(memoryStream.ToArray());
                    archive.Seek(0, SeekOrigin.Begin);
                    var ct = new ContentType(MediaTypeNames.Text.Plain);
                    var attach = new Attachment(archive, ct);
                    var nombre = DateTime.Now.Day.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0') +
                                 DateTime.Now.Month.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0') +
                                 DateTime.Now.Year.ToString(CultureInfo.InvariantCulture).Substring(2, 2);
                    attach.ContentDisposition.FileName = nombre + ".txt";
                    message.Attachments.Add(attach);
                }
            }

            log.Debug("Enviando email a " + message.To.First().Address);
            smtpClient.Send(message);
            log.Debug("Mail enviado a " + message.To.First().Address);
        }

        [Autorizacion(PermisosScato.BuscarLote)]
        public ActionResult Seleccionar(int id, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ViewBag.Items = servicio.ListarMuestrasPorLote(id, new Paginacion(ordenarPor, dirOrden));
            ViewBag.NumeroDeLote = servicio.ObtenerNumeroLote(id);
            ViewBag.LoteId = id;
            return View();
        }

        public ActionResult ImprimirLote(int loteId)
        {
            var lote = servicio.ObtenerLoteParaArchivo(loteId);

            if (lote == null)
            {
                TempData["Alerta"] = Textos.Lote_ErrorInvalido;
                TempData["TipoAlerta"] = TipoAlerta.Advertencia;
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            return new PdfLoteReporte(String.Format("Lote_{0}.pdf", lote.NumeroDeLote), lote, firmaProvider);
        }

        [DatosUsuario]
        public ActionResult GenerarArchivos(int loteId, string vista, DatosUsuario datosUsuario)
        {
            var loteDto = servicio.ObtenerLoteParaArchivo(loteId);

            if (loteDto == null)
            {
                TempData["Alerta"] = Textos.Lote_ErrorInvalido;
                TempData["TipoAlerta"] = TipoAlerta.Advertencia;
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            if (loteDto.CamaraFormatoDeArchivo == CamaraFormatoDeArchivo.Rosario)
            {
                return DescargarRosarioZip(loteDto);
            }
            if (loteDto.CamaraFormatoDeArchivo == CamaraFormatoDeArchivo.BuenosAires)
            {
                return DescargarBuenosAires(loteDto, datosUsuario);
            }
            if (loteDto.CamaraFormatoDeArchivo == CamaraFormatoDeArchivo.BahiaBlanca)
            {
                return DescargarBahiaBlanca(loteDto);
            }
            TempData["Alerta"] = Textos.Lote_FormatoCamaraError;
            TempData["TipoAlerta"] = TipoAlerta.Advertencia;
            if (vista == "LoteCreado")
            {
                return View(vista, loteId);
            }
            ListarConsulta(new BuscarLoteDto { NroLote = loteDto.NumeroDeLote }, 1, "Id", DirOrden.Asc);
            return View(vista);
        }

        public ActionResult DescargarBuenosAires(LoteDto loteDto, DatosUsuario datosUsuario)
        {
            log.Info("Se comienza la generación de archivos para la cámara " + loteDto.CamaraFormatoDeArchivo);
            var centro = servicio.ObtenerCentro(datosUsuario.CentroId);
            var presentacion = GenerarBuenosAires(loteDto, centro);
            var nombreArchivo = string.Format("AS-CABC-001-0000165-{0}-{1}.xml", centro.NumeroOrigenCamaraBsAs.PadLeft(7, '0'), presentacion.Resumen.NroSecArchivoSolicitudes.ToString("0000000"));
            using (var memoryStream = new MemoryStream())
            {
                var namespaces = new XmlSerializerNamespaces();
                namespaces.Add(string.Empty, string.Empty);
                var serializer = new XmlSerializer(typeof(Presentacion));
                serializer.Serialize(memoryStream, presentacion, namespaces);
                var fileStream = new MemoryStream(memoryStream.ToArray());
                fileStream.Seek(0, SeekOrigin.Begin);
                return File(fileStream, "application/xml", nombreArchivo);
            }
        }

        public ActionResult DescargarRosarioZip(LoteDto loteDto)
        {
            string str01, str02, str03;
            GenerarRosario(loteDto, out str01, out str02, out str03);
            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    if (str01.Length > 0)
                    {
                        var archivo01 = archive.CreateEntry("Solici01.txt");
                        using (var entryStream = archivo01.Open())
                        {
                            using (var streamWriter = new StreamWriter(entryStream, Encoding.GetEncoding(1252)))
                            {
                                streamWriter.Write(str01);
                            }
                        }
                    }

                    if (str02.Length > 0)
                    {
                        var archivo02 = archive.CreateEntry("Solici02.txt");
                        using (var entryStream = archivo02.Open())
                        {
                            using (var streamWriter = new StreamWriter(entryStream, Encoding.GetEncoding(1252)))
                            {
                                streamWriter.Write(str02);
                            }
                        }
                    }

                    if (str03.Length > 0)
                    {
                        var archivo03 = archive.CreateEntry("Solici03.txt");
                        using (var entryStream = archivo03.Open())
                        {
                            using (var streamWriter = new StreamWriter(entryStream, Encoding.GetEncoding(1252)))
                            {
                                streamWriter.Write(str03);
                            }
                        }
                    }
                }
                var fileStream = new MemoryStream(memoryStream.ToArray());
                fileStream.Seek(0, SeekOrigin.Begin);
                return File(fileStream, "application/octet-stream", "Lote_" + loteDto.NumeroDeLote + ".zip");
            }
        }

        public ActionResult DescargarBahiaBlanca(LoteDto loteDto)
        {
            string str01;
            GenerarBahiaBlanca(loteDto, out str01);
            using (var memoryStream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(memoryStream, Encoding.GetEncoding(1252)))
                {
                    streamWriter.Write(str01);
                }
                var fileStream = new MemoryStream(memoryStream.ToArray());
                fileStream.Seek(0, SeekOrigin.Begin);

                var nombre = DateTime.Now.Day.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0') +
                             DateTime.Now.Month.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0') +
                             DateTime.Now.Year.ToString(CultureInfo.InvariantCulture).Substring(2, 2);
                return File(fileStream, "application/octet-stream", nombre + ".txt");
            }
        }

        private Presentacion GenerarBuenosAires(LoteDto loteDto, CentroDto centro)
        {
            var presentacion = new Presentacion();
            var solicitudes = new List<Solicitud>();
            var clientes = new List<Cliente>();
            var proveedorDeFirma = servicio.ObtenerProveedorPorCodigoSap(firmaProvider.ObtenerFirmaSinLogo().CodigoSAP);

            for (var index = 0; index < loteDto.Muestras.Count; index++)
            {
                var muestra = loteDto.Muestras[index];
                var solicitud = new Solicitud
                {
                    TipoSolicitud = 2,
                    Producto = Convert.ToInt32(muestra.MaterialCodigoCamara ?? "0"),
                    ClientesSolicitudes = new List<ClienteSolicitud>()
                };
                if (!String.IsNullOrEmpty(muestra.TitularCartaPorteCuil))
                {
                    solicitud.ClientesSolicitudes.Add(new ClienteSolicitud
                    {
                        TipoCod = "U",
                        CodigoCliente = muestra.TitularCartaPorteCuil.Replace("-", ""),
                        RolCliente = 2
                    });
                    clientes.Add(new Cliente
                    {
                        TipoCod = "U",
                        CodigoCliente = muestra.TitularCartaPorteCuil.Replace("-", ""),
                        Nombre = muestra.TitularCartaPorte,
                        Mail = muestra.TitularCartaPorteMail
                    });
                }
                if (!String.IsNullOrEmpty(muestra.Destinatario) &&
                    muestra.Destinatario != proveedorDeFirma.Descripcion)
                {
                    solicitud.ClientesSolicitudes.Add(new ClienteSolicitud
                    {
                        TipoCod = "U",
                        CodigoCliente = muestra.DestinatarioCuil.Replace("-", ""),
                        RolCliente = 4,
                        PagaEnsayos = true
                    });
                    clientes.Add(new Cliente
                    {
                        TipoCod = "U",
                        CodigoCliente = muestra.DestinatarioCuil.Replace("-", ""),
                        Nombre = muestra.Destinatario,
                        Mail = muestra.DestinatarioMail
                    });
                }
                if (!String.IsNullOrEmpty(muestra.CorredorCuil))
                {
                    solicitud.ClientesSolicitudes.Add(new ClienteSolicitud
                    {
                        TipoCod = "U",
                        CodigoCliente = muestra.CorredorCuil.Replace("-", ""),
                        RolCliente = 3
                    });
                    clientes.Add(new Cliente
                    {
                        TipoCod = "U",
                        CodigoCliente = muestra.CorredorCuil.Replace("-", ""),
                        Nombre = muestra.Corredor
                    });
                }
                if (!String.IsNullOrEmpty(muestra.RtteComercialCuit))
                {
                    solicitud.ClientesSolicitudes.Add(new ClienteSolicitud
                    {
                        TipoCod = "U",
                        CodigoCliente = muestra.RtteComercialCuit.Replace("-", ""),
                        RolCliente = 3
                    });
                    clientes.Add(new Cliente
                    {
                        TipoCod = "U",
                        CodigoCliente = muestra.RtteComercialCuit.Replace("-", ""),
                        Nombre = muestra.RtteComercial,
                        Mail = muestra.RtteComercialMail
                    });
                }
                solicitud.Muestras = new List<Muestra>
                    {
                        new Muestra
                            {
                                OrdenMta = Convert.ToInt64(muestra.NroMuestra),
                                KilajeReal = muestra.PesoNeto ?? 0,
                                KilajeEnsayo = muestra.PesoNeto ?? 0,
                                Camion = muestra.Patente,
                                Caratula = muestra.Caratula ?? 0,
                                CantidadMuestrasOriginales = 1,
                                LocalidadProcedencia = "0",
                                FechaDescarga = muestra.FechaDescarga,
                                LocalidadDescarga = muestra.CentroCodigoPostal,
                                CartasDePorte = new List<string> {muestra.NroCartaPorte},
                                GrupoEnsayos = muestra.GrupoCodigoCamara ?? "0"
                            }
                    };
                solicitudes.Add(solicitud);
            }
            presentacion.Solicitudes = solicitudes;
            presentacion.Clientes =
                clientes.GroupBy(e => new { codigo = e.CodigoCliente }).Select(g => g.First()).ToList();
            presentacion.Resumen = new Resumen
            {
                TotalKilosReal = solicitudes.Sum(s => s.Muestras.Sum(t => t.KilajeReal)),
                TotalKilosEnsayo = solicitudes.Sum(s => s.Muestras.Sum(t => t.KilajeEnsayo)),
                CantidadSolicitudes = solicitudes.Count,
                CantidadMuestras = solicitudes.Count,
                CantidadClientes = presentacion.Clientes.Count,
                FechaEnvioCamara = DateTime.Now,
                OrigenArchivoSolicitudes = Convert.ToInt32(centro.NumeroOrigenCamaraBsAs ?? "0"),
                CentroDeEnsayos = 1,
                NroSecArchivoSolicitudes = loteDto.Id
            };

            return presentacion;
        }

        private void GenerarRosario(LoteDto loteDto, out string archivo01, out string archivo02, out string archivo03)
        {
            var stringBuilder01 = new StringBuilder();
            var stringBuilder02 = new StringBuilder();
            var stringBuilder03 = new StringBuilder();

            log.Info("Se comienza la generación de archivos para la cámara " + loteDto.CamaraFormatoDeArchivo);
            var firma = firmaProvider.ObtenerFirmaSinLogo();

            foreach (var muestra in loteDto.Muestras)
            {
                log.Info("Se comienza a procesar el archivo 01 ");
                stringBuilder01.AppendLine(
                    TxtHelper.GetTxtDataRow(
                        new Rosario01
                        {
                            NumeroMuestra = muestra.NroMuestra,
                            NombreProducto = muestra.Material,
                            CodigoProducto = Convert.ToInt32(muestra.MaterialCodigoCamara ?? "0"),
                            CuitDestinatario = Convert.ToInt64(firma.Cuit.Replace("-", "")),
                            CuitRtteComercial = Convert.ToInt64((muestra.RtteComercialCuit ?? muestra.TitularCartaPorteCuil ?? "0").Replace("-", "")),
                            CuitCorredor = Convert.ToInt64((muestra.CorredorCuil ?? "0").Replace("-", "")),
                            CodigoPagador = 1,
                            CodigoPuerto = muestra.CentroCodigoCamara != null ? (muestra.CentroCodigoCamara.Length > 3 ? Convert.ToInt32(muestra.CentroCodigoCamara.Substring(0, 3)) : Convert.ToInt32(muestra.CentroCodigoCamara)) : 0,
                            PesoNetoSeco = muestra.PesoNeto ?? 0,
                            Lacrada = "L",
                            FechaDescarga = muestra.PesoNetoFecha,
                            CodigoGrupo = Convert.ToInt32(muestra.GrupoCodigoCamara ?? "0"),
                            ServicioLacrado = muestra.CentroId != Constantes.Centro.IdSanLorenzo && muestra.MaterialCodigoSAP == Constantes.CodigosSAP.Girasol ? "N" : "S",
                            Patente = muestra.Patente,
                            RtteComercial = muestra.TitularCartaPorte ?? "",
                            CartaDePorte = muestra.CPE ?? false ? Convert.ToInt64(muestra.Sucursal + muestra.CTG) : Convert.ToInt64(muestra.NroCartaPorte),
                            NumeroCTG = muestra.CPE ?? false ? Convert.ToInt64(muestra.NroCartaPorte) : Convert.ToInt64(muestra.CTG),
                            CuitTitularCartaPorte = Convert.ToInt64((muestra.TitularCartaPorteCuil ?? "0").Replace("-", "")),
                            TitularCartaPorte = muestra.TitularCartaPorte ?? "",
                            TecnologiaDeclarada = muestra.CodigoTecnologia ?? "00",
                            Establecimiento = muestra.CodEstab ?? "",
                            DireccionPostalDestino = muestra.Direccion ?? "",
                            CodigoLocalidadONCCAProcedencia = Convert.ToInt32(muestra.ProcedenciaCodigoSap ?? "0"),
                            CodigoLocalidadONCCADestino = Convert.ToInt32(muestra.LocalidadCodigoSap ?? "0"),
                            TipoDeTransporte = muestra.TipoVehiculo == TipoVehiculo.Tren ? "V" : "C",
                            CantidadVagones = muestra.TipoVehiculo == TipoVehiculo.Tren ? muestra.CantidadVehiculos : 0,
                            IdentificadorVagon = muestra.Patente,
                            CodigoPlantaONCCADestino = Convert.ToInt64(muestra.CentroDestinoCodigoEstablecimiento ?? "0"),
                            RazonSocialCorredor = muestra.Corredor ?? string.Empty,
                            CuitIntermediario = Convert.ToInt64((muestra.IntermediarioCuit ?? "0").Replace("-", string.Empty)),
                            RazonSocialIntermediario = muestra.Intermediario ?? string.Empty,
                            CuitRepresentante = Convert.ToInt64((muestra.RtteComercialCuit ?? "0").Replace("-", string.Empty)),
                            RazonSocialRepresentante = muestra.RtteComercial ?? string.Empty,
                            Cosecha = Convert.ToInt64((muestra.Cosecha ?? "0").Replace("-", string.Empty)),
                            CodigoProcedencia = Convert.ToInt32(muestra.ProcedenciaCodigoPostal ?? 0),
                            SubCodigoProcedencia = Convert.ToInt32(muestra.ProcedenciaSubcodigoPostal ?? 0),
                        }, typeof(Rosario01).GetProperties()));

                if (Convert.ToInt64(muestra.GrupoCodigoCamara ?? "0") == 0)
                {
                    foreach (var caracteristica in muestra.Caracteristicas)
                    {
                        log.Info("Se almacena una entrada para la característica " + caracteristica.Descripcion);
                        var codigoEnsayo = 0;
                        if (!string.IsNullOrEmpty(caracteristica.CodigoCamara))
                        {
                            int.TryParse(caracteristica.CodigoCamara, out codigoEnsayo);
                        }
                        stringBuilder02.AppendLine(
                            TxtHelper.GetTxtDataRow(
                                new Rosario02
                                {
                                    NumeroMuestra = muestra.NroMuestra,
                                    CodigoTiposEnsayo = (caracteristica.Ensayo ?? " ").Substring(0, 1),
                                    CodigoEnsayo = codigoEnsayo,
                                }, typeof(Rosario02).GetProperties()));
                    }
                }

                if (muestra.DestinatarioCodigoSap != firma.CodigoSAP)
                {
                    var cuentaOrden = muestra.DestinatarioCuil ?? "0";
                    stringBuilder03.AppendLine(
                        TxtHelper.GetTxtDataRow(
                            new Rosario03
                            {
                                CuentaOrden = Convert.ToInt64(cuentaOrden.Replace("-", "")),
                                DescripcionCuentaOrden = muestra.Destinatario,
                                NumeroMuestra = muestra.NroMuestra,
                                SucursalCuentaOrden = muestra.Sucursal ?? 0
                            }, typeof(Rosario03).GetProperties()));
                }

                if (!String.IsNullOrEmpty(muestra.RtteComercial))
                {
                    var cuentaOrden = muestra.RtteComercialCuit ?? "0";
                    stringBuilder03.AppendLine(
                        TxtHelper.GetTxtDataRow(
                            new Rosario03
                            {
                                CuentaOrden = Convert.ToInt64(cuentaOrden.Replace("-", "")),
                                DescripcionCuentaOrden = muestra.RtteComercial,
                                NumeroMuestra = muestra.NroMuestra,
                                SucursalCuentaOrden = muestra.Sucursal ?? 0
                            }, typeof(Rosario03).GetProperties()));
                }

                if (!String.IsNullOrEmpty(muestra.RtteComercialVentaSecundaria))
                {
                    stringBuilder03.AppendLine(
                        TxtHelper.GetTxtDataRow(
                            new Rosario03VentaSecundaria
                            {
                                CuitRteComercialVentaSecundaria = Convert.ToInt64((muestra.RtteComercialVentaSecundariaCuit ?? "0").Replace("-", "")),
                                RteComercialVentaSecundaria = muestra.RtteComercialVentaSecundaria ?? "",
                                NumeroMuestra = muestra.NroMuestra,
                                SucursalCuentaOrden = muestra.Sucursal ?? 0
                            }, typeof(Rosario03VentaSecundaria).GetProperties()));
                }

                if (!String.IsNullOrEmpty(muestra.RtteComercialVentaSecundaria2))
                {
                    stringBuilder03.AppendLine(
                        TxtHelper.GetTxtDataRow(
                            new Rosario03VentaSecundaria2
                            {
                                CuitRteComercialVentaSecundaria2 = Convert.ToInt64((muestra.RtteComercialVentaSecundaria2Cuit ?? "0").Replace("-", "")),
                                RteComercialVentaSecundaria2 = muestra.RtteComercialVentaSecundaria2 ?? "",
                                NumeroMuestra = muestra.NroMuestra,
                                SucursalCuentaOrden = muestra.Sucursal ?? 0
                            }, typeof(Rosario03VentaSecundaria2).GetProperties()));
                }

                if (!String.IsNullOrEmpty(muestra.MercadoATermino))
                {
                    stringBuilder03.AppendLine(
                        TxtHelper.GetTxtDataRow(
                            new Rosario03MercadoATermino
                            {
                                CuitMercadoATermino = Convert.ToInt64((muestra.MercadoATerminoCuit ?? "0").Replace("-", "")),
                                MercadoATérmino = muestra.MercadoATermino ?? "",
                                NumeroMuestra = muestra.NroMuestra,
                                SucursalCuentaOrden = muestra.Sucursal ?? 0
                            }, typeof(Rosario03MercadoATermino).GetProperties()));
                }

                if (!String.IsNullOrEmpty(muestra.CorredorVentaSecundaria))
                {
                    stringBuilder03.AppendLine(
                        TxtHelper.GetTxtDataRow(
                            new Rosario03CorredorVentaSecundaria
                            {
                                CuitCorredorVentaSecundaria = Convert.ToInt64((muestra.CorredorVentaSecundariaCuit ?? "0").Replace("-", "")),
                                CorredorVentaSecundaria = muestra.CorredorVentaSecundaria ?? "",
                                NumeroMuestra = muestra.NroMuestra,
                                SucursalCuentaOrden = muestra.Sucursal ?? 0
                            }, typeof(Rosario03CorredorVentaSecundaria).GetProperties()));
                }

                if (!String.IsNullOrEmpty(muestra.RepresentanteRecibidor))
                {
                    stringBuilder03.AppendLine(
                        TxtHelper.GetTxtDataRow(
                            new Rosario03RepresentanteRecibidor
                            {
                                CuitRepresentanteRecibidor = Convert.ToInt64((muestra.RepresentanteRecibidorCuit ?? "0").Replace("-", "")),
                                RepresentanteRecibidor = muestra.RepresentanteRecibidor ?? "",
                                NumeroMuestra = muestra.NroMuestra,
                                SucursalCuentaOrden = muestra.Sucursal ?? 0
                            }, typeof(Rosario03RepresentanteRecibidor).GetProperties()));
                }

                if (!String.IsNullOrEmpty(muestra.Destino))
                {
                    stringBuilder03.AppendLine(
                        TxtHelper.GetTxtDataRow(
                            new Rosario03Destino
                            {
                                CuitDestino = Convert.ToInt64((muestra.DestinoCuit ?? "0").Replace("-", "")),
                                Destino = muestra.Destino ?? "",
                                NumeroMuestra = muestra.NroMuestra,
                                SucursalCuentaOrden = muestra.Sucursal ?? 0
                            }, typeof(Rosario03Destino).GetProperties()));
                }
            }

            archivo01 = stringBuilder01.ToString();
            archivo02 = stringBuilder02.ToString();
            archivo03 = stringBuilder03.ToString();
        }

        private void GenerarBahiaBlanca(LoteDto loteDto, out string archivo)
        {
            var stringBuilder01 = new StringBuilder();

            log.Info("Se comienza la generación de archivos para la cámara " + loteDto.CamaraFormatoDeArchivo);
            var nombreExportador = firmaProvider.ObtenerFirmaSinLogo().RazonSocial;

            foreach (var muestra in loteDto.Muestras)
            {
                log.Info("Se comienza a procesar el archivo");
                stringBuilder01.AppendLine(
                    TxtHelper.GetTxtDataRow(
                        new BahiaBlanca
                        {
                            AñoFechaDescarga = muestra.FechaCartaPorte.Year,
                            MesFechaDescarga = muestra.FechaCartaPorte.Month,
                            DiaFechaDescarga = muestra.FechaCartaPorte.Day,
                            CodigoProducto = Convert.ToInt64(muestra.MaterialCodigoCamara ?? "0"),
                            Cosecha = Convert.ToInt32((muestra.Cosecha ?? "00").Substring(0, 2)),
                            Kilos = muestra.PesoNeto.HasValue ? muestra.PesoNeto.Value : 0,
                            NombreCorredor = muestra.Corredor ?? "",
                            NombreEntregador = muestra.Entregador ?? "",
                            NombreExportador = nombreExportador,
                            NombreVendedor = muestra.TitularCartaPorte ?? "",
                            NombreProcedencia = muestra.Procedencia ?? "",
                            TipoTrans = muestra.TipoVehiculo.ToString().Substring(0, 1),
                            NumeroMuestra = Convert.ToInt64(muestra.NroMuestra),
                            NroVagon = muestra.TipoVehiculo == TipoVehiculo.Tren ? muestra.NumeroVehiculo : 0,
                            NumeroCTG = muestra.CPE ?? false ? Convert.ToInt64(muestra.NroCartaPorte) : Convert.ToInt64(muestra.CTG),
                            CPE = muestra.CPE ?? false ? Convert.ToInt64(muestra.Sucursal + muestra.CTG) : 0,
                            CartaDePorte = muestra.CPE ?? false ? 0 : Convert.ToInt64(muestra.NroCartaPorte),
                        }, typeof(BahiaBlanca).GetProperties()));
            }

            archivo = stringBuilder01.ToString();
        }

        [AllowAnonymous]
        public JsonResult EnvioDeLoteAutomatico(int centroId, int camaraId, string mail)
        {
            var result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            var centro = servicio.ObtenerCentro(centroId);
            var nroLote = centro.Descripcion.Replace(" ", "").Substring(0, 4).ToUpper() + servicio.ObtenerNumeroDocumentoGenerado().ToString(CultureInfo.InvariantCulture).PadLeft(6, '0');
            var pendientes = servicio.ListarMuestraEnvioACamaraSinLote(centroId, false) ?? new List<MuestraEnvioACamaraDto>();
            if (!pendientes.Any())
            {
                result.Data = Textos.LoteBiotecnologia_NoHayMuestrasPendientes;
                return result;
            }

            var lote = new LoteDto
            {
                NumeroDeLote = nroLote,
                Muestras = pendientes,
                Fecha = DateTime.Now,
                CamaraId = camaraId
            };
            try
            {
                var resultado = servicioComandos.Ejecutar(new CrearLote { Dto = lote }) as ResultadoCrear;

                if (resultado != null && resultado.HayErrores)
                {
                    result.Data = resultado.Errores.First();
                }
                else if (resultado != null)
                {
                    lote = servicio.ObtenerLoteParaArchivo(resultado.Id);
                    if (!string.IsNullOrEmpty(mail))
                    {
                        lote.CamaraEmail = mail;
                    }

                    if (lote.CamaraFormatoDeArchivo == CamaraFormatoDeArchivo.Rosario)
                    {
                        EnviarArchivoRosarioPorEmail(lote);
                    }
                    else if (lote.CamaraFormatoDeArchivo == CamaraFormatoDeArchivo.BuenosAires)
                    {
                        EnviarArchivoBuenosAiresPorEmail(lote, centroId);
                    }
                    else if (lote.CamaraFormatoDeArchivo == CamaraFormatoDeArchivo.BahiaBlanca)
                    {
                        EnviarArchivoBahiaBlancaPorEmail(lote);
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e, "Error al generar el lote");
                result.Data = e.Message;
            }

            return result;
        }

        [AllowAnonymous]
        public JsonResult LimpiezaMuestrasFueraFechaEstablecida( bool esPreLote, int hrDiaAnterior)
        {
            var result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
           
            try
            {
                var resultado = servicioComandos.Ejecutar(new EliminarMuestrasFueraDeFecha 
                { 
                   EsPreLote = esPreLote,
                   HrDiaAnterior = hrDiaAnterior
                }) as Resultado;

                if (resultado != null && resultado.HayErrores)
                {
                    result.Data = resultado.Errores.First();
                }
                
            }
            catch (Exception e)
            {
                log.Error(e, "Error al limpiar el lote");
                result.Data = e.Message;
            }

            return result;
        }
    }
}