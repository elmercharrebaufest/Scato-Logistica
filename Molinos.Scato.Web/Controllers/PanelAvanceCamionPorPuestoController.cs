using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Models;
using Molinos.Scato.Web.ServicioHub.Server;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.PanelAvanceCamionPorPuesto)]
    public class PanelAvanceCamionPorPuestoController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioSuscriptor servicioSuscriptor;
        private readonly IServicioOrquestador orquestador;
        private readonly IServicioComandos comandos;

        public PanelAvanceCamionPorPuestoController(
            ILogger log,
            IServicioRepositorio servicio,
            IServicioSuscriptor servicioSuscriptor,
            IServicioOrquestador orquestador,
            IServicioComandos comandos)
            : base(servicio)
        {
            this.log               = log;
            this.servicioSuscriptor = servicioSuscriptor;
            this.orquestador       = orquestador;
            this.comandos          = comandos;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario)
        {
            ViewBag.NombrePc = datosUsuario.NombrePc ?? string.Empty;
            ViewBag.PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId;
            return View();
        }

        [DatosUsuario]
        [AjaxOnly]
        public ActionResult Listar(DatosUsuario datosUsuario, int pagina = 1, int tamanioPagina = 10)
        {
            var rolesUsuario = servicio.ObtenerRolesPorNombreUsuario(datosUsuario.NombreUsuario);
            ViewBag.Items = servicio.ListarLogAvanceManualCamionPendientesPorRolesPaginado(
                rolesUsuario, datosUsuario.CentroId, pagina, tamanioPagina,
                datosUsuario.PuestoDeTrabajoId > 0 ? datosUsuario.PuestoDeTrabajoId : (int?)null);
            return View("Listar");
        }

        [DatosUsuario]
        public ActionResult ResolucionContingencia(DatosUsuario datosUsuario, int logId)
        {
            var logItem = servicio.ObtenerLogAvanceManualCamion(logId);
            if (logItem == null)
                return RedirectToAction("Index");

            comandos.Ejecutar(new MarcarLogAvanceManualCamionEditando { LogId = logId, Usuario = datosUsuario.NombreUsuario });
            try
            {
                if (logItem.PuestoDeTrabajoId.HasValue)
                {
                    PanelAvanceCamionHub.NotificarContingenciaEditando(logItem.PuestoDeTrabajoId.Value, new
                    {
                        logId         = logItem.Id,
                        nombreUsuario = datosUsuario.NombreUsuario,
                        atendido      = 1
                    });
                }
            }
            catch (Exception ex)
            {
                log.Warn(ex, "No se pudo notificar SignalR para logId {0}", logId);
            }

            ViewBag.LogId          = logItem.Id;
            ViewBag.PatenteLeida   = logItem.PatenteLeida   ?? string.Empty;
            ViewBag.PuestoNombre   = logItem.NombrePuesto   ?? string.Empty;
            ViewBag.WorkflowNombre = logItem.WorkflowNombre ?? string.Empty;
            ViewBag.Transportista  = logItem.Transportista  ?? string.Empty;

            return View();
        }

        [DatosUsuario]
        [HttpPost]
        public JsonResult LiberarRegistro(DatosUsuario datosUsuario, int logId, string motivoLiberar)
        {
            if (string.IsNullOrWhiteSpace(motivoLiberar))
                return Json(new { ok = false, mensaje = "El motivo es obligatorio para liberar el registro." });

            var logItem = servicio.ObtenerLogAvanceManualCamion(logId);
            if (logItem == null)
                return Json(new { ok = false, mensaje = "El registro de contingencia no fue encontrado." });

            var resultado = comandos.Ejecutar(new LiberarLogAvanceManualCamionConMotivo { LogId = logId, Motivo = motivoLiberar.Trim(), Usuario = datosUsuario.NombreUsuario });
            if (resultado.HayErrores)
                return Json(new { ok = false, mensaje = "No se pudo liberar el registro." });

            try
            {
                if (logItem.PuestoDeTrabajoId.HasValue)
                {
                    PanelAvanceCamionHub.NotificarContingenciaLiberada(logItem.PuestoDeTrabajoId.Value, new
                    {
                        logId    = logItem.Id,
                        atendido = 4
                    });
                }
            }
            catch (Exception ex)
            {
                log.Warn(ex, "No se pudo notificar SignalR (LiberarRegistro) para logId {0}", logId);
            }

            return Json(new { ok = true, mensaje = "El registro fue liberado correctamente." });
        }


        [DatosUsuario]
        [HttpPost]
        public JsonResult AvanzarManual(DatosUsuario datosUsuario, int logId, string patenteIngresada)
        {
            if (string.IsNullOrWhiteSpace(patenteIngresada))
                return Json(new { ok = false, mensaje = "Debe ingresar la patente del camion antes de avanzar." });

            var logItem = servicio.ObtenerLogAvanceManualCamion(logId);
            if (logItem == null)
                return Json(new { ok = false, mensaje = "El registro de contingencia no fue encontrado." });

            log.Info("AvanceManual iniciado - usuario: {0}, logId: {1}, puesto: {2}",
                datosUsuario.NombreUsuario, logId, logItem.NombrePuesto);

            var patenteReal = patenteIngresada.Trim().ToUpperInvariant();

            try
            {
                var notificacion = new NotificacionEvento
                {
                    CodigoDispositivo = logItem.CodigoDispositivoOrigen,
                    CodigoEvento      = Constantes.CodigosEventos.IdentificacionVehicular,
                    Datos = new Dictionary<string, string>
                    {
                        { "Patente",          patenteReal },
                        { "Trigger",          "Patente" },
                        { "VehiculoPresente", "true" },
                        { "FechaEvento",      DateTime.Now.ToString("o") },
                    },
                    Valores = new Dictionary<string, decimal>()
                };

                if (!string.IsNullOrEmpty(logItem.Tarjeta))
                    notificacion.Datos["Tarjeta"] = logItem.Tarjeta;

                servicioSuscriptor.Recibir(notificacion);
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error al procesar AvanzarManual vía ServicioSuscriptor para logId {0}", logId);
                return Json(new { ok = false, mensaje = "Error al registrar la resolución de la contingencia." });
            }

            try
            {
                int? logIdentificacionDestinoId = null;
                if (logItem.PuestoDeTrabajoId.HasValue)
                    logIdentificacionDestinoId = servicio.ObtenerUltimoLogIdentificacionVehicularIdPorPuesto(logItem.PuestoDeTrabajoId.Value);

                comandos.Ejecutar(new ActualizarLogAvanceManualCamionResolucion
                {
                    LogId                                = logId,
                    PatenteIngresada                    = patenteReal,
                    Usuario                              = datosUsuario.NombreUsuario,
                    LogIdentificacionVehicularDestinoId = logIdentificacionDestinoId
                });
            }
            catch (Exception ex)
            {
                log.Warn(ex, "No se pudo actualizar LogAvanceManualCamion para logId {0}", logId);
            }

            try
            {
                if (logItem.PuestoDeTrabajoId.HasValue)
                {
                    PanelAvanceCamionHub.NotificarContingenciaLiberada(logItem.PuestoDeTrabajoId.Value, new
                    {
                        logId    = logItem.Id,
                        atendido = 2
                    });
                }
            }
            catch (Exception ex)
            {
                log.Warn(ex, "No se pudo notificar SignalR (AvanzarManual) para logId {0}", logId);
            }

            log.Info("AvanceManual exitoso - logId: {0}, usuario: {1}, patente: {2}",
                logId, datosUsuario.NombreUsuario, patenteReal);

            return Json(new { ok = true, mensaje = "El camion fue avanzado correctamente." });
        }

        [DatosUsuario]
        [HttpGet]
        public JsonResult ObtenerCamaras(int logId)
        {
            var camaras = servicio.ObtenerCamarasPorLogAvanceManualCamionId(logId);
            return Json(
                camaras.Select(c => new { c.Codigo, c.Directorio }),
                JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        [HttpGet]
        public JsonResult ObtenerImagenes(int logId)
        {
            var imagenes = servicio.ObtenerImagenesPorLogAvanceManualCamionId(logId);
            return Json(
                imagenes.Select(i => new
                {
                    i.Id,
                    i.Codigo,
                    Url = Url.Action("ServirImagen", "PanelAvanceCamionPorPuesto", new { detalleId = i.Id })
                }),
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult LiberarContingencia(int logId)
        {
            var logItem = servicio.ObtenerLogAvanceManualCamion(logId);
            if (logItem == null)
                return Json(new { ok = false });

            comandos.Ejecutar(new LiberarLogAvanceManualCamion { LogId = logId });

            try
            {
                if (logItem.PuestoDeTrabajoId.HasValue)
                {
                    PanelAvanceCamionHub.NotificarContingenciaLiberada(logItem.PuestoDeTrabajoId.Value, new
                    {
                        logId    = logItem.Id,
                        atendido = 0
                    });
                }
            }
            catch (Exception ex)
            {
                log.Warn(ex, "No se pudo notificar SignalR (LiberarContingencia) para logId {0}", logId);
            }

            return Json(new { ok = true });
        }

        [HttpGet]
        public ActionResult ServirImagen(int detalleId)
        {
            var rutaFisica = servicio.ObtenerRutaImagenCaptura(detalleId);
            if (string.IsNullOrEmpty(rutaFisica) || !System.IO.File.Exists(rutaFisica))
                return HttpNotFound();

            var ext  = System.IO.Path.GetExtension(rutaFisica).ToLowerInvariant();
            var mime = ext == ".png" ? "image/png" : "image/jpeg";
            return new FileStreamResult(
                new System.IO.FileStream(rutaFisica, System.IO.FileMode.Open, System.IO.FileAccess.Read),
                mime);
        }
    }
}
