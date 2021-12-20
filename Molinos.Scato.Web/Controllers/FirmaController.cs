using System;
using System.IO;
using System.Web.Mvc;
using System.Web.UI;
using Molinos.Scato.Dominio.Comandos;
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
    public class FirmaController : BaseController
    {
        private readonly IServicioComandos servicioComandos;
        private readonly IFirmaProvider firmaProvider;
        private readonly IConfiguracionProvider configuracion;
        private readonly IServicioComandosFactory servicioComandosFactory;
        private readonly ILogger log;

        public FirmaController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos, IFirmaProvider firmaProvider,IConfiguracionProvider configuracion, IServicioComandosFactory servicioComandosFactory)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
            this.firmaProvider = firmaProvider;
            this.configuracion = configuracion;
            this.servicioComandosFactory = servicioComandosFactory;
        }

        [Autorizacion(PermisosScato.AbmFirma)]
        public ActionResult Index()
        {
            var firma = servicio.ObtenerFirma() ?? new FirmaDto();
            return View(firma);
        }

        [HttpPost]
        [Autorizacion(PermisosScato.AbmFirma)]
        [DatosUsuario]
        public ActionResult Index(DatosUsuario usuario , FirmaDto firma)
        {            
            if (ModelState.IsValid)
            {
                log.Debug("Se va a modificar datos de Empresa por el usuario: {0}",usuario.NombreUsuario);
                if (firma.LogoFile != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        firma.LogoFile.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                        firma.Logo = array;
                        firma.LogoFile = null;
                    }
                }
                if (firma.FaviconFile != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        firma.FaviconFile.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                        firma.Favicon = array;
                        firma.FaviconFile = null;
                    }
                }
                ModificarFirma(new ModificarFirma {Dto = firma});

                if (ModelState.IsValid)
                {
                    ViewBag.ResultadoOperacion = "Exitosa";
                    log.Debug("Modificación de datos de Empresa exitosa");
                    return View("Index",firma);
                }
                log.Error("Modificación de datos de Empresa con errores");
            }
            return View("Index", firma);
        }

        private void ModificarFirma(ModificarFirma model)
        {
            var hostsServiciosWeb = configuracion.AppSettings["HostsServiciosWeb"];
            log.Debug("Iniciando Modificacion de firma en servidores {0}", hostsServiciosWeb);
            foreach (var url in hostsServiciosWeb.Split('|'))
            {
                try
                {
                    log.Debug("Creando servicio para: {0}", url);
                    var servicioComandos = servicioComandosFactory.CrearServicio(url);
                    log.Debug("ejecutando comando para: {0}", url);
                    var resultado = servicioComandos.Ejecutar(model);
                    ModelState.AgregarErrores(resultado);
                }
                catch (Exception e)
                {
                    log.Error(e, "No se pudo modificar firma");
                    ModelState.AddModelError("", Textos.Error_Generico);
                }

            }
        }

        [OutputCache(Duration = 3600,Location = OutputCacheLocation.Client)]
        public FileContentResult Logo()
        {
            return File(firmaProvider.ObtenerLogo(), "image/png");
        }

        [AllowAnonymous]
        public string Favicon()
        {
            return "data:image/x-icon;base64," + Convert.ToBase64String(firmaProvider.ObtenerFavicon());
        }
    }
}
