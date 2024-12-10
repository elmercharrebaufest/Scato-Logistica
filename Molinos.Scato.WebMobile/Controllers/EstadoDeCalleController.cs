using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.WebMobile.Atributos;
using Molinos.Scato.WebMobile.Helpers;
using Newtonsoft.Json;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using WebGrease.Css.Extensions;

namespace Molinos.Scato.WebMobile.Controllers
{
    [Autorizacion(PermisosScato.EstadoDeCalle)]
    public class EstadoDeCalleController : ConsultasController
    {
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioRepositorio servicio;
        private readonly ILogger log;
        private IConfiguracionProvider configuracion;

        public EstadoDeCalleController(
            ILogger log,
            IServicioRepositorio servicio,
            IConfiguracionProvider configuracion,
            IServicioComandos servicioComandos

            ) : base(log, servicio, configuracion)
        {
            this.log = log;
            this.servicio = servicio;
            this.configuracion = configuracion;
            this.servicioComandos = servicioComandos;
        }

        public ActionResult Index()
        {
            var centro = ClaimsPrincipal.Current.GetUserClaim("CentroId");
            var centroId = int.Parse(centro.Value);
            var materialesNoGranos = servicio.ObtenerMaterialNoGranoAsignableCalle();
            var centroDto = this.servicio.ObtenerCentro(centroId);

            var tiposCalleValidas = new List<TipoCalle>() {
                TipoCalle.PreCalado,
                TipoCalle.Circular,
                TipoCalle.PostCalado,
                TipoCalle.RechazadosDemorados,
                TipoCalle.ReCalado,
                TipoCalle.Calado,
                TipoCalle.NoGranos,
            };

            var ultimasPatentesLlamadas = servicio.ListarCamionesLlamados();

            ViewBag.Llamados = JsonConvert.SerializeObject(ultimasPatentesLlamadas);
            ViewBag.MinutosEsperaCircular = centroDto?.MinutosEsperaCircular ?? 20;
            ViewBag.MinutosEsperaPrecalado = centroDto?.MinutosEsperaPrecalado ?? 30;
            ViewBag.CallesCalado = servicio.ListarCallesPorTipo(TipoCalle.Calado);

            ViewBag.CantFilasPorCalador = EstadoCaladoresActivos();

            return View(servicio.ObtenerCallesPorCentro(centroId).Where(x => x.TipoCalle == TipoCalle.NoGranos
                        ? materialesNoGranos.Any(a => a.Id == x.MaterialId)
                                && tiposCalleValidas.Contains(x.TipoCalle)
                        : tiposCalleValidas.Contains(x.TipoCalle)).ToList());
        }

        public JsonResult EstadoDeCalle()
        {
            var centro = ClaimsPrincipal.Current.GetUserClaim("CentroId");
            var centroId = int.Parse(centro.Value);
            var camiones = servicio.ObtenerEstadoDeCalle();
            camiones.ForEach(camion =>
            {
                if(camion.TipoCalle == TipoCalle.NoGranos)
                {
                    camion.EsCuitNestle = servicio.ValidarCuitNestle(camion.RecorridoId ?? 0 );
                }
            });

           


            var tiposCalleValidas = new List<TipoCalle>() {
                TipoCalle.PreCalado,
                TipoCalle.Circular,
                TipoCalle.PostCalado,
                TipoCalle.RechazadosDemorados,
                TipoCalle.ReCalado,
                TipoCalle.Calado,
                TipoCalle.NoGranos,
            };

            var calles = servicio.ObtenerCallesPorCentro(centroId).Where(x => tiposCalleValidas.Contains(x.TipoCalle));

            var ultimasPatentesLlamadas = servicio.ListarCamionesLlamados();

            var materiales = camiones.Where(x => tiposCalleValidas.Contains(x.TipoCalle))
                .Select(x => new { x.MaterialId, x.MaterialDesc })
                .Union(calles.Where(x => tiposCalleValidas.Contains(x.TipoCalle))
                .Select(x => new { x.MaterialId, x.MaterialDesc }))
                .GroupBy(x => x).Select(x => x.Key).Where(x => x.MaterialId != 0)
                .OrderBy(x => x.MaterialId);

            ViewBag.CantFilasPorCalador = EstadoCaladoresActivos();

            return Json(new { estado = camiones, materiales, calles, ultimasPatentesLlamadas }, JsonRequestBehavior.AllowGet);
        }

