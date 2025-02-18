using System;
using System.Globalization;
using System.Linq;
using System.Management.Instrumentation;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Impl;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.EXCEL;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AbmHuellaDigital)]
    public class HuellaDigitalController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public HuellaDigitalController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        [DatosUsuario]
        public ActionResult Index(string filtro, string historico, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            bool esHistorico = historico is null ? false : bool.Parse(historico);
            ListQuery(filtro, pagina, ordenarPor, dirOrden, esHistorico);
            return View((object)filtro);
        }

        [DatosUsuario]
        [AjaxOnly]
        [ActionName("Index")]
        
        public ActionResult Listar(string filtro, string historico, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            bool esHistorico = historico is null ? false : bool.Parse(historico);
            ListQuery(filtro, pagina, ordenarPor, dirOrden, esHistorico);
            return View("Listar", (object)filtro);
        }

        private void ListQuery(string filtro, int pagina, string ordenarPor, DirOrden dirOrden, bool esHistorico)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);
            ViewBag.Items = servicio.ListarHuellaDigital(string.IsNullOrWhiteSpace(filtro) ? filtro : filtro.ToUpper(), paginacion, esHistorico);
        }
        [DatosUsuario]
        public ActionResult Crear(DatosUsuario datosUsuario)
        {
            CargarCentros();
            CargarBalanzas(datosUsuario.CentroId);

            var model = new HuellaDigitalDto();


            model.FechaHoraPesaje = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
            model.Estado = true;
            model.IdCentro = datosUsuario.CentroId;
            model.EsEdicion = false;

            return View(model);

        }
        [HttpPost]
        [DatosUsuario]
        public ActionResult Crear(HuellaDigitalDto model, DatosUsuario datosUsuario)
        {
            CargarCentros();
            CargarBalanzas(model.IdCentro);

            if (!ModelState.IsValid)
                return View(model);

            if (model.IdChofer == 0)
            {
                ModelState.AddModelError(nameof(model.Chofer), "El chofer no se encuentra registrado");
                return View(model);
            }

            if (model.IdTransportista == 0)
            {
                ModelState.AddModelError(nameof(model.Transportista), "El transportista no se encuentra registrado");
                return View(model);
            }

            var centro = servicio.ObtenerCentro(model.IdCentro);

            if (centro.ValidarLimiteMinimoDePeso && centro.LimiteMinimoDePeso.HasValue && model.PesoTara <= centro.LimiteMinimoDePeso)
            {
                ModelState.AddModelError(nameof(model.PesoTara), Textos.ErrorPesoPesadaNoValido + centro.LimiteMinimoDePeso);
                return View(model);
            }

            var huellaDigitalExistente = servicio.ObtenerHuellaDigitalPorAtributo(
                model.Patente,
                model.IdTransportista,
                model.Acoplado
            );

            if (huellaDigitalExistente != null)
            {
                var transportista = servicio.ObtenerTransportista(huellaDigitalExistente.IdTransportista);
                huellaDigitalExistente.DarFormatoTransportista(transportista);

                ModelState.AddModelError(nameof(model.Patente), $"Ya existe otro registro activo para Patente, Acoplado y Transportista");
                ModelState.AddModelError(nameof(model.Acoplado), $" ");
                ModelState.AddModelError(nameof(model.Transportista), $" ");
                return View(model);
            }

            model.Usuario = datosUsuario.NombreUsuario;

            var resultado = servicioComandos.Ejecutar(new CrearHuellaDigital
            {
                Dto = model,
                Usuario = datosUsuario.NombreUsuario
            });

            if (resultado.HayErrores)
            {
                ModelState.AgregarErrores(resultado);
                return View(model);
            }

            return new AjaxEditSuccessResult();
        }



        public ActionResult Modificar(int id)
        {
            var huellaDigital = servicio.ObtenerHuellaDigital(id);
            CargarCentros();
            CargarBalanzas(huellaDigital.IdCentro);
            huellaDigital.DarFormatoChofer(servicio.ObtenerChofer(huellaDigital.IdChofer));
            huellaDigital.DarFormatoTransportista(servicio.ObtenerTransportista(huellaDigital.IdTransportista));
            huellaDigital.EsEdicion = true;
            return View(huellaDigital);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Modificar(HuellaDigitalDto model, DatosUsuario datosUsuario)
        {
            CargarCentros();
            CargarBalanzas(model.IdCentro);
            if (ModelState.IsValid)
            {

                model.Usuario = datosUsuario.NombreUsuario;
                model.EsEdicion = true;
                var centro = servicio.ObtenerCentro(model.IdCentro);

                if (centro.ValidarLimiteMinimoDePeso && centro.LimiteMinimoDePeso.HasValue && model.PesoTara <= centro.LimiteMinimoDePeso)
                {
                    ModelState.AddModelError(nameof(model.PesoTara), Textos.ErrorPesoPesadaNoValido + centro.LimiteMinimoDePeso);
                    return View(model);
                }

                var resultado = servicioComandos.Ejecutar(new ModificarHuellaDigital { Dto = model, Usuario = datosUsuario.NombreUsuario });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);

            }
            return View(model);
        }

        private void CargarCentros()
        {
            ViewBag.Centros =
                servicio.ListarCentros()
                    .OrderBy(x => x.Descripcion)
                    .ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
        }

        private void CargarBalanzas(int idCentro)
        {
            ViewBag.Balanzas =
                servicio.ListarTodasLasBalanzasActivas(idCentro)
                    .OrderBy(x => x.Nombre)
                    .ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Nombre);
        }


        [HttpPost]
        [DatosUsuario]
        public ActionResult Eliminar(int id, DatosUsuario datosUsuario)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarHuellaDigital { Id = id, Usuario = datosUsuario.NombreUsuario });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        [HttpPost]
        public ActionResult ObtenerHistorico(bool Historico)
        {
            if (Historico)
            {
                int pagina = 1;
                string ordenarPor = "Id";
                DirOrden dirOrden = DirOrden.Asc;
                var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);

                ViewBag.Items = servicio.ListarHuellaDigitalHistorico("", paginacion);
                return View();
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public JsonResult ObtenerBalanzaPorCentro(int idCentro)
        {
            var balanza = servicio.ListarTodasLasBalanzasActivas(idCentro)
                    .OrderBy(x => x.Nombre)
                    .ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Nombre);
            return Json(balanza , JsonRequestBehavior.AllowGet);
        }

        public ActionResult DescargarArchivo(string filtro, bool esHistorico)
        {

            filtro = filtro ?? string.Empty;
            var lista = servicio.ListarHuellaDigitalSinPaginacion(filtro, esHistorico);

            var resultado = new ResultadoPrevisualizar();
            resultado.Archivo = ExcelHuellaDigital.GenerarArchivoExcel(lista);

            if (!resultado.HayErrores)
            {
                byte[] file = resultado.Archivo;
                return File(file, "application/octet-stream", "HuellaDigital.xls");
            }
            return View("Index");
        }

    }
}