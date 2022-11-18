using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Molinos.Scato.Web.Controllers
{
    public class ConfiguracionCalleHidraulicaController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioOrquestador servicioOrquestador;

        public ConfiguracionCalleHidraulicaController(ILogger log,
            IServicioRepositorio servicio,
            IServicioComandos servicioComandos,
            IServicioOrquestador servicioOrquestador)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
            this.servicioOrquestador = servicioOrquestador;
        }

        public ActionResult Index(string filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListarConsulta(filtro, pagina, ordenarPor, dirOrden);
            return View();
        }

        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(string filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListarConsulta(filtro, pagina, ordenarPor, dirOrden);
            return View("Listar");
        }

        private void ListarConsulta(string filtro, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(
                ordenarPor,
                dirOrden,
                pagina,
                10);

            ViewBag.Items = servicio.ListarPaginadoCalleHidraulica(paginacion);
        }

        public ActionResult Modificar(int id)
        {
            var calleHidraulica = servicio.ObtenerCalleHidraulica(id);
            return View(calleHidraulica);
        }

        [HttpPost]
        public ActionResult Modificar(ConfiguracionCalleHidraulicaDto calleHidraulica)
        {
            try
            {
                servicioComandos.Ejecutar(new ActualizarConfiguracionCalleHidraulica { Id = calleHidraulica.Id, CodigoCartel = calleHidraulica.CodigoCartel, CodigoSensorCamaraALPR = calleHidraulica.CodigoSensorCamaraALPR, CodigoSensorCirculacion = calleHidraulica.CodigoSensorCirculacion, CodigoCamaraALPR = calleHidraulica.CodigoCamaraALPR });
            }
            catch (Exception e)
            {
                log.Error(e, $"No se pudo actualizar el sensor de la calle {calleHidraulica.CalleNombre}");
            }
            return Json("ok", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarConfiguracionCalleHidraulica { Id = id });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        public ActionResult Crear()
        {
            return View();
        }

        //[HttpPost]
        //public ActionResult Crear(ConfiguracionCalleHidraulicaDto calleHidraulica)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var resultado = servicioComandos.Ejecutar(new CrearConfiguracionCalleHidraulica { Dto = calleHidraulica });
             
        //        ModelState.AgregarErrores(resultado);
        //    }
        //    return View(calleHidraulica);
        //}
    }
}