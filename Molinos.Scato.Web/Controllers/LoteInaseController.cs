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
using System.Xml;
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
    [Autorizacion(PermisosScato.ArmarLoteInase)]
    public class LoteInaseController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos serviciosComandos;
        private readonly IFirmaProvider configuracion;
        private readonly IFirmaProvider firma;

        public LoteInaseController(ILogger log, IServicioRepositorio servicio, IServicioComandos serviciosComandos, IFirmaProvider configuracion, IFirmaProvider
             firma)
            : base(servicio)
        {
            this.log = log;
            this.serviciosComandos = serviciosComandos;
            this.configuracion = configuracion;
            this.firma = firma;
        }

        [DatosUsuario]
        public ActionResult BuscarLote(DatosUsuario datosUsuario, FiltroLoteInaseDto filtro, int pagina = 1, string ordenarPor = "Fecha", DirOrden dirOrden = DirOrden.Desc)
        {
            filtro.CentroId = datosUsuario.CentroId;
            ListarConsulta(filtro, pagina, ordenarPor, dirOrden);
            return View();
        }

        [AjaxOnly]
        [ActionName("BuscarLote")]
        [DatosUsuario]
        public ActionResult Listar(DatosUsuario datosUsuario, FiltroLoteInaseDto filtro, int pagina = 1, string ordenarPor = "Fecha", DirOrden dirOrden = DirOrden.Desc)
        {
            filtro.CentroId = datosUsuario.CentroId;
            ListarConsulta(filtro, pagina, ordenarPor, dirOrden);
            return View("Listar");
        }

        private void ListarConsulta(FiltroLoteInaseDto filtro, int pagina, string ordenarPor, DirOrden dirOrden)
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
            ViewBag.Items = servicio.ListarPaginadoLoteInase(filtro, paginacion);
        }

        public ActionResult Seleccionar(int id, int pagina = 1, string ordenarPor = "RecorridoId", DirOrden dirOrden = DirOrden.Asc)
        {
            ViewBag.Items = servicio.ListarMuestrasPorLoteInase(id, new Paginacion(ordenarPor, dirOrden, pagina, 20));
            ViewBag.NumeroDeLote = servicio.ObtenerNumeroLoteInase(id);
            ViewBag.LoteId = id;
            return View();
        }

        public ActionResult ImprimirLote(int loteId)
        {
            var lote = servicio.ObtenerLoteInaseParaImpresion(loteId);

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
                    Material = x.MaterialDescripcion,
                    Vendedor = x.Vendedor,
                    Corredor = x.Corredor,
                    FechaDescarga = x.FechaDescarga,
                    PesoNeto = x.PesoNeto,
                    NroMuestra = x.NroMuestra,
                    Localidad = x.Localidad,
                    Patente = x.Patente
                }).ToList(),
                NumeroDeLote = lote.NumeroDeLote
            };
            return new PdfLoteReporte(String.Format("Lote_{0}.pdf", lote.NumeroDeLote), loteDto, firma);
        }

        [DatosUsuario]
        public ActionResult GenerarArchivos(int loteId, DatosUsuario datosUsuario)
        {
            var loteDto = servicio.ObtenerLoteInaseParaImpresion(loteId);

            if (loteDto == null)
            {
                TempData["Alerta"] = Textos.Lote_ErrorInvalido;
                TempData["TipoAlerta"] = TipoAlerta.Advertencia;
                return RedirectToAction("Index", "ListaDeCamiones");
            }

            return DescargarRosarioZip(loteDto);
        }

        [AllowAnonymous]
        public JsonResult EnvioDeLoteAutomatico()
        {
            var result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            var lotes = servicio.ObtenerMuestrasInaseParaArchivo();

            if (lotes.Count > 0)
            {
                try
                {
                    foreach (var lote in lotes.GroupBy(x => x.CentroId))
                    {
                        var datosMail = servicio.ObtenerConfiguracionMailInase(lote.Key);
                        var resultado = EnviarMailLoteInase(lote.ToList(), datosMail, lote.Key);
                        result.Data += resultado + "\n";
                        serviciosComandos.Ejecutar(new CrearLoteInase()
                        {
                            Dto = new LoteInaseDto()
                            {
                                NombreUsuario = ConfigurationManager.AppSettings["Reportes.Username"],
                                CentroId = lote.Key,
                                Fecha = DateTime.Now,
                                Muestras = lote.ToList()
                            }
                        });
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

        public ActionResult DescargarRosarioZip(LoteInaseDto loteDto)
        {
            string str01;
            GenerarRosario(loteDto.Muestras, out str01);
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

        private void GenerarRosario(List<MuestraDeInaseDto> lote, out string archivo01)
        {
            var stringBuilder01 = new StringBuilder();

            foreach (var muestra in lote)
            {
                log.Info("Se comienza a procesar el archivo 01 ");
                stringBuilder01.AppendLine(
                    TxtHelper.GetTxtDataRow(
                        new MuestraInase
                        {
                            NumeroMuestra = muestra.NroMuestra,
                            NombreProducto = muestra.MaterialDescripcion,
                            CodigoProducto = Convert.ToInt32(muestra.CodigoCamaraMaterial ?? "0"),
                            CuitDestinatario = Convert.ToInt64(muestra.DestinatarioCuil.Replace("-", "")),
                            CuitRtteComercial = Convert.ToInt64((muestra.RtteComercialCuit ?? muestra.TitularCartaPorteCuil ?? "0").Replace("-", "")),
                            CuitCorredor = Convert.ToInt64((muestra.CorredorCuil ?? "0").Replace("-", "")),
                            CodigoPagador = 1,
                            CodigoPuerto = muestra.CodigoDeCamara != null ? (muestra.CodigoDeCamara.Length > 3 ? Convert.ToInt32(muestra.CodigoDeCamara.Substring(0, 3)) : Convert.ToInt32(muestra.CodigoDeCamara)) : 0,
                            PesoNetoSeco = muestra.PesoNeto,
                            Lacrada = "L",
                            FechaDescarga = muestra.FechaDescarga,
                            CodigoGrupo = Convert.ToInt32(muestra.CodigoCamaraGrupo ?? "0"),
                            ServicioLacrado = "S",
                            Patente = muestra.Patente,
                            RtteComercial = muestra.TitularCartaPorte ?? "",
                            CartaDePorte = muestra.CPE ?? false ? Convert.ToInt64(muestra.Sucursal + muestra.CTG) : Convert.ToInt64(muestra.CartaPorte),
                            NumeroCTG = muestra.CPE ?? false ? Convert.ToInt64(muestra.CartaPorte) : Convert.ToInt64(muestra.CTG),
                            CuitTitularCartaPorte = Convert.ToInt64((muestra.TitularCartaPorteCuil ?? "0").Replace("-", "")),
                            TitularCartaPorte = muestra.TitularCartaPorte ?? "",
                            TecnologiaDeclarada = "00",
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
                        }, typeof(MuestraInase).GetProperties()));
            }

            archivo01 = stringBuilder01.ToString();
        }

        private string EnviarMailLoteInase(List<MuestraDeInaseDto> lote, IList<ConfiguracionGeneralDto> datosMail, int centroId)
        {
            var result = "El envio del lote fue Exitoso";

            try
            {
                var smtpClient = new SmtpClient();
                ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;
                var message = new MailMessage();
                var mailDestino = datosMail.Where(x => x.Nombre.Contains("MailDestino")).FirstOrDefault().Valor;
                var destinatarios = datosMail.Where(x => x.Nombre.Contains("MailDestinatarios")).FirstOrDefault().Valor.Split(';');
                var responsable = datosMail.Where(x => x.Nombre.Contains("Responsable")).FirstOrDefault().Valor;
                var contacto = datosMail.Where(x => x.Nombre.Contains("Contacto")).FirstOrDefault().Valor;
                var atencion = datosMail.Where(x => x.Nombre.Contains("Horario")).FirstOrDefault().Valor;
                var centro = servicio.ObtenerCentro(centroId);
                message.To.Add(mailDestino);
                foreach (var address in destinatarios)
                {
                    message.CC.Add(new MailAddress(address));
                }
                message.Subject = " Resol 37/22 Muestra INASE para " + centro.Descripcion + " - " + DateTime.Now.Formatted();
                message.Body = $"Se notifican muestras para su retiro. \n " +
                    $"MOLINOSAGRO S.A. - " +
                    $"{centro.Cuit} \n" +
                    $"{centro.Planta} - {centro.Descripcion} " +
                    $"{centro.Direccion} - {centro.LocalidadDesc} \n" +
                    $"Cantidad de muestras para retirar: {lote.Count()}  \n" +
                    $"Persona de contacto: {responsable} \n" +
                    $"Contacto: {contacto} \n" +
                    $"Horario de atencion: {atencion} \n";
                log.Debug("Generando email de archivos de cámara para enviar a " + message.To.First().Address);
                string str01;
                GenerarRosario(lote, out str01);
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
    }
}
