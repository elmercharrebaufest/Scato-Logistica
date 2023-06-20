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
            var materialPasoDirecto = servicio.ObtenerConfiguracionGeneral("EstadoPlayaInterna", "PaseDirecto");
            ViewBag.ConfiguracionSwitch = ObtenerConfiguracionPasoDrecto(materialPasoDirecto.Valor);
            ViewBag.IdConfiguracion = materialPasoDirecto.Id;

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

        public JsonResult ObtenerCalles(string tiposCalleStr)
        {
            if (string.IsNullOrEmpty(tiposCalleStr))
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
            if (model.RecorridoId != null)
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

                if (!resultado.HayErrores)
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
        public JsonResult GuardarPasoDirecto(int callePaseDirecto, int materialPaseDirecto, int idConfiguracion)
        {
            var response = new RespuestaEstandarDto();

            try
            {
                var usuario = ClaimsPrincipal.Current.GetUserClaim(ClaimTypes.NameIdentifier);
                var calles = ObtenerTiposDeCallesPlanta().Where(c => c.TipoCalle == TipoCalle.PreBalanzaGranos).Select(s => s.Calles).ToList();
                var sonPaseDirecto = calles[0].Where(c => c.EsPasoDirecto);

                if (sonPaseDirecto.Count() > 0)
                {
                    sonPaseDirecto.ForEach(i =>
                    {
                        var calleAnterior = ObtenerTiposDeCallesPlantaPorId(callePaseDirecto);
                        servicioComandos.Ejecutar(new ModificarCalle
                        {
                            Dto = new CalleDto
                            {
                                Id = calleAnterior.Id,
                                EsPasoDirecto = calleAnterior.EsPasoDirecto,
                                MaterialId = calleAnterior.MaterialId,
                                Nombre = calleAnterior.Nombre,
                                Codigo = calleAnterior.Codigo,
                                CentroId = calleAnterior.CentroId,
                                TipoCalle = calleAnterior.TipoCalle
                            },
                            Llamada = false,
                            Usuario = usuario.Value
                        });
                    });
                }

                ConfiguracionGeneralDto configuracionDto = new ConfiguracionGeneralDto();
                if (callePaseDirecto != 0)
                {
                    var calle = ObtenerTiposDeCallesPlantaPorId(callePaseDirecto);

                    servicioComandos.Ejecutar(new ModificarCalle
                    {
                        Dto = new CalleDto
                        {
                            Id = callePaseDirecto,
                            EsPasoDirecto = materialPaseDirecto == 0 ? false : true,
                            MaterialId = materialPaseDirecto,
                            Nombre = calle.Nombre,
                            Codigo = calle.Codigo,
                            CentroId = calle.CentroId,
                            TipoCalle = calle.TipoCalle
                        },
                        Llamada = false,
                        Usuario = usuario.Value
                    });

                    configuracionDto = new ConfiguracionGeneralDto
                    {
                        CentroId = calle.CentroId,
                        UsuarioUltimaModificacion = usuario.Value,
                        Valor = materialPaseDirecto.ToString(),
                        Id = idConfiguracion
                    };
                }
                else
                {
                    configuracionDto = new ConfiguracionGeneralDto
                    {
                        UsuarioUltimaModificacion = usuario.Value,
                        Valor = "0",
                        Id = idConfiguracion
                    };

                    response.Mensajes.Add(new MensajeEstandarDto { Mensaje = $"No se puede asignar un material sin seleccionar una fila", TipoDeMensaje = TipoDeMensajeDeRespuesta.Warning });
                }

                servicioComandos.Ejecutar(new ModificarConfiguracionGeneral
                {
                    Dto = configuracionDto
                });
            }
            catch (Exception ex)
            {
                var errorMessage = $"Error al guardar la configuracion";
                response.Mensajes.Add(new MensajeEstandarDto { Mensaje = errorMessage, TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
            }

            return Json(response);
        }

        private List<Tuple<int, string, bool>> ObtenerConfiguracionPasoDrecto(string materialPaseDirecto)
        {
            var idMaterialPasoDirecto = servicio.ObtenerConfiguracionGeneral("EstadoPlayaInterna", "MaterialesPaseDirecto");
            List<Tuple<int, string, bool>> configuracionPaseDirecto = new List<Tuple<int, string, bool>>();
            int idMaterialPaseDirecto = Convert.ToInt16(materialPaseDirecto);
            idMaterialPasoDirecto.Valor.Split(',').ForEach(i =>
            {
                int id = Convert.ToInt32(i);
                var material = servicio.ObtenerMaterial(id);
                configuracionPaseDirecto.Add(new Tuple<int, string, bool>(id, material.DescripcionCorta != null ? material.DescripcionCorta.ToUpper() : string.Empty, material.Id.Equals(idMaterialPaseDirecto)));
            });
            configuracionPaseDirecto.Add(new Tuple<int, string, bool>(0, "NINGUNO", idMaterialPaseDirecto.Equals(0)));
            return configuracionPaseDirecto;
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
                if (calle.TipoCalle == TipoCalle.PreBalanzaGranos && calle.Bloqueada && callePlanta.Camiones.Count == 0)
                {
                    DesbloquearCalle(calle.Id);
                }
            }
            return tipoCallePlantaLista;
        }

        private CalleDto ObtenerTiposDeCallesPlantaPorId(int idCalle)
        {
            var centro = ClaimsPrincipal.Current.GetUserClaim("CentroId");
            var centroId = int.Parse(centro.Value);
            var calles = servicio.ObtenerCallesPorCentro(centroId).Where(x => listaCalles.Contains(x.TipoCalle) && !x.Deshabilitada).OrderBy(x => x.Posicion).ToList();

            CalleDto callePorId = calles.Where(c => c.Id == idCalle).First();

            return callePorId;
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
    }
}