using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
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
        IConfiguracionProvider configuracion;

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

            return View(servicio.ObtenerCallesPorCentro(centroId).Where(x => x.TipoCalle == TipoCalle.NoGranos ? materialesNoGranos.Any(a => a.Id == x.MaterialId) && x.TipoCalle != Dominio.Enums.TipoCalle.PlayaInterna : x.TipoCalle != Dominio.Enums.TipoCalle.PlayaInterna).ToList());
        }

        public JsonResult EstadoDeCalle()
        {
            var centro = ClaimsPrincipal.Current.GetUserClaim("CentroId");
            var centroId = int.Parse(centro.Value);
            var camiones = servicio.ObtenerEstadoDeCalle();
            var calles = servicio.ObtenerCallesPorCentro(centroId).Where(x => x.TipoCalle != Dominio.Enums.TipoCalle.PlayaInterna);
            var materiales = camiones.Where(x => x.TipoCalle != TipoCalle.NoGranos).Select(x => new { x.MaterialId, x.MaterialDesc })
                .Union(calles.Where(x => x.TipoCalle != TipoCalle.NoGranos).Select(x => new { x.MaterialId, x.MaterialDesc }))
                .GroupBy(x => x).Select(x => x.Key).Where(x => x.MaterialId != 0).OrderBy(x=>x.MaterialId);

            return Json(new { estado = camiones, materiales, calles }, JsonRequestBehavior.AllowGet);            
        }

        [Autorizacion(PermisosScato.EstadoDeCalleLlamar)]
        public JsonResult LlamarCalle(int calleId, int calleCaladoId)
        {
            try
            {
                var calle = servicio.ObtenerCalle(calleId);
                calle.Bloqueada = true;
                calle.FechaLLamada = DateTime.Now;
                if(calleCaladoId > 0 && (calle.TipoCalle == TipoCalle.PreCalado || calle.TipoCalle == TipoCalle.Circular))
                {
                    calle.CalleCaladoId = calleCaladoId;
                }
                servicioComandos.Ejecutar(new ModificarCalle { Dto = calle });
                EnviarMensajeLlamadoACartel(calle, calleCaladoId);
            } catch (Exception e)
            {
                log.Error(e, $"No se pudo llamar la calle {calleId}");
            }
            return Json("ok", JsonRequestBehavior.AllowGet);
        }

        [Autorizacion(PermisosScato.EstadoDeCalleLlamar)]
        public JsonResult LLamarSiguienteCalle(int materialId)
        {
            var calle = servicio.ObtenerSiguienteCalle(materialId);
            if(calle != null)
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
            if(!string.IsNullOrEmpty(motivo))
                servicioComandos.Ejecutar(new CrearLogCambioDeModalidadCalle { Dto = new LogCambioDeModalidadCalleDto { Motivo = motivo, CalleId = calleId , Usuario = usuario, Activado = activar } });
            return Json(new { Automatica = activar, MaterialId = materialId }, JsonRequestBehavior.AllowGet);
        }

        [Autorizacion(PermisosScato.EstadoDeCalleCancelar)]
        public JsonResult CancelarLlamarCalle(int calleId)
        {
            var calle = servicio.ObtenerCalle(calleId);
            calle.Bloqueada = false;
            calle.FechaLLamada = null;
            servicioComandos.Ejecutar(new ModificarCalle { Dto = calle });
            if(calle.TipoCalle != TipoCalle.Circular)
            {
                servicioComandos.Ejecutar(new MarcarUltimaCallePorRecorrido { CalleId = calleId });
            }
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
            var caracteristicasAnalizadas = model.CaladoId.HasValue ? servicio.ListarCaladoPorCaracteristicas(model.CaladoId.Value): null; 
            List<Dominio.Dto.CalleDto> calles = null;

            if (model.TipoCalidad == TipoCalidad.Otros || model.TipoCalidad == TipoCalidad.PendientesPostCalado)
                {
                    calles = servicio.ObtenerCallesPorCentro(centroId).Where(x => model.Rechazado
                    ? x.TipoCalle == TipoCalle.RechazadosDemorados && !x.Deshabilitada
                    : (x.TipoCalle == TipoCalle.PostCalado && x.TipoCalidad == TipoCalidad.Otros && model.MaterialId == x.MaterialId && !x.Deshabilitada && (caracteristicasAnalizadas.Any(ca => ca.CaracteristicaId == x.CaracteristicaDeCalidadId && ca.ValorCalado <= x.RangoCaracteristicaCalidadMaximo && ca.ValorCalado >= x.RangoCaracteristicaCalidadMinimo)))
                        ).ToList();
                }
                if (calles == null || calles.Count() == 0 )
                {
                    List<Dominio.Dto.CalleDto> callesVacias = servicio.ObtenerCallesPorCentro(centroId).Where(x => x.TipoCalle == TipoCalle.PostCalado && x.TipoCalidad != TipoCalidad.PendientesPostCalado && !x.Deshabilitada && x.Id != calleId && x.TipoCalidad != TipoCalidad.Otros && servicio.ListarTodasLasCallesPorRecorrido(x.Id).Count() == 0).ToList();

                     var  callesConCamionesConMismaCalidad = model.Rechazado 
                          ? servicio.ObtenerCallesPorCentro(centroId).Where(x => x.TipoCalle == TipoCalle.RechazadosDemorados && !x.Deshabilitada).ToList() 
                          : servicio.ObtenerCallesDeCallesPorRecorridoSegunMaterial(model.MaterialId, calle.Id, model.CalidadCamion).ToList().FindAll(c => servicio.ListarTodasLasCallesPorRecorrido(c.Id).Count() < c.CantidadDeCamiones);
                     if((callesVacias != null || callesVacias.Count() > 0) && (callesConCamionesConMismaCalidad != null || callesConCamionesConMismaCalidad.Count() > 0) && !model.Rechazado)
                      {
                          calles = callesConCamionesConMismaCalidad.Concat(callesVacias).ToList();
                      }
                }

                if (calles == null || calles.Count() == 0)
                {
                    calles = servicio.ObtenerCallesPorCentro(centroId).Where(x => model.Rechazado ? x.TipoCalle == TipoCalle.RechazadosDemorados && !x.Deshabilitada : x.TipoCalle == TipoCalle.PostCalado && x.TipoCalidad != TipoCalidad.PendientesPostCalado && !x.Deshabilitada && x.Id != calleId && x.TipoCalidad != TipoCalidad.Otros && servicio.ListarTodasLasCallesPorRecorrido(x.Id).Count() == 0).ToList();
                }
            var callesDisponibles = calles.FindAll(c => !c.Id.Equals(calleId) && servicio.ListarTodasLasCallesPorRecorrido(c.Id).Count() < c.CantidadDeCamiones );
            ViewBag.CallesPostCalado = callesDisponibles.Select(x => new SelectListItem { Selected = x.Id == calle.Id, Text = x.Nombre, Value = x.Id.ToString() }).Distinct(new SelectListItemComparable());
            
            return PartialView("_MoverCamionRechazado", model);
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

        private void EnviarMensajeLlamadoACartel(CalleDto callePrecalado, int calleCaladoId)
        {
            try
            {
                var cartel = servicio.ObtenerConfiguracionGeneral("EstadoDeCallePreCalado", "CartelLedCalador").Valor;
                var mensajeCartel = servicio.ObtenerMensajeCartelLedPorCodigo(CodigoMensajeCartelLed.LlamadoCallePrecaladoACalar);
                var calleCalado = servicio.ObtenerCalle(calleCaladoId);
                if(calleCalado != null && mensajeCartel != null && !string.IsNullOrEmpty(cartel))
                {
                    var callesDeCalador = servicio.ObtenerConfiguracionGeneral("EstadoDeCallePreCalado", calleCalado.Nombre);
                    if(callesDeCalador != null && !string.IsNullOrEmpty(callesDeCalador.Valor))
                    {
                        servicioComandos.Ejecutar(new EnviarMensajeCarteLed
                        {
                            Mensaje = $"{callePrecalado.Nombre} {mensajeCartel.Mensaje} {callesDeCalador.Valor}",
                            Codigo = cartel,
                            NumeroPrograma = mensajeCartel.Programa,
                            NumeroTrama = mensajeCartel.Trama,
                            NumeroVariable = mensajeCartel.Variable,
                            SegundosDeEspera = mensajeCartel.SegundosDeEspera
                        });
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e, "No se pudo mostrar el mensaje en Cartel Led");
            }
        }
    }
}
class SelectListItemComparable : IEqualityComparer<SelectListItem>
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
