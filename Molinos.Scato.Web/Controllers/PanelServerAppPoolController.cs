using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Ninject.Extensions.Logging;
using System.Configuration;
using System.Collections.Generic;
using log4net.Repository.Hierarchy;
using log4net;
using log4net.Appender;
using System.Web;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.PanelServerAppPool)]
    public class PanelServerAppPoolController : BaseController
    {
        private readonly IConfiguracionProvider configuracion;
        private readonly IServicioRepositorioFactory servicioFactory;
        private readonly ILogger log;

        public PanelServerAppPoolController(IConfiguracionProvider configuracion, IServicioRepositorioFactory servicioFactory, ILogger log, IServicioRepositorio servicio)
            : base(servicio)
        {
            this.configuracion = configuracion;
            this.servicioFactory = servicioFactory;
            this.log = log;
        }

        public ActionResult Index()
        {
            var servidorPorDefecto = SetearVista();
            return Listar(servidorPorDefecto, "Index");
        }

        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(string servidorNombre, string vista = "Listar")
        {
            try
            {
                var url = configuracion.AppSettings["HostsServiciosWeb"].Split('|').FirstOrDefault(x => x.Contains(servidorNombre));
                log.Debug("Creando servicio para: {0}", url);
                var servicio = servicioFactory.CrearServicio(url);
                log.Debug("ejecutando comando para: {0}", url);
                return View(vista, servicio.ObtenerEstadoServidor(servidorNombre));
            }
            catch (Exception e)
            {
                log.Error("No se pudo ejecutar el servicio" + e.Message);
                return View(vista, new PanelServerAppPoolDto
                {
                    Error = "No se pudo ejecutar el servicio " + servidorNombre + ": " + e.Message
                });
            }
        }

        private string SetearVista()
        {
            var elem = configuracion.AppSettings["HostsServiciosWeb"];
            var list = elem.Split('|').Select(x => x.Split('/')[2].Split('.')[0]).ToList();
            ViewBag.Servers = list.Select(x => new SelectListItem { Text = x, Value = x.ToString(CultureInfo.InvariantCulture), Selected = x == list.FirstOrDefault() }).ToList();

            return list.FirstOrDefault();
        }

        public JsonResult DetenerAppPool(string servidorNombre, string appPoolName)
        {
            try
            {
                var url = configuracion.AppSettings["HostsServiciosWeb"].Split('|').FirstOrDefault(x => x.Contains(servidorNombre));
                log.Debug("Creando servicio para: {0}", url);
                var servicio = servicioFactory.CrearServicio(url);
                log.Debug("ejecutando comando para: {0}", url);
                servicio.DetenerAppPool(appPoolName);
                return Json(String.Format("El AppPool {0} se ha detenido", appPoolName), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                log.Error(e, "Ocurrió un error al detener el appPool {0}", appPoolName);
                return Json(String.Format("Ocurrió un error al detener el AppPool {0}:" + e.Message, appPoolName),
                            JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult IniciarAppPool(string servidorNombre, string appPoolName)
        {
            try
            {
                var url = configuracion.AppSettings["HostsServiciosWeb"].Split('|').FirstOrDefault(x => x.Contains(servidorNombre));
                log.Debug("Creando servicio para: {0}", url);
                var servicio = servicioFactory.CrearServicio(url);
                log.Debug("ejecutando comando para: {0}", url);
                servicio.IniciarAppPool(appPoolName);
                return Json("El AppPool se ha iniciado", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                log.Error(e, "Ocurrió un error al iniciar el AppPool {0}" + e.Message, appPoolName);
                return Json(String.Format("Ocurrió un error al iniciar el AppPool {0}" + e.Message, appPoolName),
                            JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ReciclarAppPool(string servidorNombre, string appPoolName)
        {
            try
            {
                var url = configuracion.AppSettings["HostsServiciosWeb"].Split('|').FirstOrDefault(x => x.Contains(servidorNombre));
                log.Debug("Creando servicio para: {0}", url);
                var servicio = servicioFactory.CrearServicio(url);
                log.Debug("ejecutando comando para: {0}", url);
                servicio.ReciclarAppPool(appPoolName);
                return Json(String.Format("El AppPool {0} se ha reciclado", appPoolName), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                log.Error(e, "Ocurrió un error al reciclar el AppPool {0}" + e.Message, appPoolName);
                return Json(String.Format("Ocurrió un error al reciclar el AppPool {0}" + e.Message, appPoolName), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ConfigurarReciclado(string servidorNombre, string appPoolName, double tiempoReciclado)
        {
            try
            {
                var url = configuracion.AppSettings["HostsServiciosWeb"].Split('|').FirstOrDefault(x => x.Contains(servidorNombre));
                log.Debug("Creando servicio para: {0}", url);
                var servicio = servicioFactory.CrearServicio(url);
                log.Debug("ejecutando comando para: {0}", url);
                servicio.ConfigurarTiempoReciclado(appPoolName, tiempoReciclado);
                return Json("Se cambió el tiempo de reciclado", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                log.Error(e, "No se pudo configurar el recycle del pool {0}", appPoolName);
                return Json("No se pudo configurar el recycle del pool " + appPoolName);
            }
        }

        public ActionResult ObtenerEventos(string servidorNombre, int eventoId, DateTime fechaDesde, DateTime fechaHasta)
        {
            try
            {
                var url = configuracion.AppSettings["HostsServiciosWeb"].Split('|').FirstOrDefault(x => x.Contains(servidorNombre));
                log.Debug("Creando servicio para: {0}", url);
                var servicio = servicioFactory.CrearServicio(url);
                var listaLogs = servicio.ObtenerLogEventos(servidorNombre, eventoId, fechaDesde, fechaHasta);
                var serv = servicio.ObtenerEstadoServidor(servidorNombre);
                serv.ListaEventLogs = listaLogs;
                log.Debug("ejecutando comando para: {0}", url);
                return PartialView("ListarEventLogs", serv);
            }
            catch (Exception e)
            {
                log.Error("No se pudo ejecutar el servicio" + e.Message);
                return PartialView("ListarEventLogs", new PanelServerAppPoolDto
                {
                    Error = "No se pudo ejecutar el servicio " + ": " + e.Message
                });
            }
        }

        public JsonResult ObtenerLogs()
        {
            var raiz = ((Hierarchy)LogManager.GetRepository())
                                            .Root.Appenders.OfType<FileAppender>()
                                            .FirstOrDefault();

            var rutaArchivo = raiz != null ? raiz.File : string.Empty;
            var rutaCarpeta = Path.GetDirectoryName(rutaArchivo);

            List<string> archivos = new List<string>();
            DirectoryInfo dirInfo = new DirectoryInfo(rutaCarpeta);
            foreach (FileInfo fInfo in dirInfo.GetFiles())
            {
                if (fInfo.Name.Contains(".log"))
                    archivos.Add(fInfo.Name);
            }
            return Json(archivos.ToArray(), JsonRequestBehavior.AllowGet);
        }

        public FileResult DescargarLogs(string archivo)
        {
            var raiz = ((Hierarchy)LogManager.GetRepository())
                                            .Root.Appenders.OfType<FileAppender>()
                                            .FirstOrDefault();

            var rutaArchivo = raiz != null ? raiz.File : string.Empty;
            var rutaCarpeta = Path.GetDirectoryName(rutaArchivo);
            try
            {
                byte[] fileBytes = new byte[0];
                System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("RetornoArchivo", "ok"));
                try
                {
                     fileBytes = System.IO.File.ReadAllBytes(rutaCarpeta + '\\' + archivo);

                }
                catch
                {
                    using (var fs = new FileStream(rutaCarpeta + '\\' + archivo, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (var ms = new MemoryStream())
                        {
                            fs.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }
                    }
                }

                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, archivo);
            }
            catch
            {
                System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("RetornoArchivo", "error"));
                return null;
            }
        }
    }
}
