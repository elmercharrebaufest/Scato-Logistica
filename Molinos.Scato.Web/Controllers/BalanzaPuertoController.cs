using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.BalanzaPuerto)]
    public class BalanzaPuertoController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioOrquestador orquestador;
        public BalanzaPuertoController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos, IServicioOrquestador orquestador)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
            this.orquestador = orquestador;
        }


        public ActionResult Index(string filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(filtro, pagina, ordenarPor, dirOrden);
            return View((object)filtro);
        }

        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(string filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(filtro, pagina, ordenarPor, dirOrden);
            return View("Listar", (object)filtro);
        }

        private void ListQuery(string filtro, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);

            ViewBag.Items = servicio.ListarBalanzasPuertoPaginado(filtro, paginacion);
        }
        
        public ActionResult Crear()
        {
            ViewBag.Balanzas = orquestador.ListarBalanzasDePuerto().ToSelectList(x => x.Codigo, x => x.Descripcion);
            return View();
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Crear(BalanzaPuertoDto model, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                model.CentroId = datosUsuario.CentroId;

                var resultado = servicioComandos.Ejecutar(new CrearBalanzaPuerto { Dto = model });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            ViewBag.Balanzas = orquestador.ListarBalanzasDePuerto().ToSelectList(x => x.Codigo, x => x.Descripcion);
            return View(model);
        }
        
        public ActionResult Modificar(int id)
        {
            var aModificar = servicio.ObtenerBalanzaPuerto(id);
            ViewBag.Balanzas = orquestador.ListarBalanzasDePuerto().ToSelectList(x => x.Codigo, x => x.Descripcion);
            return View(aModificar);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Modificar(BalanzaPuertoDto model, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                model.CentroId = datosUsuario.CentroId;

                var resultado = servicioComandos.Ejecutar(new ModificarBalanzaPuerto { Dto = model });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            ViewBag.Balanzas = orquestador.ListarBalanzasDePuerto().ToSelectList(x => x.Codigo, x => x.Descripcion);
            return View(model);
        }

        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarBalanzaPuerto { Id = id });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }
    }
}
