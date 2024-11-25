using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Molinos.Scato.Web.Seguridad;
using Ninject.Extensions.Logging;
using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadCalado, PermisosScato.ActividadCaladoRechazar)]
    public class CaladoController : BaseController
    {
        private readonly IServicioActividadFactory<ICaladoService> factory;
        private readonly IServicioOrquestador orquestador;
        private readonly IServicioComandos servicioComandos;
        private readonly IConfiguracionProvider configuracion;
        private ILogger log;

        public CaladoController(ILogger log, IServicioActividadFactory<ICaladoService> factory, IServicioRepositorio servicio, IServicioOrquestador orquestador, IServicioComandos servicioComandos, IConfiguracionProvider configuracion)
            : base(servicio)
        {
            this.factory = factory;
            this.orquestador = orquestador;
            this.servicioComandos = servicioComandos;
            this.configuracion = configuracion;
            this.log = log;
        }

        [DatosUsuario]
        public ActionResult Index(Guid id, DatosUsuario datosUsuario, bool? caladoObligatorio = null)
        {
            log.Info("{0} - Index", id);
            if (!PermisosHelper.Is(PermisosScato.ActividadCalado) && PermisosHelper.Is(PermisosScato.ActividadCaladoRechazar))
            {
                return RedirectToAction("Index", "CaladoRechazar", new { id });
            }

            bool activarCaladoAntiguo;
            if (Boolean.TryParse(configuracion.AppSettings["ActivarCaladoAntiguo"], out activarCaladoAntiguo) && activarCaladoAntiguo)
            {
                log.Info("{0} - Index redireccionando al calado antiguo", id);
                return RedirectToAction("Index", "CaladoAntiguo", new { id, datosUsuario });
            }

            var recorrido = SetearVista(id);

            var caracteristicas = servicio.ListarCaracteristicasDeCalidadPorMaterialWorkflow(recorrido.Material.Id, recorrido.Centro.Id, recorrido.Workflow.Id).OrderByDescending(x => x.EsHumedad).ThenByDescending(x => x.PrioridadEnCalado).ThenBy(x => x.Descripcion);

            if (caracteristicas.Any(x => x.Dispositivo.Equals(TipoDispositivo.Humedimetro)))
            {
                var humedimetro = servicio.ObtenerHumedimetroPorNombrePc(datosUsuario.CentroId, datosUsuario.NombrePc);
                ViewBag.Humedimetro = humedimetro;

                if (humedimetro == null)
                {
                    TempData["Alerta"] = String.Format(Textos.Humedimetro_PuestoDeTrabajoInexistente);
                    TempData["TipoAlerta"] = TipoAlerta.Error;
                    return RedirectToAction("Index", "ListaDeCamiones");
                }
            }

            if (caracteristicas.Any(x => x.Dispositivo.Equals(TipoDispositivo.NIRS)))
            {
                var nirs = servicio.ObtenerNirsPorNombrePc(datosUsuario.CentroId, datosUsuario.NombrePc);
                ViewBag.Nirs = nirs;

                if (nirs == null)
                {
                    TempData["Alerta"] = String.Format(Textos.Nirs_PuestoDeTrabajoInexistente);
                    TempData["TipoAlerta"] = TipoAlerta.Error;
                    return RedirectToAction("Index", "ListaDeCamiones");
                }
            }

            log.Info("{0} - Fin - Index", id);

            var ultimoEstado = servicio.ObtenerEstadoUltimoRecorrido(recorrido.InstanciaWorkflow);

            if (ultimoEstado != null)
            {
                ViewBag.Estado = String.Format(Textos.MotivoRechazoCamion, ultimoEstado.fecha.ToString("dd-MM-yyyy"), ultimoEstado.Descripcion);
            }

            bool caladoEsObligatorio = false;
            if (caladoObligatorio.HasValue)
            {
                caladoEsObligatorio = caladoObligatorio.Value;
            }
            else
            {
                var resultadoConsultarCaladoObligatorio = servicioComandos.Ejecutar(new CorrespondeAnalisisObligatorio
                {
                    PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                    MaterialId = recorrido.Material.Id,
                    CentroId = recorrido.Centro.Id,
                    RecorridoId = recorrido.Id,
                    Usuario = datosUsuario.NombreUsuario
                });

                caladoEsObligatorio = !resultadoConsultarCaladoObligatorio.HayErrores && ((ResultadoCorrespondeAnalisisObligatorio)resultadoConsultarCaladoObligatorio).CorrespondeAnalisisObligatorio;
            }

            return View(new CaladoPantallaDto
            {
                CaladosPorCaracteristica = caracteristicas.Select(
                        x => new CaracteristicaPantallaDto
                        {
                            Unidad = x.UnidadDeMedida,
                            RangoMin = x.CaladoMinimo,
                            RangoMax = x.CaladoMaximo,
                            CaladoObligatorio = x.CargaEnCalado || (caladoEsObligatorio && x.IntervaloDeAnalisis),
                            AnalisisAutomatico = caladoEsObligatorio && x.IntervaloDeAnalisis,
                            Caracteristica = x.Descripcion,
                            CaracteristicaId = x.Id,
                            EsHumedad = x.EsHumedad,
                            EsPesoHelectrolitico = x.EsPesoHectolitrico,
                            ValorCalado = (x.CaladoPorDefecto != null) ? x.CaladoPorDefecto : null,
                            EnviaAnalisisObligatorio = x.NoAceptarSiSeDefineUnValor,
                            NroDeToma = 1,
                            ToleranciaSinAnalisis = x.ToleranciaSinAnalisis,
                            ToleranciaSinMensaje = x.ToleranciaSinMensaje,
                            Dispositivo = x.Dispositivo,
                            CaracteristicaNombreNirs = x.NombreNirs,
                            Modalidad = x.Dispositivo == TipoDispositivo.NIRS ? ((NirsDto)ViewBag.Nirs).Modalidad : (x.Dispositivo == TipoDispositivo.Humedimetro ? ((HumedimetroDto)ViewBag.Humedimetro).Modalidad : Modalidad.Manual),
                            TieneValorCaladoPorDefecto = x.CaladoPorDefecto != null
                        }).ToList(),
                MuestraConjunto = null,
                NumeroOrden = recorrido.Centro.Descripcion.Substring(0, 3).Trim().ToUpper() + recorrido.Calado.Id.ToString(CultureInfo.InvariantCulture).PadLeft(8, '0'),
                WorkflowDefinicionId = recorrido.WorkflowDefinicionId,
                WorkflowInstanceId = recorrido.InstanciaWorkflow,
                CicloDeCalado = servicio.ObtenerCantidadCalados(id),
                NumeroDocumentoIngreso = recorrido.NumeroDocumentoIngreso,
                CentroId = recorrido.Centro.Id,
                Fecha = DateTime.Now,
                NirsCodigoProducto = servicio.ObtenerMaterialNirsCodigoProducto(recorrido.Material.Id) ?? "",
                CamionSeleccionadoAnalisisIntervalo = caladoEsObligatorio,
                MaterialId = recorrido.Material.Id,
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                RecorridoId = recorrido.Id,
                EnvioAInase = recorrido.EnvioMuestraInase
            });
        }

        private RecorridoDto SetearVista(Guid id)
        {
            log.Info("{0} - SetearVista", id);
            var recorrido = servicio.ObtenerRecorridoPorGuid(id);
            //Si el calado vacio no se creó en la actividad inicializar calado lo creo aca
            if (recorrido.Calado == null)
            {
                log.Warn("Se va a crear el calado vacio para el workflow: {0}", recorrido.InstanciaWorkflow);
                var resultado = servicioComandos.Ejecutar(new ModificarRecorridoCalado { WorkflowInstanceId = recorrido.InstanciaWorkflow });
                if (!resultado.HayErrores)
                {
                    recorrido = servicio.ObtenerRecorrido(recorrido.Id);
                }
            }
            var vehiculo = recorrido.Vehiculo;
            var info = servicio.ObtenerInformacionCartaPorte(recorrido.Id);
            var cupo = servicio.ObtenerCupoPorRecorrido(recorrido.Id);
            var esEpa = servicio.EsRecorridoSojaEPA(id);

            ViewBag.PatenteOriginal = recorrido.Patente;
            ViewBag.Patente = recorrido.Centro.ReingresaPatenteEnCalado ? null : recorrido.Patente;
            ViewBag.PatenteAcoplado = vehiculo != null ? vehiculo.PatenteAcoplado : string.Empty;
            ViewBag.FechaIngreso = string.Format(CultureInfo.CurrentCulture, "{0:d/M/yyyy}", recorrido.FechaInicio);
            ViewBag.TipoComercial = recorrido.TipoComercial.Descripcion;
            ViewBag.TitularCP = info.TitularCartaPorte;
            ViewBag.NumeroDocumentoIngreso = recorrido.NumeroDocumentoIngreso;
            ViewBag.TarjetaDeAcceso = recorrido.TarjetaDeAcceso;
            ViewBag.AgenteCompras = !string.IsNullOrEmpty(info.AgenteCompras) ? info.AgenteCompras : Textos.No;
            ViewBag.TipoDocumentoIngreso = recorrido.TipoDocumentoIngreso;
            ViewBag.TieneEntregador = !string.IsNullOrEmpty(info.Entregador) ? info.Entregador : Textos.No;
            ViewBag.RtteComercial = !string.IsNullOrEmpty(info.RtteComercial) ? info.RtteComercial : Textos.No;
            ViewBag.CTG = !string.IsNullOrEmpty(info.CTG) ? info.CTG : Textos.No;
            ViewBag.MotivoManual = servicio.ListarMotivosHumedad().ToSelectList(x => x.Id.ToString(), x => x.Descripcion);
            ViewBag.RangosDeRedondeo = servicio.ListarRangosDeRedondeoPorMaterial(recorrido.Material.Id).ToJson();
            ViewBag.MaterialId = recorrido.Material.Id;
            ViewBag.TipoVehiculo = recorrido.TipoVehiculo;
            ViewBag.EsEgreso = recorrido.Workflow.TipoDeWorkflow == TipoDeWorkflow.Egreso;
            ViewBag.Material = recorrido.Material.Descripcion;
            ViewBag.NoRechazaEnCalado = recorrido.TipoComercial.NoRechazaEnCalado;
            ViewBag.Rechazado = recorrido.Rechazado;
            ViewBag.TrigoEspecial = info.TrigoEspecial;
            ViewBag.RecorridoId = recorrido.Id;
            ViewBag.CupoEsFabrica = false;
            ViewBag.Variedad = recorrido.TipoVariedadCodigo != Constantes.TipoVariedadMaterial.Estandar ? recorrido.TipoVariedadDescripcion : string.Empty;
            ViewBag.ColorFondoVariedad = recorrido.TipoVariedadCodigo == Constantes.TipoVariedadMaterial.EPAyEUDR ? Constantes.ValoresPorDefecto.ColorFondoSojaEPAyEUDR
                                    : recorrido.TipoVariedadCodigo == Constantes.TipoVariedadMaterial.EUDR ? Constantes.ValoresPorDefecto.ColorFondoSojaEUDR
                                    : recorrido.TipoVariedadCodigo == Constantes.TipoVariedadMaterial.EPA ? Constantes.ValoresPorDefecto.ColorFondoSojaEPA
                                    : recorrido.TipoVariedadCodigo == Constantes.TipoVariedadMaterial.Sustentable ? Constantes.ValoresPorDefecto.ColorFondoSojaSustentable
                                    : string.Empty;
            ViewBag.ColorTextoVariedad = recorrido.TipoVariedadCodigo == Constantes.TipoVariedadMaterial.EPAyEUDR ? Constantes.ValoresPorDefecto.ColorTextoSojaEPAyEUDR
                                    : recorrido.TipoVariedadCodigo == Constantes.TipoVariedadMaterial.EUDR ? Constantes.ValoresPorDefecto.ColorTextoSojaEUDR
                                    : recorrido.TipoVariedadCodigo == Constantes.TipoVariedadMaterial.EPA ? Constantes.ValoresPorDefecto.ColorTextoSojaEPA
                                    : recorrido.TipoVariedadCodigo == Constantes.TipoVariedadMaterial.Sustentable ? Constantes.ValoresPorDefecto.ColorTextoSojaSustentable
                                    : string.Empty;

            if (cupo != null)
            {
                ViewBag.CupoEsFabrica = cupo.Camara == "03"; //03 es de fabrica (hasta el momento es el unico establecido)
            }

            ViewBag.AnalisisGirasolOleico = recorrido.Material.Oleico;

            var desdeEsMenor = recorrido.Centro.HorarioDesde < recorrido.Centro.HorarioHasta;
            var desde = recorrido.Centro.HorarioDesde;
            var hasta = recorrido.Centro.HorarioHasta;

            if (info.EnvioDirectoCamara && recorrido.Material.CodigoSAP == configuracion.AppSettings["CodigoSapSemillaSoja"] && recorrido.Centro.HorarioDesde.HasValue && recorrido.Centro.HorarioHasta.HasValue && ((desdeEsMenor && desde <= DateTime.Now.Hour && DateTime.Now.Hour < hasta) ||
                               (!desdeEsMenor && !(hasta <= DateTime.Now.Hour && DateTime.Now.Hour < desde))))
            {
                ViewBag.EnvioDirectoACamara = string.Format(Textos.ProveedorRangoEnvioCamara);
            }
            if (info.EnvioCamaraInase && recorrido.Material.CodigoSAP == configuracion.AppSettings["CodigoSapSemillaSoja"]
                && recorrido.TipoVehiculo != TipoVehiculo.Tren && recorrido.TipoVehiculo != TipoVehiculo.Bitren)
            {
                ViewBag.EnvioCamaraInase = string.Format(Textos.CaladoMuestraInase);
                recorrido.EnvioMuestraInase = true;
            }
            log.Info("{0} - Fin - SetearVista", id);
            return recorrido;
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Calado(CaladoPantallaDto caladopantalla, DatosUsuario datosUsuario)
        {
            if (caladopantalla.MuestraConjunto != null && VerificarMuestraConjunto(caladopantalla))
            {
                TempData["Alerta"] = Textos.Calado_ErrorEnMuestraConjunto;
                TempData["TipoAlerta"] = TipoAlerta.Error;
                log.Error("{0} - Calado_ErrorEnMuestraConjunto - Calado", caladopantalla.WorkflowInstanceId);
                return RedirectToAction("Index", new { id = caladopantalla.WorkflowInstanceId });
            }
            log.Info("{0} - Calado", caladopantalla.WorkflowInstanceId);
            var caracteristicas = new CaladoPorCaracteristicaDto[0];
            if (caladopantalla.CaladosPorCaracteristica != null)
            {
                caracteristicas = caladopantalla.CaladosPorCaracteristica.Where(x => x.ValorCalado.HasValue).Select(x => new CaladoPorCaracteristicaDto
                {
                    AnalisisPreliminar = x.AnalisisPreliminar,
                    CaracteristicaId = x.CaracteristicaId,
                    ValorCalado = x.ValorCalado,
                    EsHumedad = x.EsHumedad,
                    AnalisisAutomatico = x.AnalisisAutomatico
                }).ToArray();
            }

            log.Debug("{0} - Calado Creo ControlRecorridoDto", caladopantalla.WorkflowInstanceId);
            var controlRecorrido = new ControlRecorridoDto
            {
                WorkflowInstanceId = caladopantalla.WorkflowInstanceId,
                NombreUsuario = datosUsuario.NombreUsuario,
                Actividad = Textos.ActCalado,
                ActividadXaml = "Calado",
                Decision = false,
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                Comentario = caladopantalla.Comentario
            };
            log.Debug("{0} - Calado llamo al servicio", caladopantalla.WorkflowInstanceId);
            var serivce = factory.CrearServicio(caladopantalla.WorkflowDefinicionId);
            var resultado = serivce.Calado(caracteristicas, caladopantalla.CicloDeCalado, caladopantalla.MuestraConjunto, caladopantalla.NumeroOrden, false, caladopantalla.WorkflowInstanceId, controlRecorrido);
            log.Debug("{0} - Calado - llamada al servicio completa", caladopantalla.WorkflowInstanceId);
            if (!resultado.HayErrores)
            {
                log.Debug("{0} - Calado - no hay errores, guardo la muestra de humedad", caladopantalla.WorkflowInstanceId);
                GuardarMuestraDeHumedad(caladopantalla, datosUsuario);
                GuardarMuestraDeNirs(caladopantalla, datosUsuario);
                if(caladopantalla.MaterialId == 4 && caladopantalla.EnvioAInase)
                    GuardarMuestraInase(caladopantalla, datosUsuario);
                log.Debug("{0} - Fin - Calado", caladopantalla.WorkflowInstanceId);
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            ModelState.AgregarErrores(resultado);
            TempData["Alerta"] = Textos.Calado_ErrorEnLaCarga;
            TempData["TipoAlerta"] = TipoAlerta.Error;
            log.Error("{0} - Fin - Calado", caladopantalla.WorkflowInstanceId);
            return RedirectToAction("Index", new { id = caladopantalla.WorkflowInstanceId });
        }

        private bool VerificarMuestraConjunto(CaladoPantallaDto caladopantalla)
        {
            log.Info("{0} - Calado - Verifico muestra conjunto", caladopantalla.WorkflowInstanceId);

            int? muestraConj;
            try
            {
                muestraConj = Convert.ToInt32(caladopantalla.MuestraConjunto);
            }
            catch (Exception)
            {
                muestraConj = null;
            }
            return muestraConj.HasValue &&
                   servicio.MuestraConjuntoFueUtilizada(caladopantalla.WorkflowInstanceId, muestraConj.Value);
        }

        public ActionResult TomarHumedad(string humedimetro, long fecha)
        {
            try
            {
                log.Info("Se tomará la humedad en modalidad automática para el humedimetro {0}", humedimetro);
                var ejecutarTomaDeHumedad = new EjecutarAnalisisHumedad { CodigoDispositivo = humedimetro, FechaDeInicio = new DateTime(fecha) };
                var resultado = orquestador.Ejecutar(ejecutarTomaDeHumedad);
                var hayHumedad = resultado.Valores != null && resultado.Valores.Any(a => a.Key == "AnalisisHumedad");
                log.Info("Llamada al orquestador exitosa. Hay Humedad = {0}", hayHumedad);
                if (resultado.Mensaje.Codigo == 207)
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
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
                log.Error(ex, "Error en tomar humedad para el humedimetro con Id {0}", humedimetro);
                return Json(Textos.Humedad_AutomaticaError, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult TomarPH(string humedimetro, long fecha)
        {
            try
            {
                log.Info("Se tomará el peso hectolitrico en modalidad automática para el humedimetro {0}", humedimetro);
                var ejecutarTomaDeHumedad = new EjecutarAnalisisHumedad { CodigoDispositivo = humedimetro, FechaDeInicio = new DateTime(fecha) };
                var resultado = orquestador.Ejecutar(ejecutarTomaDeHumedad);
                var hayPH = resultado.Valores != null && resultado.Valores.Any(a => a.Key == "PH");
                log.Info("Llamada al orquestador exitosa. Hay Peso Hectolitrico = {0}", hayPH);
                if (resultado.Mensaje.Codigo == 207)
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
                if (resultado.Mensaje.Codigo != 0)
                {
                    log.Error("(" + Textos.Codigo + ":{0}) " + Textos.PH_AutomaticaError + "\r\n{1}", resultado.Mensaje.Codigo, resultado.Mensaje.Descripcion);
                    return Json("(" + Textos.Codigo + ":" + resultado.Mensaje.Codigo + ") " + Textos.PH_AutomaticaError + "\r\n" + resultado.Mensaje.Descripcion, JsonRequestBehavior.AllowGet);
                }

                return hayPH
                           ? Json(resultado.Valores.First(f => f.Key == "PH").Value, JsonRequestBehavior.AllowGet)
                           : Json(Textos.PH_AutomaticaError, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error en tomar el peso hectolitríco para el humedimetro con Id {0}", humedimetro);
                return Json(Textos.PH_AutomaticaError, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult TomarHumedadPH(string humedimetro, long fecha)
        {
            var respuesta = RespuestaEstandarDto.Crear<dynamic>();
            try
            {
                log.Debug("Se tomará el la Humedad y PH en modalidad automática para el humedimetro {0}", humedimetro);
                var ejecutarTomaDeHumedad = new EjecutarAnalisisHumedad { CodigoDispositivo = humedimetro, FechaDeInicio = new DateTime(fecha) };


                var resultado = orquestador.Ejecutar(ejecutarTomaDeHumedad);
                var hayHumedad = resultado.Valores != null && resultado.Valores.Any(a => a.Key == "AnalisisHumedad");
                var hayPH = resultado.Valores != null && resultado.Valores.Any(a => a.Key == "PH");

                log.Debug("Llamada al orquestador exitosa. Hay Humedad = {0}", hayHumedad);
                log.Debug("Llamada al orquestador exitosa. Hay PH = {0}", hayPH);

                decimal? humedad = null;
                decimal? ph = null;

                if (hayHumedad)
                {
                    humedad = resultado.Valores.First(f => f.Key == "AnalisisHumedad").Value;
                    log.Debug("Humedad = {0}", humedad);
                }
                else
                {
                    respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = Textos.Humedad_AutomaticaError, TipoDeMensaje = TipoDeMensajeDeRespuesta.Warning });
                }

                if (hayPH)
                {
                    ph = resultado.Valores.First(f => f.Key == "PH").Value;
                    log.Debug("PH = {0}", ph);
                }
                else
                {
                    respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = Textos.PH_AutomaticaError, TipoDeMensaje = TipoDeMensajeDeRespuesta.Warning });
                }

                var humedimetroRespuesta = new HumedimetroRespuestaDto
                {
                    Humedad = humedad,
                    PH = ph
                };
                respuesta.Data = humedimetroRespuesta;

                return Json(respuesta, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error en tomar la Humedad y PH para el humedimetro con Id {0}", humedimetro);
                var mensaje = new MensajeEstandarDto
                {
                    TipoDeMensaje = TipoDeMensajeDeRespuesta.Error,
                    Mensaje = ex.Message
                };
                respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = ex.Message, TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });

                return Json(respuesta, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult TomarAnalisis(string nirs, string material)
        {
            try
            {
                log.Info("Se tomará la humedad en modalidad automática para el humedimetro {0}", nirs);
                var ejecutarAnalisis = new EjecutarAnalisis { CodigoDispositivo = nirs, Material = material };
                var resultado = orquestador.Ejecutar(ejecutarAnalisis);
                var hayAnalisis = resultado.Valores != null && resultado.Valores.Any();
                log.Info("Llamada al NIRS exitosa. Hay Humedad = {0}", hayAnalisis);
                if (resultado.Mensaje.Codigo == 207)
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
                if (resultado.Mensaje.Codigo != 0)
                {
                    log.Error("(" + Textos.Codigo + ":{0}) " + Textos.Nirs_AutomaticoError + "\r\n{1}", resultado.Mensaje.Codigo, resultado.Mensaje.Descripcion);
                    return Json("(" + Textos.Codigo + ":" + resultado.Mensaje.Codigo + ") " + Textos.Nirs_AutomaticoError + "\r\n" + resultado.Mensaje.Descripcion, JsonRequestBehavior.AllowGet);
                }

                return hayAnalisis
                           ? Json(resultado.Valores, JsonRequestBehavior.AllowGet)
                           : Json(Textos.Nirs_AutomaticoError, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error en analizar caracteristicas para el NIRS con Codigo {0}", nirs);
                return Json(Textos.Nirs_AutomaticoError, JsonRequestBehavior.AllowGet);
            }

            //return Json(new Dictionary<string, decimal> {
            //    { "proteina", (decimal)3.35},
            //    { "humedad", (decimal)5.55555},
            //    { "PH", (decimal)8.88},

            //}, JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult Rechazar(string codigoWf, int workflowDefinicionId, Guid instanceId, DatosUsuario datosUsuario)
        {
            log.Info("{0} - Calado Rechazar", instanceId);
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
            log.Info("{0} - Calado rechazado", workflowDefinicionId);
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

        [DatosUsuario]
        public void CancelarAnalisisObligatorio(DatosUsuario datosUsuario, int puestoDeTrabajoId, int materialId, int recorridoId, bool camionSeleccionadoAnalisisIntervalo)
        {
            if (datosUsuario.PuestoDeTrabajoId > 0 && camionSeleccionadoAnalisisIntervalo)
            {
                var resultadoActualizar = servicioComandos.Ejecutar(new CorrespondeAnalisisObligatorio
                {
                    PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                    MaterialId = materialId,
                    CentroId = datosUsuario.CentroId,
                    RecorridoId = recorridoId,
                    Usuario = datosUsuario.NombreUsuario,
                    Cancelar = true
                });
                if (resultadoActualizar.HayErrores)
                {
                    log.Debug($"{recorridoId} - {datosUsuario.PuestoDeTrabajoId} - Calado - ocurrió un error al cancelar la fecha de ultimo analis: {resultadoActualizar.Errores.First().Value}");
                }
            }
        }

        private void GuardarMuestraDeHumedad(CaladoPantallaDto caladopantalla, DatosUsuario datosUsuario)
        {
            log.Info("{0} - Calado GuardarMuestraDeHumedad", caladopantalla.WorkflowInstanceId);
            if (caladopantalla.CaladosPorCaracteristica != null)
            {
                var humedad = caladopantalla.CaladosPorCaracteristica.FirstOrDefault(x => x.EsHumedad);
                if (humedad != null)
                {
                    var modalidad = humedad.Dispositivo == TipoDispositivo.Humedimetro ? caladopantalla.HumedimetroModalidad :
                                        (humedad.Dispositivo == TipoDispositivo.NIRS ? humedad.Modalidad : Modalidad.Manual);
                    servicioComandos.Ejecutar(new CrearMuestraDeHumedad
                    {
                        Dto = new MuestraDeHumedadDto
                        {
                            CentroId = caladopantalla.CentroId,
                            HumedimetroId = caladopantalla.HumedimetroId,
                            CicloDeCalado = caladopantalla.CicloDeCalado,
                            Fecha = DateTime.Now,
                            NroDeToma = humedad.NroDeToma,
                            Usuario = datosUsuario.NombreUsuario,
                            ValorFinal = humedad.ValorCalado ?? 0,
                            ValorLeido = modalidad == Modalidad.Manual ? (humedad.ValorCalado ?? 0) : (humedad.ValorAutomatico ?? 0),
                            Modalidad = modalidad == Modalidad.Manual ? Textos.Manual : Textos.Automatico,
                            NumeroDocumentoIngreso = caladopantalla.NumeroDocumentoIngreso,
                            WorkflowInstanceId = caladopantalla.WorkflowInstanceId,
                            NumeroOrden = caladopantalla.NumeroOrden,
                            MotivoHumedadManualId = modalidad == Modalidad.Automática && humedad.ValorCalado != humedad.ValorAutomatico ? caladopantalla.MotivoHumedadManualId : null,
                            Dispositivo = humedad.Dispositivo == TipoDispositivo.Humedimetro ? "GAC" : (humedad.Dispositivo == TipoDispositivo.NIRS ? "NIRS" : "")
                        }
                    });
                    log.Info("{0} - Calado - Muestra guardada", caladopantalla.WorkflowInstanceId);
                }
            }
            log.Info("{0} - Calado Fin - GuardarMuestraDeHumedad", caladopantalla.WorkflowInstanceId);
        }

        private void GuardarMuestraDeNirs(CaladoPantallaDto caladopantalla, DatosUsuario datosUsuario)
        {
            log.Info("{0} - Calado GuardarMuestraDeNirs", caladopantalla.WorkflowInstanceId);
            if (caladopantalla.CaladosPorCaracteristica != null)
            {
                var valores = caladopantalla.CaladosPorCaracteristica.Where(x => x.Dispositivo.Equals(TipoDispositivo.NIRS));

                foreach (var valor in valores)
                {
                    servicioComandos.Ejecutar(new CrearMuestraDeNirs
                    {
                        Dto = new MuestraDeNirsDto
                        {
                            CentroId = caladopantalla.CentroId,
                            NirsId = caladopantalla.NirsId,
                            CicloDeCalado = caladopantalla.CicloDeCalado,
                            Fecha = DateTime.Now,
                            NroDeToma = valor.NroDeToma,
                            Usuario = datosUsuario.NombreUsuario,
                            ValorFinal = valor.ValorCalado ?? 0,
                            ValorLeido = valor.Modalidad == Modalidad.Manual ? (valor.ValorCalado ?? 0) : (valor.ValorAutomatico ?? 0),
                            Modalidad = valor.Modalidad == Modalidad.Manual ? Textos.Manual : Textos.Automatico,
                            NumeroDocumentoIngreso = caladopantalla.NumeroDocumentoIngreso,
                            WorkflowInstanceId = caladopantalla.WorkflowInstanceId,
                            NumeroOrden = caladopantalla.NumeroOrden,
                        }
                    });
                    log.Info("{0} - Nirs - Muestra guardada", caladopantalla.WorkflowInstanceId);
                }
            }
            log.Info("{0} - Nirs Fin - GuardarMuestraDeNirs", caladopantalla.WorkflowInstanceId);
        }

        private void GuardarMuestraInase(CaladoPantallaDto caladopantalla, DatosUsuario datosUsuario)
        {
            log.Info("{0} - Calado GuardarMuestraDeInase", caladopantalla.WorkflowInstanceId);

            servicioComandos.Ejecutar(new CrearMuestraDeInase
            {
                Dto = new MuestraDeInaseDto
                {
                    WorkflowInstanceId = caladopantalla.WorkflowInstanceId
                }
            });
            log.Info("{0} - Inase - Muestra guardada", caladopantalla.WorkflowInstanceId);
        }
    }
}