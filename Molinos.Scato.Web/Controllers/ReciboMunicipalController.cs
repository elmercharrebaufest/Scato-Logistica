using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AbmReciboMunicipal)]
    public class ReciboMunicipalController : BaseController
    {
        private readonly IServicioComandos servicioComandos;
        private ILogger log;

        public ReciboMunicipalController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, string filtro, string ordenarPor = "FechaActivacion", DirOrden dirOrden = DirOrden.Asc, int pagina = 1)
        {
            ListarConsulta(filtro, pagina, ordenarPor, dirOrden, datosUsuario.CentroId);
            ViewBag.SoloLectura = "true";
            return View((object)filtro);
        }

        [DatosUsuario]
        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(DatosUsuario datosUsuario, string filtro, string ordenarPor = "FechaActivacion", DirOrden dirOrden = DirOrden.Asc, int pagina = 1)
        {
            ListarConsulta(filtro, pagina, ordenarPor, dirOrden, datosUsuario.CentroId);
            return View("Listar", (object)filtro);
        }

        private void ListarConsulta(string filtro, int pagina, string ordenarPor, DirOrden dirOrden, int centroId)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, itemsPorPagina: 10);
            ViewBag.Items = servicio.ListarPaginadoReciboMunicipal(filtro, paginacion, centroId);
        }

        [DatosUsuario]
        public ActionResult Crear(DatosUsuario datosUsuario)
        {
            var recibo = new ReciboMunicipalDto { CentroId = datosUsuario.CentroId };
            SetearVista(recibo.CentroId);
            return View(recibo);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Crear(ReciboMunicipalDto model, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                model.TipoVehiculo = (TipoVehiculo?)model.TipoVehiculoId;
                var resultado = servicioComandos.Ejecutar(new CrearReciboMunicipal { Dto = model, Usuario = datosUsuario.NombreUsuario });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
                SetearVista(datosUsuario.CentroId);
            }
            return View(model);
        }

        public ActionResult Modificar(int id)
        {
            var recibo = servicio.ObtenerReciboMunicipal(id);
            SetearVista(recibo.CentroId);
            recibo.TipoVehiculoId = (int?)recibo.TipoVehiculo;
            return View(recibo);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Modificar(ReciboMunicipalDto recibo, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                recibo.TipoVehiculo = (TipoVehiculo?)recibo.TipoVehiculoId;
                var resultado = servicioComandos.Ejecutar(new ModificarReciboMunicipal { Dto = recibo, Usuario = datosUsuario.NombreUsuario });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
                SetearVista(datosUsuario.CentroId);
            }
            return View(recibo);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Eliminar(int id, DatosUsuario datosUsuario)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarReciboMunicipal { Id = id, Usuario = datosUsuario.NombreUsuario });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }
        [DatosUsuario]
        public void SetearVista(int centroId) 
        {
            var pesoMaximoPorTipoVehiculo = servicio.ListarPesoMaximoPorTipoVehiculoPorCentro(centroId);
            ViewBag.TiposVehiculo = pesoMaximoPorTipoVehiculo.ToSelectList(f => ((int)f.TipoVehiculo).ToString(), f => f.TipoVehiculo.DisplayText());
        }

    }
}
