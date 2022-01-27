using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    [Autorizacion(PermisosScato.AbmCategoriaCamiones)]
    public class CategoriaCamionesController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public CategoriaCamionesController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        public ActionResult Index(string filtro, int pagina = 1, string ordenarPor = "Patente", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(filtro, pagina, ordenarPor, dirOrden);
            return View((object)filtro);
        }

        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(string filtro, int pagina = 1, string ordenarPor = "Patente", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(filtro, pagina, ordenarPor, dirOrden);
            return View("Listar", (object)filtro);
        }

        private void ListQuery(string filtro, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);

            ViewBag.Items = servicio.ListarCategoriaCamiones(filtro, paginacion);
        }

        public ActionResult Crear()
        {
            SetearCategoriasVehiculoDropDownList();
            return View();
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Crear(CategoriaVehiculoDto model, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new CrearCategoriaVehiculo { Dto = model, Usuario = datosUsuario.NombreUsuario });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            SetearCategoriasVehiculoDropDownList();
            return View(model);
        }

        public ActionResult Modificar(int id)
        {
            var vehiculoAModificar = servicio.ObtenerCategoriaVehiculo(id);
            SetearCategoriasVehiculoDropDownList();
            return View(vehiculoAModificar);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Modificar(CategoriaVehiculoDto model, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new ModificarCategoriaVehiculo { Dto = model, Usuario = datosUsuario.NombreUsuario });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            SetearCategoriasVehiculoDropDownList();
            return View(model);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Eliminar(int id, DatosUsuario datosUsuario)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarCategoriaVehiculo { Id = id, Usuario = datosUsuario.NombreUsuario });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        private void SetearCategoriasVehiculoDropDownList()
        {
            ViewBag.TiposDeVehiculos = ListarTiposVehiculo();
        }

        public List<SelectListItem> ListarTiposVehiculo()
        {
            var enums = Enum.GetValues(typeof(TipoVehiculo)).Cast<TipoVehiculo>();
            var listaTipoVehiculos = new List<SelectListItem>();
            foreach (var item in enums)
            {
                int value = (int)item;
                if (value != -1)
                {
                    var tipoVehiculo = new SelectListItem
                    {
                        Value = value.ToString(),
                        Text = item.DisplayEnum()
                    };
                    listaTipoVehiculos.Add(tipoVehiculo);
                }
            }
            return listaTipoVehiculos;
        }

    }
}