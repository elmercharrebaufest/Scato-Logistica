using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.WebMobile.Atributos;
using Molinos.Scato.WebMobile.Helpers.Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.WebMobile.ViewModel;
using Ninject.Extensions.Logging;
using System;
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
        private readonly ILogger log;

        public TableroComandoPuertoController(IServicioComandos servicioComandos, IServicioRepositorio servicio, ILogger log)
        {
            this.servicioComandos = servicioComandos;
            this.servicio = servicio;
            this.log = log;
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
            var model = cargarModeloAutomatismo();
            return PartialView("_Crear", model);
        }

        [AjaxOnly]
        [HttpPost]
        public ActionResult Crear(AutomatismoNoGranoViewModel model)
        {
            var automatismo = model.AutomatismoNoGrano;
            var resultado = servicioComandos.Ejecutar(new CrearAutomatismoNoGrano { Dto = automatismo });

            if (!resultado.HayErrores)
            {
                model.ListaAutomatismoNoGrano = ListarAutomatismo();
                model.EstadoGeneralAutomatismoNoGrano = ObtenerEstadoGeneralAutomatismoNoGrano();
                return PartialView("_Listar", model);
            }
            var result = new JsonResult { Data = null, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            result.Data = new MensajeEstandarDto { Key="Error",Mensaje = string.Join(" - ", resultado.Errores.Select(kvp => kvp.Value.ToString())),TipoDeMensaje = TipoDeMensajeDeRespuesta.Error };

            return result;
        }

        [HttpGet]
        public ActionResult Modificar(int id)
        {
            AutomatismoNoGranoViewModel model = cargarModeloAutomatismo();
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

        public ActionResult Modificar(AutomatismoNoGranoViewModel modelo)
        {
            var automatismoActual = servicio.ObtenerAutomatismoNoGrano(modelo.AutomatismoNoGrano.Id);
            var automatismo = modelo.AutomatismoNoGrano;
            automatismo.Activo = automatismoActual.Activo;
            automatismo.CallePlanta = automatismoActual.CallePlanta;
            automatismo.CallePlayaInterna = automatismoActual.CallePlayaInterna;

            var resultado = servicioComandos.Ejecutar(new ModificarAutomatismoNoGrano { Dto = automatismo });
            if (!resultado.HayErrores)
            {
                modelo.ListaAutomatismoNoGrano = ListarAutomatismo();
                modelo.EstadoGeneralAutomatismoNoGrano = ObtenerEstadoGeneralAutomatismoNoGrano();
                return PartialView("_Listar", modelo);
            }
            var result = new JsonResult { Data = null, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            result.Data = new MensajeEstandarDto { Key = "Error", Mensaje = string.Join(" - ", resultado.Errores.Select(kvp => kvp.Value.ToString())), TipoDeMensaje = TipoDeMensajeDeRespuesta.Error };
            return result;
        }

        [HttpGet]
        public ActionResult Eliminar(int id)
        {
            AutomatismoNoGranoDto automatismo = new AutomatismoNoGranoDto();
            automatismo.Id = id;
            return PartialView("_Eliminar", automatismo);
        }

        [HttpPost]
        public ActionResult Eliminar(AutomatismoNoGranoDto automatismo)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarAutomatismoNoGrano { Id = automatismo.Id });
            if (!resultado.HayErrores)
            {
                AutomatismoNoGranoViewModel model = new AutomatismoNoGranoViewModel();
                model.ListaAutomatismoNoGrano = ListarAutomatismo();
                model.EstadoGeneralAutomatismoNoGrano = ObtenerEstadoGeneralAutomatismoNoGrano();
                return PartialView("_Listar", model);
            }
            var result = new JsonResult { Data = null, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            result.Data = new MensajeEstandarDto { Key = "Error", Mensaje = string.Join(" - ", resultado.Errores.Select(kvp => kvp.Value.ToString())), TipoDeMensaje = TipoDeMensajeDeRespuesta.Error };

            return result;
        }

        private ListaPaginada<AutomatismoNoGranoDto> ListarAutomatismo()
        {
            //recuperar una lista de automatismo y asignarla a model
            var listaAutomatismos = servicio.ListarAutomatismoNoGrano();
            var listaPaginada = new ListaPaginada<AutomatismoNoGranoDto>(listaAutomatismos, 1, 15, listaAutomatismos.Count);
            return listaPaginada;
        }

        private AutomatismoNoGranoViewModel cargarModeloAutomatismo()
        {
            AutomatismoNoGranoViewModel model = new AutomatismoNoGranoViewModel();
            var callesPlanta = servicio.ListarCallesActivasAutomatismoNoGranoPorTipo(Dominio.Enums.TipoCalle.PlantaNoGranos);
            var callesPlayaInterna = servicio.ListarCallesDisponiblesPorTipoAutomatismoNoGrano(TipoCalle.PlayaInterna);
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

        [HttpGet]
        public ActionResult ModificarCallePlanta(int id)
        {
            AutomatismoNoGranoConfiguracionViewModel model = new AutomatismoNoGranoConfiguracionViewModel();
            model.CallePlanta = servicio.ObtenerCalle(id);
            return PartialView("_ModificarCallePlanta", model);
        }

        [HttpPost]
        public ActionResult ModificarCallePlanta(AutomatismoNoGranoConfiguracionViewModel model)
        {
            var calleEditada = model.CallePlanta;
            var calle = servicio.ObtenerCalle(calleEditada.Id);
            calle.Nombre = calleEditada.Nombre;
            calle.CantidadDeCamiones = calleEditada.CantidadDeCamiones;

            var resultado = servicioComandos.Ejecutar(new ModificarCalle { Dto = calle });
            if (!resultado.HayErrores)
            {
                var modelo = CargarListasDeConfiguracion();
                return PartialView("_ListarCallePlanta", modelo);
            }
            var result = new JsonResult { Data = null, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            result.Data = new MensajeEstandarDto { Key = "Error", Mensaje = string.Join(" - ", resultado.Errores.Select(kvp => kvp.Value.ToString())), TipoDeMensaje = TipoDeMensajeDeRespuesta.Error };
            return result;
        }

        [HttpGet]
        public ActionResult ModificarPuntoDeCarga(int id)
        {
            AutomatismoNoGranoConfiguracionViewModel model = new AutomatismoNoGranoConfiguracionViewModel();
            model.PuntoDeCarga = servicio.ObtenerPuntoDeCarga(id);
            return PartialView("_ModificarPuntoDeCarga", model);
        }

        [HttpPost]
        public ActionResult ModificarPuntoDeCarga(AutomatismoNoGranoConfiguracionViewModel model)
        {
            var puntoDeCargaEditado = model.PuntoDeCarga;
            var puntoDeCarga = servicio.ObtenerPuntoDeCarga(puntoDeCargaEditado.Id);
            puntoDeCarga.Descripcion = puntoDeCargaEditado.Descripcion;
            puntoDeCarga.CantidadMaximaDeCamiones = puntoDeCargaEditado.CantidadMaximaDeCamiones;

            var resultado = servicioComandos.Ejecutar(new ModificarPuntoDeCarga { Dto = puntoDeCarga });
            if (!resultado.HayErrores)
            {
                var modelo = CargarListasDeConfiguracion();
                return PartialView("_ListarPuntoDeCarga", modelo);
            }
            var result = new JsonResult { Data = null, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            result.Data = new MensajeEstandarDto { Key = "Error", Mensaje = string.Join(" - ", resultado.Errores.Select(kvp => kvp.Value.ToString())), TipoDeMensaje = TipoDeMensajeDeRespuesta.Error };

            return result;
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
            if (!nuevoEstado)
            {
                var incluidoEnAutomatismoNoGrano = servicio.ListarAutomatismoNoGrano().Any(x => x.Almacen.Id==almacen.Id && x.Activo);
                var incluidoEnAutomatismoGrano = servicio.ListarAutomatismoGrano().Any(x => x.AlmacenId == almacen.Id && x.Activo);
                if (!incluidoEnAutomatismoGrano && !incluidoEnAutomatismoNoGrano)
                {
                    resultado = servicioComandos.Ejecutar(new ModificarAlmacen { Dto = almacen });
                }
                else
                {
                    resultado.Error("Error", "No se Puede Actualizar el Estado, Hay Automatismos Activos Asociados");
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
                var incluidoEnAutomatismoNoGrano = servicio.ListarAutomatismoNoGrano().Any(x => x.PuntoDeCarga.Id==punto.Id && x.Activo);

                if (!incluidoEnAutomatismoNoGrano)
                {
                    resultado = servicioComandos.Ejecutar(new ModificarPuntoDeCarga { Dto = punto });
                }
                else
                {
                    resultado.Error("Error", "No se Puede Actualizar el Estado, Hay Automatismos Activos Asociados");
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
                var incluidoEnAutomatismoNoGrano = servicio.ListarAutomatismoNoGrano().Any(x => x.CallePlayaInternaId == calle.Id && x.Activo);
                if (!incluidoEnAutomatismoNoGrano)
                {
                    resultado = servicioComandos.Ejecutar(new ModificarCalle { Dto = calle });
                }
                else
                {
                    resultado.Error("Error", "No se puede modificar mientras tenga asociado un automatismo activo");
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

        private bool ObtenerEstadoGeneralAutomatismoNoGrano()
        {
            var configuracionAutomatismoNoGrano = servicio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.TableroComandoPuerto, Constantes.ConfiguracionGeneral.LlamadoAutomatico.NoGranos);
            var result = false;
            if (configuracionAutomatismoNoGrano != null)
            {
                bool valor = false;
                var conversion = bool.TryParse(configuracionAutomatismoNoGrano.Valor, out valor);
                if (conversion)
                {
                    result = valor;
                }
            }
            return result;
        }
    }
}