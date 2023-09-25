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
            model.CargarDatos(servicio, ObteneerIdCentro(), new AutomatismoGranoDto());

            return View(model);
        }

        public ActionResult Configuraciones()
        {
            AutomatismoGranosViewModel model = new AutomatismoGranosViewModel();
            model.ListaAutomatismoGrano = ListarAutomatismo();
            model.CargarDatos(servicio, ObteneerIdCentro(), new AutomatismoGranoDto());
            return View(model);
        }

        private ListaPaginada<AutomatismoGranoDto> ListarAutomatismo()
        {
            List<AutomatismoGranoDto> model = new List<AutomatismoGranoDto>();
            var automatismos = servicio.ListarAutomatismoGrano();
            var items = new ListaPaginada<AutomatismoGranoDto>(automatismos, 1, 15, automatismos.Count);

            return items;
        }

        public ActionResult Eliminar(int id)
        {
            var respuesta = new RespuestaEstandarDto();
            if (ModelState.IsValid)
            {
                var resultadoHidraulicas = servicioComandos.Ejecutar(new EliminarAutomatismoHidraulicas { IdAutomatismo = id });

                if (!resultadoHidraulicas.HayErrores)
                {
                    var resultadoAutomatismo = servicioComandos.Ejecutar(new EliminarAutomatismoGranos { IdAutomatismo = id });

                    ModelState.AgregarErrores(resultadoAutomatismo);

                    if (!resultadoAutomatismo.HayErrores && !resultadoHidraulicas.HayErrores)
                    {
                        respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "Eliminacion de automatismo de Granos Exitosa", TipoDeMensaje = TipoDeMensajeDeRespuesta.Success });
                    }
                    else
                    {
                        respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "Error al eliminar la relacion automatismo-hidraulica de Granos", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                    }
                }
                else
                {
                    respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "Error al eliminar el automatismo de Granos", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                }
            }

            return Json(respuesta);
        }

        [HttpGet]
        [AjaxOnly]
        public ActionResult Crear()
        {
            var model = new AutomatismoGranosViewModel();
            model.ListaAutomatismoGrano = ListarAutomatismo();
            model.CargarDatos(servicio, ObteneerIdCentro(), model.AutomatismoGrano);

            return PartialView("_CrearAutomatismo", model);
        }

        public ActionResult Crear(AutomatismoGranosViewModel model)
        {
            var respuesta = new RespuestaEstandarDto();
            if (ModelState.IsValid)
            {
                var resultadoAutomatismo = (ResultadoCrear)servicioComandos.Ejecutar(new CrearAutomatismoGranos { Dto = model.AutomatismoGrano });
                if (!resultadoAutomatismo.HayErrores)
                {
                    var idCreacion = resultadoAutomatismo.Id;

                    var resultadoHidraulicas = servicioComandos.Ejecutar(new CrearAutomatismoHidraulicas { Hidraulicas = model.AutomatismoGrano.Hidraulicas, IdAutomatismo = idCreacion });

                    ModelState.AgregarErrores(resultadoAutomatismo);

                    if (!resultadoAutomatismo.HayErrores && !resultadoHidraulicas.HayErrores)
                    {
                        respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "Creacion de automatismo de Granos Exitosa", TipoDeMensaje = TipoDeMensajeDeRespuesta.Success });
                    }
                    else
                    {
                        respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "Error al crear la relacion automatismo-hidraulica de Granos", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                    }
                }
                else
                {
                    respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "Error al crear el automatismo de Granos", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                }
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

            model.CargarDatos(servicio, ObteneerIdCentro(), dto);
            model.AutomatismoGrano.Hidraulicas = null;
            return PartialView("_ModificarAutomatismo", model);
        }

        public ActionResult Modificar(AutomatismoGranosViewModel model)
        {
            var respuesta = new RespuestaEstandarDto();
            if (ModelState.IsValid)
            {
                var resultadoAutomatismo = servicioComandos.Ejecutar(new ModificarAutomatismoGranos { Dto = model.AutomatismoGrano });
                if (!resultadoAutomatismo.HayErrores)
                {
                    ModelState.AgregarErrores(resultadoAutomatismo);

                    if (!resultadoAutomatismo.HayErrores)
                    {
                        respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "Modificacion de automatismo de Granos Exitosa", TipoDeMensaje = TipoDeMensajeDeRespuesta.Success });
                    }
                    else
                    {
                        respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "Error al modificar la relacion automatismo-hidraulica de Granos", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                    }
                }
                else
                {
                    respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "Error al modificar el automatismo de Granos", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                }
            }

            return Json(respuesta);
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
            var model = new CalleViewModel();
            var calle = servicio.ObtenerCalle(id);
            model.Id = calle.Id;
            model.Descripcion = calle.Nombre;
            model.Camiones = calle.CantidadDeCamiones;

            return PartialView("_ModificarCallePH", model);
        }

        public ActionResult ModificarConfiguracionCallePH(CalleViewModel model)
        {
            var respuesta = new RespuestaEstandarDto();
            if (ModelState.IsValid)
            {
                var resultadoCallePH = servicioComandos.Ejecutar(new ModificarCallePreHidraulica { Dto = new CalleAutomatismoDto { Id = model.Id, Descripcion = model.Descripcion, Camiones = model.Camiones } });

                ModelState.AgregarErrores(resultadoCallePH);

                if (!resultadoCallePH.HayErrores)
                {
                    respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "Modificacion de calle PH de Granos Exitosa", TipoDeMensaje = TipoDeMensajeDeRespuesta.Success });
                }
                else
                {
                    respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "Error al modificar la calle PH de Granos", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                }
            }

            return Json(respuesta);
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
            resultado.Insert(0, new SelectListItem { Value = "", Text = Textos.Variedad_Estandar });

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

        public ActionResult ActualizarEstadoPaseDirecto(int id, bool valor)
        {
            var respuesta = new RespuestaEstandarDto();

            var resultado = servicioComandos.Ejecutar(new ModificarPasoDirectoAutomatismoGrano
            {
                Id = id,
                EsPasoDirecto = valor
            });

            if (!resultado.HayErrores)
            {
                respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "Actualizacion del estado de Pase Directo fue exitoso", TipoDeMensaje = TipoDeMensajeDeRespuesta.Success });
            }
            else
            {
                respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "Error en la actualizacion del estado de Pase Directo", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
            }

            return Json(respuesta);
        }

        public ActionResult ActualizarEstadoLlamadoVolcable(int id, bool valor)
        {
            var jsonResult = new JsonResult { Data = new MensajeEstandarDto(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            var respuesta = new RespuestaEstandarDto();
            var resultado = servicioComandos.Ejecutar(new ModificarLlamadoVolcableAutomatismoGrano
            {
                Id = id,
                EsLLamadoVolcable = valor
            });

            if (!resultado.HayErrores)
            {
                respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "Actualizacion del estado de Llamado Volcable fue exitoso", TipoDeMensaje = TipoDeMensajeDeRespuesta.Success });
                jsonResult.Data = respuesta;
            }
            else
            {
                respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "Error en la actualizacion del estado de Llamado Volcable", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
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
                respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "Actualizacion del estado de Calle Pre Balanza fue exitoso", TipoDeMensaje = TipoDeMensajeDeRespuesta.Success });
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
                respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "Actualizacion del estado de Calle Pre Hidraulico fue exitoso", TipoDeMensaje = TipoDeMensajeDeRespuesta.Success });
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
                respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "Actualizacion del estado de Hidraulica fue exitoso", TipoDeMensaje = TipoDeMensajeDeRespuesta.Success });
            }
            else
            {
                respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "Error en la actualizacion del estado de Hidraulica", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
            }
            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ValidarEstadoHidraulica(int id)
        {
            var respuesta = new RespuestaEstandarDto();
            var hidraulicas = servicio.ListarHidraulicasAutomatizadas().Where(c => c.ActivoAutomatico == false && c.CentroId == ObteneerIdCentro()).Select(s => s.HidraulicaId);

            var automatismo = servicio.ListarAutomatismoGrano();
            foreach (var item in hidraulicas)
            {
                foreach (var item2 in automatismo)
                {
                    item2.Hidraulicas.Remove(item);
                }
            }
            if (automatismo.Any(c => c.Hidraulicas.Count == 1 && c.Hidraulicas.Contains(id)))
            {
                respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "Validacion del estado de Hidraulica fue exitoso", TipoDeMensaje = TipoDeMensajeDeRespuesta.Success });
            }
            else
            {
                respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "Error en la validacion del estado de Hidraulica", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
            }
            return Json(respuesta);
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
                respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "Actualizacion del estado escalable en Hidraulica fue exitoso", TipoDeMensaje = TipoDeMensajeDeRespuesta.Success });
            }
            else
            {
                respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "Error en la actualizacion del estado escalable en hidraulica", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
            }
            return Json(respuesta);
        }

        public ActionResult VerificarLlamadoUnoAUno(int id)
        {
            var respuesta = new JsonResult();
            try
            {
                var resultado = servicio.ObtenerAutomatismoGranos(id);

                respuesta.Data = new { Mensaje = resultado.Llamado1a1, TipoDeMensaje = TipoDeMensajeDeRespuesta.Success };
            }
            catch (System.Exception)
            {
                respuesta.Data = new { TipoDeMensaje = TipoDeMensajeDeRespuesta.Error };
            }

            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ActualizarEstadoPaseDirectoUnoAUno(int id, bool valor)
        {
            var respuesta = new RespuestaEstandarDto();

            var resultado = servicioComandos.Ejecutar(new ModificarPasoDirectoUnoAUnoAutomatismoGrano
            {
                Id = id,
                EsPasoDirecto = valor ? true : false,
                LlamadoUnoAUno = valor ? false : true
            });

            if (!resultado.HayErrores)
            {
                respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "Actualizacion del estado de Pase Directo 1 a 1 fue exitoso", TipoDeMensaje = TipoDeMensajeDeRespuesta.Success });
            }
            else
            {
                respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = "Error en la actualizacion del estado de Pase Directo 1 a 1", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
            }

            return Json(respuesta);
        }

        public ActionResult ObtenerHidraulicasEscalables(int id, bool valor)
        {
            var respuesta = new RespuestaEstandarDto();
            var automatismo = servicio.ObtenerAutomatismoGranos(id);
            var hidraulicas = servicio.ListarHidraulicasAutomatizadas().Where(c => c.Estado != EstadoHidraulica.Inhabilitado && c.CentroId == ObteneerIdCentro());

            if (valor)
            {
                hidraulicas = hidraulicas.Where(c => c.EsEscalable == true);
            }

            hidraulicas = hidraulicas.ToList();

            var hidraulicasEscalables = MapearHidraulicas(hidraulicas.Where(c => c.ActivoAutomatico == true).ToList(), automatismo.Hidraulicas).Select(s => new { label = s.Text, value = s.Value, selected = s.Selected });

            return Json(hidraulicasEscalables, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerCalidad(int id)
        {
            var respuesta = new RespuestaEstandarDto();
            var listaCalidad = servicio.ListarCaracteristicasDeCalidadPorMaterial(id, ObteneerIdCentro());
            var calidadPorMaterial = listaCalidad.Select(s => new SelectListItem { Text = s.Descripcion, Value = s.Id.ToString() }).ToList();

            calidadPorMaterial.Insert(0, new SelectListItem { Value = "", Text = Textos.Default_Calidad });

            return Json(calidadPorMaterial, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AjaxOnly]
        public ActionResult ListarAutomatismoGrano()
        {
            var model = new AutomatismoGranosViewModel();

            model.CargarDatos(servicio, ObteneerIdCentro(), model.AutomatismoGrano);
            model.ListaAutomatismoGrano = ListarAutomatismo();
            return PartialView("_ListarPanel", model);
        }

        private int ObteneerIdCentro()
        {
            var usuario = ClaimsPrincipal.Current.GetUserClaim(ClaimTypes.NameIdentifier);
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