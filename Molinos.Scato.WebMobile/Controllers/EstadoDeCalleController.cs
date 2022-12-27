using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.WebMobile.Atributos;
using Molinos.Scato.WebMobile.Helpers;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;

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
            ViewBag.MinutosEsperaCircular = centroDto?.MinutosEsperaCircular ?? 20;
            ViewBag.MinutosEsperaPrecalado = centroDto?.MinutosEsperaPrecalado ?? 30;

            var limiteFilasPrecaladoLlamadas = this.servicio.ObtenerConfiguracionGeneral("EstadoDeCallePreCalado", "LimiteFilasLlamadas").Valor;
            ViewBag.LimiteFilasPrecaladoLlamadas = limiteFilasPrecaladoLlamadas != null ? int.Parse(limiteFilasPrecaladoLlamadas) : 3;

            return View(servicio.ObtenerCallesPorCentro(centroId).Where(x => x.TipoCalle == TipoCalle.NoGranos
                        ? materialesNoGranos.Any(a => a.Id == x.MaterialId)
                                && x.TipoCalle != TipoCalle.PlayaInterna
                                && x.TipoCalle != TipoCalle.EnTransito
                                && x.TipoCalle != TipoCalle.PlantaNoGranos
                        : x.TipoCalle != TipoCalle.PlayaInterna
                                && x.TipoCalle != TipoCalle.EnTransito
                                && x.TipoCalle != TipoCalle.PlantaNoGranos).ToList());
        }

        public JsonResult EstadoDeCalle()
        {
            var centro = ClaimsPrincipal.Current.GetUserClaim("CentroId");
            var centroId = int.Parse(centro.Value);
            var camiones = servicio.ObtenerEstadoDeCalle();
            var calles = servicio.ObtenerCallesPorCentro(centroId).Where(x => x.TipoCalle != TipoCalle.PlayaInterna
                                                                                && x.TipoCalle != TipoCalle.PlantaNoGranos
                                                                                && x.TipoCalle != TipoCalle.EnTransito);
            var materiales = camiones.Where(x => x.TipoCalle != TipoCalle.NoGranos
                                                    && x.TipoCalle != TipoCalle.EnTransito
                                                    && x.TipoCalle != TipoCalle.PlantaNoGranos)
                .Select(x => new { x.MaterialId, x.MaterialDesc })
                .Union(calles.Where(x => x.TipoCalle != TipoCalle.NoGranos
                                                    && x.TipoCalle != TipoCalle.EnTransito
                                                    && x.TipoCalle != TipoCalle.PlantaNoGranos)
                .Select(x => new { x.MaterialId, x.MaterialDesc }))
                .GroupBy(x => x).Select(x => x.Key).Where(x => x.MaterialId != 0)
                .OrderBy(x => x.MaterialId);

            return Json(new { estado = camiones, materiales, calles }, JsonRequestBehavior.AllowGet);
        }

        [Autorizacion(PermisosScato.EstadoDeCalleLlamar)]
        public JsonResult LlamarCalle(int calleId, int calleCaladoId)
        {
            var response = new MensajeEstandarDto();
            try
            {
                var calle = servicio.ObtenerCalle(calleId);
                response = LlamadasPrecaladoLimiteFilas(calle);
                if (response.TipoDeMensaje != TipoDeMensajeDeRespuesta.Error)
                {
                    calle.Bloqueada = true;
                    calle.FechaLLamada = DateTime.Now;
                    if (calleCaladoId > 0 && (calle.TipoCalle == TipoCalle.PreCalado || calle.TipoCalle == TipoCalle.Circular))
                    {
                        calle.CalleCaladoId = calleCaladoId;
                    }
                    servicioComandos.Ejecutar(new ModificarCalle { Dto = calle, Llamada = true });
                    EnviarMensajeLlamadoACartel(calle, calleCaladoId);
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
                servicioComandos.Ejecutar(new ModificarCalle { Dto = calle });
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
            servicioComandos.Ejecutar(new ModificarCalle { Dto = calle });
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
            List<Dominio.Dto.CalleDto> calles = null;

            if (model.TipoCalidad == TipoCalidad.Otros || model.TipoCalidad == TipoCalidad.PendientesPostCalado)
            {
                calles = servicio.ObtenerCallesPorCentro(centroId).Where(x => model.Rechazado
                ? x.TipoCalle == TipoCalle.RechazadosDemorados && !x.Deshabilitada
                : (x.TipoCalle == TipoCalle.PostCalado && x.TipoCalidad == TipoCalidad.Otros && model.MaterialId == x.MaterialId && !x.Deshabilitada && (caracteristicasAnalizadas.Any(ca => ca.CaracteristicaId == x.CaracteristicaDeCalidadId && ca.ValorCalado <= x.RangoCaracteristicaCalidadMaximo && ca.ValorCalado >= x.RangoCaracteristicaCalidadMinimo)))
                    ).ToList();
            }
            if (calles == null || calles.Count() == 0)
            {
                List<Dominio.Dto.CalleDto> callesVacias = servicio.ObtenerCallesPorCentro(centroId).Where(x => x.TipoCalle == TipoCalle.PostCalado && x.TipoCalidad != TipoCalidad.PendientesPostCalado && !x.Deshabilitada && x.Id != calleId && x.TipoCalidad != TipoCalidad.Otros && servicio.ListarCallePorRecorridoPorCalleId(x.Id).Count() == 0).ToList();

                var callesConCamionesConMismaCalidad = model.Rechazado
                     ? servicio.ObtenerCallesPorCentro(centroId).Where(x => x.TipoCalle == TipoCalle.RechazadosDemorados && !x.Deshabilitada).ToList()
                     : servicio.ObtenerCallesDeCallesPorRecorridoSegunMaterial(model.MaterialId, calle.Id, model.CalidadCamion).ToList().FindAll(c => servicio.ListarCallePorRecorridoPorCalleId(c.Id).Count() < c.CantidadDeCamiones);
                if ((callesVacias != null || callesVacias.Count() > 0) && (callesConCamionesConMismaCalidad != null || callesConCamionesConMismaCalidad.Count() > 0) && !model.Rechazado)
                {
                    calles = callesConCamionesConMismaCalidad.Concat(callesVacias).ToList();
                }
            }

            if (calles == null || calles.Count() == 0)
            {
                calles = servicio.ObtenerCallesPorCentro(centroId).Where(x => model.Rechazado ? x.TipoCalle == TipoCalle.RechazadosDemorados && !x.Deshabilitada : x.TipoCalle == TipoCalle.PostCalado && x.TipoCalidad != TipoCalidad.PendientesPostCalado && !x.Deshabilitada && x.Id != calleId && x.TipoCalidad != TipoCalidad.Otros && servicio.ListarCallePorRecorridoPorCalleId(x.Id).Count() == 0).ToList();
            }
            var callesDisponibles = calles.FindAll(c => !c.Id.Equals(calleId) && servicio.ListarCallePorRecorridoPorCalleId(c.Id).Count() < c.CantidadDeCamiones);
            ViewBag.CallesPostCalado = callesDisponibles.Where(x => !x.Bloqueada).Select(x => new SelectListItem { Selected = x.Id == calle.Id, Text = x.Nombre, Value = x.Id.ToString() }).Distinct(new SelectListItemComparable());

            return PartialView("_MoverCamionRechazado", model);
        }

        [Autorizacion(PermisosScato.EstadoDeCalleLlamar)]
        public JsonResult LlamarCallePostCalado(int calleId)
        {
            try
            {
                var calle = servicio.ObtenerCalle(calleId);
                calle.Bloqueada = true;
                calle.FechaLLamada = DateTime.Now;

                servicioComandos.Ejecutar(new ModificarCalle { Dto = calle });
                EnviarMensajeLlamadoACartelPostCalado(calle);
            }
            catch (Exception e)
            {
                log.Error(e, $"No se pudo llamar la calle {calleId}");
            }
            return Json("ok", JsonRequestBehavior.AllowGet);
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

        public JsonResult ConfirmarReasignacionCalle(Guid instanciaWorflow, int calleId)
        {
            var result = servicioComandos.Ejecutar(
               new ReasignarCamionPostcalado
               {
                   InstanciaWorkflow = instanciaWorflow,
                   CalleId = calleId
               });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private CantidadPrecaladoCircularHelper EstadoDeCallesBloqueada()
        {
            var cantBloqueadas = servicio.ContarCallesBloqueadas();
            return cantBloqueadas;
        }

        private void EnviarMensajeLlamadoACartel(CalleDto callePrecalado, int calleCaladoId)
        {
            try
            {
                var cartel = servicio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.EstadoDeCallePreCalado, Constantes.ConfiguracionGeneral.PreCalado.CartelLedCalador);

                var esCircular = callePrecalado.TipoCalle == TipoCalle.Circular;

                var historialMensajeCartel = new HistorialMensajeCartelLedDto();

                var codigo = servicio.ObtenerCodigoMensaje(calleCaladoId);

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

                    servicioComandos.Ejecutar(new EnviarMensajeCartelLed
                    {
                        Mensaje = resultadoInsertarCartelLed.Mensaje,
                        Codigo = cartel?.Valor,
                        NumeroTrama = resultadoInsertarCartelLed.NumeroTrama,
                        NumeroPrograma = resultadoInsertarCartelLed.NumeroPrograma,
                        NumeroVariable = resultadoInsertarCartelLed.NumeroVariable,
                    });
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
                        break;
                    }
            }
        }

        private MensajeEstandarDto LlamadasPrecaladoLimiteFilas(CalleDto calle)
        {
            var mensaje = new MensajeEstandarDto() { TipoDeMensaje = TipoDeMensajeDeRespuesta.Success };
            var cantBloqueadas = EstadoDeCallesBloqueada();
            int.TryParse(servicio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.EstadoDeCallePreCalado, Constantes.ConfiguracionGeneral.PreCalado.LimiteFilasLlamadas).Valor, out int limiteDeFilasTotales);
            switch (calle.TipoCalle)
            {
                case TipoCalle.PreCalado:
                    {
                        if (cantBloqueadas.CantidadTotal == limiteDeFilasTotales)
                        {
                            mensaje = new MensajeEstandarDto
                            {
                                TipoDeMensaje = TipoDeMensajeDeRespuesta.Error,
                                //limite de filas totales
                                Mensaje = "1"
                            };
                        }

                        break;
                    }

                case TipoCalle.Circular:
                    {
                        if (cantBloqueadas.CantidadTotal == limiteDeFilasTotales)
                        {
                            mensaje = new MensajeEstandarDto
                            {
                                TipoDeMensaje = TipoDeMensajeDeRespuesta.Error,
                                //limite de filas totales
                                Mensaje = "1"
                            };
                        }

                        //material de soja reserva dos slots del cartel para circular
                        if (calle.MaterialId == 4 && cantBloqueadas.CantidadCircular == limiteDeFilasTotales - 4)
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
    }
}

internal class SelectListItemComparable : IEqualityComparer<SelectListItem>
{
    public bool Equals(SelectListItem x, SelectListItem y)
    {
        return x.Value.Equals(y.Value);
    }

    public int GetHashCode(SelectListItem obj)
    {
        return obj.Value.GetHashCode();
    }
}