using Molinos.Scato.Dominio;
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
using System.Text;
using System.Web.Mvc;
using WebGrease.Css.Extensions;
using static Molinos.Scato.Dominio.Constantes;

namespace Molinos.Scato.WebMobile.Controllers
{
    [Autorizacion(PermisosScato.EstadoDeCallePlayaInterna)]
    public class EstadoPlayaInternaController : ConsultasController
    {
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioRepositorio servicio;
        private readonly ILogger log;
        private IConfiguracionProvider configuracion;
        private List<TipoCalle> listaCalles;

        public EstadoPlayaInternaController(
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
            this.listaCalles = new List<TipoCalle> {
                TipoCalle.PlayaInterna, 
                TipoCalle.PlantaNoGranos, 
                TipoCalle.EnTransito,
                TipoCalle.SalidaNoGranos,
                TipoCalle.EsperaAduanaNoGranos,
                TipoCalle.EnTransitoGranos,
                TipoCalle.PreBalanzaGranos,
                TipoCalle.SalidaGranos
            };
        }

        public ActionResult Index()

        {
            var tipoCallePlantaLista = ObtenerTiposDeCallesPlanta();
            var configuraciones = ObtenerConfiguracionSwitch().Valor.Split(',');
            ViewBag.ConfiguracionSwitch = configuraciones;
            ViewBag.IdConfiguracion = ObtenerConfiguracionSwitch().Id;
            return View(tipoCallePlantaLista);
        }

