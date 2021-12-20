using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Windows.Forms.DataVisualization.Charting;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.GraficoDePlanta)]
    public class GraficoDePlantaController : BaseController
    {
        private readonly IServicioComandos servicioComandos;
        private readonly IListaDeWorkflows listaDeWorkflows;
        private readonly IConfiguracionProvider configuracion;
        private readonly ILogger log;

        public GraficoDePlantaController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos, IListaDeWorkflows listaDeWorkflows, IConfiguracionProvider configuracion)
            : base(servicio)
        {
            this.servicioComandos = servicioComandos;
            this.listaDeWorkflows = listaDeWorkflows;
            this.configuracion = configuracion;
            this.log = log;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario)
        {
            var codigoSapSoja = ConfigurationManager.AppSettings["CodigoSapSemillaSoja"];
            var material = servicio.ObtenerMaterialPorCodigoSap(codigoSapSoja);
            if (material != null)
            {
                ViewBag.MaterialId = material.Id;
                ViewBag.MaterialDescripcion = material.Descripcion;
            }
            var materiales = SetearCabezera(datosUsuario.CentroId, true, true);
            ViewBag.Materiales = servicio.BuscarMaterialesPorCentro(datosUsuario.CentroId, string.Empty, 0).Select(x => new SelectListItem()
            {
                Text = x.MaterialDesc,
                Value = x.MaterialId.ToString()
            }).ToList();

            ViewBag.Hidraulicas = new MultiSelectList(servicio.ListarHidraulicasPorCentro(datosUsuario.CentroId), "Codigo", "Codigo");

            return View(materiales);
        }

        [DatosUsuario]
        public ActionResult RetornoPartialDelIndex(DatosUsuario datosUsuario, bool mostrarIngresos, bool esGrano)
        {
            var materiales = SetearCabezera(datosUsuario.CentroId, mostrarIngresos, esGrano);

            return View("_GraficoDePlantaCabezera", materiales);
        }

        private List<EstadoMaterialDto> SetearCabezera(int centroId, bool mostrarIngresos, bool esGrano)
        {
            var materiales = servicio.ListarEstadoPlanta(centroId, mostrarIngresos, esGrano);
            var cupos = ListadoCupoController.ActualizarCuposOtorgados(servicio, servicioComandos, configuracion, centroId);
            ViewBag.MostrarIngresos = mostrarIngresos;
            var cupeados = cupos.Sum(x => x.Otorgados);
            ViewBag.Cupeados = cupeados;
            var arribados = cupos.Sum(x => x.ArribadosDia);
            ViewBag.Arribados = arribados;
            ViewBag.Descargados = cupos.Sum(x => x.Descargados);
            ViewBag.Cumplimiento = cupeados == 0 ? 0 : arribados * 100 / cupeados;
            return materiales;
        }

        [DatosUsuario]
        public JsonResult GenerarGraficoDePlanta(DatosUsuario datosUsuario)
        {
            log.Debug("Obteniendo Datos gráfico de planta, usuario: {0}", datosUsuario.NombreUsuario);
            return Json(listaDeWorkflows.ListarGraficoDePlanta(datosUsuario.CentroId), JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult GenerarGraficoCamionesPorHora(DatosUsuario datosUsuario, GraficoCamionesHoraDto model)
        {
            log.Debug("Obteniendo Datos Grafico Camiones Por Hora, usuario: {0}", datosUsuario.NombreUsuario);
            var result = servicio.ObtenerGraficoDeCamionesPorHora(model, datosUsuario.CentroId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult GenerarGraficoCamionesPorDia(DatosUsuario datosUsuario, GraficoCamionesDiaDto model)
        {
            log.Debug("Obteniendo Datos Grafico Camiones Por Dia, usuario: {0}", datosUsuario.NombreUsuario);
            return Json(servicio.ObtenerGraficoDeCamionesPorDia(model, datosUsuario.CentroId), JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult GenerarGraficoEficienciaHidraulicas(DatosUsuario datosUsuario, GraficoEficienciaHidraulicasDto model)
        {
            log.Debug("Obteniendo Datos Grafico Eficiencia Hidraulicas, usuario: {0}", datosUsuario.NombreUsuario);
            return Json(servicio.ObtenerGraficoDeEficienciaHidraulicas(model, datosUsuario.CentroId, configuracion.AppSettings["CodigoHidraulicaVagon"]), JsonRequestBehavior.AllowGet);
        }


        [DatosUsuario]
        public ActionResult GenerarGraficoToneladasPorRangoDeDia(DatosUsuario datosUsuario, GraficoToneladasRangoDeDiasDto model)
        {
            log.Debug("Obteniendo Datos Grafico Toneladas por Rango de Dias, usuario: {0}", datosUsuario.NombreUsuario);
            return Json(servicio.ObtenerGraficoToneladasPorRangoDeDias(model, datosUsuario.CentroId), JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult Modificar(DatosUsuario datosUsuario, string actividad)
        {
            var actividadNew = servicio.ObtenerGraficoDePlanta(actividad, datosUsuario.CentroId);
            ViewBag.CentroId = datosUsuario.CentroId;
            ViewBag.NombreActividad = actividad;
            return View("Modificar", actividadNew);
        }

        [DatosUsuario]
        [HttpPost]
        public ActionResult Modificar(DatosUsuario datosUsuario, GraficoDePlantaDto model)
        {
            log.Debug("Se va a Modificar Datos gráfico de planta, usuario: {0}", datosUsuario.NombreUsuario);
            if (ModelState.ContainsKey("Id"))
            {
                ModelState["Id"].Errors.Clear();
            }
            if (ModelState.IsValid)
            {
                model.CentroId = datosUsuario.CentroId;
                var resultado = servicioComandos.Ejecutar(new ModificarGraficoDePlanta { Dto = model });

                if (!resultado.HayErrores)
                {
                    log.Debug("Datos gráfico de planta modificados correctamente");
                    return new AjaxEditSuccessResult();
                }
                log.Error("Hubo un problema al modificar datos grafico de planta: {0}", resultado.Errores.FirstOrDefault().Value);
                ModelState.AgregarErrores(resultado);
            }
            log.Error("Model inválido");
            ViewBag.CentroId = model.CentroId;
            ViewBag.NombreActividad = model.NombreActividad;
            return View(model);
        }

        [DatosUsuario]
        public ActionResult Cabecera(DatosUsuario datosUsuario)
        {
            var codigoSapSoja = ConfigurationManager.AppSettings["CodigoSapSemillaSoja"];
            var material = servicio.ObtenerMaterialPorCodigoSap(codigoSapSoja);
            if (material != null)
            {
                ViewBag.MaterialId = material.Id;
                ViewBag.MaterialDescripcion = material.Descripcion;
            }
            var materiales = SetearCabezera(datosUsuario.CentroId, true, true);

            return View(materiales);
        }
    }
}