        [Autorizacion(PermisosScato.EstadoDeCalleLlamar)]
        public JsonResult LlamarCalle(int calleId, int? calleCaladoId)
        {
            calleCaladoId = calleCaladoId ?? 0;

            var response = new MensajeEstandarDto();
            try
            {
                var calle = servicio.ObtenerCalle(calleId);
                if (calle.FechaLLamada.HasValue)
                {
                    response = (new MensajeEstandarDto { Mensaje = $"La fila {calle.Nombre} ya está siendo llamada.", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                    return Json(response, JsonRequestBehavior.AllowGet);
                }

                response = LlamadasPrecaladoLimiteFilas(calle, calleCaladoId);
                if (response.TipoDeMensaje != TipoDeMensajeDeRespuesta.Error)
                {
                    calle.Bloqueada = true;
                    calle.FechaLLamada = DateTime.Now;
                    if (calleCaladoId > 0 && (calle.TipoCalle == TipoCalle.PreCalado || calle.TipoCalle == TipoCalle.Circular))
                    {
                        calle.CalleCaladoId = calleCaladoId.Value;
                    }
                    servicioComandos.Ejecutar(new LlamarCalle { Dto = calle, Llamada = true });
                    EnviarMensajeLlamadoACartel(calle);
                }
            }
            catch (Exception e)
            {
                log.Error(e, $"No se pudo llamar la calle {calleId}");
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [Autorizacion(PermisosScato.EstadoDeCalleLlamar)]
        public JsonResult LLamarSiguienteCalle(int materialId)
        {
            var calle = servicio.ObtenerSiguienteCalle(materialId);
            if (calle != null)
            {
                calle.Bloqueada = true;
                calle.FechaLLamada = DateTime.Now;
                servicioComandos.Ejecutar(new LlamarCalle { Dto = calle });
            }
            return Json("ok", JsonRequestBehavior.AllowGet);
        }

        [Autorizacion(PermisosScato.EstadoDeCalleLlamar)]
        public JsonResult LLamadoAutomaticoCalle(int calleId, int materialId, bool activar, string motivo)
        {
            var calle = servicio.ObtenerCalle(calleId);
            calle.Automatica = activar;
            calle.MaterialId = materialId;
            var usuario = ClaimsPrincipal.Current.FindFirst(System.IdentityModel.Claims.ClaimTypes.NameIdentifier).Value;
            servicioComandos.Ejecutar(new ModificarCalle { Dto = calle });
            if (!string.IsNullOrEmpty(motivo))
                servicioComandos.Ejecutar(new CrearLogCambioDeModalidadCalle { Dto = new LogCambioDeModalidadCalleDto { Motivo = motivo, CalleId = calleId, Usuario = usuario, Activado = activar } });
            return Json(new { Automatica = activar, MaterialId = materialId }, JsonRequestBehavior.AllowGet);
        }

        [Autorizacion(PermisosScato.EstadoDeCalleCancelar)]
        public JsonResult CancelarLlamarCalle(int calleId)
        {
            var calle = servicio.ObtenerCalle(calleId);
            calle.Bloqueada = false;
            calle.FechaLLamada = null;
            servicioComandos.Ejecutar(new LlamarCalle { Dto = calle, Llamada = false });
            if (calle.TipoCalle != TipoCalle.Circular)
            {
                servicioComandos.Ejecutar(new MarcarUltimaCallePorRecorrido { CalleId = calleId });
            }

            CancelarLlamadoPorTipo(calle);

            return Json("ok", JsonRequestBehavior.AllowGet);
        }

        [Autorizacion(PermisosScato.EstadoDeCalleLlamar)]
        public ActionResult MoverRechazado(string patente, int calleId)
        {
            var centro = ClaimsPrincipal.Current.GetUserClaim("CentroId");
            var usuario = ClaimsPrincipal.Current.GetUserClaim(ClaimTypes.NameIdentifier);
            var centroId = int.Parse(centro.Value);
            var model = servicio.ObtenerInfoPatente(patente, calleId);
            if (!(model is null))
                model.PermisoReasignarCallePostCalado = servicio.TienePermiso(usuario.Value, PermisosScato.ReasignacionCallesPostCalado);
            var calle = servicio.ObtenerCalle(calleId);
            var caracteristicasAnalizadas = model.CaladoId.HasValue ? servicio.ListarCaladoPorCaracteristicas(model.CaladoId.Value) : null;
            List<CalleDto> calles = new List<CalleDto>();

            // Sólo si el camión ya esta en una calle con alguna calidad específica o en pendiente
            if (model.TipoCalidad == TipoCalidad.Otros || model.TipoCalidad == TipoCalidad.PendientesPostCalado)
            {
                // Obtiene calle rechazado o calle con mismas características
                calles = servicio.ObtenerCallesPorCentro(centroId).Where(x => model.Rechazado
                ? x.TipoCalle == TipoCalle.RechazadosDemorados && !x.Deshabilitada
                : (x.TipoCalle == TipoCalle.PostCalado && x.TipoCalidad == TipoCalidad.Otros && model.MaterialId == x.MaterialId && !x.Deshabilitada && caracteristicasAnalizadas.Any(ca => ca.CaracteristicaId == x.CaracteristicaDeCalidadId && ca.ValorCalado <= x.RangoCaracteristicaCalidadMaximo && ca.ValorCalado >= x.RangoCaracteristicaCalidadMinimo) && servicio.ListarCallePorRecorridoPorCalleId(x.Id).Select(c => c.EsSojaEPA).FirstOrDefault() == model.EsSojaEPA)
                    ).ToList();
            }
            if (!calles.Any())
            {
                // Obtiene calles vacias sin contar las calles con alguna calidad específica y pendiente
                List<CalleDto> callesVacias = servicio.ObtenerCallesPorCentro(centroId).Where(x => x.TipoCalle == TipoCalle.PostCalado && x.TipoCalidad != TipoCalidad.PendientesPostCalado && !x.Deshabilitada && x.Id != calleId && x.TipoCalidad != TipoCalidad.Otros && !servicio.ListarCallePorRecorridoPorCalleId(x.Id).Any()).ToList();

                // Obtiene calle rechazado o calle con camiones con mismas características
                var callesConCamionesConMismaCalidad = model.Rechazado
                     ? servicio.ObtenerCallesPorCentro(centroId).Where(x => x.TipoCalle == TipoCalle.RechazadosDemorados && !x.Deshabilitada).ToList()
                     : servicio.ObtenerCallesDeCallesPorRecorridoSegunMaterial(model.MaterialId, calle.Id, model.CalidadCamion, model.EsSojaEPA, model.EsSojaIMPO).ToList().FindAll(c => servicio.ListarCallePorRecorridoPorCalleId(c.Id).Count() < c.CantidadDeCamiones);
                if (callesVacias != null && callesConCamionesConMismaCalidad != null && !model.Rechazado)
                {
                    calles = callesConCamionesConMismaCalidad.Concat(callesVacias).ToList();
                }
            }

            if (!calles.Any())
            {
                // Obtiene calles vacias sin contar las calles con alguna calidad específica y pendiente
                calles = servicio.ObtenerCallesPorCentro(centroId).Where(x => model.Rechazado ? x.TipoCalle == TipoCalle.RechazadosDemorados && !x.Deshabilitada : x.TipoCalle == TipoCalle.PostCalado && x.TipoCalidad != TipoCalidad.PendientesPostCalado && !x.Deshabilitada && x.Id != calleId && x.TipoCalidad != TipoCalidad.Otros && !servicio.ListarCallePorRecorridoPorCalleId(x.Id).Any()).ToList();
            }
            var callesDisponibles = calles.FindAll(c => !c.Id.Equals(calleId) && servicio.ListarCallePorRecorridoPorCalleId(c.Id).Count() < c.CantidadDeCamiones);
            ViewBag.CallesPostCalado = callesDisponibles.Where(x => !x.Bloqueada).Distinct().Select(x => new SelectListItem { Selected = x.Id == calle.Id, Text = x.Nombre, Value = x.Id.ToString() });
            if (model.EsSojaIMPO)
            {
                model.InstanciaWorflow = new Guid();
            }
            return PartialView("_MoverCamionRechazado", model);
        }

        [Autorizacion(PermisosScato.EstadoDeCalleLlamar)]
        public JsonResult LlamarCallePostCalado(int calleId)
        {
            var response = new RespuestaEstandarDto();
            try
            {
                var calle = servicio.ObtenerCalle(calleId);
                if (calle.FechaLLamada.HasValue)
                {
                    response.Mensajes.Add(new MensajeEstandarDto { Mensaje = $"La fila {calle.Nombre} ya está siendo llamada.", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                    return Json(response, JsonRequestBehavior.AllowGet);
                }

                calle.Bloqueada = true;
                calle.FechaLLamada = DateTime.Now;

                servicioComandos.Ejecutar(new LlamarCalle { Dto = calle });
                EnviarMensajeLlamadoACartelPostCalado(calle);
            }
            catch (Exception e)
            {
                log.Error(e, $"No se pudo llamar la calle {calleId}");
                response.Mensajes.Add(new MensajeEstandarDto { Mensaje = $"Ocurrió un error en el llamado.", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ConfirmarRechazado(Guid instanciaWorflow)
        {
            servicioComandos.Ejecutar(
               new CrearCallePorRecorrido
               {
                   TipoCalle = TipoCalle.RechazadosDemorados,
                   InstanciaWorkflow = instanciaWorflow
               });
            return Json("ok", JsonRequestBehavior.AllowGet);
        }

        public JsonResult ConfirmarReasignacionCalle(Guid instanciaWorflow, int calleId , int? cargaDeCupoId)
        {
            var calleInicioId = servicio.ObtenerCalleInicial(instanciaWorflow);

            var result = servicioComandos.Ejecutar(
               new ReasignarCamionPostcalado
               {
                   InstanciaWorkflow = instanciaWorflow,
                   CalleId = calleId,
                   CargaDeCupoId = cargaDeCupoId
               });

            var calleInicialVacia = servicio.ListarCallePorRecorridoPorCalleId(calleInicioId).Count();
            if (calleInicialVacia == 0)
            {
                LimpiarCartelReasignacionUltimoCamion(calleInicioId);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CambioCalleCalado(int calleCaladoId)
        {
            var limiteFilasPrecalado = this.servicio.ObtenerConfiguracionGeneral("EstadoDeCallePreCalado", "LimiteFilasLlamadas").Valor;
            var limiteFilasPrecaladoLlamadas = limiteFilasPrecalado != null ? int.Parse(limiteFilasPrecalado) : 3;

            var result = servicio.CaladorLleno(calleCaladoId, limiteFilasPrecaladoLlamadas);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private int EstadoCaladoresActivos()
        {
            var caladoresActivos = servicio.CaladoresActivos();
            var limiteFilasPrecaladoLlamadasMensaje = this.servicio.ObtenerConfiguracionGeneral("EstadoDeCallePreCalado", "LimiteFilasLlamadas").Valor;
            var limiteFilasPrecaladoLlamadas = limiteFilasPrecaladoLlamadasMensaje != null ? int.Parse(limiteFilasPrecaladoLlamadasMensaje) : 3;
            return limiteFilasPrecaladoLlamadas / caladoresActivos;
        }

        private CantidadPrecaladoCircularHelper EstadoDeCallesBloqueada(int? calleCaladoId)
        {
            var cantBloqueadas = servicio.ContarCallesBloqueadas(calleCaladoId);
            return cantBloqueadas;
        }

        private void EnviarMensajeLlamadoACartel(CalleDto callePrecalado)
        {
            try
            {
                var cartel = servicio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.EstadoDeCallePreCalado, Constantes.ConfiguracionGeneral.PreCalado.CartelLedCalador);

                var esCircular = callePrecalado.TipoCalle == TipoCalle.Circular;

                var codigo = servicio.ObtenerCodigoMensaje(callePrecalado.CalleCaladoId);

                var orden = callePrecalado.MaterialId == 4 ? servicio.ObtenerOrdenCircular(codigo) : (int?)null;

                var resultadoSlot = (ResultadoMensajeCartelLed)servicioComandos.Ejecutar(new InsertarSlotMensajeCartelLed() { CalleId = callePrecalado.Id, Codigo = codigo, OrdenCircular = orden, EsCircular = esCircular });

                servicioComandos.Ejecutar(new EnviarMensajeCartelLed
                {
                    Mensaje = $"{resultadoSlot.Mensaje}",
                    Codigo = cartel.Valor,
                    NumeroPrograma = resultadoSlot.NumeroPrograma,
                    NumeroTrama = resultadoSlot.NumeroTrama,
                    NumeroVariable = resultadoSlot.NumeroVariable,
                    SegundosDeEspera = resultadoSlot.SegundosDeEspera,
                });
            }
            catch (Exception e)
            {
                log.Error(e, "No se pudo mostrar el mensaje en Cartel Led");
            }
        }

        private void EnviarMensajeLlamadoACartelPostCalado(CalleDto callePostCalado)
        {
            try
            {
                var cartel = servicio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.EstadoDeCallePostCalado, Constantes.ConfiguracionGeneral.PostCalado.CartelLedPostCalado);
                if (cartel != null)
                {
                    var resultadoInsertarCartelLed = servicioComandos.Ejecutar(new InsertarSlotMensajeCartelLed()
                    {
                        Codigo = CodigoMensajeCartelLed.LlamadoCallePostcalado,
                        CalleId = callePostCalado.Id
                    }) as ResultadoMensajeCartelLed;

                    if (!resultadoInsertarCartelLed.HayErrores)
                    {
                        servicioComandos.Ejecutar(new EnviarMensajeCartelLed
                        {
                            Mensaje = resultadoInsertarCartelLed.Mensaje,
                            Codigo = cartel?.Valor,
                            NumeroTrama = resultadoInsertarCartelLed.NumeroTrama,
                            NumeroPrograma = resultadoInsertarCartelLed.NumeroPrograma,
                            NumeroVariable = resultadoInsertarCartelLed.NumeroVariable,
                        });
                    }
                    else
                    {
                        log.Error(resultadoInsertarCartelLed.Errores.First().Value);
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e, "No se pudo mostrar el mensaje en Cartel Led");
            }
        }

        private void CancelarLlamadoPorTipo(CalleDto calle)
        {
            switch (calle.TipoCalle)
            {
                case TipoCalle.PostCalado:
                    {
                        var resultado = servicioComandos.Ejecutar(new LimpiarHistorialMensajeCartelLed()
                        {
                            Codigo = CodigoMensajeCartelLed.LlamadoCallePostcalado,
                            CalleId = calle.Id
                        }) as ResultadoMensajeCartelLedReordenado;

                        var cartel = servicio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.EstadoDeCallePostCalado, Constantes.ConfiguracionGeneral.PostCalado.CartelLedPostCalado);
                        foreach (var mensajeCartelLed in resultado.ListaDeMensajes)
                        {
                            servicioComandos.Ejecutar(new EnviarMensajeCartelLed
                            {
                                Mensaje = mensajeCartelLed.HistorialMensajeCartelLed?.Mensaje ?? "-",
                                Codigo = cartel?.Valor,
                                NumeroTrama = mensajeCartelLed.Trama,
                                NumeroPrograma = mensajeCartelLed.Programa,
                                NumeroVariable = mensajeCartelLed.Variable,
                            });
                        }
                        break;
                    }

                case TipoCalle.PreCalado:
                case TipoCalle.Circular:
                    {
                        if (calle.CalleCaladoId != 0)
                        {
                            var codigo = servicio.ObtenerCodigoMensaje(calle.CalleCaladoId);
                            var resultado = servicioComandos.Ejecutar(new LimpiarHistorialMensajeCartelLed()
                            {
                                Codigo = codigo,
                                CalleId = calle.Id,
                            }) as ResultadoMensajeCartelLedReordenado;

                            var cartel = servicio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.EstadoDeCallePreCalado, Constantes.ConfiguracionGeneral.PreCalado.CartelLedCalador);
                            foreach (var mensajeCartelLed in resultado.ListaDeMensajes)
                            {
                                servicioComandos.Ejecutar(new EnviarMensajeCartelLed
                                {
                                    Mensaje = mensajeCartelLed.HistorialMensajeCartelLed?.Mensaje ?? "-",
                                    Codigo = cartel?.Valor,
                                    NumeroTrama = mensajeCartelLed.Trama,
                                    NumeroPrograma = mensajeCartelLed.Programa,
                                    NumeroVariable = mensajeCartelLed.Variable,
                                });
                            }
                        }
                        break;
                    }
            }
        }

        private MensajeEstandarDto LlamadasPrecaladoLimiteFilas(CalleDto calle, int? calleCaladoId)
        {
            var mensaje = new MensajeEstandarDto() { TipoDeMensaje = TipoDeMensajeDeRespuesta.Success };
            var cantBloqueadas = EstadoDeCallesBloqueada(calleCaladoId);
            int.TryParse(servicio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.EstadoDeCallePreCalado, Constantes.ConfiguracionGeneral.PreCalado.LimiteFilasLlamadas).Valor, out int limiteDeFilasTotales);
            var cantFilasPorCalador = EstadoCaladoresActivos();
            var maxFilasPrecalado = cantFilasPorCalador - 1;

            if (cantBloqueadas.CantidadTotal == cantFilasPorCalador)
            {
                mensaje = new MensajeEstandarDto
                {
                    TipoDeMensaje = TipoDeMensajeDeRespuesta.Error,
                    //limite de filas totales
                    Mensaje = "1"
                };
            }

            switch (calle.TipoCalle)
            {
                case TipoCalle.PreCalado:
                    {
                        if (cantBloqueadas.CantidadPrecalado == maxFilasPrecalado)
                            mensaje = new MensajeEstandarDto
                            {
                                TipoDeMensaje = TipoDeMensajeDeRespuesta.Error,
                                //limite de filas circulares para material soja
                                Mensaje = "3"
                            };
                        break;
                    }
                case TipoCalle.Circular:
                    {
                        //material de soja reserva dos slots del cartel para circular
                        if (calle.MaterialId == 4 && cantBloqueadas.CantidadCircular == 1)
                        {
                            mensaje = new MensajeEstandarDto
                            {
                                TipoDeMensaje = TipoDeMensajeDeRespuesta.Error,
                                //limite de filas circulares para material soja
                                Mensaje = "2"
                            };
                        }

                        break;
                    }
            }
            return mensaje;
        }

        private void LimpiarCartelReasignacionUltimoCamion(int calleId)
        {
            var calle = servicio.ObtenerCalle(calleId);
            calle.Bloqueada = false;
            calle.FechaLLamada = null;
            servicioComandos.Ejecutar(new LlamarCalle { Dto = calle, Llamada = false });
          
            CancelarLlamadoPorTipo(calle);
        }
    }
}

