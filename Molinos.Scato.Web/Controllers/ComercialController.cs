using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;
using NPOI.OpenXmlFormats.Wordprocessing;

namespace Molinos.Scato.Web.Controllers
{
    
    public class ComercialController : BaseController
    {
        private readonly IServicioComandos servicioComandos;
        private ILogger log;

        public ComercialController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos) 
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        
        public ActionResult Index(string filtro, string ordenarPor = "Descripcion", DirOrden dirOrden = DirOrden.Asc, int pagina = 1)
        {
            ListarConsulta(filtro, pagina, ordenarPor, dirOrden);
            ViewBag.SoloLectura = "true";
            return View();
        }

        
        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(string filtro, string ordenarPor = "Descripcion", DirOrden dirOrden = DirOrden.Asc, int pagina = 1)
        {
            ListarConsulta(filtro, pagina, ordenarPor, dirOrden);
            return View("Listar");
        }

        
        private void ListarConsulta(string filtro, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, itemsPorPagina: 10);
            ViewBag.Items = servicio.ListarPaginadoComerciales(filtro, paginacion);
        }

        [DatosUsuario]
        public ActionResult Crear(DatosUsuario datosUsuario)
        {
            var newComercial = new ComercialDto {};
            return View(newComercial);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Crear(ComercialDto model, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new CrearComercial { Dto = model, Usuario = datosUsuario.NombreUsuario});
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
            var comercial = servicio.ObtenerComercial(id);
           
            return View(comercial);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Modificar(ComercialDto comercial, DatosUsuario datosUsuario)
        {
            
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new ModificarComercial { Dto = comercial, Usuario = datosUsuario.NombreUsuario});
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();                    
                }
                ModelState.AgregarErrores(resultado);
            }           
            return View(comercial);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Eliminar(int id, DatosUsuario datosUsuario)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarComercial { Id = id, Usuario = datosUsuario.NombreUsuario});
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        
    }
}
