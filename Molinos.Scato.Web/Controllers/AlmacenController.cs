using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AbmAlmacen)]
    public class AlmacenController : BaseController
    {
        private readonly IServicioComandos servicioComandos;
        private ILogger log;

        public AlmacenController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos) 
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, string filtro, string ordenarPor = "Descripcion", DirOrden dirOrden = DirOrden.Asc, int pagina = 1)
        {
            ListarConsulta(filtro, pagina, ordenarPor, dirOrden, datosUsuario.CentroId);
            ViewBag.SoloLectura = "true";
            return View();
        }

        [DatosUsuario]
        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(DatosUsuario datosUsuario, string filtro, string ordenarPor = "Descripcion", DirOrden dirOrden = DirOrden.Asc, int pagina = 1)
        {
            ListarConsulta(filtro, pagina, ordenarPor, dirOrden, datosUsuario.CentroId);
            return View("Listar");
        }

        private void ListarConsulta(string filtro, int pagina, string ordenarPor, DirOrden dirOrden, int centroId)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, itemsPorPagina: 10);
            ViewBag.Items = servicio.ListarPaginadoAlmacenes(filtro, paginacion, centroId);
        }

        [DatosUsuario]
        public ActionResult Crear(DatosUsuario datosUsuario)
        {
            var asd = new AlmacenDto {CentroId = datosUsuario.CentroId};
            GetTipoSoja(asd);

            return View(asd);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Crear(AlmacenDto model, DatosUsuario datosUsuario)
        {
            PostTipoSoja(model);
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new CrearAlmacen { Dto = model, Usuario = datosUsuario.NombreUsuario});
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
            var almacen = servicio.ObtenerAlmacen(id);
            GetTipoSoja(almacen);
           
            return View(almacen);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Modificar(AlmacenDto almacen, DatosUsuario datosUsuario)
        {
            PostTipoSoja( almacen);
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new ModificarAlmacen { Dto = almacen, Usuario = datosUsuario.NombreUsuario});
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();                    
                }
                ModelState.AgregarErrores(resultado);
            }           
            return View(almacen);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Eliminar(int id, DatosUsuario datosUsuario)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarAlmacen { Id = id, Usuario = datosUsuario.NombreUsuario});
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        private void GetTipoSoja(AlmacenDto almacen)
        {
            if (almacen.EsSojaSustentable == true)
            {
                almacen.TipoSoja = Constantes.TipoSoja.Sustentable;
            }
            else if (almacen.EsSojaEPA != false)
            {
                almacen.TipoSoja = Constantes.TipoSoja.EPA;
            }
        }

        private void PostTipoSoja(AlmacenDto almacen)
        {
             almacen.EsSojaSustentable = almacen.TipoSoja == Constantes.TipoSoja.Sustentable;
             almacen.EsSojaEPA = almacen.TipoSoja == Constantes.TipoSoja.EPA;
        }
    }
}