        public JsonResult EstadoDeCalle()
        {
            var centro = ClaimsPrincipal.Current.GetUserClaim("CentroId");
            var centroId = int.Parse(centro.Value);
            var camiones = servicio.ObtenerEstadoDeCalle();
            camiones.ForEach(camion =>
            {

                if (camion.EsSojaIMPO)
                {
                    camion.ColorFondo = ValoresPorDefecto.ColorFondoSojaIMPO;
                    camion.ColorTexto = ValoresPorDefecto.ColorTextoSojaIMPO;
                }

            });
            var calles = servicio.ObtenerCallesPorCentro(centroId).Where(x => listaCalles.Contains(x.TipoCalle));
            var materiales = camiones.Where(x => listaCalles.Contains(x.TipoCalle))
                .Select(x => new { x.MaterialId, x.MaterialDesc })
                .GroupBy(x => x).Select(x => x.Key).Where(x => x.MaterialId != 0);

            return Json(new { estado = camiones, materiales, calles }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EstadoSwitch()
        {
            var centro = ClaimsPrincipal.Current.GetUserClaim("CentroId");
            var centroId = int.Parse(centro.Value);
            var camiones = servicio.ObtenerEstadoDeCalle();
            var calles = servicio.ObtenerCallesPorCentro(centroId).Where(x => listaCalles.Contains(x.TipoCalle));
            var materiales = camiones.Where(x => listaCalles.Contains(x.TipoCalle))
                .Select(x => new { x.MaterialId, x.MaterialDesc })
                .GroupBy(x => x).Select(x => x.Key).Where(x => x.MaterialId != 0);

            return Json(new { estado = camiones, materiales, calles }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerCalles(string tiposCalleStr)
        {
            if(string.IsNullOrEmpty(tiposCalleStr))
                return Json(new List<TipoCallePlantaDto>(), JsonRequestBehavior.AllowGet);

            var tiposCalle = new List<TipoCalle>();
            var arrTipoCallesStr = tiposCalleStr.Split(',');
            foreach (var tipoCalleStr in arrTipoCallesStr)
            {
                var tipoCalleInt = int.Parse(tipoCalleStr);
                tiposCalle.Add((TipoCalle)tipoCalleInt);
            }
            var tipoCallePlantaLista = ObtenerTiposDeCallesPlanta();
            return Json(tipoCallePlantaLista.Where(x => tiposCalle.Contains(x.TipoCalle)).ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult MostrarDetalleCamion(string patente, int calleId)
        {
            var model = servicio.ObtenerInfoPatente(patente, calleId);
            if(model.RecorridoId != null)
            {
                var usuario = ClaimsPrincipal.Current.GetUserClaim(ClaimTypes.NameIdentifier);
                model.CorrespondeConfirmarCargaDescarga = servicio.ExisteConfirmacionCargaDescargaDeRecorrido(model.RecorridoId ?? 0) && servicio.TienePermiso(usuario.Value, PermisosScato.ConfirmacionCargaDescarga);
            }
            return PartialView("_DetalleCamion", model);
        }

        [HttpPost]
        public ActionResult ConfirmarCargaDescarga(Guid workflowInstance, int recorridoId)
        {
            var response = new RespuestaEstandarDto();
            try
            {
                var usuario = ClaimsPrincipal.Current.GetUserClaim(ClaimTypes.NameIdentifier);
                var confirmacion = new ConfirmacionCargaDescargaDto()
                {
                    RecorridoId = recorridoId,
                    FechaConfirmacion = DateTime.Now,
                    Confirmado = true,
                    PendienteConfirmacion = false,
                    NombreUsuario = usuario.Value
                };
                var resultado = servicioComandos.Ejecutar(new ActualizarConfirmacionCargaDescarga
                {
                    Dto = confirmacion
                });

                if(!resultado.HayErrores)
                {
                    var controlRecorrido = new ControlRecorridoDto()
                    {
                        WorkflowInstanceId = workflowInstance,
                        Actividad = EtapaWorkflow.ConfirmacionCargaDescarga,
                        ActividadXaml = EtapaWorkflow.ConfirmacionCargaDescarga,
                        NombreUsuario = usuario.Value
                    };
                    resultado = servicioComandos.Ejecutar(new CrearControlRecorrido
                    {
                        Dto = controlRecorrido
                    });
                }

                if (resultado.HayErrores)
                {
                    foreach (var item in resultado.Errores)
                    {
                        response.Mensajes.Add(new MensajeEstandarDto { Mensaje = item.Value, TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                    }
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"Error al confirmar Carga/Descarga para el workflow {workflowInstance}";
                response.Mensajes.Add(new MensajeEstandarDto { Mensaje = errorMessage, TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
            }
            return Json(response, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult GuardarEstadoSwitch(int id , bool configuracionSoja , bool configuracionMaiz , bool configuracionTrigo , bool configuracionGirasol)
        {
            var response = new RespuestaEstandarDto();
            try
            {
                var centro = ClaimsPrincipal.Current.GetUserClaim("CentroId");
                var centroId = int.Parse(centro.Value);
                var usuario = ClaimsPrincipal.Current.GetUserClaim(ClaimTypes.NameIdentifier);
                
                servicioComandos.Ejecutar(new ModificarConfiguracionGeneral
                {
                    Dto = new ConfiguracionGeneralDto
                    {
                        CentroId = centroId,
                        UsuarioUltimaModificacion = usuario.Value,
                        Valor = GenerarValorConfiguracion(configuracionSoja, configuracionMaiz, configuracionTrigo, configuracionGirasol),
                        Id= id
                    }
                });


            }
            catch (Exception ex)
            {
                var errorMessage = $"Error al guardar la configuracion";
                response.Mensajes.Add(new MensajeEstandarDto { Mensaje = errorMessage, TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
            }
            return Json(response, JsonRequestBehavior.AllowGet);

        }

        private ConfiguracionGeneralDto ObtenerConfiguracionSwitch()
        {
              return servicio.ObtenerConfiguracionGeneral("EstadoPlayaInterna" , "PaseDirecto"); 
        }

        private List<TipoCallePlantaDto> ObtenerTiposDeCallesPlanta()
        {
            var centro = ClaimsPrincipal.Current.GetUserClaim("CentroId");
            
            var centroId = int.Parse(centro.Value);
            var camiones = servicio.ObtenerEstadoDeCalle();
            camiones.ForEach(camion =>
            {
                if (camion.EsSojaIMPO)
                {
                    camion.ColorFondo = ValoresPorDefecto.ColorFondoSojaIMPO;
                    camion.ColorTexto = ValoresPorDefecto.ColorTextoSojaIMPO;
                }

            });
            var calles = servicio.ObtenerCallesPorCentro(centroId).Where(x => listaCalles.Contains(x.TipoCalle) && !x.Deshabilitada).OrderBy(x => x.Posicion).ToList();
            var tipoCallePlantaLista = new List<TipoCallePlantaDto>();

            foreach (var calle in calles)
            {
                var tipoCallePlanta = tipoCallePlantaLista.FirstOrDefault(q => q.TipoCalle == calle.TipoCalle);
                if (tipoCallePlanta == null)
                {
                    tipoCallePlanta = new TipoCallePlantaDto
                    {
                        TipoCalle = calle.TipoCalle
                    };
                    tipoCallePlantaLista.Add(tipoCallePlanta);
                }
                var callePlanta = new CallePlantaDto
                {
                    CalleId = calle.Id,
                    CalleDesc = calle.Nombre,
                    ColorFondo = calle.ColorFondo ?? "#000",
                    ColorTexto = calle.ColorTexto ?? "#fff",
                    LimiteDeCamiones = calle.CantidadDeCamiones,
                    Bloqueada = calle.Bloqueada,
                    EsPrimero = false,
                    EsUltimo = false,
                    EsPasoDirecto = calle.EsPasoDirecto
                };
                tipoCallePlanta.Calles.Add(callePlanta);

                foreach (var camion in camiones.Where(q => q.CalleId == calle.Id))
                {
                    var camionPlanta = new CamionPlantaDto
                    {
                        CamionId = camion.Id,
                        Patente = camion.Patente,
                        Escalable = camion.Escalable,
                        UltimoDeLaFila = camion.UltimoDeLaFila,
                        ColorFondo = camion.ColorFondo ?? "#000",
                        ColorTexto = camion.ColorTexto ?? "#fff",
                        CalleId = camion.CalleId,
                        Rechazado = camion.Rechazado,
                        FechaIngreso = camion.FechaIngeso,
                        MaterialId = camion.MaterialId,
                        Calidad = camion.Calidad,
                        EsSojaEPA = camion.EsSojaEPA,
                        EsSojaIMPO = camion.EsSojaIMPO
                       
                    };
                    callePlanta.Camiones.Add(camionPlanta);
                }
                if(calle.TipoCalle == TipoCalle.PreBalanzaGranos && calle.Bloqueada && callePlanta.Camiones.Count == 0)
                {
                    DesbloquearCalle(calle.Id);
                }
            }
            return tipoCallePlantaLista;
        }

        private void DesbloquearCalle(int calleId)
        {
            try
            {
                var calle = servicio.ObtenerCalle(calleId);
                calle.Bloqueada = false;
                calle.FechaLLamada = null;
                servicioComandos.Ejecutar(new ModificarCalle { Dto = calle });
            }
            catch (Exception e)
            {
                log.Error(e, "Error al desbloquear calle PreBalanzaGranos");
            }
        }

        private string GenerarValorConfiguracion(bool configuracionSoja, bool configuracionMaiz, bool configuracionTrigo, bool configuracionGirasol)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(configuracionSoja ? "1," : "0,");
            stringBuilder.Append(configuracionMaiz ? "1," : "0,");
            stringBuilder.Append(configuracionTrigo ? "1," : "0,");
            stringBuilder.Append(configuracionGirasol ? "1" : "0");
            return stringBuilder.ToString();
        }
    }
}