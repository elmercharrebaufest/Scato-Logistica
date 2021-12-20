using System;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.WebOperaciones.Atributos;
using Molinos.Scato.WebOperaciones.Helpers;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.WebOperaciones.Controllers
{
    [Autorizacion(PermisosScato.ScatoPuerto)]
    public class CamaraController : Controller
    {
        private readonly ILogger log;
        private readonly IServicioRepositorio servicio;
        private readonly IServicioOrquestador servicioOrquestador;


        public CamaraController(ILogger log, IServicioRepositorio servicio, IServicioOrquestador servicioOrquestador)
        {
            this.log = log;
            this.servicio = servicio;          
            this.servicioOrquestador = servicioOrquestador;
        }
        public ActionResult Index()
        {
            ViewBag.Camaras = servicio.ListarVideoCamarasPuerto().ToSelectList(x => x.Codigo, x => x.Codigo);
            return View();
        }

        public ActionResult TomarFoto(string codigo)
        {
            log.Debug("Pidiendo foto de la camara {0}", codigo);
            var resultado = servicioOrquestador.Ejecutar(new EjecutarTomarFoto
            {
                CodigoDispositivo = codigo
            });
            log.Debug("Resultado de pedir foto de la camara {0}: {1}", codigo, resultado);
            if (resultado.Mensaje.Codigo != 0)
            {
                return null;
            }
            return Json(Convert.ToBase64String(((ResultadoTomarFoto)resultado).Imagen), JsonRequestBehavior.AllowGet);
        }
    }
}