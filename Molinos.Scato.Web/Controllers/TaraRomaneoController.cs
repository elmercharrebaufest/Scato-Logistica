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
    [Autorizacion(PermisosScato.AbmTaraRomaneo)]
    public class TaraRomaneoController : BaseController
    {
        private readonly IServicioComandos servicioComandos;
        private ILogger log;

        public TaraRomaneoController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }


        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, string filtro, string ordenarPor = "Codigo", DirOrden dirOrden = DirOrden.Asc, int pagina = 1)
        {
            ListarConsulta(filtro, pagina, ordenarPor, dirOrden, datosUsuario.CentroId);
            return View();
        }
     

        [DatosUsuario]
        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(DatosUsuario datosUsuario, string filtro, string ordenarPor = "Codigo", DirOrden dirOrden = DirOrden.Asc, int pagina = 1)
        {
            ListarConsulta(filtro, pagina, ordenarPor, dirOrden, datosUsuario.CentroId);
            return View("Listar", (object)filtro);
        }


        
        private void ListarConsulta(string filtro, int pagina, string ordenarPor, DirOrden dirOrden, int centroId)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, itemsPorPagina: 10);

            ViewBag.Items = servicio.ListarPaginadoTaraRomaneo(filtro, paginacion, centroId);
        }

        [DatosUsuario]
        public ActionResult Crear(DatosUsuario datosUsuario)
        {
            var romaneo = new TaraRomaneoDto {CentroId = datosUsuario.CentroId};
            return View(romaneo);
        }

        [HttpPost]
        public ActionResult Crear(TaraRomaneoDto model)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new CrearTaraRomaneo { Dto = model });
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
            var romaneo = servicio.ObtenerTaraRomaneo(id);
            

            return View(romaneo);
        }

        [HttpPost]
        public ActionResult Modificar(TaraRomaneoDto romaneo)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new ModificarTaraRomaneo { Dto = romaneo });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();                    
                }
                ModelState.AgregarErrores(resultado);
            }
            
            return View(romaneo);
        }

        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarTaraRomaneo { Id = id });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }


    }
}
