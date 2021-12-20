using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Molinos.Scato.Servicios.Orquestador;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadCalado)]
    public class CaladoAntiguoController : BaseController
    {
        private readonly IServicioActividadFactory<ICaladoService> factory;
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioOrquestador orquestador;
        private ILogger log;

        public CaladoAntiguoController(ILogger log, IServicioActividadFactory<ICaladoService> factory, IServicioRepositorio servicio, IServicioComandos servicioComandos, IServicioOrquestador orquestador)
            : base(servicio)
        {
            this.factory = factory;
            this.servicioComandos = servicioComandos;
            this.orquestador = orquestador;
            this.log = log;
        }

        [DatosUsuario]
        public ActionResult Index(Guid id, DatosUsuario datosUsuario)
        {
            SetearVista(id, datosUsuario);

            return View();
        }

        private void SetearVista(Guid id, DatosUsuario datosUsuario)
        {
            var recorrido = servicio.ObtenerRecorridoPorGuid(id);
            
            ObtenerCaracteristicasObligatorias(recorrido.Material.Id, recorrido.Centro.Id);

            var caladoAnterior = servicio.ObtenerCaladoAnterior(id);
            ViewBag.WorkflowDefinicionId = recorrido.WorkflowDefinicionId;
            ViewBag.Patente = recorrido.Centro.ReingresaPatenteEnCalado ? null : recorrido.Patente;
            ViewBag.PatenteOriginal = recorrido.Patente;
            ViewBag.TipoDocumentoIngreso = recorrido.TipoDocumentoIngreso;
            ViewBag.NumeroDocumentoIngreso = recorrido.NumeroDocumentoIngreso;
            ViewBag.Material = recorrido.Material.Descripcion;
            ViewBag.TipoComercial = recorrido.TipoComercial.Descripcion;
            ViewBag.CicloDeCalado = servicio.ObtenerCantidadCalados(id);
            ViewBag.NumeroDeOrden = caladoAnterior != null ? caladoAnterior.NumeroOrden : recorrido.Centro.Descripcion.Substring(0, 3).Trim().ToUpper() + recorrido.Calado.Id.ToString(CultureInfo.InvariantCulture).PadLeft(8, '0');
            ViewBag.Workflow = recorrido.Workflow.Codigo;
            ViewBag.Guid = id;
            ViewBag.Caracteristicas = servicio.ListarCaracteristicasDeCalidadPorMaterialSinHumedad(recorrido.Material.Id, recorrido.Centro.Id).OrderBy(c => c.Descripcion).ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
            ViewBag.Humedimetros = servicio.ListarHumedimetrosPorCentro(datosUsuario.CentroId).OrderBy(c => c.Descripcion).ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
            ViewBag.EsEgreso = servicio.ObtenerWorkflowPorCodigo(recorrido.Workflow.Codigo).TipoDeWorkflow == TipoDeWorkflow.Egreso;
            ViewBag.NoRechazaEnCalado = recorrido.TipoComercial.NoRechazaEnCalado;
            ViewBag.Rechazado = recorrido.Rechazado;
            //cargo datos de humedad
            var humedad = servicio.ObtenerCaracteristicaDeCalidadHumedad(recorrido.Material.Id, recorrido.Centro.Id);
            if (humedad != null)
            {
                ViewBag.HumedadId = humedad.Id;
                ViewBag.HumedadDescripcion = humedad.Descripcion;
                ViewBag.HumedadRango = humedad.CaladoMinimo.Formatted() + " - " + humedad.CaladoMaximo.Formatted();
                ViewBag.HumedadUnidad = humedad.UnidadDeMedida;                
            }
            else
            {
                ViewBag.HumedadId = 0;
                ViewBag.HumedadDescripcion = "";
                ViewBag.HumedadRango = " - ";
                ViewBag.HumedadUnidad = "";
            }
            ViewBag.TipoVehiculo = recorrido.TipoVehiculo;
        }

        private void ObtenerCaracteristicasObligatorias(int materialId, int centroId)
        {
            var obligatorias = servicio.ListarCaracteristicasDeCalidadObligatorias(materialId, centroId);
            var obliga = String.Join(",", obligatorias.Select(x => x.Id.ToString(CultureInfo.InvariantCulture)).ToArray());
            ViewBag.CaracteristicasObligatorias = obliga;
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Calado(string calaldosPorCaracteristicas, string workflow, int workflowDefinicionId, int ciclo, string muestraConjunto, string orden, string muestras, int? muestraElegida, Guid guid, DatosUsuario datosUsuario)
        {
            if (muestras != "[]" && muestraElegida.HasValue)
            {
                ConfirmarMuestra(muestras, muestraElegida.Value, guid);
            }
            else
            {
                TempData["Alerta"] = Textos.Calado_MuestraObligatoria;
                TempData["TipoAlerta"] = TipoAlerta.Error;
                return RedirectToAction("Index", new { id = guid });
            }

            var listaCalaldosPorCaracteristicas = calaldosPorCaracteristicas.FromJson<CaladoPorCaracteristicaDto[]>();

                var controlRecorrido = new ControlRecorridoDto
                {
                    WorkflowInstanceId = guid,
                    NombreUsuario = datosUsuario.NombreUsuario,
                    Actividad = Textos.ActCalado,
                    ActividadXaml = "Calado",
                    Decision = false,
                    PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId
                };

            var serivce = factory.CrearServicio(workflowDefinicionId);
            var resultado = serivce.Calado(listaCalaldosPorCaracteristicas, ciclo, muestraConjunto, orden, false, guid, controlRecorrido);
            if (!resultado.HayErrores)
            {
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            ModelState.AgregarErrores(resultado);
            TempData["Alerta"] = Textos.Calado_ErrorEnLaCarga;
            TempData["TipoAlerta"] = TipoAlerta.Error;
            return RedirectToAction("Index", new { id = guid });
        }

        public ActionResult ValidarPatente(string patente, string patenteOriginal)
        {
            if (patente.ToLower() != patenteOriginal.ToLower())
            {
                TempData["Alerta"] = Textos.Pesada_PatenteInvalida;
                TempData["TipoAlerta"] = TipoAlerta.Advertencia;
                return Json(new { resultado = "ERROR", url = Url.Action("Index", "ListaDeCamiones") }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { resultado = "OK", url = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ConfirmarMuestra(string muestras, int numeroSeleccionada, Guid guid)
        {
            return RedirectToAction("Index", new { id = guid });
        }

        public ActionResult ObtenerHumedimetro(int humedimetroId)
        {
            var humedimetro = servicio.ObtenerHumedimetro(humedimetroId);
            return Json(new { Descripcion = humedimetro.Descripcion, Modalidad = (int?)humedimetro.Modalidad }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerCaracteristica(int caracteristicaId)
        {
            var caracteristica = servicio.ObtenerCaracteristicaDeCalidad(caracteristicaId);
            return Json(new
                        {
                            Unidad = caracteristica.UnidadDeMedida,
                            RangoMin = caracteristica.CaladoMinimo,
                            RangoMax = caracteristica.CaladoMaximo,
                            CargaEnCalado = caracteristica.CargaEnCalado,
                            EnvioACamara = caracteristica.SituacionEnvioACamara
                        }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TomarHumedad(int humedimetroId)
        {
            try
            {
                log.Info("Se tomará la humedad en modalidad automática para el humedimetro con Id {0}", humedimetroId);
                var humedimetro = servicio.ObtenerHumedimetro(humedimetroId);
                
                var ejecutarTomaDeHumedad = new EjecutarAnalisisHumedad() { CodigoDispositivo = humedimetro.Codigo };
                var resultado = orquestador.Ejecutar(ejecutarTomaDeHumedad);
                var hayHumedad = resultado.Valores != null && resultado.Valores.Any(a => a.Key == "AnalisisHumedad");
                log.Info("Llamada al orquestador exitosa. Hay Humedad = {0}", hayHumedad);
                if (resultado.Mensaje.Codigo != 0)
                {
                    log.Error("(" + Textos.Codigo + ":{0}) " + Textos.Humedad_AutomaticaError + "\r\n{1}", resultado.Mensaje.Codigo, resultado.Mensaje.Descripcion);
                    return Json("(" + Textos.Codigo + ":" + resultado.Mensaje.Codigo + ") " + Textos.Humedad_AutomaticaError + "\r\n" + resultado.Mensaje.Descripcion, JsonRequestBehavior.AllowGet);
                }
                return hayHumedad
                           ? Json(resultado.Valores.First(f => f.Key == "AnalisisHumedad").Value, JsonRequestBehavior.AllowGet)
                           : Json(Textos.Humedad_AutomaticaError, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error en tomar humedad para el humedimetro con Id {0}", humedimetroId);
                return Json(Textos.Humedad_AutomaticaError, JsonRequestBehavior.AllowGet);
            }
        }

        [DatosUsuario]
        public ActionResult Rechazar(string codigoWf, int workflowDefinicionId, Guid instanceId, DatosUsuario datosUsuario)
        {
            ViewBag.Motivos = servicio.ListarMotivos().ToSelectList(x => x.Descripcion, x => x.Descripcion);
            ViewBag.Workflow = codigoWf;
            ViewBag.WorkflowDefinicionId = workflowDefinicionId;
            var controlRecorrido = new ControlRecorridoDto
            {
                WorkflowInstanceId = instanceId,
                NombreUsuario = datosUsuario.NombreUsuario,
                Actividad = Textos.ActCalado + "/" + Textos.Rechazar,
                ActividadXaml = "Calado",
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId
            };

            return View("_TransportistaRechazado", controlRecorrido);
        }

        [DatosUsuario]
        public ActionResult TransportistaRechazado(string workflow, int workflowDefinicionId, ControlRecorridoDto controlRecorrido)
        {
            controlRecorrido.Decision = true;
            var serivce = factory.CrearServicio(workflowDefinicionId);
            var listaCalaldosPorCaracteristicas = new CaladoPorCaracteristicaDto[1];
            var resultado = serivce.Calado(listaCalaldosPorCaracteristicas, 0, "", "", true, controlRecorrido.WorkflowInstanceId, controlRecorrido);
            if (!resultado.HayErrores)
            {
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            ModelState.AgregarErrores(resultado);
            return RedirectToAction("Index", new { id = controlRecorrido.WorkflowInstanceId });
        }
    }
}
