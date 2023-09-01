using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
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
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AbmConfiguracionEficienciaCalado)]
    public class ConfiguracionEficienciaCaladoController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioOrquestador servicioOrquestador;
        private readonly IFirmwareFactory firmwareFactory;

        public ConfiguracionEficienciaCaladoController(ILogger log,
            IServicioRepositorio servicio,
            IServicioComandos servicioComandos,
            IServicioOrquestador servicioOrquestador,
            IFirmwareFactory firmwareFactory)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
            this.servicioOrquestador = servicioOrquestador;
            this.firmwareFactory = firmwareFactory;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario)
        {
            var vm = ListQuery(datosUsuario.CentroId);
            return View(vm);
        }

        private ConfiguracionEficienciaCaladoViewModel ListQuery(int centroId)
        {
            var configuraciones = servicio.ListarConfiguracionesGenerales(Constantes.ConfiguracionGeneral.Pantalla.EficienciaCalado, centroId);
            var model = new ConfiguracionEficienciaCaladoViewModel();
            if (configuraciones != null)
            {
                var configuracionHorario = configuraciones.Where(c => c.Nombre == Constantes.ConfiguracionGeneral.EficienciaCalado.HorarioTurno).FirstOrDefault();
                var configuracionesEficienciaCalles = configuraciones.Where(c => c.Nombre == Constantes.ConfiguracionGeneral.EficienciaCalado.EficienciaCalles).FirstOrDefault();

                var configuracion = new ConfiguracionesEficienciaCaladoDto()
                {
                    EficienciaCalle = configuracionesEficienciaCalles,
                    Horario = configuracionHorario
                };
                model.Configuraciones = configuracion;

                var listaEficienciaCalles = configuracionesEficienciaCalles?.Valor.FromJson<List<EficienciaCaladoValoresDto>>();
                var listaPaginada = new ListaPaginada<EficienciaCaladoValoresDto>(listaEficienciaCalles, 1, 10, listaEficienciaCalles.Count);
                model.Items = listaPaginada;

                var horarios = configuracionHorario?.Valor.FromJson<ConfiguracionHorarioDto>();
                model.Horarios = horarios;
            }
            return model;
        }

        [DatosUsuario]
        public ActionResult Crear(DatosUsuario datosUsuario)
        {
            SetearVista(datosUsuario.CentroId);
            return View(new EficienciaCaladoValoresDto());
        }

        [DatosUsuario]
        [HttpPost]
        public ActionResult Crear(DatosUsuario datosUsuario, EficienciaCaladoValoresDto model)
        {
            if (ModelState.IsValid)
            {
                var resultado = new Resultado();

                if (string.IsNullOrEmpty(model.Nombre))
                {
                    ModelState.AddModelError("NombreCalle", "No existe calle seleccionada");
                }
                else
                {
                    var configuracion = servicio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.EficienciaCalado, Constantes.ConfiguracionGeneral.EficienciaCalado.EficienciaCalles, datosUsuario.CentroId);
                    var configuracionCalles = configuracion.Valor.FromJson<List<EficienciaCaladoValoresDto>>();
                    configuracionCalles.Add(model);
                    configuracion.Valor = configuracionCalles.ToJson<List<EficienciaCaladoValoresDto>>();
                    configuracion.UsuarioUltimaModificacion = datosUsuario.NombreUsuario;
                    configuracion.FechaUltimaModificacion = DateTime.Now;
                    resultado = servicioComandos.Ejecutar(new ModificarConfiguracionGeneral { Dto = configuracion });
                }

                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                } else
                {
                    resultado.Error("NoSeEjecutoCrearEficienciaCalle", "El Comando ModificarConfiguracionGeneral No fue Ejecutado");
                }

                ModelState.AgregarErrores(resultado);
            }
            SetearVista(datosUsuario.CentroId, model);
            return View(model);
        }

        [DatosUsuario]
        public ActionResult Modificar(int id, DatosUsuario datosUsuario)
        {
            var aModificar = servicio.ObtenerEficienciaCalle(id, datosUsuario.CentroId);
            SetearVista(datosUsuario.CentroId, aModificar);
            return View(aModificar);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Modificar(DatosUsuario datosUsuario, EficienciaCaladoValoresDto model)
        {
            if (ModelState.IsValid)
            {
                var resultado = new Resultado();

                var configuracion = servicio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.EficienciaCalado, Constantes.ConfiguracionGeneral.EficienciaCalado.EficienciaCalles, datosUsuario.CentroId);
                var configuracionCalles = configuracion.Valor.FromJson<List<EficienciaCaladoValoresDto>>();
                var configuracionDto = configuracionCalles.Where(c => c.Id == model.Id).FirstOrDefault();
                if(configuracionDto!=null) configuracionDto.Cantidad = model.Cantidad;
                configuracion.Valor = configuracionCalles.ToJson<List<EficienciaCaladoValoresDto>>();
                configuracion.UsuarioUltimaModificacion = datosUsuario.NombreUsuario;
                configuracion.FechaUltimaModificacion = DateTime.Now;
                resultado = servicioComandos.Ejecutar(new ModificarConfiguracionGeneral { Dto = configuracion });

                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                else
                {
                    resultado.Error("NoSeEjecutoCrearEficienciaCalle", "El Comando ModificarConfiguracionGeneral No fue Ejecutado");
                }

                ModelState.AgregarErrores(resultado);
            }

            return View(model);
        }

        [DatosUsuario]
        [HttpPost]
        public ActionResult Eliminar(int id, DatosUsuario datosUsuario)
        {
            var configuracion = servicio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.EficienciaCalado, Constantes.ConfiguracionGeneral.EficienciaCalado.EficienciaCalles, datosUsuario.CentroId);
            var configuracionCalles = configuracion.Valor.FromJson<List<EficienciaCaladoValoresDto>>();
            var removido = configuracionCalles.RemoveAll(r => r.Id == id);
            configuracion.Valor = configuracionCalles.ToJson<List<EficienciaCaladoValoresDto>>();
            configuracion.UsuarioUltimaModificacion = datosUsuario.NombreUsuario;
            configuracion.FechaUltimaModificacion = DateTime.Now;
            var resultado = servicioComandos.Ejecutar(new ModificarConfiguracionGeneral { Dto = configuracion });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        [DatosUsuario]
        public void SetearVista(int centroId, EficienciaCaladoValoresDto model = null)
        {
            if(model != null)
            {
                ViewBag.Calles = new List<EficienciaCaladoValoresDto> { model }.ToSelectList(x => x.Id.ToString(), x => x.Nombre);
            } else
            {
                var calles = servicio.ListarCallesCalado(centroId);
                var configuracionDeCalles = servicio.ListarConfiguracionesGeneralesPorNombres(Constantes.ConfiguracionGeneral.Pantalla.EficienciaCalado, new List<string> { Constantes.ConfiguracionGeneral.EficienciaCalado.EficienciaCalles }, centroId).FirstOrDefault();
                var listaEficienciaCalles = configuracionDeCalles?.Valor.FromJson<List<EficienciaCaladoValoresDto>>();
                var listaIds = listaEficienciaCalles.Select(x => x.Id).ToList();
                ViewBag.Calles = calles.Where(c => !listaIds.Contains(c.Id)).ToSelectList(x => x.Id.ToString(), x => x.Nombre);
            }
        }

        [DatosUsuario]
        [HttpPost]
        public ActionResult ActualizarHorario(string desde, string hasta, DatosUsuario datosUsuario)
        {
            var resultado = new Resultado();
            var configuracionesHorario = servicio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.EficienciaCalado, Constantes.ConfiguracionGeneral.EficienciaCalado.HorarioTurno, datosUsuario.CentroId);

            var model = new ConfiguracionHorarioDto()
            {
                    HoraEntrada = desde,
                    HoraSalida = hasta
            };
            configuracionesHorario.Valor = model.ToJson<ConfiguracionHorarioDto>();
            configuracionesHorario.UsuarioUltimaModificacion = datosUsuario.NombreUsuario;
            configuracionesHorario.FechaUltimaModificacion = DateTime.Now;
            resultado = servicioComandos.Ejecutar(new ModificarConfiguracionGeneral { Dto = configuracionesHorario });
            
            return Json(resultado);
        }
    }
}