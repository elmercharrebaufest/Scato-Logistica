using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.WebMobile.Atributos;
using Molinos.Scato.WebMobile.Helpers;
using Molinos.Scato.WebMobile.ViewModel;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;

namespace Molinos.Scato.WebMobile.Controllers
{
    [Autorizacion(PermisosScato.SupervisorLogistica, PermisosScato.GaritaIngreso, PermisosScato.PlayeroPlanta, PermisosScato.PuestoComandoLogistica)]
    public class TableroComandoLogisticaController : Controller
    {
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioRepositorio servicio;
        private readonly ILogger log;

        public TableroComandoLogisticaController(IServicioComandos servicioComandos, IServicioRepositorio servicio, ILogger log)
        {
            this.servicioComandos = servicioComandos;
            this.servicio = servicio;
            this.log = log;
        }

        public ActionResult Index()
        {
            AutomatismoGranosViewModel model = new AutomatismoGranosViewModel();
            model.ListaAutomatismoGrano = ListarAutomatismo();
            model.CargarDatos(servicio, ObtenerIdCentro(), new AutomatismoGranoDto());
            return View(model);
        }

        public ActionResult Configuraciones()
        {
            AutomatismoGranosViewModel model = new AutomatismoGranosViewModel();
            model.ListaAutomatismoGrano = ListarAutomatismo();
            model.CargarDatos(servicio, ObtenerIdCentro(), new AutomatismoGranoDto());
            return View(model);
        }

        private ListaPaginada<AutomatismoGranoDto> ListarAutomatismo()
        {
            var automatismos = servicio.ListarAutomatismoGrano();
            return new ListaPaginada<AutomatismoGranoDto>(automatismos, 1, 10, automatismos.Count);
        }

