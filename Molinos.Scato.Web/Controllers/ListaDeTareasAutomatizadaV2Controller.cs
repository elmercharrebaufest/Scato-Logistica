using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Models;
using Molinos.Scato.Web.Seguridad;
using Molinos.Scato.Dominio.Comandos.ResultadoServicio;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AbmListaDeTareasAutomatizada)]
    public class ListaDeTareasAutomatizadaV2Controller : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos comandos;
        private readonly IServicioOrquestador servicioOrquestador;
        private readonly IListaDeWorkflows workflows;

        public ListaDeTareasAutomatizadaV2Controller(
            ILogger log,
            IServicioRepositorio servicio,
            IServicioComandos comandos,
            IServicioOrquestador servicioOrquestador,
            IListaDeWorkflows workflows)
            : base(servicio)
        {
            this.log = log;
            this.comandos = comandos;
            this.servicioOrquestador = servicioOrquestador;
            this.workflows = workflows;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario)
        {
            var puestos = servicio.ListarPuestosDeTrabajoPorNombrePc(datosUsuario.NombrePc, datosUsuario.CentroId)
                .Where(x => !x.InvisibleEnListaDeTareas)
                .ToArray();

            var puestosV2 = puestos.Select(p => new PuestoCaladoModel
            {
                Id = p.Id,
                NombrePuesto = p.NombrePuesto,
                EstadoConexion = p.EstadoConexion,
                MensajeConexion = p.MensajeConexion,
                Patente = null,
                ReconocimientoExitoso = false,
                MensajeError = null,
                ImagenBase64 = null
            }).ToList();

            foreach (var puesto in puestosV2)
            {
                var primerElementoResultado = comandos.Ejecutar(new DesencolarIdentificacionVehicular
                {
                    PuestoDeTrabajoId = puesto.Id,
                    Eliminar = false
                }) as ResultadoEncolarIdentificacionVehicular;

                var primero = primerElementoResultado.PrimerElementoCola;
                if (primero != null)
                {
                    puesto.ColaIdentificacionVehicularId = primero.Id;
                    puesto.Patente = primero.Patente;
                    puesto.ReconocimientoExitoso = primero.ReconocimientoExitoso;
                    puesto.MensajeError = primero.MensajeError;
                    puesto.ImagenBase64 = primero.ImagenBase64;
                }
            }

            var alertarAnalisisObligatorio = (ResultadoAlertarAnalisisObligatorio)comandos.Ejecutar(new AlertarAnalisisObligatorio
            {
                CentroId = datosUsuario.CentroId,
                ListaPuestoDeTrabajoId = puestos.Select(x => x.Id).ToList()
            });

            ViewBag.AlertarAnalisisObligatorio = alertarAnalisisObligatorio.AlertarAnalisisObligatorio;
            ViewBag.MaterialAlertarAnalisisObligatorio = string.Join(" - ", alertarAnalisisObligatorio.Material);
            ViewBag.PantallaPrincipal = PermisosHelper.Is(PermisosScato.BalanzaAutomatica);

            return View(puestosV2);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Index(int id, string patente, DatosUsuario datosUsuario)
        {
            if (string.IsNullOrEmpty(patente))
                return Json(new { mensaje = string.Format(Textos.Error_Requerido, new object[] { Textos.Patente }), valida = false });

            patente = patente.ToUpperInvariant();
            var recorrido = servicio.ObtenerDatosRecorridoActivoPorTarjetaOPatente(TipoIdentificacionPorPuesto.IngresoPorPatente, patente, null);
            var proximaActividad = new ProximaAccionDto();
            if (recorrido != null)
                proximaActividad = workflows.ObtenerWorkflowProximaAccion(recorrido.InstanciaWorkflow);

            if (!string.IsNullOrEmpty(proximaActividad.Mensaje))
                return Json(new { mensaje = proximaActividad.Mensaje, valida = false });

            var puestoDto = new PuestoDeTrabajoDto
            {
                Id = id,
                Lectura = recorrido.TarjetaDeAcceso,
            };
            var resultado = servicio.ValidarProximaActividadPorPuesto(recorrido, proximaActividad.ProximaAccion, new List<PuestoDeTrabajoDto> { puestoDto }, datosUsuario.NombreUsuario);
            if (!resultado.Valida)
                return Json(new { mensaje = resultado.MensajeError, valida = false });

            comandos.Ejecutar(new DesencolarIdentificacionVehicular
            {
                PuestoDeTrabajoId = id,
                Eliminar = true
            });

            return Json(new
            {
                url = Url.Action("Index", resultado.ProximaActividad, new { id = resultado.InstanceId }),
                valida = true,
                puestoId = resultado.PuestoDeTrabajoId
            });
        }

        [HttpPost]
        public ActionResult OmitirCamion(int puestoDeTrabajoId)
        {
            var resultado = comandos.Ejecutar(new DesencolarIdentificacionVehicular
            {
                PuestoDeTrabajoId = puestoDeTrabajoId,
                Eliminar = true
            }) as ResultadoEncolarIdentificacionVehicular;

            if (resultado.HayErrores)
            {
                return Json(new
                {
                    MensajeError = resultado.Errores.First().Value,
                });
            }

            var siguiente = resultado.PrimerElementoCola;

            return Json(new
            {
                Id = siguiente?.Id ?? 0,
                Patente = siguiente?.Patente,
                ReconocimientoExitoso = siguiente?.ReconocimientoExitoso ?? false,
                MensajeError = siguiente?.MensajeError,
                ImagenBase64 = siguiente?.ImagenBase64
            });
        }

        [HttpPost]
        public ActionResult ValidarPatente(string patente)
        {
            try
            {
                if (string.IsNullOrEmpty(patente))
                    return Json(new { valida = false, mensaje = string.Format(Textos.Error_Requerido, new object[] { Textos.Patente }) });

                var recorrido = servicio.ObtenerDatosRecorridoActivoPorTarjetaOPatente(TipoIdentificacionPorPuesto.IngresoPorPatente, patente, null);
                if (recorrido == null)
                    return Json(new { valida = false, mensaje = string.Empty });
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error al validar la patente.");
                return Json(new { valida = false, mensaje = "Ocurrió un error interno al validar patente" });
            }

            return Json(new { valida = true, mensaje = string.Empty });
        }
    }
}
