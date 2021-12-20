using System;
using System.Web;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Filtros;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AbmLibroMovimientosExistenciaGranos)]
    public class LibroMovimientosExistenciaGranosController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public LibroMovimientosExistenciaGranosController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario)
        {
            var fecha = DateTime.Now.AddDays(-7);
            SetearVista(datosUsuario);
            return View(new LibroMovimientosExistenciaGranosDto { FechaDesde = fecha, FechaHasta = DateTime.Now });
        }

        public void SetearVista(DatosUsuario datosUsuario)
        {
            ViewBag.LibroOnccaEsPorDescripcionCorta = servicio.LibroOnccaEsPorDescripcionCorta(datosUsuario.CentroId);
        }

        [DatosUsuario]
        [HttpPost]
        [HttpParamAction]
        public ActionResult Imprimir(DatosUsuario datosUsuario, LibroMovimientosExistenciaGranosDto dto)
        {
            if (ModelState.IsValid)
            {
                dto.FechaHasta = dto.FechaHasta.FinDelDia();
                var resultado = servicioComandos.Ejecutar(new LibroMovimientosExistenciaGranosImpresion { Dto = dto, CodigoDeImpresion = "LibroMovimientosExistenciaGranos", CentroId = datosUsuario.CentroId });
                if (!resultado.HayErrores)
                {
                    TempData["Alerta"] = Textos.ImpresionEnviada;
                    TempData["TipoAlerta"] = TipoAlerta.Exito;
                    return RedirectToAction("Index", "ListaDeCamiones");
                }
                ModelState.AgregarErrores(resultado);
            }
            SetearVista(datosUsuario);
            return View("Index",dto);
        }

        [DatosUsuario]
        [HttpPost]
        [HttpParamAction]
        public ActionResult GenerarExcel(DatosUsuario datosUsuario, LibroMovimientosExistenciaGranosDto dto)
        {
            if (ModelState.IsValid)
            {
                dto.FechaHasta = dto.FechaHasta.FinDelDia();
                var resultado = servicioComandos.Ejecutar(new LibroMovimientosExistenciaGranosExcel
                    {
                        Dto = dto,
                        CodigoDeImpresion = "LibroMovimientosExistenciaGranos",
                        CentroId = datosUsuario.CentroId
                    });
                if (!resultado.HayErrores)
                {
                    byte[] file = ((ResultadoPrevisualizar) resultado).Archivo;

                    if (System.Web.HttpContext.Current != null)
                    {
                        System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("RetornoExportacion", "ok"));
                    }
                    return File(file, "application/octet-stream", "LibroMovimientosExistenciaGranos.xls");
                }
                if (!resultado.HayErrores)
                {
                    TempData["Alerta"] = Textos.ImpresionEnviada;
                    TempData["TipoAlerta"] = TipoAlerta.Exito;
                    return RedirectToAction("Index", "ListaDeCamiones");
                }
                ModelState.AgregarErrores(resultado);
            }
            SetearVista(datosUsuario);
            return View("Index", dto);
        }

        [DatosUsuario]
        [HttpPost]
        [HttpParamAction]
        public ActionResult GenerarPdf(DatosUsuario datosUsuario, LibroMovimientosExistenciaGranosDto dto)
        {
            if (ModelState.IsValid)
            {
                dto.FechaHasta = dto.FechaHasta.FinDelDia();
                var resultado =
                    servicioComandos.Ejecutar(new LibroMovimientosExistenciaGranosPdf
                        {
                            Dto = dto,
                            CodigoDeImpresion = "LibroMovimientosExistenciaGranos",
                            CentroId = datosUsuario.CentroId
                        });
                if (!resultado.HayErrores)
                {
                    byte[] file = ((ResultadoPrevisualizar) resultado).Archivo;

                    if (System.Web.HttpContext.Current != null)
                    {
                        System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("RetornoExportacion", "ok"));
                    }
                    return File(file, "application/octet-stream", "LibroMovimientosExistenciaGranos.pdf");
                }


                if (!resultado.HayErrores)
                {
                    TempData["Alerta"] = Textos.ImpresionEnviada;
                    TempData["TipoAlerta"] = TipoAlerta.Exito;
                    return RedirectToAction("Index", "ListaDeCamiones");
                }
                ModelState.AgregarErrores(resultado);
            }
            SetearVista(datosUsuario);
            return View("Index", dto);
        }

        [DatosUsuario]
        [HttpPost]
        [HttpParamAction]
        public ActionResult CalcularHojas(DatosUsuario datosUsuario, LibroMovimientosExistenciaGranosDto dto)
        {
            if (ModelState.IsValid)
            {
                dto.FechaHasta = dto.FechaHasta.FinDelDia();
                var resultado = servicio.LibroMovimientosExistenciaGranosCalcularHojas(dto, datosUsuario.CentroId);
                ViewBag.CantidadDeHojas = resultado;
            }
            SetearVista(datosUsuario);
            return View("Index", dto);
        }
    }
}