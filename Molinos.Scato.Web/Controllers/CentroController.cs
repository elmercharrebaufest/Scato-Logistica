using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class CentroController : BaseController
    {
        private readonly IServicioComandos servicioComandos;
        private ILogger log;

        public CentroController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        [DatosUsuario]
        public ActionResult Crear()
        {
            CargarCamaras();
            CargarProvincias(null);
            return View();
        }

        [DatosUsuario]
        [HttpPost]
        public ActionResult Crear(CentroDto model, DatosUsuario datosUsuario)
        {
            ModelState["Id"].Errors.Clear();
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new CrearCentro { Dto = model, NombreUsuario = datosUsuario.NombreUsuario, Usuario = datosUsuario.NombreUsuario });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            CargarCamaras();
            CargarProvincias(null);
            return View(model);
        }

        [Autorizacion(PermisosScato.AbmCentro)]
        [DatosUsuario]
        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(DatosUsuario datosUsuario, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(datosUsuario, pagina, ordenarPor, dirOrden);

            ViewBag.ResultadoOperacion = "";
            return View("Listar");
        }
        
        [Autorizacion(PermisosScato.AbmCentro)]
        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(datosUsuario, pagina, ordenarPor, dirOrden);

            ViewBag.ResultadoOperacion = "";
            return View();
        }

        private void ListQuery(DatosUsuario datosUsuario, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);

            ViewBag.Items = servicio.ListarPaginadoCentrosPorUsuario(datosUsuario.NombreUsuario, paginacion);
        }

        public ActionResult Modificar(int id)
        {
            var centroAModificar = servicio.ObtenerCentro(id);
            CargarCamaras();
            CargarProvincias(centroAModificar);
            return View(centroAModificar);
        }

        [Autorizacion(PermisosScato.AbmCentro)]
        [DatosUsuario]
        [HttpPost]
        public ActionResult Modificar(CentroDto centro, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new ModificarCentro { Dto = centro, Usuario = datosUsuario.NombreUsuario });
                CargarCamaras();
                CargarProvincias(centro);
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            return View("Modificar", centro);
        }

        private void CargarCamaras()
        {
            ViewBag.Camaras = servicio.ListarCamaras().OrderBy(c => c.Descripcion).ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
        }

        private void CargarProvincias(CentroDto model)
        {
            var provincias = servicio.ListarProvincias().OrderBy(p => p.Descripcion);

            int provinciaId = 0;
            if (model != null && model.ProvinciaId != null)
            {
                provinciaId = model.ProvinciaId.Value;
            }

            ViewBag.Provincias = provincias.ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
            ViewBag.Localidades = servicio.ListarLocalidadesPorProvincia(provinciaId)
                            .ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
        }

        [ActionName("CargarLocalidades")]
        public JsonResult CargarLocalidades(int? provinciaId)
        {
            if (provinciaId != 0 && provinciaId != null)
            {
                var localidades =
                    servicio.ListarLocalidadesPorProvincia((int)provinciaId).OrderBy(l => l.Descripcion)
                            .ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
                return Json(localidades, JsonRequestBehavior.AllowGet);
            }
            return Json(new List<SelectList>(), JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public JsonResult ListarCentros(DatosUsuario datosUsuario)
        {
            var centros = servicio.ListarCentrosPorUsuario(datosUsuario.NombreUsuario);
            return Json(centros, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SeleccionarCentro(DatosUsuario datosUsuario, int? centroId)
        {
            if (centroId > 0)
            {
                var centro = servicio.ObtenerCentro(centroId.Value);
                var balanzas = servicio.ListarBalanzasActivas(centroId.Value, Dominio.Enums.TipoVehiculo.Camión);
                var cookie = new CookieUsuario();
                var balanzaDto = balanzas.FirstOrDefault();
                var balanzaId = balanzaDto != null ? balanzaDto.Id: 0;
                cookie.ActualizarValor("CentroId", centro.Id.ToString(CultureInfo.InvariantCulture));
                cookie.ActualizarValor("CentroDescripcion", centro.Descripcion);
                cookie.ActualizarValor("CentroCodigoSap", centro.CodigoSAP);
                cookie.ActualizarValor("BalanzaId", balanzaId.ToString(CultureInfo.InvariantCulture));
                datosUsuario.CentroId = centro.Id;
                datosUsuario.CentroDescripcion = centro.Descripcion;
                datosUsuario.BalanzaId = balanzaId;
            }
            return RedirectToAction("Index", "ListaDeCamiones");
        }
    }
}
