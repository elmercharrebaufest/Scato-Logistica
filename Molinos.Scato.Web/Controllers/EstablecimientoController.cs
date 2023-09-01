using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Impl;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.EXCEL;
using Molinos.Scato.Web.Filtros;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using PdfSharp.Pdf.Filters;
using WebGrease.Css.Extensions;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AbmEstablecimiento)]
    public class EstablecimientoController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioRepositorio miservicio;
        private static string _filtroActual { get; set; } = "";

        public EstablecimientoController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos) : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
            miservicio = servicio;
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
            _filtroActual= filtro;
            ListQuery(filtro, pagina, ordenarPor, dirOrden);
            return View("Listar", (object)filtro);
        }

        private void ListQuery(string filtro, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);

            ViewBag.Items = servicio.ListarPaginadoEstablecimientos(filtro, paginacion);
        }

        public ActionResult Crear()
        {
            CargarVistas();
            return View();
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Crear(EstablecimientoDto model, string corredores, DatosUsuario datosUsuario)
        {

            if (ModelState.IsValid || model.Id == 0)
            {
                model.Proveedor = null;
                model.Corredor = null;
                model.CorredorId = null;
                model.CorredoresAsociados = corredores.FromJson<List<ProveedorDto>>();
                var resultado = servicioComandos.Ejecutar(new CrearEstablecimiento { Dto = model, Usuario = datosUsuario.NombreUsuario });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            CargarVistas(model);
            return View(model);
        }

        public ActionResult Modificar(int id)
        {
            var establecimientoAModificar = servicio.ObtenerEstablecimiento(id);
            CargarVistas(establecimientoAModificar);
            return View(establecimientoAModificar);

        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Modificar(EstablecimientoDto model, string corredores, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                model.Proveedor = null;
                model.Corredor = null;
                model.CorredorId = null;
                model.CorredoresAsociados = corredores.FromJson<List<ProveedorDto>>();
                var resultado = servicioComandos.Ejecutar(new ModificarEstablecimiento { Dto = model, Usuario = datosUsuario.NombreUsuario });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            CargarVistas(model);
            return View(model);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Eliminar(int id, DatosUsuario datosUsuario)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarEstablecimiento { Id = id, Usuario = datosUsuario.NombreUsuario });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        public JsonResult CargarLocalidades(int? provinciaId)
        {
            if (provinciaId != 0 && provinciaId != null)
            {
                var localidades =
                    servicio.ListarLocalidadesPorProvincia((int)provinciaId).OrderBy(x => x.Descripcion)
                            .ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion + "(" + x.CodigoAfip + ")");
                return Json(localidades, JsonRequestBehavior.AllowGet);
            }
            return Json(new List<SelectList>(), JsonRequestBehavior.AllowGet);
        }

        private void CargarVistas(EstablecimientoDto model = null)
        {
            var provincias = servicio.ListarProvincias();
            var comerciales = servicio.ListarComerciales();

            int provinciaId = 0;
            if (model != null && model.ProvinciaId != null)
            {
                provinciaId = model.ProvinciaId.Value;
            }

            ViewBag.Provincias = provincias.ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
            ViewBag.Localidades = servicio.ListarLocalidadesPorProvincia(provinciaId).OrderBy(x => x.Descripcion)
                            .ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion + "(" + x.CodigoAfip + ")");
            ViewBag.Comerciales = comerciales.ToSelectList(x => x.Id.ToString(), x => x.Descripcion);
            var configuracionEPA = servicio.ListarConfiguracionesGenerales(Constantes.ConfiguracionGeneral.Pantalla.EstablecimientoPantalla);
            ViewBag.RangoMaxEPA = configuracionEPA.Where(c => c.Nombre == "RangoMax").FirstOrDefault()?.Valor;
            ViewBag.RangoMinEPA = configuracionEPA.Where(c => c.Nombre == "RangoMin").FirstOrDefault()?.Valor;
        }

        
        
        [DatosUsuario]
        public ActionResult DescargarArchivo(DatosUsuario datosUsuario)
        {
            string ordenarPor = "Id";
            DirOrden dirOrden = DirOrden.Asc;
           var paginacion = new Paginacion(ordenarPor, dirOrden, 1,0);
            var paginado = miservicio.ListarPaginadoEstablecimientos(_filtroActual, paginacion);
            var items = paginado.Items;

            var resultado = new ResultadoPrevisualizar();
            var generadorExcel = new ExcelEstablecimientos();
            generadorExcel.GenerarArchivo(resultado, items);

            if (!resultado.HayErrores)
            {
                byte[] file = resultado.Archivo;
                return File(file, "application/octet-stream","EstablecimientosLibro.xls");
            }
            return View("Index");
        }
    }
}
