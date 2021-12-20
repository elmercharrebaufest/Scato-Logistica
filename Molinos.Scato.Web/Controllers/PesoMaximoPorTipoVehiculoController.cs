using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AbmTipoDeVehiculo)]
    public class PesoMaximoPorTipoVehiculoController : BaseController
    {
        private readonly IServicioComandos servicioComandos;
        private ILogger log;

        public PesoMaximoPorTipoVehiculoController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos) 
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, string filtro, string ordenarPor = "TipoVehiculo", DirOrden dirOrden = DirOrden.Asc, int pagina = 1)
        {
            ListarConsulta(datosUsuario, filtro, pagina, ordenarPor, dirOrden);           
            return View();
        }

        [DatosUsuario]
        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(DatosUsuario datosUsuario,string filtro, string ordenarPor = "TipoVehiculo", DirOrden dirOrden = DirOrden.Asc, int pagina = 1)
        {
            ListarConsulta(datosUsuario, filtro, pagina, ordenarPor, dirOrden);
            return View("Listar");
        }

        private void ListarConsulta(DatosUsuario datosUsuario,string filtro, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, itemsPorPagina: 10);
            ViewBag.Items = servicio.ListarPaginadoPesoMaximoPorTipoVehiculo(paginacion, datosUsuario.CentroId);
        }

        [DatosUsuario]
        public ActionResult Crear()
        {
            var tipo = new PesoMaximoPorTipoVehiculoDto();
            return View(tipo);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Crear(PesoMaximoPorTipoVehiculoDto model, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                model.CentroId = datosUsuario.CentroId;
                var resultado = servicioComandos.Ejecutar(new CrearPesoMaximoPorTipoVehiculo { Dto = model , Usuario = datosUsuario.NombreUsuario });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            return View(model);
        }

        public ActionResult Modificar(int id)
        {
            var almacen = servicio.ObtenerPesoMaximoPorTipoVehiculo(id);
            return View(almacen);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Modificar(PesoMaximoPorTipoVehiculoDto tipo, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                tipo.CentroId = datosUsuario.CentroId;
                var resultado = servicioComandos.Ejecutar(new ModificarPesoMaximoPorTipoVehiculo { Dto = tipo, Usuario = datosUsuario.NombreUsuario });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            return View(tipo);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Eliminar(int id, DatosUsuario datosUsuario)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarPesoMaximoPorTipoVehiculo { Id = id ,Usuario = datosUsuario.NombreUsuario});
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }
    }
}
