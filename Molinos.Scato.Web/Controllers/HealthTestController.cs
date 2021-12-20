using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.Web.Administration;
using Molinos.Scato.Servicios;
using Ninject.Extensions.Logging;
namespace Molinos.Scato.Web.Controllers
{
    public class HealthTestController : Controller
    {
        private readonly IServicioRepositorio servicioRepositorio;
        private readonly IConfiguracionProvider configuracion;
        private readonly ILogger log;
        private static bool servidorOperando;
        private static readonly object LockObject = new object();

        public HealthTestController(ILogger log, IServicioRepositorio servicioRepositorio, IConfiguracionProvider configuracion)
        {
            this.log = log;
            this.servicioRepositorio = servicioRepositorio;
            this.configuracion = configuracion;
        }


        public ActionResult Index()
        {
            log.Debug("Iniciando Health Test");
            try
            {
                servicioRepositorio.ObtenerTipoDocumentoIdentidad(1);
                servidorOperando = true;
                log.Info("Health Test OK");
                return Content("OK");
            }
            catch (Exception exception)
            {
                log.Error(exception, "Health Test: ocurrió un error al acceder a la capa de servicios");
                var reciclar = false;
                lock (LockObject)
                {
                    if (servidorOperando)
                    {
                        reciclar = true;
                        servidorOperando = false;
                    }
                }
                if (reciclar)
                {
                    log.Info("Reciclando Application Pools...");
                    Task.Run(() => RecycleAppPools());
                }
                else
                {
                    log.Warn("Ya existe un reciclado de App Pools en proceso. Salteando el reciclado...");
                }
                
                throw;
            }
        }


        private void RecycleAppPools()
        {
            try
            {
                var serverManager = new ServerManager();

                var listaAppPool = ListarAppsPool();

                foreach (var s in listaAppPool)
                {
                    log.Debug("Intentando reciclar Pool '{0}'", s);
                    var ap = serverManager.ApplicationPools[s];
                    if (ap == null)
                    {
                        log.Error("No se encontró el AppPool '{0}'", s);
                    }
                    else if (ap.State == ObjectState.Stopped)
                    {
                        log.Debug("Iniciando Application Pool '{0}'", s);
                        ap.Start();
                        log.Debug("Application Pool '{0}' iniciado", s);
                    }
                    else
                    {
                        log.Debug("Reciclando Application Pool '{0}'", s);
                        ap.Recycle();
                        log.Debug("Application Pool '{0}' reciclado", s);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "Ocurrió un error al reciclar los application pools");
            }
        }


        private IEnumerable<string> ListarAppsPool()
        {
            var valores = configuracion.AppSettings["AppPools"];
            return valores != null ? valores.Split(';').ToList() : new List<string>();
        }
        
    }
}
