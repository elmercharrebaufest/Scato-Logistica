using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
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
    public class SugerenciaController : BaseController
    {
        private readonly IServicioComandos servicioComandos;

        public SugerenciaController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.servicioComandos = servicioComandos;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, string textoSugerencia)
        {
            var dto = new SugerenciaDto() {CentroId = datosUsuario.CentroId, Fecha = DateTime.Now, NombreUsuario = datosUsuario.NombreUsuario,
                                            TextoSugerencia = textoSugerencia, Url = HttpContext.Request.UrlReferrer.AbsoluteUri};
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new CrearSugerencia() { Dto = dto });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            return View();
        }
      
    }
}
