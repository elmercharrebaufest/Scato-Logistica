using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
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
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AbmListaDeTareasAutomatizada)]
    public class ListaDeTareasAutomatizadaController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos comandos;
        private readonly IServicioOrquestador servicioOrquestador;
        private readonly IListaDeWorkflows workflows;

        public ListaDeTareasAutomatizadaController(ILogger log, IServicioRepositorio servicio, IServicioComandos comandos, IServicioOrquestador servicioOrquestador, IListaDeWorkflows workflows)
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
            var puestosDeTrabajo = servicio.ListarPuestosDeTrabajoPorNombrePc(datosUsuario.NombrePc, datosUsuario.CentroId).ToArray();

            var alertarAnalisisObligatorio = (ResultadoAlertarAnalisisObligatorio)comandos.Ejecutar(new AlertarAnalisisObligatorio { CentroId = datosUsuario.CentroId, ListaPuestoDeTrabajoId = puestosDeTrabajo.Select(x => x.Id).ToList() });
            ViewBag.AlertarAnalisisObligatorio = alertarAnalisisObligatorio.AlertarAnalisisObligatorio;
            ViewBag.MaterialAlertarAnalisisObligatorio = string.Join(" - ", alertarAnalisisObligatorio.Material);

            ViewBag.PuestosDeTrabajo = puestosDeTrabajo.ToJson();
            ViewBag.CentroId = datosUsuario.CentroId;
            ViewBag.PantallaPrincipal = PermisosHelper.Is(PermisosScato.BalanzaAutomatica);
            ViewBag.MostrarValidacionEtapaAutomatica = TempData["FlagMostrarValidacionEtapaAutomatica"] ?? false;
            ViewBag.MensajeValidacionEtapaAutomatica = TempData["messageEtapaAutomatica"] ?? string.Empty;
            return View();
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Index(string puestosDeTrabajo, DatosUsuario datosUsuario)
        {
            var puesto = puestosDeTrabajo.FromJson<PuestoDeTrabajoDto>();
            if(puesto == null || !puesto.PidePatente)
            {
                return Json(new { mensaje = "El puesto de trabajo no requiere patente", valida = false });
            }
            if (string.IsNullOrEmpty(puesto.Patente))
            {
                return Json(new { mensaje = string.Format(Textos.Error_Requerido, new object[] { Textos.Patente }), valida = false });
            }

            var patente = puesto.Patente.ToUpper();

            //No volvemos a validar el numero de tarjeta en el post porque inferimos que ,si tiene un recorrido asignado,
            //paso por las validaciones de Asignacion de tarjeta y si no tiene, la siguiente validacion se encargara:
            var recorrido = servicio.ObtenerDatosRecorridoActivo(patente, new List<string> { puesto.Lectura });
            var proximaActividad = new ProximaAccionDto();
            if (recorrido != null)
            {
                proximaActividad = workflows.ObtenerWorkflowProximaAccion(recorrido.InstanciaWorkflow);
            }
            if (!String.IsNullOrEmpty(proximaActividad.Mensaje))
            {
                log.Error(proximaActividad.Mensaje);
                return Json(new { mensaje = proximaActividad.Mensaje, valida = false });
            }

            var resultado = servicio.ValidarProximaActividadPorPuesto(recorrido, proximaActividad.ProximaAccion, new List<PuestoDeTrabajoDto> { puesto }, datosUsuario.NombreUsuario);

            if (resultado.Valida)
            {
                comandos.Ejecutar(new EliminarLecturaDeTarjeta { Id = resultado.PuestoDeTrabajoId });
                EjecutarDispositivosDeEntrada(resultado);
                return Json(new
                {
                    url = Url.Action("Index", resultado.ProximaActividad, new { id = resultado.InstanceId }),
                    valida = true,
                    puestoId = resultado.PuestoDeTrabajoId
                });
            }
            return Json(new { mensaje = resultado.MensajeError, valida = false });
        }

        private void EjecutarDispositivosDeEntrada(ValidarProximaAccionPorPuestoDto lecturaPuestoDeTrabajo)
        {
            log.Debug("Ejecutando dispositivos de entrada para el puesto: {0}", lecturaPuestoDeTrabajo.PuestoDeTrabajoId);

            if (lecturaPuestoDeTrabajo.VideoCamaras != null && lecturaPuestoDeTrabajo.VideoCamaras.Any())
            {
                var date = DateTime.Now;
                foreach (var videoCamara in lecturaPuestoDeTrabajo.VideoCamaras)
                {
                    try
                    {
                        log.Debug("Ejecutando Camara para el puesto: {0}", lecturaPuestoDeTrabajo.PuestoDeTrabajoId);
                        var resultado = servicioOrquestador.Ejecutar(
                            new EjecutarTomarFoto
                            {
                                CodigoDispositivo = videoCamara.Codigo,
                                FilePath = videoCamara.Directorio,
                                SubPath = DateTime.Today.ToString("yyyyMMdd"),
                                FileName = FotoCamionHelper.GenerarNombre(lecturaPuestoDeTrabajo.CodigoSapCentro, lecturaPuestoDeTrabajo.NumeroDocumentoIngreso, lecturaPuestoDeTrabajo.Patente, lecturaPuestoDeTrabajo.ProximaActividad, date, TipoVehiculo.Camión)
                            });
                        date = date.AddSeconds(1);
                        if (resultado.Mensaje.Codigo != 0)
                        {
                            log.Error("Fallo la Apertura del dispositivo: {0}", resultado.Mensaje.Descripcion);
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error(e, "Fallo la foto del dispositivo: {0}", lecturaPuestoDeTrabajo.Entrada);
                    }
                }
            }

            foreach (var dispositivo in lecturaPuestoDeTrabajo.Entrada)
            {
                log.Debug("Ejecutando Barrera de entrada {1} para el puesto: {0}", lecturaPuestoDeTrabajo.PuestoDeTrabajoId, dispositivo);
                var resultado = servicioOrquestador.Ejecutar(new EjecutarAperturaBarrera
                {
                    CodigoDispositivo = dispositivo

                });

                if (resultado.Mensaje.Codigo != 0)
                {
                    log.Error("Fallo la Apertura del dispositivo: {0}", resultado.Mensaje.Descripcion);
                }
            }
        }

        public JsonResult EliminarLectura(int puestoDeTrabajoId)
        {
            var resultado = (ResultadoEliminarPrimerLectura)comandos.Ejecutar(new EliminarPrimerLectura { PuestoDeTrabajoId = puestoDeTrabajoId });
            return Json(new { resultado.ProximaLectura, resultado.ProximaPatente, resultado.ProximaPatenteLeida, resultado.OcrActivo, resultado.ReconocimientoExitoso }, JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult SessionExpirada(DatosUsuario datosUsuario)
        {
            log.Info("El usuario {0} en la PC {1} fue redirigido a ADFS para reautenticar", datosUsuario.NombreUsuario, datosUsuario.NombrePc);
            TempData["Alerta"] = Textos.SesionExpirada;
            TempData["TipoAlerta"] = TipoAlerta.Informacion;
            return RedirectToAction("Index");
        }

        [DatosUsuario]
        public ActionResult ErrorGenerico(DatosUsuario datosUsuario)
        {
            log.Info("El usuario {0} en la PC {1} obtuvo un error al arrancar la actividad", datosUsuario.NombreUsuario, datosUsuario.NombrePc);
            TempData["Alerta"] = Textos.Error_Generico;
            TempData["TipoAlerta"] = TipoAlerta.Error;
            return RedirectToAction("Index");
        }

    }
}
