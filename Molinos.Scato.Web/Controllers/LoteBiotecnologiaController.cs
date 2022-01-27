using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Xml.Serialization;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Filtros;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Molinos.Scato.Web.Models.ArchivosTxt;
using Molinos.Scato.Web.Models.ArchivosXml;
using Molinos.Scato.Web.PDF;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ArmarLoteBiotecnologia)]
    public class LoteBiotecnologiaController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos serviciosComandos;
        private readonly IFirmaProvider configuracion;
        private readonly IFirmaProvider firma;

        public LoteBiotecnologiaController(ILogger log, IServicioRepositorio servicio, IServicioComandos serviciosComandos, IFirmaProvider configuracion, IFirmaProvider
             firma)
            : base(servicio)
        {
            this.log = log;
            this.serviciosComandos = serviciosComandos;
            this.configuracion = configuracion;
            this.firma = firma;
        }

        public ActionResult Index()
        {
            SetearVista();
            return View();
        }

        private void SetearVista()
        {
            var camaras =
                servicio.ListarCamaras()
                        .OrderBy(c => c.Descripcion)
                        .ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
            ViewBag.Camaras = camaras;
        }



        [HttpPost]
        [HttpParamAction]
        [DatosUsuario]
        public ActionResult Descargar(LoteBiotecnologiaDto loteDto, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var materiaPorCentro = servicio.ObtenerMaterialPorCentro(datosUsuario.CentroId, loteDto.MaterialId);
                if (materiaPorCentro == null || !materiaPorCentro.RequiereTecnologia)
                {
                    ModelState.AddModelError("Material", Textos.MaterialRequiereTecnologia);
                }
                else if (!servicio.ExistenMuestraEnvioACamaraIntactaPendientes(loteDto.MaterialId, datosUsuario.CentroId))
                {
                    ModelState.AddModelError("Material", Textos.LoteBiotecnologia_NoHayMuestrasPendientes);
                }
                else
                {
                    loteDto.NombreUsuario = datosUsuario.NombreUsuario;
                    loteDto.CentroId = datosUsuario.CentroId;
                    loteDto.CentroDesc = datosUsuario.CentroDescripcion.Replace(" ", "").Substring(0, 3).ToUpper();
                    loteDto.Fecha = DateTime.Now;
                    loteDto.CamaraFormatoDeArchivo = servicio.ObtenerCamara(loteDto.CamaraId).FormatoDeArchivo;
                    try
                    {
                        var resultado = (ResultadoCrear)serviciosComandos.Ejecutar(new CrearLoteBiotecnologia { Dto = loteDto });
                        if (resultado != null && !resultado.HayErrores)
                        {
                            loteDto = servicio.ObtenerLoteBiotecnologiaParaArchivo(resultado.Id);

                            if (loteDto.CamaraFormatoDeArchivo == CamaraFormatoDeArchivo.Rosario)
                            {
                                return DescargarRosarioZip(loteDto, true);
                            }
                            if (loteDto.CamaraFormatoDeArchivo == CamaraFormatoDeArchivo.BuenosAires)
                            {
                                return DescargarBuenosAires(loteDto, datosUsuario);
                            }
                            if (loteDto.CamaraFormatoDeArchivo == CamaraFormatoDeArchivo.BahiaBlanca)
                            {
                                return DescargarBahiaBlanca(loteDto);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error(e, "Error al generar el lote biotecnologia");
                        ModelState.AddModelError("Material", Textos.Error_LoteBiotecnologia);
                    }
                }
            }
            SetearVista();
            return View("Index", loteDto);

        }

        [HttpPost]
        [HttpParamAction]
        [DatosUsuario]
        public ActionResult ListadoDetalle(LoteBiotecnologiaDto loteDto, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var materiaPorCentro = servicio.ObtenerMaterialPorCentro(datosUsuario.CentroId, loteDto.MaterialId);
                if (materiaPorCentro == null || !materiaPorCentro.RequiereTecnologia)
                {
                    ModelState.AddModelError("Material", Textos.MaterialRequiereTecnologia);
                }
                else if (!servicio.ExistenMuestraEnvioACamaraIntactaPendientes(loteDto.MaterialId, datosUsuario.CentroId))
                {
                    ModelState.AddModelError("Material", Textos.LoteBiotecnologia_NoHayMuestrasPendientes);
                }
                else
                {
                    var muestrasElegidas = servicio.ListarMuestraEnvioACamaraBiotecnologiaSinLote(loteDto.MaterialId, loteDto.CamaraId, datosUsuario.CentroId) ?? new List<MuestraEnvioACamaraBiotecnoligiaDto>();

                    var lote = new LoteDto
                    {
                        Muestras = muestrasElegidas.Select(x => new MuestraEnvioACamaraDto
                        {
                            Material = x.Material,
                            Vendedor = x.Vendedor,
                            Corredor = x.Corredor,
                            FechaDescarga = x.FechaDescarga,
                            PesoNeto = x.PesoNeto,
                            NroMuestra = x.NroMuestra,
                            Localidad = x.Localidad,
                            Patente = x.Patente
                        }
                                ).ToList(),
                        NumeroDeLote = string.Empty
                    };
                    if (System.Web.HttpContext.Current != null)
                    {
                        System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("RetornoExportacion", "ok"));
                    }
                    return new PdfLoteReporte(String.Format("Muestras_Pendientes.pdf"), lote, firma);
                }
            }
            SetearVista();
            return View("Index", loteDto);

        }

        [DatosUsuario]
        public ActionResult BuscarLote(DatosUsuario datosUsuario, BuscarLoteBiotecnologiaDto filtro, int pagina = 1, string ordenarPor = "Fecha", DirOrden dirOrden = DirOrden.Desc)
        {
            filtro.CentroId = datosUsuario.CentroId;
            ListarConsulta(filtro, pagina, ordenarPor, dirOrden);
            return View();
        }

        [AjaxOnly]
        [ActionName("BuscarLote")]
        [DatosUsuario]
        public ActionResult Listar(DatosUsuario datosUsuario, BuscarLoteBiotecnologiaDto filtro, int pagina = 1, string ordenarPor = "Fecha", DirOrden dirOrden = DirOrden.Desc)
        {
            filtro.CentroId = datosUsuario.CentroId;
            ListarConsulta(filtro, pagina, ordenarPor, dirOrden);
            return View("Listar");
        }




        private void ListarConsulta(BuscarLoteBiotecnologiaDto filtro, int pagina, string ordenarPor, DirOrden dirOrden)
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
            ViewBag.Items = servicio.ListarPaginadoBiotecnologiaLote(filtro, paginacion);
            var camaras =
                servicio.ListarCamaras()
                        .OrderBy(c => c.Descripcion)
                        .ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
            ViewBag.Camaras = camaras;
        }

        public ActionResult Seleccionar(int id, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ViewBag.Items = servicio.ListarMuestrasPorLoteBiotecnologia(id, new Paginacion(ordenarPor, dirOrden, pagina, 20));
            ViewBag.NumeroDeLote = servicio.ObtenerNumeroLoteBiotecnologia(id);
            ViewBag.LoteId = id;
            return View();
        }

        public ActionResult ImprimirLote(int loteId)
        {
            var lote = servicio.ObtenerLoteBiotecnologiaParaImpresion(loteId);

            if (lote == null)
            {
                TempData["Alerta"] = Textos.Lote_ErrorInvalido;
                TempData["TipoAlerta"] = TipoAlerta.Advertencia;
                return RedirectToAction("Index", "ListaDeCamiones");
            }

            var loteDto = new LoteDto
            {
                Muestras = lote.Muestras.Select(x => new MuestraEnvioACamaraDto
                {
                    Material = x.Material,
                    Vendedor = x.Vendedor,
                    Corredor = x.Corredor,
                    FechaDescarga = x.FechaDescarga,
                    PesoNeto = x.PesoNeto,
                    NroMuestra = x.NroMuestra,
                    Localidad = x.Localidad,
                    Patente = x.Patente
                }
                    ).ToList(),
                NumeroDeLote = lote.NumeroDeLote
            };
            return new PdfLoteReporte(String.Format("Lote_{0}.pdf", lote.NumeroDeLote), loteDto, firma);
        }

        [DatosUsuario]
        public ActionResult GenerarArchivos(int loteId, string vista, DatosUsuario datosUsuario)
        {
            var loteDto = servicio.ObtenerLoteBiotecnologiaParaArchivo(loteId);

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
            ListarConsulta(new BuscarLoteBiotecnologiaDto { NroLote = loteDto.NumeroDeLote }, 1, "Id", DirOrden.Asc);
            return View(vista);
        }


        public ActionResult DescargarBuenosAires(LoteBiotecnologiaDto loteDto, DatosUsuario datosUsuario)
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

                if (System.Web.HttpContext.Current != null)
                {
                    System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("RetornoExportacion", "ok"));
                }
                return File(fileStream, "application/xml", nombreArchivo);
            }
        }

        [AllowAnonymous]
        public JsonResult EnvioDeLoteAutomatico(int centroId, int materialId, int camaraId)
        {

            var result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            var materiaPorCentro = servicio.ObtenerMaterialPorCentro(centroId, materialId);

            if (materiaPorCentro == null || !materiaPorCentro.RequiereTecnologia)
            {
                result.Data = Textos.MaterialRequiereTecnologia;
            }
            else if (!servicio.ExistenMuestraEnvioACamaraIntactaPendientes(materialId, centroId))
            {
                result.Data = Textos.LoteBiotecnologia_NoHayMuestrasPendientes;
            }
            else
            {
                var centro = servicio.ObtenerCentro(centroId);
                var camara = servicio.ObtenerCamara(camaraId);
                var loteDto = new LoteBiotecnologiaDto
                {
                    Fecha = DateTime.Now,
                    CamaraFormatoDeArchivo = camara.FormatoDeArchivo,
                    CamaraEmail = camara.Email,
                    CentroId = centroId,
                    MaterialId = materialId,
                    CamaraId = camaraId,
                    CentroDesc = centro.Descripcion.Replace(" ", "").Substring(0, 3).ToUpper(),
                    NombreUsuario = ConfigurationManager.AppSettings["Reportes.Username"]

                };
                try
                {
                    var resultado = (ResultadoCrear)serviciosComandos.Ejecutar(new CrearLoteBiotecnologia { Dto = loteDto });
                    if (resultado != null && !resultado.HayErrores)
                    {
                        loteDto = servicio.ObtenerLoteBiotecnologiaParaArchivo(resultado.Id);

                        if (loteDto.CamaraFormatoDeArchivo == CamaraFormatoDeArchivo.Rosario)
                        {
                          result.Data = EnviarArchivoRosarioPorEmail(loteDto);
                        }
                        if (loteDto.CamaraFormatoDeArchivo == CamaraFormatoDeArchivo.BuenosAires)
                        {
                            result.Data = EnviarArchivoBuenosAiresPorEmail(loteDto);
                        }
                        if (loteDto.CamaraFormatoDeArchivo == CamaraFormatoDeArchivo.BahiaBlanca)
                        {
                           result.Data = EnviarArchivoBahiaBlancaPorEmail(loteDto);
                        }
                    }
                }
                catch (Exception e)
                {
                    log.Error(e, "Error al generar el lote biotecnologia");
                    result.Data = e.Message;
                }
            }

            return result;
        }

        public ActionResult DescargarRosarioZip(LoteBiotecnologiaDto loteDto, bool generarSoloArchivo1 = false)
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

                    if (str02.Length > 0 && !generarSoloArchivo1)
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

                    if (str03.Length > 0 && !generarSoloArchivo1)
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
                if (System.Web.HttpContext.Current != null)
                {
                    System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("RetornoExportacion", "ok"));
                }
                return File(fileStream, "application/octet-stream", "Lote_" + loteDto.NumeroDeLote + ".zip");
            }
        }



        public ActionResult DescargarBahiaBlanca(LoteBiotecnologiaDto loteDto)
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

                if (System.Web.HttpContext.Current != null)
                {
                    System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("RetornoExportacion", "ok"));
                }
                return File(fileStream, "application/octet-stream", nombre + ".txt");
            }
        }

        private void GenerarRosario(LoteBiotecnologiaDto loteDto, out string archivo01, out string archivo02, out string archivo03)
        {
            var stringBuilder01 = new StringBuilder();
            var stringBuilder02 = new StringBuilder();
            var stringBuilder03 = new StringBuilder();

            log.Info("Se comienza la generación de archivos para la cámara " + loteDto.CamaraFormatoDeArchivo);
            var firmaSinLogo = configuracion.ObtenerFirmaSinLogo();

            foreach (var muestra in loteDto.Muestras)
            {
                log.Info("Se comienza a procesar el archivo 01 ");
                stringBuilder01.AppendLine(
                    TxtHelper.GetTxtDataRow(
                        new Rosario01
                        {
                            NumeroMuestra = muestra.NroMuestra,
                            NombreProducto = muestra.Material,
                            CodigoProducto = Convert.ToInt32(muestra.CodigoCamaraMaterial ?? "0"),
                            CuitDestinatario = Convert.ToInt64(muestra.DestinatarioCuil.Replace("-", "")),
                            CuitRtteComercial = Convert.ToInt64((muestra.RtteComercialCuit ?? muestra.TitularCartaPorteCuil ?? "0").Replace("-", "")),
                            CuitCorredor = Convert.ToInt64((muestra.CorredorCuil ?? "0").Replace("-", "")),
                            CodigoPagador = 1,
                            CodigoPuerto = muestra.CodigoDeCamara != null ? (muestra.CodigoDeCamara.Length > 3 ? Convert.ToInt32(muestra.CodigoDeCamara.Substring(0, 3)) : Convert.ToInt32(muestra.CodigoDeCamara)) : 0,
                            PesoNetoSeco = muestra.PesoNeto ?? 0,
                            Lacrada = "L",
                            FechaDescarga = muestra.FechaDescarga,
                            CodigoGrupo = Convert.ToInt32(muestra.CodigoCamaraGrupo ?? "0"),
                            ServicioLacrado = "S",
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
                            CantidadVagones = muestra.TipoVehiculo == TipoVehiculo.Tren ? muestra.CantidadDeVagones : 0,
                            IdentificadorVagon = muestra.Patente,
                            CodigoPlantaONCCADestino = Convert.ToInt64(muestra.CodigoEstablecimiento ?? "0"),
                            RazonSocialCorredor = muestra.Corredor ?? string.Empty,
                            CuitIntermediario = Convert.ToInt64((muestra.IntermediarioCuit ?? "0").Replace("-", string.Empty)),
                            RazonSocialIntermediario = muestra.Intermediario ?? string.Empty,
                            CuitRepresentante = Convert.ToInt64((muestra.RtteComercialCuit ?? "0").Replace("-", string.Empty)),
                            RazonSocialRepresentante = muestra.RtteComercial ?? string.Empty,
                            Cosecha = Convert.ToInt64((muestra.Cosecha ?? "0").Replace("-", string.Empty)),
                            CodigoProcedencia = Convert.ToInt32(muestra.ProcedenciaCodigoPostal ?? 0),
                            SubCodigoProcedencia = Convert.ToInt32(muestra.ProcedenciaSubcodigoPostal ?? 0)
                        }, typeof(Rosario01).GetProperties()));


                if ((!String.IsNullOrEmpty(muestra.DestinatarioCodigoSap) && muestra.DestinatarioCodigoSap != firmaSinLogo.CodigoSAP) || (muestra.DestinatarioCodigoSap == firmaSinLogo.CodigoSAP && muestra.RtteComercial != null))
                {
                    log.Info("Se procesa último archivo con destinatario {0} y Rtte Comercial {1}", muestra.Destinatario, muestra.RtteComercial);

                    if (muestra.DestinatarioCodigoSap != firmaSinLogo.CodigoSAP)
                    {
                        var cuentaOrden = muestra.DestinatarioCuil ?? "0";

                        stringBuilder03.AppendLine(
                            TxtHelper.GetTxtDataRow(
                                new Rosario03
                                {
                                    CuentaOrden = Convert.ToInt64(cuentaOrden.Replace("-", "")),
                                    DescripcionCuentaOrden = muestra.Destinatario,
                                    NumeroMuestra = muestra.NroMuestra
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
                                    NumeroMuestra = muestra.NroMuestra
                                }, typeof(Rosario03).GetProperties()));
                    }
                }
            }

            archivo01 = stringBuilder01.ToString();
            archivo02 = stringBuilder02.ToString();
            archivo03 = stringBuilder03.ToString();
        }

        private string EnviarArchivoRosarioPorEmail(LoteBiotecnologiaDto loteDto)
        {
            var result = "El envio del lote fue Exitoso";

            try
            {
                var smtpClient = new SmtpClient();
                ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;
                var message = new MailMessage();
                foreach (var address in loteDto.CamaraEmail.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    message.To.Add(new MailAddress(address));
                }
                message.Subject = " Generación de archivos Biotecnología para " + loteDto.CentroDesc + " - " + DateTime.Now.Formatted();
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
            catch (Exception e)
            {
                log.Error(e.Message);
                result = e.Message;
            }

            return result;
        }

        private string EnviarArchivoBuenosAiresPorEmail(LoteBiotecnologiaDto loteDto)
        {
            var result = "El envio del lote fue Exitoso";

            try
            {
                var smtpClient = new SmtpClient();
                ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;
                var message = new MailMessage();
                message.To.Add(new MailAddress(loteDto.CamaraEmail));
                message.Subject = "Generación de archivos para " + loteDto.CentroDesc + " - " + DateTime.Now.Formatted();
                message.Body = "Archivos adjuntados";
                log.Debug("Generando email de archivos de cámara para enviar a " + message.To.First().Address);
                var centro = servicio.ObtenerCentro(loteDto.CentroId);
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
            catch (Exception e)
            {
                log.Error(e.Message);
                result = e.Message;
            }

            return result;
        }

        private string EnviarArchivoBahiaBlancaPorEmail(LoteBiotecnologiaDto loteDto)
        {
            var result = "El envio del lote fue Exitoso";

            try
            {
                var smtpClient = new SmtpClient();
                ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;
                var message = new MailMessage();
                message.To.Add(new MailAddress(loteDto.CamaraEmail));
                message.Subject = "Generación de archivos para " + loteDto.CentroDesc + " - " + DateTime.Now.Formatted();
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
            catch (Exception e)
            {
                log.Error(e.Message);
                result = e.Message;
            }

            return result;
        }


        private Presentacion GenerarBuenosAires(LoteBiotecnologiaDto loteDto, CentroDto centro)
        {
            var presentacion = new Presentacion();
            presentacion.Clientes = new List<Cliente>();
            presentacion.Solicitudes = new List<Solicitud>();
            presentacion.Resumen = new Resumen();
            var solicitudes = new List<Solicitud>();
            var clientes = new List<Cliente>();
            var proveedorDeFirma = servicio.ObtenerProveedorPorCodigoSap(configuracion.ObtenerFirmaSinLogo().CodigoSAP);
            foreach (var muestra in loteDto.Muestras)
            {
                var solicitud = new Solicitud
                {
                    TipoSolicitud = 2,
                    Producto = Convert.ToInt32(muestra.CodigoCamaraMaterial ?? "0"),
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
                                GrupoEnsayos = muestra.CodigoCamaraGrupo ?? "0"
                            }
                    };
                solicitudes.Add(solicitud);
            }
            presentacion.Solicitudes = solicitudes;
            presentacion.Clientes = clientes.GroupBy(e => new { codigo = e.CodigoCliente }).Select(g => g.First()).ToList();


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

        private void GenerarBahiaBlanca(LoteBiotecnologiaDto loteDto, out string archivo)
        {
            var stringBuilder01 = new StringBuilder();

            log.Info("Se comienza la generación de archivos para la cámara " + loteDto.CamaraFormatoDeArchivo);
            var nombreExportador = configuracion.ObtenerFirmaSinLogo().RazonSocial;
            foreach (var muestra in loteDto.Muestras)
            {
                log.Info("Se comienza a procesar el archivo");
                muestra.CodigoCamaraMaterial = muestra.CodigoCamaraMaterial ?? "0";
                var codigoProducto = muestra.CodigoCamaraMaterial.Length > 5 ? muestra.CodigoCamaraMaterial.Substring(0, 5) : muestra.CodigoCamaraMaterial;
                stringBuilder01.AppendLine(
                    TxtHelper.GetTxtDataRow(
                        new BahiaBlanca
                        {
                            AñoFechaDescarga = muestra.FechaEmision.Year,
                            MesFechaDescarga = muestra.FechaEmision.Month,
                            DiaFechaDescarga = muestra.FechaEmision.Day,
                            CodigoProducto = Convert.ToInt64(codigoProducto),
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

    }
}
