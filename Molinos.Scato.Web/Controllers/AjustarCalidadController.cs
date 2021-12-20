using System;
using System.Collections.Generic;
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
    [Autorizacion(PermisosScato.AjustarCalidad)]
    public class AjustarCalidadController : BaseController
    {
        private readonly IServicioComandos servicioComandos;
        private ILogger log;

        public AjustarCalidadController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.servicioComandos = servicioComandos;
            this.log = log;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, FiltroRecorridoModel filtro, string ordenarPor = "NumeroDocumentoIngreso", DirOrden dirOrden = DirOrden.Asc, int pagina = 1)
        {
            ListarConsulta(new FiltroRecorridoModel { NumeroDocumentoIngreso = "0" }, pagina, ordenarPor, dirOrden, datosUsuario.CentroId);

            return View();
        }

        [DatosUsuario]
        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(DatosUsuario datosUsuario, FiltroRecorridoModel filtro, string ordenarPor = "NumeroDocumentoIngreso", DirOrden dirOrden = DirOrden.Asc, int pagina = 1)
        {
            ListarConsulta(filtro, pagina, ordenarPor, dirOrden, datosUsuario.CentroId);
            return View("Listar");
        }

        private void ListarConsulta(FiltroRecorridoModel filtro, int pagina, string ordenarPor, DirOrden dirOrden, int centroId)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, itemsPorPagina: 10);

            ViewBag.Items = servicio.ListarPaginadoRecorridosPorDocumentoPatenteYCentro(filtro.TipoDocumentoIngreso, filtro.NumeroDocumentoIngreso, filtro.Patente, "", centroId, paginacion);
        }

        [DatosUsuario]
        public ActionResult Seleccionar(int id)
        {
            var recorrido = servicio.ObtenerRecorrido(id);
            
            return View("AjustarCalidad", recorrido);
        }

        public JsonResult ObtenerCaracteristicas(int caladoId)
        {
            if (caladoId > 0)
            {
                var caracteristicasParaAjustar = servicio.ListarCaracteristicasParaAjustesDeCalidad(caladoId);
                return Json(new { caladosPorCaracteristica = caracteristicasParaAjustar, }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { caladosPorCaracteristica = new List<CaladoDto>() }, JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public JsonResult Modificar(string calaldoPorCaracteristica, string tipoDoc, string numeroDoc, int caladoId, DatosUsuario datosUsuario)
        {
            var ajustes = calaldoPorCaracteristica.FromJson<AjusteDeCalidadDto>();

            var tipo = (TipoDocumentoIngreso)Enum.Parse(typeof(TipoDocumentoIngreso), tipoDoc);

            var resultado = servicioComandos.Ejecutar(new ModificarValorCalado { Dto = ajustes, TipoDoc = tipo, NumeroDoc = numeroDoc, Usuario = datosUsuario.NombreUsuario });
            if (!resultado.HayErrores)
            {
                var caracteristicasParaAjustar = servicio.ListarCaracteristicasParaAjustesDeCalidad(caladoId);
                return Json( new { caladosPorCaracteristica = caracteristicasParaAjustar }, JsonRequestBehavior.AllowGet);
            }
            ModelState.AgregarErrores(resultado);
            return Json(new { resultado = "error" }, JsonRequestBehavior.AllowGet);
        }

    }
}
