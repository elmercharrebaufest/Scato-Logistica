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
   [Autorizacion(PermisosScato.AbmTalonario)]
    public class TalonarioController : BaseController
    {
        private readonly IServicioComandos servicioComandos;
        private ILogger log;

        public TalonarioController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }


        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, string filtro, string ordenarPor = "Descripcion", DirOrden dirOrden = DirOrden.Asc, int pagina = 1)
        {
       
            
            ListarConsulta(filtro, pagina, ordenarPor, dirOrden, datosUsuario.CentroId);
            return View();
        }
  

        [DatosUsuario]
        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(DatosUsuario datosUsuario, string filtro, string ordenarPor = "Descripcion", DirOrden dirOrden = DirOrden.Asc, int pagina = 1)
        {
            ListarConsulta(filtro, pagina, ordenarPor, dirOrden, datosUsuario.CentroId);
            return View("Listar", (object)filtro);
        }


        
        private void ListarConsulta(string filtro, int pagina, string ordenarPor, DirOrden dirOrden, int centroId)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, itemsPorPagina: 10);

            ViewBag.Items = servicio.ListarPaginadoTalonario(filtro, paginacion, centroId);
        }


        [DatosUsuario]
        public ActionResult Crear(DatosUsuario datosUsuario)
        {
            var talonario = new TalonarioDto {CentroId = datosUsuario.CentroId};
            return View(talonario);
        }

        [HttpPost]
        public ActionResult Crear(TalonarioDto model)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new CrearTalonario { Dto = model });
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
            var talonario = servicio.ObtenerTalonario(id);
            

            return View(talonario);
        }

        [HttpPost]
        public ActionResult Modificar(TalonarioDto talonario)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new ModificarTalonario { Dto = talonario });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();                    
                }
                ModelState.AgregarErrores(resultado);
            }
            
            return View(talonario);
        }

        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarTalonario { Id = id });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }
    }
}
