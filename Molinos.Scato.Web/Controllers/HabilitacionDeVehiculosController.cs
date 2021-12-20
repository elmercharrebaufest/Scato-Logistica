using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.HabilitacionDeVehiculos)]
    public class HabilitacionDeVehiculosController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public HabilitacionDeVehiculosController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos, IServicioOrquestador servicioOrquestador)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario)
        {
            Listar(datosUsuario.CentroId);
            return View();
        }
        private void Listar(int centroId)
        {
            var paginacion = new Paginacion("TipoVehiculo",  DirOrden.Asc, 1, 40);
            ViewBag.Items = servicio.ListarPaginadoPesoMaximoPorTipoVehiculo(paginacion, centroId);
        }

        [DatosUsuario]
        public JsonResult ModificarModalidad(DatosUsuario datosUsuario, int id)
        {
            try
            {

                var almacen = servicio.ObtenerPesoMaximoPorTipoVehiculo(id);
                almacen.CentroId = datosUsuario.CentroId;
                almacen.Activo = !almacen.Activo;
                var resultado = servicioComandos.Ejecutar(new ModificarPesoMaximoPorTipoVehiculo { Dto = almacen, Usuario = datosUsuario.NombreUsuario });

                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(Textos.Error_ActualizarGenerico, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
