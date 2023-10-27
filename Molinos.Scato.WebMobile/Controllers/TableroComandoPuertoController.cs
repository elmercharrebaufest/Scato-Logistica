using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.WebMobile.Atributos;
using Molinos.Scato.WebMobile.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Molinos.Scato.WebMobile.Controllers
{
    [Autorizacion(PermisosScato.TableroComandoPuerto)]
    public class TableroComandoPuertoController : Controller
    {
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioRepositorio servicio;

        public TableroComandoPuertoController(IServicioComandos servicioComandos, IServicioRepositorio servicio)
        {
            this.servicioComandos = servicioComandos;
            this.servicio = servicio;
        }

        public ActionResult Index()
        {
            AutomatismoNoGranoViewModel model = new AutomatismoNoGranoViewModel();
            model.ListaAutomatismoNoGrano = ListarAutomatismo();
            model.EstadoGeneralAutomatismoNoGrano = ObtenerEstadoGeneralAutomatismoNoGrano();
            return View(model);
        }

        public ActionResult Configuraciones()
        {
            var model = CargarListasDeConfiguracion();
            return View(model);
        }

        [HttpGet]
        public ActionResult Crear()
        {
            var model = CargarModeloAutomatismo();
            return PartialView("_Crear", model);
        }

        [HttpPost]
        public ActionResult Crear(AutomatismoNoGranoViewModel model)
        {
            var respuesta = new RespuestaEstandarDto();
            if (ModelState.IsValid)
            {
                var resultadoAutomatismo = (ResultadoCrear)servicioComandos.Ejecutar(new CrearAutomatismoNoGrano { Dto = model.AutomatismoNoGrano });
                if (resultadoAutomatismo.HayErrores)
                {
                    foreach (var item in resultadoAutomatismo.Errores.Values)
                    {
                        respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = item, TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                    }
                }
            }

            return Json(respuesta);
        }

        [HttpGet]
        public ActionResult Modificar(int id)
        {
            AutomatismoNoGranoViewModel model = CargarModeloAutomatismo();
            var automatismo = servicio.ObtenerAutomatismoNoGrano(id);
            model.CallesPlanta = new List<SelectListItem> { new SelectListItem { Text = automatismo.CallePlanta.Nombre, Value = automatismo.CallePlanta.Id.ToString(), Selected = true } };
            model.CallesPlayaInterna = new List<SelectListItem> { new SelectListItem { Text = automatismo.CallePlayaInterna.Nombre, Value = automatismo.CallePlayaInterna.Id.ToString(), Selected = true } };
            automatismo.CallePlantaId = automatismo.CallePlanta.Id;
            automatismo.CallePlayaInternaId = automatismo.CallePlayaInterna.Id;
            automatismo.AlmacenId = automatismo.Almacen.Id;
            automatismo.PuntoDeCargaId = automatismo.PuntoDeCarga.Id;
            model.AutomatismoNoGrano = automatismo;
            return PartialView("_Modificar", model);
        }

        [HttpPost]
        public ActionResult Modificar(AutomatismoNoGranoViewModel modelo)
        {
            var respuesta = new RespuestaEstandarDto();
            var automatismo = servicio.ObtenerAutomatismoNoGrano(modelo.AutomatismoNoGrano.Id);

            automatismo.PuntoDeCargaId = modelo.AutomatismoNoGrano.PuntoDeCargaId;
            automatismo.AlmacenId = modelo.AutomatismoNoGrano.AlmacenId;

            if (ModelState.IsValid)
            {
                var resultadoAutomatismo = servicioComandos.Ejecutar(new ModificarAutomatismoNoGrano { Dto = automatismo });
                if (resultadoAutomatismo.HayErrores)
                {
                    foreach (var item in resultadoAutomatismo.Errores.Values)
                    {
                        respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = item, TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                    }
                }
            }

            return Json(respuesta);
        }

        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            var response = new RespuestaEstandarDto();
            var resultado = servicioComandos.Ejecutar(new EliminarAutomatismoNoGrano { Id = id });
            AgregarErroresARespuesta(resultado, response);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ModificarCallePlanta(int id)
        {
            var calle = servicio.ObtenerCalle(id);

            var model = new CalleViewModel
            {
                Id = calle.Id,
                Descripcion = calle.Nombre,
                Camiones = calle.CantidadDeCamiones
            };

            return PartialView("_ModificarCallePlanta", model);
        }

        [HttpPost]
        public ActionResult ModificarCallePlanta(CalleViewModel model)
        {
            var respuesta = new RespuestaEstandarDto();
            var calle = servicio.ObtenerCalle(model.Id);
            calle.Nombre = model.Descripcion;
            calle.CantidadDeCamiones = model.Camiones;

            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new ModificarCalle { Dto = calle });
                if (resultado.HayErrores)
                {
                    foreach (var item in resultado.Errores.Values)
                    {
                        respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = item, TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                    }
                }
            }

            return Json(respuesta);
        }

        [HttpGet]
        public ActionResult ModificarPuntoDeCarga(int id)
        {
            var calle = servicio.ObtenerPuntoDeCarga(id);

            var model = new PundoDeCargaVM
            {
                Id = calle.Id,
                Descripcion = calle.Descripcion,
                Camiones = calle.CantidadMaximaDeCamiones ?? 0
            };
            return PartialView("_ModificarPuntoDeCarga", model);
        }

        [HttpPost]
        public ActionResult ModificarPuntoDeCarga(PundoDeCargaVM model)
        {
            var respuesta = new RespuestaEstandarDto();
            var puntoDeCarga = servicio.ObtenerPuntoDeCarga(model.Id);
            puntoDeCarga.Descripcion = model.Descripcion;
            puntoDeCarga.CantidadMaximaDeCamiones = model.Camiones;

            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new ModificarPuntoDeCarga { Dto = puntoDeCarga });
                if (resultado.HayErrores)
                {
                    foreach (var item in resultado.Errores.Values)
                    {
                        respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = item, TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                    }
                }
            }

            return Json(respuesta);
        }

        private ListaPaginada<AutomatismoNoGranoDto> ListarAutomatismo()
        {
            //recuperar una lista de automatismo y asignarla a model
            var listaAutomatismos = servicio.ListarAutomatismoNoGrano();
            var listaPaginada = new ListaPaginada<AutomatismoNoGranoDto>(listaAutomatismos, 1, 15, listaAutomatismos.Count);
            return listaPaginada;
        }

        public ActionResult ActualizarEstadoAutomatismoGeneral(bool nuevoEstado)
        {
            var result = new JsonResult { Data = null, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            var configuracionAutomatismoNoGrano = servicio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.TableroComandoPuerto, Constantes.ConfiguracionGeneral.LlamadoAutomatico.NoGranos);

            if (configuracionAutomatismoNoGrano == null)
            {
                result.Data = new MensajeEstandarDto { Key = "Error", Mensaje = Textos.ConfiguracionGeneral_Inexistente, TipoDeMensaje = TipoDeMensajeDeRespuesta.Warning };
                return result;
            }

            configuracionAutomatismoNoGrano.Valor = nuevoEstado.ToString();
            var resultado = servicioComandos.Ejecutar(new ModificarConfiguracionGeneral
            {
                Dto = configuracionAutomatismoNoGrano
            });

            if (!resultado.HayErrores)
            {
                result.Data = new MensajeEstandarDto { Key = "Exito", Mensaje = "Configuracion de Llamado Automatico de No Granos Exitosa", TipoDeMensaje = TipoDeMensajeDeRespuesta.Success };
                if (ObtenerEstadoGeneralAutomatismoNoGrano() == false)
                {
                    servicioComandos.Ejecutar(new ActualizarAutomatismoNoGranosEstado
                    {
                        Estado = false
                    });
                }
            }


            return result;
        }

        private AutomatismoNoGranoConfiguracionViewModel CargarListasDeConfiguracion()
        {
            AutomatismoNoGranoConfiguracionViewModel model = new AutomatismoNoGranoConfiguracionViewModel();
            var listaCallePlanta = servicio.ListarCallesPorTipo(TipoCalle.PlantaNoGranos);
            var listaPaginadaCallePlanta = new ListaPaginada<CalleDto>(listaCallePlanta, 1, listaCallePlanta.Count, listaCallePlanta.Count);
            var listaPuntoDeCarga = servicio.ListarPuntoDeCarga();
            var listaPaginadaPuntoDeCarga = new ListaPaginada<PuntoDeCargaDto>(listaPuntoDeCarga, 1, listaPuntoDeCarga.Count, listaPuntoDeCarga.Count);
            var listaAlmacen = servicio.ListarAlmacenes();
            var listaPaginadaAlmacen = new ListaPaginada<AlmacenDto>(listaAlmacen, 1, listaAlmacen.Count, listaAlmacen.Count);
            model.ListaCallePlanta = listaPaginadaCallePlanta;
            model.ListaPuntoDeCarga = listaPaginadaPuntoDeCarga;
            model.ListaAlmacen = listaPaginadaAlmacen;
            model.EstadoGeneralAutomatismoNoGrano = ObtenerEstadoGeneralAutomatismoNoGrano();
            return model;
        }

        public ActionResult ActualizarEstadoAutomatismoNoGrano(bool nuevoEstado, string id)
        {
            var result = new JsonResult { Data = null, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            var automatismo = servicio.ObtenerAutomatismoNoGrano(int.Parse(id));
            automatismo.Activo = nuevoEstado;
            var resultado = servicioComandos.Ejecutar(new ModificarAutomatismoNoGrano { Dto = automatismo });

            if (!resultado.HayErrores)
            {
                result.Data = new MensajeEstandarDto { Key = "Exito", Mensaje = "Actualizacion de Llamado Automatico de No Granos Exitosa", TipoDeMensaje = TipoDeMensajeDeRespuesta.Success };
            }
            else
            {
                result.Data = new MensajeEstandarDto { Key = "Error", Mensaje = string.Join(" - ", resultado.Errores.Select(kvp => kvp.Value.ToString())), TipoDeMensaje = TipoDeMensajeDeRespuesta.Success };
            }
            return result;
        }

        public ActionResult ActualizarEstadoAlmacen(bool nuevoEstado, string id)
        {
            var resultado = new Resultado();
            var result = new JsonResult { Data = null, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            var almacen = servicio.ObtenerAlmacen(int.Parse(id));
            almacen.EstadoAutomatismo = nuevoEstado;

            var automatismoNoGranoActivo = ObtenerEstadoGeneralAutomatismoNoGrano();
            var automatismoGranoActivo = ObtenerEstadoGeneralAutomatismoGrano();

            if (!nuevoEstado)
            {
                var incluidoEnAutomatismoNoGrano = servicio.ListarAutomatismoNoGrano().Any(x => x.Almacen.Id == almacen.Id && x.Activo && automatismoNoGranoActivo);
                var incluidoEnAutomatismoGrano = servicio.ListarAutomatismoGrano().Any(x => x.AlmacenId == almacen.Id && x.Activo && automatismoGranoActivo);
                if (!incluidoEnAutomatismoGrano && !incluidoEnAutomatismoNoGrano)
                {
                    resultado = servicioComandos.Ejecutar(new ModificarAlmacen { Dto = almacen });
                }
                else
                {
                    resultado.Error("Error", Textos.Automatismo_CalleUtilizadaEnAutomatismoActivo);
                }
            }
            else
            {
                resultado = servicioComandos.Ejecutar(new ModificarAlmacen { Dto = almacen });
            }

            if (!resultado.HayErrores)
            {
                result.Data = new MensajeEstandarDto { Key = "Exito", Mensaje = "Actualizacion de Almacen Exitosa", TipoDeMensaje = TipoDeMensajeDeRespuesta.Success };
            }
            else
            {
                result.Data = new MensajeEstandarDto { Key = "Error", Mensaje = string.Join(" - ", resultado.Errores.Select(kvp => kvp.Value.ToString())), TipoDeMensaje = TipoDeMensajeDeRespuesta.Error };
            }
            return result;
        }

        public ActionResult ActualizarEstadoPuntoDeCarga(bool nuevoEstado, string id)
        {
            var resultado = new Resultado();
            var result = new JsonResult { Data = null, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            var punto = servicio.ObtenerPuntoDeCarga(int.Parse(id));
            punto.EstadoAutomatismo = nuevoEstado;
            if (!nuevoEstado)
            {
                var automatismoNoGranoActivo = ObtenerEstadoGeneralAutomatismoNoGrano();
                var incluidoEnAutomatismoNoGrano = servicio.ListarAutomatismoNoGrano().Any(x => x.PuntoDeCarga.Id == punto.Id && x.Activo && automatismoNoGranoActivo);

                if (!incluidoEnAutomatismoNoGrano)
                {
                    resultado = servicioComandos.Ejecutar(new ModificarPuntoDeCarga { Dto = punto });
                }
                else
                {
                    resultado.Error("Error", Textos.Automatismo_CalleUtilizadaEnAutomatismoActivo);
                }
            }
            else
            {
                resultado = servicioComandos.Ejecutar(new ModificarPuntoDeCarga { Dto = punto });
            }

            if (!resultado.HayErrores)
            {
                result.Data = new MensajeEstandarDto { Key = "Exito", Mensaje = "Actualizacion de PuntoD De Carga Exitosa", TipoDeMensaje = TipoDeMensajeDeRespuesta.Success };
            }
            else
            {
                result.Data = new MensajeEstandarDto { Key = "Error", Mensaje = string.Join(" - ", resultado.Errores.Select(kvp => kvp.Value.ToString())), TipoDeMensaje = TipoDeMensajeDeRespuesta.Error };
            }
            return result;
        }

        public ActionResult ActualizarEstadoCallePlanta(bool nuevoEstado, string id)
        {
            var resultado = new Resultado();
            var result = new JsonResult { Data = null, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            var calle = servicio.ObtenerCalle(int.Parse(id));
            calle.ActivoAutomatico = nuevoEstado;
            if (!nuevoEstado)
            {
                var automatismoNoGranoActivo = ObtenerEstadoGeneralAutomatismoNoGrano();
                var incluidoEnAutomatismoNoGrano = servicio.ListarAutomatismoNoGrano().Any(x => x.CallePlantaId == calle.Id && x.Activo && automatismoNoGranoActivo);
                if (!incluidoEnAutomatismoNoGrano)
                {
                    resultado = servicioComandos.Ejecutar(new ModificarCalle { Dto = calle });
                }
                else
                {
                    resultado.Error("Error", Textos.Automatismo_CalleUtilizadaEnAutomatismoActivo);
                }
            }
            else
            {
                resultado = servicioComandos.Ejecutar(new ModificarCalle { Dto = calle });
            }

            if (!resultado.HayErrores)
            {
                result.Data = new MensajeEstandarDto { Key = "Exito", Mensaje = "Actualizacion de Calle Exitosa", TipoDeMensaje = TipoDeMensajeDeRespuesta.Success };
            }
            else
            {
                result.Data = new MensajeEstandarDto { Mensaje = string.Join(" - ", resultado.Errores.Select(kvp => kvp.Value.ToString())), TipoDeMensaje = TipoDeMensajeDeRespuesta.Error };
            }
            return result;
        }

        [HttpGet]
        [AjaxOnly]
        public ActionResult ListarAutomatismoNoGrano()
        {
            AutomatismoNoGranoViewModel model = new AutomatismoNoGranoViewModel
            {
                ListaAutomatismoNoGrano = ListarAutomatismo(),
                EstadoGeneralAutomatismoNoGrano = ObtenerEstadoGeneralAutomatismoNoGrano()
            };
            return PartialView("_Listar", model);
        }

        [HttpGet]
        [AjaxOnly]
        public ActionResult ListarCallePlanta()
        {
            var modelo = CargarListasDeConfiguracion();
            return PartialView("_ListarCallePlanta", modelo);
        }

        [HttpGet]
        [AjaxOnly]
        public ActionResult ListarPuntoDeCarga()
        {
            var modelo = CargarListasDeConfiguracion();
            return PartialView("_ListarPuntoDeCarga", modelo);
        }

        public ActionResult ObtenerPuntosDeCargaPorMaterialId(int calleId)
        {
            var materialId = servicio.ObtenerMaterialIdPorCalleId(calleId);

            var prehidraulicas = servicio.ListarPuntosDeCargaActivosAutomatismoNoGrano()
                .Where(y => y.MaterialesId.Contains(materialId));

            return Json(prehidraulicas.Select(x => new SelectListItem { Text = x.Descripcion, Value = x.Id.ToString() })
                 .ToList(), JsonRequestBehavior.AllowGet);
        }

        private AutomatismoNoGranoViewModel CargarModeloAutomatismo()
        {
            AutomatismoNoGranoViewModel model = new AutomatismoNoGranoViewModel();
            var callesPlanta = servicio.ListarCallesActivasAutomatismoNoGranoPorTipo(TipoCalle.PlantaNoGranos);
            var callesPlayaInterna = servicio.ListarCallesPlayaInternaAutomatismoDisponibles();
            model.CallesPlanta = callesPlanta
                 .Select(x => new SelectListItem { Text = x.Nombre, Value = x.Id.ToString() })
                 .ToList();
            model.CallesPlayaInterna = callesPlayaInterna
                 .Select(x => new SelectListItem { Text = x.Nombre, Value = x.Id.ToString() })
                 .ToList();
            var almacenes = servicio.ListarAlmacenesActivosAutomatismoNoGrano();
            var puntos = servicio.ListarPuntosDeCargaActivosAutomatismoNoGrano();
            model.PuntosDeCarga = puntos
                 .Select(x => new SelectListItem { Text = x.Descripcion, Value = x.Id.ToString() })
                 .ToList();
            model.Almacenes = almacenes
                 .Select(x => new SelectListItem { Text = x.Descripcion, Value = x.Id.ToString() })
                 .ToList();
            return model;
        }

        private bool ObtenerEstadoGeneralAutomatismoNoGrano()
        {
            var configuracionAutomatismo = servicio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.TableroComandoPuerto, Constantes.ConfiguracionGeneral.LlamadoAutomatico.NoGranos);
            var result = false;
            if (configuracionAutomatismo != null)
            {
                bool valor = false;
                if (bool.TryParse(configuracionAutomatismo.Valor, out valor))
                {
                    result = valor;
                }
            }
            return result;
        }

        private bool ObtenerEstadoGeneralAutomatismoGrano()
        {
            var configuracionAutomatismo = servicio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.TableroComandoLogistica, Constantes.ConfiguracionGeneral.LlamadoAutomatico.Granos);
            var result = false;
            if (configuracionAutomatismo != null)
            {
                bool valor = false;
                if (bool.TryParse(configuracionAutomatismo.Valor, out valor))
                {
                    result = valor;
                }
            }
            return result;
        }

        private void AgregarErroresARespuesta(Resultado resultado, RespuestaEstandarDto response)
        {
            if (resultado.HayErrores)
            {
                foreach (var item in resultado.Errores)
                {
                    response.Mensajes.Add(new MensajeEstandarDto { Mensaje = item.Value, TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                }
            }
        }
    }
}