        public ActionResult Eliminar(int id)
        {
            var respuesta = new RespuestaEstandarDto();
            try
            {
                var resultadoAutomatismo = servicioComandos.Ejecutar(new EliminarAutomatismoGranos { IdAutomatismo = id });
                if (!resultadoAutomatismo.HayErrores)
                    respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "Eliminación de automatismo de granos exitosa", TipoDeMensaje = TipoDeMensajeDeRespuesta.Success });
                else
                    respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = string.Join(",", resultadoAutomatismo.Errores.Values), TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
            }
            catch (Exception ex)
            {
                log.Error(ex, $"Error al eliminar automatismo con id {id}");
                respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "Se produjo un error al eliminar el automatismo de granos", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
            }

            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AjaxOnly]
        public ActionResult Crear()
        {
            var model = new AutomatismoGranosViewModel();
            model.CargarDatos(servicio, ObtenerIdCentro(), model.AutomatismoGrano);
            return PartialView("_CrearAutomatismo", model);
        }

        [HttpPost]
        public ActionResult Crear(AutomatismoGranosViewModel model)
        {
            var respuesta = new RespuestaEstandarDto();
            try
            {
                if (ModelState.IsValid)
                {
                    var resultadoAutomatismo = servicioComandos.Ejecutar(new CrearAutomatismoGranos { Dto = model.AutomatismoGrano });
                    if (!resultadoAutomatismo.HayErrores)
                        respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "Creación de automatismo de granos exitosa", TipoDeMensaje = TipoDeMensajeDeRespuesta.Success });
                    else
                        respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = string.Join(",", resultadoAutomatismo.Errores.Values), TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                    ModelState.AgregarErrores(resultadoAutomatismo);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, $"Error al crear automatismo");
                respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "Se produjo un error al crear el automatismo de granos", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
            }

            return Json(respuesta);
        }

        [HttpGet]
        [AjaxOnly]
        public ActionResult Modificar(int id)
        {
            var dto = servicio.ObtenerAutomatismoGranos(id);
            var model = new AutomatismoGranosViewModel();
            model.AutomatismoGrano = dto;
            model.CargarDatos(servicio, ObtenerIdCentro(), dto);
            model.AutomatismoGrano.Hidraulicas = null;
            model.AutomatismoGrano.TipoVariedades = null;
            return PartialView("_ModificarAutomatismo", model);
        }

        [HttpPost]
        public ActionResult Modificar(AutomatismoGranosViewModel model)
        {
            var respuesta = new RespuestaEstandarDto();
            try
            {
                if (ModelState.IsValid)
                {
                    var resultadoAutomatismo = servicioComandos.Ejecutar(new ModificarAutomatismoGranos { Dto = model.AutomatismoGrano });
                    if (!resultadoAutomatismo.HayErrores)
                        respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "Modificación de automatismo de granos exitosa", TipoDeMensaje = TipoDeMensajeDeRespuesta.Success });
                    else
                        respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = string.Join(",", resultadoAutomatismo.Errores.Values), TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                    ModelState.AgregarErrores(resultadoAutomatismo);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, $"Error al modificar automatismo con id {model.AutomatismoGrano.Id}");
                respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "Se produjo un error al modificar el automatismo de granos", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
            }

            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AjaxOnly]
        public ActionResult ModificarConfiguracionCallePB(int id)
        {
            var model = new CalleViewModel();
            var calle = servicio.ObtenerCalle(id);
            model.Id = calle.Id;
            model.Descripcion = calle.Nombre;
            model.Camiones = calle.CantidadDeCamiones;
            return PartialView("_ModificarCallePB", model);
        }

        [HttpPost]
        public ActionResult ModificarConfiguracionCallePB(CalleViewModel model)
        {
            var respuesta = new RespuestaEstandarDto();
            if (ModelState.IsValid)
            {
                var resultadoCallePB = servicioComandos.Ejecutar(new ModificarCallePrebalanza { Dto = new CalleAutomatismoDto { Id = model.Id, Descripcion = model.Descripcion, Camiones = model.Camiones } });

                ModelState.AgregarErrores(resultadoCallePB);

                if (!resultadoCallePB.HayErrores)
                {
                    respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "Modificacion de calle PB de Granos Exitosa", TipoDeMensaje = TipoDeMensajeDeRespuesta.Success });
                }
                else
                {
                    respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "Error al modificar la calle PB de Granos", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                }
            }

            return Json(respuesta);
        }

        [HttpGet]
        [AjaxOnly]
        public ActionResult ModificarConfiguracionCallePH(int id)
        {
            var model = new AutomatismoGranoCallePHViewModel();

            var listaAutomatismoTipoLlamado = servicio.ListarAutomatismoTipoLlamado();
            var selectListAutomatismoTipoLlamado = listaAutomatismoTipoLlamado.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Descripcion
            }).ToList();

            var calle = servicio.ObtenerCalle(id);
            model.Id = calle.Id;
            model.Descripcion = calle.Nombre;
            model.Camiones = calle.CantidadDeCamiones;
            model.AutomatismoTipoLlamadoId = calle.AutomatismoTipoLlamadoId;
            model.ListaAutomatismoTipoLlamado = selectListAutomatismoTipoLlamado;
            return PartialView("_ModificarCallePH", model);
        }

        public ActionResult ModificarConfiguracionCallePH(AutomatismoGranoCallePHViewModel model)
        {
            var respuesta = new RespuestaEstandarDto();
            if (ModelState.IsValid)
            {
                var calleAutomatismo = new CalleAutomatismoDto
                {
                    Id = model.Id,
                    Descripcion = model.Descripcion,
                    Camiones = model.Camiones,
                    AutomatismoTipoLlamadoId = model.AutomatismoTipoLlamadoId
                };

                var resultadoCallePH = servicioComandos.Ejecutar(new ModificarCallePreHidraulica { Dto = calleAutomatismo });

                ModelState.AgregarErrores(resultadoCallePH);

                if (!resultadoCallePH.HayErrores)
                {
                    respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "Modificacion de calle PH de Granos Exitosa", TipoDeMensaje = TipoDeMensajeDeRespuesta.Success });
                }
                else
                {
                    if (resultadoCallePH.Errores.TryGetValue("AutomatismoTipoLlamado", out string mensajeError))
                    {
                        respuesta.Mensajes.Add(new MensajeEstandarDto
                        {
                            Mensaje = mensajeError,
                            TipoDeMensaje = TipoDeMensajeDeRespuesta.Error
                        });
                    }
                    else
                    {
                        respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "Error al modificar la calle PH de Granos", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                    }
                }
            }

            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AjaxOnly]
        public JsonResult ListarVariedades(int materialId)
        {
            var variedades = servicio.ListarTipoVariedadPorMaterial(materialId).Where(c => !c.Borrado).ToList();
            var resultado = variedades.Select(m => new SelectListItem
            {
                Value = m.TipoVariedadId.ToString(),
                Text = m.TipoVariedadDescripcion
            }).ToList();
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AjaxOnly]
        public JsonResult ListarAlmacenesPorMaterial(int materialId)
        {
            var almacenes = servicio.ListarAlmacenesPorMaterial(ObtenerIdCentro(), materialId).OrderBy(c => c.Descripcion).Select(x => new AlmacenDto { Id = x.Id, Descripcion = x.Descripcion });
            var resultado = almacenes.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Descripcion
            }).ToList();
            resultado.Insert(0, new SelectListItem { Value = "", Text = Textos.Default_Almacen });
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ActualizarEstadoAutomatismoGeneral(bool nuevoEstado)
        {
            var jsonResult = new JsonResult { Data = new MensajeEstandarDto(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            var respuesta = new RespuestaEstandarDto();
            var configuracionAutomatismoGrano = servicio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.TableroComandoLogistica, Constantes.ConfiguracionGeneral.LlamadoAutomatico.Granos);

            if (configuracionAutomatismoGrano == null)
            {
                respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "No existe la configuración de Llamado Automatico de Granos", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                jsonResult.Data = respuesta;

                return jsonResult;
            }

            configuracionAutomatismoGrano.Valor = nuevoEstado.ToString();
            var resultado = servicioComandos.Ejecutar(new ModificarConfiguracionGeneral
            {
                Dto = configuracionAutomatismoGrano
            });

            if (!resultado.HayErrores)
            {
                respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "Configuracion de Llamado Automatico de Granos Exitosa", TipoDeMensaje = TipoDeMensajeDeRespuesta.Success });
                if (!nuevoEstado)
                {
                    servicioComandos.Ejecutar(new ActualizarAutomatismoGranosEstado
                    {
                        Estado = false
                    });
                }
                jsonResult.Data = respuesta;
            }

            return jsonResult;
        }

        public ActionResult ActualizarEstadoAutomatismoLlamadoPreBalanza(bool nuevoEstado)
        {
            var jsonResult = new JsonResult { Data = new MensajeEstandarDto(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            var respuesta = new RespuestaEstandarDto();
            var configuracionAutomatismoGrano = servicio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.TableroComandoLogistica, Constantes.ConfiguracionGeneral.LlamadoAutomatico.PreBalanza);

            if (configuracionAutomatismoGrano == null)
            {
                respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "No existe la configuración de Llamado Automatico de prebalanza", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                jsonResult.Data = respuesta;

                return jsonResult;
            }

            configuracionAutomatismoGrano.Valor = nuevoEstado.ToString();
            var resultado = servicioComandos.Ejecutar(new ModificarConfiguracionGeneral
            {
                Dto = configuracionAutomatismoGrano
            });

            if (!resultado.HayErrores)
            {
                respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "Configuracion de Llamado Automatico de prebalanza exitosa", TipoDeMensaje = TipoDeMensajeDeRespuesta.Success });
                jsonResult.Data = respuesta;
            }

            return jsonResult;
        }

        public ActionResult ActualizarEstadoLlamadoVolcable(int id, bool valor)
        {
            var jsonResult = new JsonResult { Data = new MensajeEstandarDto(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            var respuesta = new RespuestaEstandarDto();
            var configuracionAutomatismoGrano = servicio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.TableroComandoLogistica, Constantes.ConfiguracionGeneral.LlamadoAutomatico.Granos);

            if (configuracionAutomatismoGrano.Valor.ToString() == "False")
            {
                respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "No se Puede Procesar. Debe habilitar primero el llamado Volcable", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                jsonResult.Data = respuesta;
                return jsonResult;
            }

            var resultadoModificarLlamadoVolcableAutomatismo = servicioComandos.Ejecutar(new ModificarLlamadoVolcableAutomatismoGrano
            {
                Id = id,
                EsLLamadoVolcable = valor
            });

            if (!resultadoModificarLlamadoVolcableAutomatismo.HayErrores)
            {
                respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "Actualización del estado de Llamado Volcable fue exitoso", TipoDeMensaje = TipoDeMensajeDeRespuesta.Success });
                jsonResult.Data = respuesta;
            }
            else if (resultadoModificarLlamadoVolcableAutomatismo.HayErrores)
            {
                respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = string.Join(",", resultadoModificarLlamadoVolcableAutomatismo.Errores.Values), TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                jsonResult.Data = respuesta;
            }

            return jsonResult;
        }

        public ActionResult ModificarEstadoCallePB(int id, bool valor)
        {
            var respuesta = new RespuestaEstandarDto();

            var resultado = servicioComandos.Ejecutar(new ModificarEstadoCallePrebalanza
            {
                Id = id,
                ActivoAutomatico = valor
            });

            if (!resultado.HayErrores)
            {
                respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "Actualización del estado de Calle Pre Balanza fue exitoso", TipoDeMensaje = TipoDeMensajeDeRespuesta.Success });
            }
            else
            {
                string error = string.Join(", ", resultado.Errores.Values);
                respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = error, TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
            }

            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ModificarEstadoCallePH(int id, bool valor)
        {
            var respuesta = new RespuestaEstandarDto();

            var resultado = servicioComandos.Ejecutar(new ModificarEstadoCallePreHidraulica
            {
                Id = id,
                ActivoAutomatico = valor
            });

            if (!resultado.HayErrores)
            {
                respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "Actualización del estado de Calle Pre Hidraulico fue exitoso", TipoDeMensaje = TipoDeMensajeDeRespuesta.Success });
            }
            else
            {
                string error = string.Join(", ", resultado.Errores.Values);
                respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = error, TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
            }
            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ModificarEstadoHidraulica(int id, bool valor)
        {
            var respuesta = new RespuestaEstandarDto();

            var resultado = servicioComandos.Ejecutar(new ModificarEstadoHidraulica
            {
                Id = id,
                ActivoAutomatico = valor
            });

            if (!resultado.HayErrores)
            {
                respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "Actualización del estado de Hidraulica fue exitoso", TipoDeMensaje = TipoDeMensajeDeRespuesta.Success });
            }
            else
            {
                respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = string.Join(" - ", resultado.Errores.Select(kvp => kvp.Value.ToString())), TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
            }
            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ModificarEstadoEsEscalableHidraulica(int id, bool valor)
        {
            var respuesta = new RespuestaEstandarDto();

            var resultado = servicioComandos.Ejecutar(new ModificarEstadoEscalableHidraulica
            {
                Id = id,
                EsEscalable = valor
            });

            if (!resultado.HayErrores)
            {
                respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "Actualización del estado escalable en Hidraulica fue exitoso", TipoDeMensaje = TipoDeMensajeDeRespuesta.Success });
            }
            else
            {
                respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = string.Join(" - ", resultado.Errores.Select(kvp => kvp.Value.ToString())), TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
            }
            return Json(respuesta);
        }

        public ActionResult ObtenerHidraulicasEscalables(int? id, bool valor)
        {
            var respuesta = new RespuestaEstandarDto();
            var automatismo = servicio.ObtenerAutomatismoGranos(id.GetValueOrDefault());
            var hidraulicas = servicio.ListarHidraulicasAutomatizadas().Where(c => c.Estado != EstadoHidraulica.Inhabilitado && c.CentroId == ObtenerIdCentro());

            if (valor)
            {
                hidraulicas = hidraulicas.Where(c => c.EsEscalable == true);
            }

            hidraulicas = hidraulicas.ToList();

            var hidraulicasEscalables = MapearHidraulicas(hidraulicas.Where(c => c.ActivoAutomatico == true).ToList(), automatismo != null ? automatismo.Hidraulicas : new List<int>()).Select(s => new { label = s.Text, value = s.Value, selected = s.Selected });

            return Json(hidraulicasEscalables, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerCalidad(int id)
        {
            var respuesta = new RespuestaEstandarDto();
            var listaCalidad = servicio.ListarCaracteristicasDeCalidadPorMaterial(id, ObtenerIdCentro());
            var calidadPorMaterial = listaCalidad.Select(s => new SelectListItem { Text = s.Descripcion, Value = s.Id.ToString() }).ToList();

            calidadPorMaterial.Insert(0, new SelectListItem { Value = "", Text = Textos.Default_Calidad });

            return Json(calidadPorMaterial, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AjaxOnly]
        public ActionResult ListarAutomatismoGrano()
        {
            var model = new AutomatismoGranosViewModel();
            model.CargarDatos(servicio, ObtenerIdCentro(), model.AutomatismoGrano);
            model.ListaAutomatismoGrano = ListarAutomatismo();
            return PartialView("_ListarPanel", model);
        }

        private int ObtenerIdCentro()
        {
            var centro = ClaimsPrincipal.Current.GetUserClaim("CentroId").ToString();
            return int.Parse(centro.Split(':').Last());
        }

        private List<SelectListItem> MapearHidraulicas(IList<LlamadoAutomaticoHidraulicaDto> hidraulicas, List<int> idSeleccionados)
        {
            var opciones = hidraulicas.Select(h => new SelectListItem
            {
                Value = h.Id.ToString(),
                Text = h.HidraulicaNombre,
                Selected = idSeleccionados.Contains(h.Id)
            }).ToList();
            return new MultiSelectList(opciones, "Value", "Text", idSeleccionados).ToList();
        }
    }
}