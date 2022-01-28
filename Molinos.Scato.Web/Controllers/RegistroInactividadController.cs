using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ninject.Extensions.Logging;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Web.Models;

namespace Molinos.Scato.Web.Controllers
{
    public class RegistroInactividadController : BaseController
    {
        private readonly IServicioActividadFactory<ICaladoService> factory;
        private readonly IServicioOrquestador orquestador;
        private readonly IServicioComandos servicioComandos;
        private readonly IConfiguracionProvider configuracion;
        private ILogger log;

        public RegistroInactividadController(ILogger log, IServicioActividadFactory<ICaladoService> factory, IServicioRepositorio servicio, IServicioOrquestador orquestador, IServicioComandos servicioComandos, IConfiguracionProvider configuracion)
            : base(servicio)
        {
            this.factory = factory;
            this.orquestador = orquestador;
            this.servicioComandos = servicioComandos;
            this.configuracion = configuracion;
            this.log = log;
        }

        // GET: /RegistroInactividad/
        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario)
        {
            var modelo = new RegistroInactividadDto();
            try
            {
                var resultado = servicioComandos.Ejecutar(new CrearRegistroInactividad
                {
                    Dto = new RegistroInactividadDto
                    {
                        Usuario = datosUsuario.NombreUsuario,
                        MotivoId = null,
                        FechaInicio = DateTime.Now,
                        FechaFinal = null,
                        PuestoTrabajoId = datosUsuario.PuestoDeTrabajoId
                    }
                }) as ResultadoCrear;
                log.Info("Registro de inactividad de usuario creado");
                modelo.Id = resultado.Id;
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error al agregar registro de inactividad del usuario");
            }
            ViewBag.MotivosInactividad =
                servicio.ListarMotivosInactividad().Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Descripcion,
                    Selected = false
                });
            return View("_Index", modelo);
        }

        [DatosUsuario]
        public ActionResult RegistrarInactividadUsuario(int motivoInactividadId, int idRegistroInactividad, DatosUsuario datosUsuario)
        {
            try
            {
                var aModificar = servicio.ObtenerRegistroInactividad(idRegistroInactividad);
                aModificar.FechaFinal = DateTime.Now;
                aModificar.MotivoId = motivoInactividadId;
                var resultado = servicioComandos.Ejecutar(new ModificarRegistroInactividad { Dto = aModificar });
                log.Info("Registro de inactividad de usuario actualizado");
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error al actualizar registro de inactividad del usuario");
            }
            return Json("1");
        }

        [DatosUsuario]
        public ActionResult ConsultarUltimoCamionCalado(DatosUsuario datosUsuario)
        {
            var mostrarPopUp = false;
            try
            {
                var centro = servicio.ObtenerCentro(datosUsuario.CentroId);

                var puesto = servicio.ObtenerPuestoDeTrabajoPorNombrePc(datosUsuario.NombrePc, datosUsuario.CentroId);
                var controlRecorrido = servicio.ObtenerUltimoCaladoPorPuestoDeTrabajo(puesto.Id);
                if (controlRecorrido != null) // cuando no tiene ningún camión calado aún
                {
                    var fechaActual = DateTime.Now;
                    var diferencia = fechaActual.Subtract(controlRecorrido.Fecha); // diferencia entre último calado por puesto de trabajo
                    if (diferencia.TotalMinutes > (centro.MinutosInactividadCalado ?? 15))
                    {
                        var ultimoRegistroInactividad = servicio.ObtenerUltimoRegistroInactividadPorUsuario(datosUsuario.NombreUsuario);
                        if (ultimoRegistroInactividad != null && fechaActual.Subtract(ultimoRegistroInactividad.FechaFinal ?? ultimoRegistroInactividad.FechaInicio).TotalMinutes > (centro.MinutosInactividadCalado ?? 15))
                        {
                            mostrarPopUp = true;
                        }
                        else
                        {
                            mostrarPopUp = ultimoRegistroInactividad == null;
                        }
                    }
                }
                else
                {
                    mostrarPopUp = true;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error al actualizar registro de inactividad del usuario");
            }
            return Json(new { mostrarPopUpInactividad = mostrarPopUp }, JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult ValidaInactividad(DatosUsuario datosUsuario)
        {
            var valida = servicio.ObtenerPuestoDeTrabajoPorNombrePc(datosUsuario.NombrePc, datosUsuario.CentroId);
            return Json(new { validar = valida != null ? valida.ActivarRegistroInactividad : false }, JsonRequestBehavior.AllowGet);
        }

    }
}
