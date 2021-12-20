using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AbmReglasDeAnalisisObligatorio)]
    public class ReglaDeAnalisisObligatorioController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public ReglaDeAnalisisObligatorioController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, string filtro = "", int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(filtro, pagina, ordenarPor, dirOrden, datosUsuario.CentroId);
            return View();
        }

        [AjaxOnly]
        [ActionName("Index")]
        [DatosUsuario]
        public ActionResult Listar(DatosUsuario datosUsuario, string filtro = "", int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(filtro, pagina, ordenarPor, dirOrden, datosUsuario.CentroId);
            return View("Listar");
        }

        private void ListQuery(string filtro, int pagina, string ordenarPor, DirOrden dirOrden, int centroId)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);
            ViewBag.Items = servicio.ListarReglaDeAnalisisObligatorioPaginado(paginacion, filtro, centroId);
        }

        [DatosUsuario]
        public ActionResult Crear()
        {
            var provincias = servicio.ListarProvincias().ToSelectList(f => f.Id.ToString(), f => f.Descripcion);
            ViewBag.Provincias = provincias;
            var reglaDeAnalisisObligatorio = new ReglaDeAnalisisObligatorioDto { };
            return View(reglaDeAnalisisObligatorio);
        }

        [DatosUsuario]
        [HttpPost]
        public ActionResult Crear(DatosUsuario datosUsuario, ReglaDeAnalisisObligatorioDto model)
        {
            if (ModelState.IsValid)
            {
                model.CentroId = datosUsuario.CentroId;
                var resultado = (ResultadoCrear)servicioComandos.Ejecutar(new CrearReglaDeAnalisisObligatorio { Dto = model, Usuario = datosUsuario.NombreUsuario });

                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            return View(model);
        }

        [DatosUsuario]
        public ActionResult Modificar(int id)
        {
            var provincias = servicio.ListarProvincias().ToSelectList(f => f.Id.ToString(), f => f.Descripcion);
            ViewBag.Provincias = provincias;
            var aModificar = servicio.ObtenerReglaDeAnalisisObligatorio(id);
            return View(aModificar);
        }

        [DatosUsuario]
        [HttpPost]
        public ActionResult Modificar(DatosUsuario datosUsuario, string descuentos, ReglaDeAnalisisObligatorioDto model)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new ModificarReglaDeAnalisisObligatorio
                {
                    Dto = model,
                    Usuario = datosUsuario.NombreUsuario
                });

                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            ViewBag.descuentos = descuentos;
            return View(model);
        }

        [DatosUsuario]
        [HttpPost]
        public ActionResult Eliminar(int id, DatosUsuario datosUsuario)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarReglaDeAnalisisObligatorio { Id = id, Usuario = datosUsuario.NombreUsuario });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

    }
}
