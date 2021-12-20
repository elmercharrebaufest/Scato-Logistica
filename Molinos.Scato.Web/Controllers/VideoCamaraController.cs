using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    public class VideoCamaraController : BaseController
    {
        private ILogger log;
        private readonly IServicioOrquestador orquestador;
        private readonly IServicioComandos servicioComandos;

        public VideoCamaraController(ILogger log, IServicioRepositorio servicio, IServicioOrquestador orquestador, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.orquestador = orquestador;
            this.servicioComandos = servicioComandos;
        }
        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario)
        {
            var camaras = servicio.ListarCamarasPorNombrePc("PuestoVC", datosUsuario.CentroId).ToList();
            log.Debug(string.Join(", ", camaras.ToArray()));
            ViewBag.Camaras = camaras;
            return View();
        }
    }
}
