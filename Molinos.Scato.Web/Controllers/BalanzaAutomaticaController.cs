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
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Mvc;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.BalanzaAutomatica)]
    public class BalanzaAutomaticaController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioOrquestador orquestador;
        private readonly IServicioActividadFactory<IPesadaService> pesadaFactory;
        private readonly IServicioActividadFactory<IEjecutarService> ejecutarFactory;
        private readonly IListaDeWorkflows workflows;
        private readonly IServicioEstadoPuesto estadoPuesto;
        private readonly IServicioNotificarUsuario notificador;

        public BalanzaAutomaticaController(ILogger log, IServicioNotificarUsuario notificador, IServicioRepositorio servicio, IServicioComandos servicioComandos,
            IServicioOrquestador orquestador, IServicioActividadFactory<IPesadaService> pesadaFactory, IServicioActividadFactory<IEjecutarService> ejecutarFactory,
            IListaDeWorkflows workflows, IServicioEstadoPuesto estadoPuesto)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
            this.orquestador = orquestador;
            this.pesadaFactory = pesadaFactory;
            this.workflows = workflows;
            this.estadoPuesto = estadoPuesto;
            this.ejecutarFactory = ejecutarFactory;
            this.notificador = notificador;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario)
        {
            ViewBag.MostrarValidacionEtapaAutomatica = TempData["FlagMostrarValidacionEtapaAutomatica"] ?? false;
            ViewBag.MensajeValidacionEtapaAutomatica = TempData["messageEtapaAutomatica"] ?? string.Empty;
            ListarConsulta(datosUsuario);
            return View();
        }

        private void ListarConsulta(DatosUsuario datosUsuario)
        {
            var balanzas = servicio.ListarPuestosAutomaticosporCentro(datosUsuario.CentroId);
            foreach (var balanza in balanzas)
            {
                if (!string.IsNullOrEmpty(balanza.IntercomunicadorCodigo))
                {
                    var puertoDeAudio = orquestador.ObtenerIntercomunicadorPuertoDeAudio(balanza.IntercomunicadorCodigo);
                    var intercomunicador = GetIntercomunicadorDispositivoConfig(balanza.BalanzaId.ToString(), balanza.IntercomunicadorCodigo, puertoDeAudio);
                    balanza.IntercomunicadorDispositivo = intercomunicador;
                }
            }
            ViewBag.Balanzas = balanzas.Where(x => x.Orden.HasValue).OrderBy(x => x.Orden).Union(balanzas.Where(x => !x.Orden.HasValue).OrderBy(x => x.NombreBalanza)).ToList();
            ViewBag.Eventos = servicio.ListarErrorBalanzas(balanzas.Select(x => x.PuestoId).ToList()).ToList();
            ViewBag.PantallaPrincipal = servicio.RedireccionarAListaAutomatizada(datosUsuario.NombrePc, datosUsuario.CentroId);
            var vagones = servicio.ListarVagonesEnPesada(datosUsuario.CentroId).OrderBy(x => x.NumeroPatente);
            ViewBag.Vagones = vagones.ToSelectList(f => f.NumeroPatente, f => f.NumeroPatente);
        }

        public void Prueba(int id, string cp, string pat, string mat, Guid guid, string prox, string error)
        {
            var notificacion = new NotificacionPesadaAutomaticaDto
            {
                Id = id,
                Error = error,
                CartaPorte = cp,
                Diferencia = (2500 - 0).ToString(),
                Peso = "0",
                Entregador = "No",
                Material = mat,
                WorkflowInstanceId = guid,
                TipoPeso = "Peso Neto",
                Patente = pat,
                Actividad = prox,
                NoRedirecciona = true,
                TipoPesoOrigen = "Peso Neto Org",
                PesoBrutoOrigen = "0",
                PesoNetoOrigen = "0"
            };
            var notificacionDto = new NotificacionDto
            {
                Hora = DateTime.Now,
                Grupo = "Automaticas",
                Mensaje = notificacion.ToJson(),
                TipoAlerta = TipoAlerta.Automatica,
                Leido = string.IsNullOrWhiteSpace(error),
                PuestoId = id
            };
            try
            {
                log.Debug("Iniciando conexion signalR");

                notificador.Notificar(notificacionDto);
            }
            catch
            {
                throw;
            }
        }

        public ActionResult RedireccionarPesada(Guid id, string proxima, int notificacionId)
        {
            EliminarNotificacion(notificacionId);
            return RedirectToAction("Index", proxima, new { id, automatizadoFull = true });
        }

        [DatosUsuario]
        public ActionResult VerificarPatente(string patente, int puestoId, string tarjeta, int? peso, DatosUsuario datosUsuario)
        {
            tarjeta = string.IsNullOrEmpty(tarjeta) || string.IsNullOrWhiteSpace(tarjeta) ? null : tarjeta;
            patente = !string.IsNullOrEmpty(patente) ? patente : null;
            var recorrido = servicio.ObtenerDatosRecorridoActivoSinTarjeta(patente, tarjeta);
            var proximaActividad = new ProximaAccionDto();
            var color = string.Empty;

            if (recorrido == null)
            {
                return Json("Tarjeta no asociada a un camión", JsonRequestBehavior.AllowGet);
            }
            if (patente.ToLower() != recorrido.Patente.ToLower())
            {
                return Json("Patente no reconocida", JsonRequestBehavior.AllowGet);
            }

            var puestos = new List<PuestoDeTrabajoDto>
                    {
                        new PuestoDeTrabajoDto {Lectura = tarjeta, Id = puestoId }
                    };

            proximaActividad = workflows.ObtenerWorkflowProximaAccion(recorrido.InstanciaWorkflow);
            color = proximaActividad?.ProximaAccion?.ToUpper()?.Trim() == "PESADABRUTO" ? "AMARILLO" :
                    proximaActividad?.ProximaAccion?.ToUpper()?.Trim() == "PESADATARA" ? "VERDE" : string.Empty;

            if (!String.IsNullOrEmpty(proximaActividad.Mensaje))
            {
                log.Error(proximaActividad.Mensaje);
                return Json(new { mensaje = proximaActividad.Mensaje, valida = false }, JsonRequestBehavior.AllowGet);
            }

            var resultado = servicio.ValidarProximaActividadPorPuesto(recorrido, proximaActividad.ProximaAccion, puestos, datosUsuario.NombreUsuario);

            log.Debug($"Ejecutando etapa { proximaActividad.ProximaAccion }: Para vehiculo: { patente } y tarjeta: { tarjeta }");
            log.Debug($"Peso tomado { peso }: Para vehiculo: { patente } y tarjeta: { tarjeta }");
            //if (recorrido.TipoVehiculo != TipoVehiculo.Tren && recorrido.TipoVehiculo != TipoVehiculo.Bitren) {
            //    while (true)
            //    {
            //        if (estadoPuesto.ValidarEstadoPuesto(puestoId))
            //            break;

            //        Thread.Sleep(5000);
            //    }
            //}

            if (resultado.Valida)
            {
                var balanzaId = servicio.ObtenerBalanzaAsociadaAPuestoAutomatico(resultado.PuestoDeTrabajoId);
                var serviciowf = pesadaFactory.CrearServicio(resultado.WorkflowDefinicionId);
                var resultadoActividad = serviciowf.Pesada(resultado.InstanceId,
                     peso ?? 0, 0, null, null, balanzaId.Id, null,
                     false, DateTime.Now, new ControlRecorridoDto
                     {
                         WorkflowInstanceId = resultado.InstanceId,
                         NombreUsuario = datosUsuario.NombreUsuario,
                         Actividad = resultado.ProximaActividad,
                         ActividadXaml = resultado.ProximaActividad,
                         Decision = false,
                         PuestoDeTrabajoId = resultado.PuestoDeTrabajoId,
                         Automatizado = peso == null || peso == 0,

                         CartaDePorte = recorrido.CartaDePorte,
                         Entregador = recorrido.Entregador,
                         Material = recorrido.Material,
                         Patente = recorrido.Patente,
                         PesoOrigenBruto = recorrido.PesoBrutoOrigen,
                         PesoOrigenTara = recorrido.PesoTaraOrigen,
                         TipoVehiculo = recorrido.TipoVehiculo,
                         Tarjeta = recorrido.TarjetaDeAcceso,
                         PesoBruto = recorrido.PesoBruto,
                         PesoTara = recorrido.PesoTara,
                         PesoOrigenNeto = recorrido.PesoNetoOrigen,
                         Calle = recorrido.Calle
                     });

                EncenderSemaforoVagon(color, puestoId);
            }
            return Json("ok", JsonRequestBehavior.AllowGet);
        }

        public void EliminarNotificacion(int notificacionId)
        {
            servicioComandos.Ejecutar(new ModificarNotificacion { Id = notificacionId });
        }

        public ActionResult ConfirmarNotificacion(int notificacionId)
        {
            EliminarNotificacion(notificacionId);
            return Json("ok", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Pesar(int puestoId)
        {
            var balanza = servicio.ObtenerBalanzaPorPuestoDeTrabajoSinTipoVehiculo(puestoId);
            var resultado = orquestador.Ejecutar(new EjecutarPesaje { CodigoDispositivo = balanza.CodigoCabezal });
            var peso = (int)resultado.Valores["Pesaje"];
            return Json(peso, JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult CambiarModalidad(int puestoId, DatosUsuario datosUsuario)
        {
            var puesto = servicio.ObtenerPuestoDeTrabajo(puestoId);
            puesto.PausaAutoFull = !puesto.PausaAutoFull;
            servicioComandos.Ejecutar(new ModificarPuestoDeTrabajo { Dto = puesto, Usuario = datosUsuario.NombreUsuario });
            return Json("ok", JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult ObtenerVagonesEnBalanza(DatosUsuario datosUsuario)
        {
            var vagones = servicio.ListarVagonesEnPesada(datosUsuario.CentroId).OrderBy(x => x.NumeroPatente);
            var lista = vagones.ToSelectList(f => f.NumeroPatente, f => f.NumeroPatente);

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult ConfirmarEspera(Guid guid, int notificacionId, DatosUsuario datosUsuario)
        {
            var recorrido = servicio.ObtenerRecorridoPorGuid(guid);
            var serviciowf = ejecutarFactory.CrearServicio(recorrido.WorkflowDefinicionId);
            var controlRecorrido = new ControlRecorridoDto
            {
                Actividad = Textos.ActEsperaConfirmacion,
                ActividadXaml = "EsperaConfirmacion",
                WorkflowInstanceId = recorrido.InstanciaWorkflow,
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                NombreUsuario = datosUsuario.NombreUsuario,
                Decision = true,
                Comentario = $"Etapa aceptada por {datosUsuario.NombrePc}"
            };
            serviciowf.Ejecutar(guid, controlRecorrido);
            try
            {
                EliminarNotificacion(notificacionId);
            }
            catch (Exception e)
            {
                log.Debug("Error al eliminar notificacion:" + e.Message);
            }

            return Json("ok", JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult AccionesEspeciales(int puestoId, DatosUsuario datosUsuario)
        {
            var barrerasApertura = servicio.ObtenerDispositivosMaestroApertura(puestoId);
            ViewBag.BarrerasAperturaMaestro = barrerasApertura != null ? barrerasApertura.Split(',').ToList() : new List<string>();
            var barrerasCierre = servicio.ObtenerDispositivosMaestroCierre(puestoId);
            ViewBag.BarrerasCierreMaestro = barrerasCierre != null ? barrerasCierre.Split(',').ToList() : new List<string>();
            ViewBag.PuestoId = puestoId;
            ViewBag.BalanzaId = servicio.ObtenerBalanzaPorPuestoDeTrabajoAutomatico(puestoId);
            ViewBag.PuestoPausado = servicio.EsPuestoPausado(puestoId);
            return PartialView();
        }

        [DatosUsuario]
        public ActionResult ActivarMaestro(DatosUsuario datosUsuario, int puestoId, string codigo, string motivo)
        {
            try
            {
                orquestador.Ejecutar(new EjecutarAperturaBarreraMaestro { CodigoDispositivo = codigo });
            }
            catch (Exception e)
            {
                return Json("Error al abrir la barrera", JsonRequestBehavior.AllowGet);
            }
            servicioComandos.Ejecutar(new CrearLogTarjetaSupervisor { PuestoDeTrabajoId = puestoId, Motivo = motivo, Usuario = datosUsuario.NombreUsuario });

            return Json("ok", JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult CerearBalanza(int balanzaId, DatosUsuario datosUsuario)
        {
            log.Info("Balanza Id {0}, inicio de CerearBalanza", balanzaId);
            var balanza = servicio.ObtenerBalanza(balanzaId);

            if (!CereoEstaPermitido(balanza, datosUsuario))
            {
                return Json(new { data = Textos.BalanzaACero_ExcedePesoMaximo }, JsonRequestBehavior.AllowGet);
            }

            if (balanza.Modalidad != Modalidad.Manual)
            {
                log.Info("Se iniciará el cereo para la balanza con Id {0}", balanza.Id);
                var resultado =
                    orquestador.Ejecutar(new EjecutarCereoCabezal { CodigoDispositivo = balanza.CodigoCabezal });
                log.Info("Llamada al orquestador exitosa.");

                if (resultado.Mensaje.Codigo != 0)
                {
                    log.Info("Cereo Fallido para la balanza con Id {0}", balanza.Id);
                    NotificarCereoFallido(balanza, datosUsuario);

                    return Json(new { data = resultado.Mensaje.Descripcion }, JsonRequestBehavior.AllowGet);
                }
            }
            log.Info("Cereo Exitoso para la balanza con Id {0}", balanza.Id);
            var resultadoRepositorio = ActualizarEstadoBalanzaCereada(balanzaId);
            log.Info("Estado Actualizado para la balanza con Id {0} de forma {0}", resultadoRepositorio.HayErrores);

            return Json(!resultadoRepositorio.HayErrores ? "ok" : resultadoRepositorio.Errores.Values.FirstOrDefault(), JsonRequestBehavior.AllowGet);
        }

        private bool CereoEstaPermitido(BalanzaDto balanza, DatosUsuario datosUsuario)
        {
            log.Info("Validando si excede el peso maximo de cereo");
            if (balanza.Modalidad == Modalidad.Manual)
            {
                log.Info("Balanza manual Id {0}, cereo aprobado", balanza.Id);
                return true;
            }
            log.Info("Se tomará el peso en modalidad automática para la balanza con Id {0}", balanza.Id);
            var pesoTomado = orquestador.Ejecutar(new EjecutarPesaje { CodigoDispositivo = balanza.CodigoCabezal });
            var hayPesaje = pesoTomado.Valores != null && pesoTomado.Valores.Any(a => a.Key == "Pesaje");
            log.Info("Llamada al orquestador exitosa. Hay Peso = {0}", hayPesaje);

            if (!hayPesaje || balanza.MaximoValorCereo < pesoTomado.Valores.First(f => f.Key == "Pesaje").Value)
            {
                NotificarCereoFallido(balanza, datosUsuario);
                return false;
            }
            return true;
        }

        private void NotificarCereoFallido(BalanzaDto balanza, DatosUsuario datosUsuario)
        {
            var htmlIconoColor = new StringBuilder();
            htmlIconoColor.AppendLine("<span class=\"iconoColor\" style=\"background-color:");
            htmlIconoColor.AppendLine(balanza.Color);
            htmlIconoColor.AppendLine(";\">&nbsp;&nbsp;&nbsp;&nbsp;</span>");

            notificador.Notificar(new NotificacionDto
            {
                Grupo = datosUsuario.CentroId + "|" + PermisosScato.Administradores.ToString(),//TODO : rever  grupo!
                Mensaje = String.Format(Textos.Notificacion_PendienteCereo, balanza.Nombre, htmlIconoColor),
                TipoAlerta = TipoAlerta.Sobre
            });
        }

        private Resultado ActualizarEstadoBalanzaCereada(int balanzaId)
        {
            var resultado = servicioComandos.Ejecutar(new ModificarBalanzaEstaEnCero
            {
                BalanzaId = balanzaId,
                EstaEnCero = true
            });
            if (resultado.HayErrores)
            {
                return resultado;
            }
            var cookie = new CookieUsuario();
            cookie.ActualizarValor("BalanzaId", balanzaId.ToString(CultureInfo.InvariantCulture));

            return resultado;
        }

        [DatosUsuario]
        [Autorizacion(PermisosScato.ForzarCereo)]
        public ActionResult Avanzar(int balanzaId, string motivo, DatosUsuario datosUsuario)
        {
            log.Info("Se va a forzar el paso por cero de la Balanza con Id: {0} por el usuario: {1}", balanzaId, datosUsuario.NombreUsuario);
            var resultadoRepositorio = ActualizarEstadoBalanzaCereada(balanzaId);
            log.Info("Estado Actualizado para la balanza con Id {0} de forma {1}", balanzaId, resultadoRepositorio.HayErrores);

            log.Info("Se va a crear el Motivo Forzar Cero por el usuario: {0}", datosUsuario.NombreUsuario);
            ResultadoCrear resultadoCrear = servicioComandos.Ejecutar(new CrearMotivoForzarCero { BalanzaId = balanzaId, Fecha = DateTime.Now, Usuario = datosUsuario.NombreUsuario, Motivo = motivo }) as ResultadoCrear;

            return Json(!resultadoRepositorio.HayErrores ? (resultadoCrear != null && !resultadoCrear.HayErrores) ? "ok" : resultadoCrear != null ? resultadoCrear.Errores.Values.FirstOrDefault() : Textos.Error_Generico : resultadoRepositorio.Errores.Values.FirstOrDefault(), JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult TomarPeso(int balanzaId, Guid instanceId, string actividad, DatosUsuario datosUsuario)
        {
            try
            {
                log.Info("Se tomará el peso en modalidad para la balanza con Id {0}", balanzaId);
                var balanza = servicio.ObtenerBalanza(balanzaId);
                

                var resultado = EjecutarPesaje(balanza.CodigoCabezal);

                if (resultado.Mensaje.Codigo != 0)
                {
                    log.Error("Error al tomar la pesada en balanza {0}. Mensaje: {1} - {2}",
                              balanzaId, resultado.Mensaje.Codigo, resultado.Mensaje.Descripcion);
                    return
                        Json(
                            "(" + Textos.Codigo + ":" + resultado.Mensaje.Codigo + ") " + Textos.Pesada_AutomaticaError +
                            "\r\n" + resultado.Mensaje.Descripcion, JsonRequestBehavior.AllowGet);
                }
                servicioComandos.Ejecutar(new CrearLogActividad()
                {
                    Dto = new LogActividadDto
                    {
                        Actividad = $"Peso Tomado: {resultado.Valores["Pesaje"]}",
                        Fecha = DateTime.Now,
                        WorkflowInstanceId = instanceId,
                        ActividadXaml = actividad
                    },
                    Usuario = datosUsuario.NombreUsuario
                });
                log.Info("Se tomo el peso {0} la balanza con Id {1}, por el usuario {2}", resultado.Valores["Pesaje"], balanza.Id, datosUsuario.NombreUsuario);
                return Json(resultado.Valores["Pesaje"], JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error en tomar peso para la balanza con Id {0}", balanzaId);
                return Json(Textos.Pesada_AutomaticaError, JsonRequestBehavior.AllowGet);
            }
        }

        private ResultadoEjecutar EjecutarPesaje(string codigoCabezal)
        {
            ResultadoEjecutar resultado = null;
            var hayPesaje = false;
            var contador = 0;
            var ejecutarPesaje = new EjecutarPesaje { CodigoDispositivo = codigoCabezal };
            while (contador++ < 5 && !hayPesaje)
            {
                try
                {
                    resultado = orquestador.Ejecutar(ejecutarPesaje);
                    hayPesaje = resultado.Mensaje.Codigo == 0;
                    log.Debug("Llamada al orquestador exitosa. Hay peso en cabezal {0} = {1}", codigoCabezal, hayPesaje);
                    if (!hayPesaje)
                    {
                        //Esperar 50ms para hacer la próxima llamada
                        Thread.Sleep(50);
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex, "Error en tomar peso para la balanza con cabezal {0}", codigoCabezal);
                    resultado = null;
                }
            }
            return resultado;
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult PesadaExportacion(int balanzaId, TipoPesada tipoPesada, bool rechazado, string actividadXaml, Guid workflowInstanceId, int workflowDefinicionId, int? peso, string patente, DatosUsuario datosUsuario)
        {
            log.Debug("Puesto de Trabajo Post {0}", datosUsuario.PuestoDeTrabajoId);
            if (ModelState.IsValid) //Todos los datos obligatorios fueron ingresados
            {
                var balanza = servicio.ObtenerBalanza(balanzaId);

                if (balanza.EstaEnCero && peso.HasValue) //Balanza en condiciones de pesar
                {
                    var centro = servicio.ObtenerCentro(datosUsuario.CentroId);
                    if (centro.ValidarLimiteMinimoDePeso && centro.LimiteMinimoDePeso.HasValue && peso.Value <= centro.LimiteMinimoDePeso)
                    {
                        return Json(Textos.ErrorPesoPesadaNoValido + centro.LimiteMinimoDePeso);
                    }
                    var cookie = new CookieUsuario();
                    cookie.ActualizarValor("BalanzaId", balanzaId.ToString(CultureInfo.InvariantCulture));
                    var controlRecorrido = new ControlRecorridoDto
                    {
                        Actividad = Textos.Pesada + " (" + tipoPesada.DisplayEnum() + ")" + (rechazado ? "/Rechazo" : string.Empty),
                        ActividadXaml = actividadXaml,
                        WorkflowInstanceId = workflowInstanceId,
                        PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                        NombreUsuario = datosUsuario.NombreUsuario,
                        Decision = rechazado,
                        Comentario = "",
                        Mensaje = ""
                    };
                    var proximaAccion = workflows.ObtenerWorkflowProximaAccion(workflowInstanceId);
                    if (!proximaAccion.ProximaAccion.StartsWith("Pesada" + tipoPesada.ToString()) && proximaAccion.ProximaAccion != "PesadaCargaExportacion") //Controla la actividad actual con la pantalla actual
                    {
                        log.Error("El vehículo no se encuentra en la pesada: {0}", tipoPesada.ToString());
                        return Json(Textos.Error_EtapaIncorrecta);
                    }
                    var servicioWf = pesadaFactory.CrearServicio(workflowDefinicionId);
                    var resultadoActividad = servicioWf.Pesada(workflowInstanceId, peso.Value, 0, null, null, balanzaId, null, false, DateTime.Now, controlRecorrido);
                    if (!resultadoActividad.HayErrores) //Peso tomado correctamente
                    {
                        servicioComandos.Ejecutar(new ModificarBalanzaEstaEnCero
                        {
                            BalanzaId = balanzaId,
                            EstaEnCero = false
                        });
                        return Json("OK", JsonRequestBehavior.AllowGet);
                    }
                    return Json(resultadoActividad.Errores.First().Value, JsonRequestBehavior.AllowGet);
                }
                //Balanza no está en cero
                return !balanza.EstaEnCero
                           ? Json(Textos.Pesada_BalanzaNoCero)
                           : Json(Textos.Pesada_Error);
            } //Hubieron errores
            return Json(Textos.Pesada_Error);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Carga(Guid workflowInstanceId, bool cargaParcial, int workflowDefinicionId, DatosUsuario datosUsuario)
        {
            var controlRecorrido = new ControlRecorridoDto
            {
                WorkflowInstanceId = workflowInstanceId,
                NombreUsuario = datosUsuario.NombreUsuario,
                Actividad = Textos.ActConfirmacionDeCargaDescarga,
                ActividadXaml = "PesadaCargaExportacion/Carga",
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                Decision = cargaParcial,
                //Mensaje = "Cancelado"
            };

            var serviciowf = ejecutarFactory.CrearServicio(workflowDefinicionId);

            var resultado = serviciowf.Ejecutar(controlRecorrido.WorkflowInstanceId, controlRecorrido);
            if (!resultado.HayErrores)
            {
                return Json(new { responseText = "OK", CargaParcial = cargaParcial });
            }
            return new ContentResult { Content = resultado.Errores.Values.FirstOrDefault() ?? Textos.Error_ActualizarGenerico };
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult ValidarVagon(string vagon, int puestoId, DatosUsuario datosUsuario)
        {
            var tren = servicio.ObtenerDatosRecorridoActivoSinTarjeta(vagon, null);
            var proximaActividad = workflows.ObtenerWorkflowProximaAccion(tren.InstanciaWorkflow);
            var color = "ROJO";

            if (proximaActividad?.ProximaAccion?.ToUpper()?.Trim() == "PESADABRUTO")
            {
                EncenderSemaforoVagon(color, puestoId);
            }

            return new JsonResult()
            {
                Data = new NotificacionPesadaAutomaticaDto
                {
                    Id = puestoId,
                    Patente = vagon,
                    CartaPorte = tren.CartaDePorte,
                    Material = tren.Material,
                    Entregador = tren.Entregador ? "Si" : "No",
                    Peso = "0",
                    Diferencia = "0",
                    Actividad = proximaActividad.Mensaje == null ? proximaActividad.ProximaAccion : null,
                    DifPeso = tren.PesoBruto.HasValue ? (tren.PesoBrutoOrigen - tren.PesoBruto).ToString() : "0",
                    TipoVehiculo = tren.TipoVehiculo.ToString(),
                    TipoComercial = tren.TipoComercial,
                    DifNeto = tren.PesoTara.HasValue ? (tren.PesoNetoOrigen - (tren.PesoTara - tren.PesoBruto)).ToString() : "0",
                    TipoPeso = proximaActividad.Mensaje == null ? proximaActividad.ProximaAccion.Contains("Bruto") ? "Bruto:" :
                    proximaActividad.ProximaAccion.Contains("Tara") ? "Tara:" : "Peso:" : null,
                    PesoBrutoOrigen = tren.PesoBrutoOrigen.ToString(),
                    PesoNetoOrigen = tren.PesoNetoOrigen.ToString(),
                    Error = proximaActividad.Mensaje,
                    PesoBruto = tren.PesoBruto,
                    PesoTara = tren.PesoTara,
                    DocumentoIngreso = tren.TipoDocumento.ToString(),
                    WorkflowInstanceId = tren.InstanciaWorkflow
                }
            };
        }

        public ActionResult ObtenerEtapaExpo(Guid recorrido)
        {
            var ultimoLog = servicio.ObtenerUltimoLog(recorrido);
            var actividad = ultimoLog.Actividad != "Pesada Bruto Exportacion" && ultimoLog.Actividad != "Pesada Tara Exportacion" && ultimoLog.Actividad != "Confirmacion de Carga/Descarga" ? "Error" : ultimoLog.Actividad;
            var peso = servicio.ObtenerPesoNetoExportacion(recorrido);
            return Json(new { Actividad = actividad, PesoNeto = peso }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PrenderApagarDispositivo(string codigoDispositivo, bool activar)
        {
            var urlServer = new Uri(ConfigurationManager.AppSettings["ICWebServerUrl"]);

            orquestador.PrenderApagarDispositivo(codigoDispositivo, activar, urlServer.Host);
            return Json(true);
        }

        private void EncenderSemaforoVagon(string color, int puestoId)
        {
            string semaforo = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(color))
                {
                    var puestoTrabajo = servicio.ObtenerPuestoDeTrabajo(puestoId);

                    semaforo = color == "ROJO"     ? puestoTrabajo?.SemaforoRojoCodigo :
                               color == "AMARILLO" ? puestoTrabajo?.SemaforoAmarilloCodigo :
                               color == "VERDE"    ? puestoTrabajo?.SemaforoVerdeCodigo : string.Empty;

                    if (!string.IsNullOrEmpty(semaforo))
                    {
                        log.Info("Se envia cambio de estado al semaforo : {0}, color : {1}", semaforo, color);
                        var resultadoSemaforo = orquestador.Ejecutar(new EjecutarAperturaBarrera { CodigoDispositivo = semaforo });
                        if (resultadoSemaforo.Mensaje.Codigo == 0)
                        {
                            log.Info("Se envio cambio de estado al semaforo : {0}, color : {1} correctamente.", semaforo, color);
                            notificador.Notificar(new NotificacionDto
                            {
                                Grupo = "Automaticas",
                                Mensaje = new NotificacionSemaforoVagonesAutomaticaDto { PuestoId = puestoId, Color = color }.ToJson(),
                                TipoAlerta = TipoAlerta.CambioEstadoSemaforo,
                                PuestoId = puestoId
                            });
                        }
                        else
                        {
                            log.Info("Mensaje de error Semaforo vagones codigo: {0}, descripcion : {1}", resultadoSemaforo.Mensaje.Codigo, resultadoSemaforo.Mensaje.Descripcion);
                        }
                    }
                    else
                    {
                        log.Info("Mensaje de error Semaforo vagones, no existe semaforo configurado para el color : {0}, puestoDeTrabajoId : {1}", color, puestoId);
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Ocurrio un error al establecer el semafo : {0} al color {1} , Error : {2}", semaforo, color, e);
            }
        }

        private IntercomunicadorDispositivoDto GetIntercomunicadorDispositivoConfig(string uniqueId, string codigoIntercomunicador, int? puertoAudio)
        {
            var intercomunicadorDispositivo = new IntercomunicadorDispositivoDto
            {
                UniqueId = uniqueId,
                Codigo = codigoIntercomunicador,
                AudioPort = (puertoAudio.HasValue) ? puertoAudio.Value.ToString() : string.Empty,
                ICPCConfig = ConfigurationManager.AppSettings["ICPCConfig"],
                ICWebServerUrl = ConfigurationManager.AppSettings["ICWebServerUrl"],
                ICWSServerUrl = ConfigurationManager.AppSettings["ICWSServerUrl"],
                DeviceActivationUrl = Url.Action("PrenderApagarDispositivo", "BalanzaAutomatica"),
                PublishingPathListen = codigoIntercomunicador + Constantes.IntercomunicadorDireccion.HaciaLaWeb,
                PublishingPathSpeak = Constantes.IntercomunicadorDireccion.DesdeLaWeb + codigoIntercomunicador,
            };

            return intercomunicadorDispositivo;
        }
    }
}