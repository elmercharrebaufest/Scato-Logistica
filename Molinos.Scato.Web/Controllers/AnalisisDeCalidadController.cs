using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadAnalisisDeCalidad)]
    public class AnalisisDeCalidadController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioActividadFactory<IAnalisisDeCalidadService> factory;
        private readonly IConfiguracionProvider configuracion;

        public AnalisisDeCalidadController(ILogger log, IServicioRepositorio servicio, IServicioActividadFactory<IAnalisisDeCalidadService> factory, IConfiguracionProvider configuracion)
            : base(servicio)
        {
            this.log = log;
            this.factory = factory;
            this.configuracion = configuracion;
        }

        public ActionResult Index(Guid id)
        {
            var recorrido = servicio.ObtenerRecorridoPorGuid(id);
            if (recorrido.Calado == null) //No hubo calado previo
            {
                TempData["Alerta"] = Textos.AnalisisDeCalidad_ErroNoHayCaladoPrevio;
                TempData["TipoAlerta"] = TipoAlerta.Advertencia;
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            SetearVista(recorrido);
            var model = new CargaDeAnalisisDeCalidadModel
            {
                PatenteOriginal = recorrido.Patente,
                ValidaPatente = recorrido.Centro.ReingresaPatenteEnCalado,
                NumeroDeOrden = recorrido.Calado.NumeroOrden,
                TipoVehiculo = recorrido.TipoVehiculo,
                RecorridoId = recorrido.Id,
                Comentario = recorrido.Calado.Comentario,
            };
            return View(model);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Index(string workflowNombre, int workflowDefinicionId, Guid workflowInstancia, CargaDeAnalisisDeCalidadModel model, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                if (model.Patente.ToLower() != model.PatenteOriginal.ToLower()) //Patente ingresada incorrectamente
                {
                    TempData["Alerta"] = Textos.Pesada_PatenteInvalida;
                    TempData["TipoAlerta"] = TipoAlerta.Advertencia;
                    return Json(new { resultado = "OK", url = Url.Action("Index", "ListaDeCamiones") }, JsonRequestBehavior.AllowGet);
                }

                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var listaAnalisisPorCaracteristicas = serializer.Deserialize<AnalisisPorCaracteristicaDto[]>(model.AnalisisPorCaracteristicas);

                //Valido los rangos
                foreach (var f in listaAnalisisPorCaracteristicas)
                {
                    var rangos = f.Rango.Split('-');
                    if (f.ValorAnalisis.HasValue &&
                        f.ValorAnalisis.Value < decimal.Parse(rangos[0]))
                    {
                        return new ContentResult
                        {
                            Content = f.Caracteristica + ": " + Textos.AnalisisDeCalidad_FueraDeRango
                        };
                    }
                    if (!f.ValorAnalisis.HasValue)
                    {
                        return new ContentResult
                        {
                            Content = f.Caracteristica + ": " + Textos.Error_CaracteristicaRequerida
                        };
                    }

                }

                var controlRecorrido = new ControlRecorridoDto
                {
                    Actividad = Textos.ActAnalisisDeCalidad,
                    ActividadXaml = "AnalisisDeCalidad",
                    WorkflowInstanceId = workflowInstancia,
                    PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                    NombreUsuario = datosUsuario.NombreUsuario
                };

                var servicioWf = factory.CrearServicio(workflowDefinicionId);

                log.Info("Se llamará a la actividad AnalisisDeCalidad con el numero de orden = {0} e instancia de workflow = {1}", model.NumeroDeOrden, workflowInstancia);
                var resultadoActividad = servicioWf.AnalisisDeCalidad(workflowInstancia, model.NumeroDeOrden,
                                                                          listaAnalisisPorCaracteristicas, controlRecorrido);
                return !resultadoActividad.HayErrores
                            ? new ContentResult { Content = "OK" }
                            : new ContentResult { Content = resultadoActividad.Errores.First().Value };
            }

            return new ContentResult { Content = ModelState.Values.First(w => w.Errors.Count > 0).Errors.First().ErrorMessage };
        }

        private void SetearVista(RecorridoDto recorrido)
        {
            var info = servicio.ObtenerInformacionCartaPorte(recorrido.Id);
            var cupo = servicio.ObtenerCupoPorRecorrido(recorrido.Id);
            var mensajes = servicio.ListarAnalisisYCaladoPorCaracteristicaConAdvertencia(recorrido.InstanciaWorkflow);

            ViewBag.CaracteristicasMensajes = mensajes;
            ViewBag.EsSojaSustentable = recorrido.Establecimiento != null;
            ViewBag.WorkflowDefinicionId = recorrido.WorkflowDefinicionId;
            ViewBag.Material = recorrido.Material.Descripcion;
            ViewBag.CicloDeCalado = recorrido.Calado.CicloDeCalado;
            ViewBag.TipoComercial = recorrido.TipoComercial.Descripcion;
            ViewBag.DocumentoIngreso = recorrido.TipoDocumentoIngreso;
            ViewBag.NumeroDocumentoIngreso = recorrido.NumeroDocumentoIngreso;
            ViewBag.WorkflowNombre = recorrido.Workflow.Codigo;
            ViewBag.WorkflowInstancia = recorrido.InstanciaWorkflow;
            ViewBag.TrigoEspecial = info.TrigoEspecial;
            ViewBag.AgenteCompras = !string.IsNullOrEmpty(info.AgenteCompras) ? info.AgenteCompras : Textos.No;
            ViewBag.TieneEntregador = !string.IsNullOrEmpty(info.Entregador) ? info.Entregador : Textos.No;
            ViewBag.RtteComercial = !string.IsNullOrEmpty(info.RtteComercial) ? info.RtteComercial : Textos.No;
            ViewBag.CTG = !string.IsNullOrEmpty(info.CTG) ? info.CTG : Textos.No;
            ViewBag.TitularCP = info.TitularCartaPorte;
            ViewBag.FechaIngreso = string.Format(CultureInfo.CurrentCulture, "{0:d/M/yyyy}", recorrido.FechaInicio);
            ViewBag.Patente = recorrido.Patente;
            ViewBag.PatenteAcoplado = recorrido.Vehiculo != null ? recorrido.Vehiculo.PatenteAcoplado : string.Empty;
            ViewBag.TarjetaDeAcceso = recorrido.TarjetaDeAcceso;
            ViewBag.CupoEsFabrica = false;
            ViewBag.EsTrigoPan = recorrido.Material.CodigoSAP == "19908027";
            if (cupo != null)
            {
                ViewBag.CupoEsFabrica = cupo.Camara == "03"; //03 es de fabrica (hasta el momento es el unico establecido)
            }
            var calados = recorrido.Calado.CaladosPorCaracteristica.Where(w => w.AnalisisPreliminar).Select(s => s.Caracteristica).ToList();
            ViewBag.Caracteristicas = servicio.ListarCaracteristicasDeCalidadPorMaterialWorkflow(recorrido.Material.Id, recorrido.Centro.Id, recorrido.Workflow.Id).Where(w => !calados.Contains(w.Descripcion)).ToSelectList(s => s.Id.ToString(CultureInfo.InvariantCulture), s => s.Descripcion);

            var desdeEsMenor = recorrido.Centro.HorarioDesde < recorrido.Centro.HorarioHasta;
            var desde = recorrido.Centro.HorarioDesde;
            var hasta = recorrido.Centro.HorarioHasta;
            if (info.EnvioDirectoCamara && recorrido.Material.CodigoSAP == configuracion.AppSettings["CodigoSapSemillaSoja"] && recorrido.Centro.HorarioDesde.HasValue && recorrido.Centro.HorarioHasta.HasValue && ((desdeEsMenor && desde <= recorrido.Calado.FechaCreacion.Value.Hour && recorrido.Calado.FechaCreacion.Value.Hour < hasta) ||
                                            (!desdeEsMenor && !(hasta <= recorrido.Calado.FechaCreacion.Value.Hour && recorrido.Calado.FechaCreacion.Value.Hour < desde))))
            {
                ViewBag.EnvioDirectoACamara = string.Format(Textos.ProveedorRangoEnvioCamara);
            }
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

        public ActionResult ObtenerCaracteristica(int caracteristicaId, Guid instanceId)
        {
            var calado = servicio.ObtenerCaladoPorGuid(instanceId);
            decimal? valor = null;
            if (calado != null && calado.CaladosPorCaracteristica != null)
            {
                var caladoPorCaracteristica = calado.CaladosPorCaracteristica.FirstOrDefault(w => w.CaracteristicaId == caracteristicaId);
                valor = caladoPorCaracteristica != null ? caladoPorCaracteristica.ValorCalado : null;
            }
            var caracteristica = servicio.ObtenerCaracteristicaDeCalidad(caracteristicaId);
            return Json(new { caracteristica.Id, caracteristica.Descripcion, caracteristica.UnidadDeMedida, caracteristica.CaladoMinimo, caracteristica.CaladoMaximo, ValorCalado = valor, EsAutomatizable = caracteristica.Dispositivo != TipoDispositivo.Ninguno }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerCaracteristicasCaladas(Guid instanceId)
        {
            var calado = servicio.ObtenerCaladoPorGuid(instanceId);
            if (calado != null && calado.CaladosPorCaracteristica != null)
            {
                var lista = calado.CaladosPorCaracteristica.Where(w => w.AnalisisPreliminar).ToList();
                foreach (var calidad in lista)
                {
                    calidad.EsAutomatizable = servicio.ObtenerCaracteristicaDeCalidad(calidad.CaracteristicaId).Dispositivo != TipoDispositivo.Ninguno;
                }
                return Json(lista, JsonRequestBehavior.AllowGet);
            }
            return Json("0", JsonRequestBehavior.AllowGet);
        }
    }
}
