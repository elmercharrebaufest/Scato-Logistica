using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web.Mvc;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.DashboardCardless)]
    public class DashboardCardlessController : BaseController
    {
        private readonly ILogger log;

        public DashboardCardlessController(ILogger log, IServicioRepositorio servicio)
            : base(servicio)
        {
            this.log = log;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, FiltroDashboardCardlessDto filtro)
        {
            if (filtro == null || filtro.FechaDesde == default(DateTime))
            {
                filtro = new FiltroDashboardCardlessDto
                {
                    FechaDesde = DateTime.Today.AddDays(-7),
                    FechaHasta = DateTime.Today
                };
            }

            var puestos = servicio.ListarPuestosDeTrabajoPorCentro(datosUsuario.CentroId)
                .Where(p => !string.IsNullOrEmpty(p.CodigoConfigIdentificacionVehicular));
            ViewBag.Puestos = new SelectList(puestos, "Id", "NombrePuesto");
            ViewBag.Filtro = filtro;

            return View(filtro);
        }

        [DatosUsuario]
        [HttpPost]
        public JsonResult ObtenerCamionesPorDia(DatosUsuario datosUsuario, FiltroDashboardCardlessDto filtro)
        {
            log.Debug("Dashboard Cardless: ObtenerCamionesPorDia, usuario: {0}", datosUsuario.NombreUsuario);
            var resultado = servicio.ObtenerCamionesPorDia(filtro);
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        [HttpPost]
        public JsonResult ObtenerPatentePorCamara(DatosUsuario datosUsuario, FiltroDashboardCardlessDto filtro)
        {
            log.Debug("Dashboard Cardless: ObtenerPatentePorCamara, usuario: {0}", datosUsuario.NombreUsuario);
            var resultado = servicio.ObtenerPatentePorCamara(filtro);
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        [HttpPost]
        public JsonResult ObtenerReconocimientoPorDiaSemana(DatosUsuario datosUsuario, FiltroDashboardCardlessDto filtro)
        {
            log.Debug("Dashboard Cardless: ObtenerReconocimientoPorDiaSemana, usuario: {0}", datosUsuario.NombreUsuario);
            var resultado = servicio.ObtenerReconocimientoPorDiaSemana(filtro);
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        [HttpPost]
        public JsonResult ObtenerVehiculoPorDia(DatosUsuario datosUsuario, FiltroDashboardCardlessDto filtro)
        {
            log.Debug("Dashboard Cardless: ObtenerVehiculoPorDia, usuario: {0}", datosUsuario.NombreUsuario);
            var resultado = servicio.ObtenerVehiculoPorDia(filtro);
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        [HttpPost]
        public JsonResult ObtenerReconocimientoPorProveedor(DatosUsuario datosUsuario, FiltroDashboardCardlessDto filtro)
        {
            log.Debug("Dashboard Cardless: ObtenerReconocimientoPorProveedor, usuario: {0}", datosUsuario.NombreUsuario);
            var resultado = servicio.ObtenerReconocimientoPorProveedor(filtro);
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        [HttpPost]
        public JsonResult ObtenerPromedioIntentos(DatosUsuario datosUsuario, FiltroDashboardCardlessDto filtro)
        {
            log.Debug("Dashboard Cardless: ObtenerPromedioIntentos, usuario: {0}", datosUsuario.NombreUsuario);
            var barras = servicio.ObtenerPromedioIntentosPorDia(filtro);
            var resumen = servicio.ObtenerResumenIntentos(filtro);
            return Json(new { barras, resumen }, JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        [HttpPost]
        public JsonResult ObtenerCapturasFallidas(DatosUsuario datosUsuario, FiltroCapturasFallidasDto filtro, int pagina = 1)
        {
            log.Debug("Dashboard Cardless: ObtenerCapturasFallidas, usuario: {0}", datosUsuario.NombreUsuario);
            var resultado = servicio.ObtenerCapturasFallidas(filtro, pagina);
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ObtenerImagen(int id)
        {
            var rutaFisica = servicio.ObtenerRutaImagenCaptura(id);
            if (string.IsNullOrEmpty(rutaFisica) || !System.IO.File.Exists(rutaFisica))
                return HttpNotFound();

            var extension = Path.GetExtension(rutaFisica).ToLowerInvariant();
            var mime = extension == ".png" ? "image/png" : "image/jpeg";
            return new FileStreamResult(new System.IO.FileStream(rutaFisica, System.IO.FileMode.Open, System.IO.FileAccess.Read), mime);
        }

        [DatosUsuario]
        [HttpPost]
        public ActionResult DescargarCapturasFallidas(DatosUsuario datosUsuario, FiltroCapturasFallidasDto filtro)
        {
            log.Debug("Dashboard Cardless: DescargarCapturasFallidas, usuario: {0}", datosUsuario.NombreUsuario);

            var capturas = servicio.ObtenerCapturasFallidasParaDescarga(filtro);

            using (var ms = new MemoryStream())
            {
                using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
                {
                    foreach (var captura in capturas.Where(c => !string.IsNullOrEmpty(c.RutaImagen)))
                    {
                        var rutaFisica = captura.RutaImagen;
                        if (!System.IO.File.Exists(rutaFisica))
                            continue;

                        var nombreArchivo = string.Format("{0}_{1}{2}",
                            captura.FechaEvento.ToString("yyyyMMdd_HHmmss"),
                            captura.CodigoCamara,
                            Path.GetExtension(rutaFisica));

                        var entry = zip.CreateEntry(nombreArchivo);
                        using (var entryStream = entry.Open())
                        using (var fileStream = new FileStream(rutaFisica, FileMode.Open, FileAccess.Read))
                        {
                            fileStream.CopyTo(entryStream);
                        }
                    }
                }

                var resultado = new MemoryStream(ms.ToArray());
                resultado.Seek(0, SeekOrigin.Begin);
                return new FileStreamResult(resultado, "application/octet-stream")
                {
                    FileDownloadName = "CapturasFallidas.zip"
                };
            }
        }
    }
}
