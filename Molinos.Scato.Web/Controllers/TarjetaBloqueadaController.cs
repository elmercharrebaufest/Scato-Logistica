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
    [Autorizacion(PermisosScato.AbmTarjetaBloqueada)]
    public class TarjetaBloqueadaController : BaseController
    {
        private readonly IServicioComandos servicioComandos;
        private ILogger log;

        public TarjetaBloqueadaController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos) 
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, string filtro, string ordenarPor = "Numero", DirOrden dirOrden = DirOrden.Asc, int pagina = 1)
        {
            ListarConsulta(filtro, pagina, ordenarPor, dirOrden, datosUsuario.CentroId);
            return View();
        }

        [DatosUsuario]
        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(DatosUsuario datosUsuario, string filtro, string ordenarPor = "Numero", DirOrden dirOrden = DirOrden.Asc, int pagina = 1)
        {
            ListarConsulta(filtro, pagina, ordenarPor, dirOrden, datosUsuario.CentroId);
            return View("Listar");
        }

        private void ListarConsulta(string filtro, int pagina, string ordenarPor, DirOrden dirOrden, int centroId)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, itemsPorPagina: 10);
            ViewBag.Items = servicio.ListarPaginadoTarjetasBloqueadas(filtro, paginacion, centroId);
        }

        [DatosUsuario]
        public ActionResult Bloquear(DatosUsuario datosUsuario)
        {
            var tarjeta = new TarjetaBloqueadaDto {CentroId = datosUsuario.CentroId};
            return View(tarjeta);
        }

        [HttpPost]
        public ActionResult Bloquear(TarjetaBloqueadaDto model)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new BloquearTarjeta {Dto = model});
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
            var tarjeta = servicio.ObtenerTarjetaBloqueada(id);
            return View(tarjeta);
        }

        [HttpPost]
        public ActionResult Modificar(TarjetaBloqueadaDto tarjeta)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new ModificarTarjetaBloqueada { Dto = tarjeta });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            return View(tarjeta);
        }

        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarTarjetaBloqueada { Id = id });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }
    }
}
