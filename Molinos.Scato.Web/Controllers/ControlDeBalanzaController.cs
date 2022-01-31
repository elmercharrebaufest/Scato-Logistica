using System;
using System.Globalization;
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
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadControlDeBalanza)]
    public class ControlDeBalanzaController : BaseController
    {
        private readonly IServicioActividadFactory<IControlDeBalanza2Service> factory;
        private readonly IServicioComandos servicioComandos;
        private readonly IListaDeWorkflows listaDeWorkflows;
        private readonly IServicioOrquestador orquestador;
        private readonly ILogger log;

        public ControlDeBalanzaController(ILogger log, IServicioActividadFactory<IControlDeBalanza2Service> factory, IServicioRepositorio servicio, IServicioComandos servicioComandos, IListaDeWorkflows listaDeWorkflows, IServicioOrquestador orquestador)
            : base(servicio)
        {
            this.factory = factory;
            this.listaDeWorkflows = listaDeWorkflows;
            this.orquestador = orquestador;
            this.servicioComandos = servicioComandos;
            this.log = log;
        }

        [DatosUsuario]
        public ActionResult Index(Guid id, DatosUsuario datosUsuario)
        {
            var controlBalanza = new ControlDeBalanzaDto();
            var datos = servicio.ObtenerDatosDeInstanciaPorGuid(id);
            var balanzas = servicio.ListarBalanzasActivasPorNombrePc(datosUsuario.CentroId, datosUsuario.NombrePc, datos.TipoVehiculo).OrderBy(o => o.Nombre).ToSelectList(f => f.Id.ToString(CultureInfo.InvariantCulture), f => f.Nombre);
            if (datosUsuario.NombrePc == "NoTienePuesto")
            {
                controlBalanza.Error = Textos.Pesada_ErrorPuestoDeTrabajo;
            }
            else if (balanzas.Count == 0)
            {
                controlBalanza.Error = Textos.Pesada_ErrorNoHayBalanzas;
            }
            ViewBag.Balanzas = balanzas;
            return View(controlBalanza);
        }
        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult PesarDetalle(ControlDeBalanzaDto controlDeBalanza)
        {
            var workflow = listaDeWorkflows.ObtenerWorkflowPorPatente(controlDeBalanza.Patente);
            if (workflow == null)
            {
                controlDeBalanza.Error = Textos.ControlDeBalanza_PatenteInexistente;
            }
            else if (workflow.ProximaAccion != "ControlDeBalanza")
            {
                controlDeBalanza.Error = Textos.ControlDeBalanza_Error;
            }
            else
            {
                var recorrido = servicio.ObtenerRecorridoPorGuid(workflow.Id);
                if (!recorrido.ControlBalanza)
                {
                    controlDeBalanza.Error = Textos.ControlDeBalanza_ControlFinalizado;
                }
                else
                {
                    var controlDb = servicio.ObtenerControlDeBalanzaPorRecorrido(recorrido.Id);
                    if (controlDb == null)
                    {
                        controlDeBalanza.Error = Textos.ControlDeBalanza_ControlInicioError;
                    }
                    else
                    {
                        controlDeBalanza = controlDb;
                    }
                }
            }
            return View("PesarDetalle", controlDeBalanza);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Confirmar(string muestrasTomadas, Guid instanceId, string observaciones, int controlDeBalanzaId, TipoPesada tipoPesada, DatosUsuario datosUsuario,int balanzaId, bool finalizar = false)
        {
            try
            {
                if (string.IsNullOrEmpty(muestrasTomadas) ||  muestrasTomadas.Length < 3)
                {
                    return new ContentResult { Content = Textos.ControlDeBalanza_PesadasError };
                }

                var controles = muestrasTomadas.FromJson<ControlDeBalanzaPesadaDto[]>();
                if (controles != null && controles.Any())
                {
                    controles.ToList().ForEach(f => f.Fecha = f.Fecha.ToLocalTime());
                    var controlBalanza = servicio.ObtenerControlDeBalanza(controlDeBalanzaId) ?? new ControlDeBalanzaDto
                    {
                        InstanciaWorkflow = instanceId,
                        TipoPesada = tipoPesada
                    };
                    controlBalanza.Observaciones = observaciones;
                    controlBalanza.ControlesDeBalanzasPesadas = controles;
                    var resultado = servicioComandos.Ejecutar(new ControlDeBalanzaComando { Dto = controlBalanza, Finalizar = finalizar });
                    if (!resultado.HayErrores)
                    {
                        var controlRecorrido = new ControlRecorridoDto
                        {
                            Actividad = Textos.ActControlDeBalanza,
                            ActividadXaml = "ControlDeBalanza",
                            WorkflowInstanceId = instanceId,
                            PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                            NombreUsuario = datosUsuario.NombreUsuario
                        };
                        
                        var recorrido = servicio.ObtenerDatosDeInstanciaPorGuid(controlBalanza.InstanciaWorkflow);
                        var contratoService = factory.CrearServicio(recorrido.WorkflowDefinicionId);
                        resultado = contratoService.ControlDeBalanza2(controlBalanza.InstanciaWorkflow, controlRecorrido, balanzaId);
                        if (resultado.HayErrores)
                        {
                            return new ContentResult {Content = resultado.Errores.FirstOrDefault().Value};
                        }
                        if (!finalizar)
                        {
                            return new ContentResult {Content = "OK"};
                        }
                        TempData["Alerta"] = Textos.ControlDeBalanaza_FinalizadoExitosamente;
                        TempData["TipoAlerta"] = TipoAlerta.Exito;
                        return new ContentResult { Content = "OK" };
                    }
                    return resultado.HayErrores ? new ContentResult { Content = Textos.ControlDeBalanza_CrearError } : new ContentResult { Content = "OK" };
                }
                return new ContentResult { Content = Textos.ControlDeBalanza_PesadasError };
            }
            catch (Exception e)
            {
                return new ContentResult { Content = Textos.ControlDeBalanza_CrearError };
            }
        }
        public ActionResult ObtenerBalanza(int balanzaId)
        {
            var balanza = servicio.ObtenerBalanza(balanzaId);
            return Json(new { Descripcion = balanza.Nombre, Modalidad = (int?)balanza.Modalidad, balanza.PuestoDeTrabajo }, JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult TomarPeso(int balanzaid, DatosUsuario datosUsuario)
        {
            try
            {
                log.Info("Se tomará el peso en modalidad automática para la balanza con Id {0}", balanzaid);
                var balanza = servicio.ObtenerBalanza(balanzaid);
                if (datosUsuario.NombrePc == "NoTienePuesto")
                {
                    log.Error("No se pudo identificar el puesto de trabajo");
                    return Json(Textos.Pesada_ErrorPuestoDeTrabajo, JsonRequestBehavior.AllowGet);
                }
                if (datosUsuario.NombrePc != balanza.PuestoDeTrabajo)
                {
                    log.Error("El puesto de trabajo para la balanza con id {0} no coincide con el actual puesto de trabajo {1}", balanzaid, datosUsuario.NombrePc);
                    return Json(Textos.Pesada_ErrorBalanzaPuestoDeTrabajo, JsonRequestBehavior.AllowGet);
                }
                var ejecutarPesaje = new EjecutarPesaje { CodigoDispositivo = balanza.CodigoCabezal };
                var resultado = orquestador.Ejecutar(ejecutarPesaje);
                var hayPesaje = resultado.Valores != null && resultado.Valores.Any(a => a.Key == "Pesaje");
                log.Info("Llamada al orquestador exitosa. Hay Peso = {0}", hayPesaje);
                if (resultado.Mensaje.Codigo != 0)
                {
                    log.Error("(" + Textos.Codigo + ":{0}) " + Textos.Pesada_AutomaticaError + "\r\n{1}", resultado.Mensaje.Codigo, resultado.Mensaje.Descripcion);
                    return Json("(" + Textos.Codigo + ":" + resultado.Mensaje.Codigo + ") " + Textos.Pesada_AutomaticaError + "\r\n" + resultado.Mensaje.Descripcion, JsonRequestBehavior.AllowGet);
                }
                return hayPesaje
                           ? Json(resultado.Valores.First(f => f.Key == "Pesaje").Value, JsonRequestBehavior.AllowGet)
                           : Json(Textos.Pesada_AutomaticaError, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error en tomar peso para la balanza con Id {0}", balanzaid);
                return Json(Textos.Pesada_AutomaticaError, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ObtenerPesadas(string patente)
        {
            try
            {
                var workflow = listaDeWorkflows.ObtenerWorkflowPorPatente(patente);
                if (workflow != null && workflow.ProximaAccion == "ControlDeBalanza")
                {
                    var recorrido = servicio.ObtenerRecorridoPorGuid(workflow.Id);
                    var controlDb = servicio.ObtenerControlDeBalanzaPorRecorrido(recorrido.Id);
                    if (recorrido.ControlBalanza && controlDb != null && controlDb.ControlesDeBalanzasPesadas.Any())
                    {
                        return Json(controlDb.ControlesDeBalanzasPesadas.OrderBy(o => o.Fecha).ToList(),JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception)
            {
                return Json("0", JsonRequestBehavior.AllowGet);
            }
            return Json("0", JsonRequestBehavior.AllowGet);
        }
    }
}
