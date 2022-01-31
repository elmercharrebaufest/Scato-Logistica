using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    public class BalanzaACeroController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos comando;
        private readonly IServicioNotificarUsuario notificador;
        private readonly IServicioOrquestador orquestador;
        private readonly IServicioActividadFactory<IBalanzaACeroService> factory;

        public BalanzaACeroController(ILogger log, IServicioRepositorio servicio, IServicioComandos comando, IServicioNotificarUsuario notificador, IServicioOrquestador orquestador, IServicioActividadFactory<IBalanzaACeroService> factory)
            : base(servicio)
        {
            this.log = log;
            this.comando = comando;
            this.notificador = notificador;
            this.orquestador = orquestador;
            this.factory = factory;
        }

        public ActionResult BalanzaACero(int modalidad)
        {
            ViewBag.Modalidad = modalidad;
            return PartialView("BalanzaACero");
        }

        [DatosUsuario]
        public ActionResult BalanzaEnCero(int balanzaId, DatosUsuario datosUsuario, Guid? instanciaWorkflow, int? workflowDefinicionId)
        {
            
            var enCero = ActualizarEstadoBalanzaCereada(balanzaId);
            if (!enCero.HayErrores && instanciaWorkflow.HasValue && workflowDefinicionId.HasValue)
            {
                var controlRecorrido = new ControlRecorridoDto
                {
                    Actividad = Textos.ActBalanzaACero,
                    ActividadXaml = "BalanzaACero",
                    WorkflowInstanceId = instanciaWorkflow.Value,
                    PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                    NombreUsuario = datosUsuario.NombreUsuario
                };
                var servicioWf = factory.CrearServicio(workflowDefinicionId.Value);
                servicioWf.BalanzaACero(instanciaWorkflow.Value, controlRecorrido);
                return Json(new { data =  "true" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { data = !enCero.HayErrores ? "true" : enCero.Errores.Values.FirstOrDefault(), url = Url.Action("Index", "ListaDeCamiones") }, JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult CerearBalanza(int balanzaId, DatosUsuario datosUsuario, Guid? instanciaWorkflow, int? workflowDefinicionId)
        {
            log.Info("Balanza Id {0}, inicio de CerearBalanza", balanzaId);
            var balanza = servicio.ObtenerBalanza(balanzaId);

            if (!CereoEstaPermitido(balanza, datosUsuario))
            {
                return Json(new { data = Textos.BalanzaACero_ExcedePesoMaximo }, JsonRequestBehavior.AllowGet);
            }

            if (balanza.Modalidad != Modalidad.Manual)
            {

                log.Info("Se iniciará el cereo para la balanza con Id {0}", balanza.Id);
                var resultado =
                    orquestador.Ejecutar(new EjecutarCereoCabezal {CodigoDispositivo = balanza.CodigoCabezal});
                log.Info("Llamada al orquestador exitosa.");

                if (resultado.Mensaje.Codigo != 0)
                {
                    log.Info("Cereo Fallido para la balanza con Id {0}", balanza.Id);
                    NotificarCereoFallido(balanza, datosUsuario);

                    return Json(new {data = resultado.Mensaje.Descripcion}, JsonRequestBehavior.AllowGet);
                }
            }
            log.Info("Cereo Exitoso para la balanza con Id {0}", balanza.Id);
            var resultadoRepositorio = ActualizarEstadoBalanzaCereada(balanzaId);
            log.Info("Estado Actualizado para la balanza con Id {0} de forma {0}", resultadoRepositorio.HayErrores);
            if (!resultadoRepositorio.HayErrores && instanciaWorkflow.HasValue && workflowDefinicionId.HasValue)
            {
                var controlRecorrido = new ControlRecorridoDto
                {
                    Actividad = Textos.ActBalanzaACero,
                    ActividadXaml = "BalanzaACero",
                    WorkflowInstanceId = instanciaWorkflow.Value,
                    PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                    NombreUsuario = datosUsuario.NombreUsuario
                };
                var servicioWf = factory.CrearServicio(workflowDefinicionId.Value);
                servicioWf.BalanzaACero(instanciaWorkflow.Value, controlRecorrido);
                return Json(new { data =  "true" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { data = !resultadoRepositorio.HayErrores ? "true" : resultadoRepositorio.Errores.Values.FirstOrDefault() }, JsonRequestBehavior.AllowGet);
        }


        private bool CereoEstaPermitido(BalanzaDto balanza, DatosUsuario datosUsuario)
        {
            log.Info("Validando si excede el peso maximo de cereo");
            if (balanza.Modalidad == Modalidad.Manual)
            {
                log.Info("Balanza manual Id {0}, cereo aprobado", balanza.Id);
                return true;
            }
            log.Info("Se tomará el peso en modalidad automática para la balanza con Id {0}", balanza.Id);
            var pesoTomado = orquestador.Ejecutar(new EjecutarPesaje { CodigoDispositivo = balanza.CodigoCabezal });
            var hayPesaje = pesoTomado.Valores != null && pesoTomado.Valores.Any(a => a.Key == "Pesaje");
            log.Info("Llamada al orquestador exitosa. Hay Peso = {0}", hayPesaje);

            if (!hayPesaje || balanza.MaximoValorCereo < pesoTomado.Valores.First(f => f.Key == "Pesaje").Value)
            {
                NotificarCereoFallido(balanza, datosUsuario);
                return false;
            }
            return true;
        }

        private void NotificarCereoFallido(BalanzaDto balanza, DatosUsuario datosUsuario)
        {
            var htmlIconoColor = new StringBuilder();
            htmlIconoColor.AppendLine("<span class=\"iconoColor\" style=\"background-color:");
            htmlIconoColor.AppendLine(balanza.Color);
            htmlIconoColor.AppendLine(";\">&nbsp;&nbsp;&nbsp;&nbsp;</span>");

            notificador.Notificar(new NotificacionDto
            {
                Grupo = datosUsuario.CentroId + "|" + PermisosScato.Administradores.ToString(),//TODO : rever  grupo!
                Mensaje = String.Format(Textos.Notificacion_PendienteCereo, balanza.Nombre, htmlIconoColor),
                TipoAlerta = TipoAlerta.Sobre
            });
        }

        private Resultado ActualizarEstadoBalanzaCereada(int balanzaId)
        {
            var resultado = comando.Ejecutar(new ModificarBalanzaEstaEnCero
            {
                BalanzaId = balanzaId,
                EstaEnCero = true
            });
            if (resultado.HayErrores)
            {
                return resultado;
            }
            var cookie = new CookieUsuario();
            cookie.ActualizarValor("BalanzaId", balanzaId.ToString(CultureInfo.InvariantCulture));

            return resultado;
        }

        [DatosUsuario]
        public ActionResult Index(Guid id, DatosUsuario datosUsuario)
        {
            var recorrido = servicio.ObtenerRecorridoPorGuid(id);
            int balanzaId;
            int.TryParse(recorrido.DatosProximaActividad, out balanzaId);

            BalanzaDto balanza = null;
            if (balanzaId == 0 && datosUsuario.PuestoDeTrabajoId != 0)
            {
                balanza = servicio.ObtenerBalanzaPorPuestoDeTrabajo(datosUsuario.PuestoDeTrabajoId, recorrido.TipoVehiculo);
            }
            else if (balanzaId == 0)
            {
                balanzaId = recorrido.Workflow.TipoDeWorkflow == TipoDeWorkflow.Ingreso ? recorrido.BalanzaTaraId ?? recorrido.BalanzaBrutoId ?? 0 :
                          recorrido.BalanzaBrutoId ?? recorrido.BalanzaTaraId ?? 0;
                if (balanzaId != 0)
                {
                    balanza = servicio.ObtenerBalanza(balanzaId);
                }
                else
                {
                    balanza = servicio.ListarBalanzasActivasPorNombrePc(datosUsuario.CentroId, datosUsuario.NombrePc, recorrido.TipoVehiculo).FirstOrDefault();
                }
            }
            else
            {
                balanza = servicio.ObtenerBalanza(balanzaId);
            }
            
            if (balanza == null)
            {
                TempData["Alerta"] = Textos.BalanzaACero_ErrorBalanza;
                TempData["TipoAlerta"] = TipoAlerta.Error;
                return RedirectToAction("Index", "ListaDeCamiones");
                
            }
            ViewBag.Modalidad = balanza.Modalidad;
            ViewBag.InstanciaWorkflow = id;
            ViewBag.BalanzaId = balanza.Id;
            ViewBag.WorkflowDefinicionId = recorrido.WorkflowDefinicionId;

            var almacen = new AlmacenDto();
            if (recorrido.Almacen != null)
            {
                almacen = recorrido.Almacen;
            }
            SetearVista(recorrido, datosUsuario);
            var modelo = new Pesada
            {
                WorkflowInstanceId = recorrido.InstanciaWorkflow,
                PatenteOriginal = recorrido.Patente,
                AlmacenId = almacen.Id,
                AlmacenDesc = almacen.DescripcionCorta,
                BalanzaId = datosUsuario.BalanzaId,
                CalleDesc = recorrido.CalleDesc,
                HidraulicaDesc = recorrido.HidraulicasDesc,
                ActividadXaml = "BalanzaACero",
                TipoVehiculo = recorrido.TipoVehiculo,
                Rechazado = recorrido.Rechazado,
                TipoDeWorkflow = recorrido.Workflow.TipoDeWorkflow
            };
            return View(modelo);
        }

        private void SetearVista(RecorridoDto recorrido, DatosUsuario datosUsuario)
        {
            ViewBag.DocumentoIngreso = recorrido.TipoDocumentoIngreso;
            ViewBag.NumeroDocumentoIngreso = recorrido.NumeroDocumentoIngreso;
            ViewBag.Material = recorrido.Material != null ? recorrido.Material.Descripcion : "";
            ViewBag.TipoComercial = recorrido.TipoComercial.Descripcion;
            ViewBag.PesoBrutoOrigen = recorrido.PesoBrutoOrigen;
            ViewBag.PesoTaraOrigen = recorrido.PesoTaraOrigen;
            ViewBag.PesoBruto = recorrido.PesoBruto;
            ViewBag.PesoTara = recorrido.PesoTara;
            ViewBag.Workflow = recorrido.Workflow.Codigo;
            ViewBag.WorkflowDefinicionId = recorrido.WorkflowDefinicionId;
            ViewBag.TieneEntregador = recorrido.Vehiculo != null && servicio.CartaPorteTieneEntregador(recorrido.Vehiculo.CartaPorteId)
                                          ? Textos.Si
                                          : Textos.No;

            ViewBag.TieneAnalisis = null;
            ViewBag.TieneDescuentoDeHumedad = null;
            ViewBag.ModificaAlmacen = recorrido.Centro.ModificaAlmacenEnPesada;
            ViewBag.Escalable = null;
           
            var Vehiculo = recorrido.TipoVehiculo;

            if (Vehiculo == TipoVehiculo.CamiónC || Vehiculo == TipoVehiculo.CamiónD || Vehiculo == TipoVehiculo.CamiónE)
            {
                ViewBag.Escalable = Textos.Escalable + '(' + Vehiculo + ')';
            }
        }


        [DatosUsuario]
        [Autorizacion(PermisosScato.ForzarCereo)]
        public ActionResult Avanzar(int balanzaId,string comentario, DatosUsuario datosUsuario, Guid? instanciaWorkflow, int? workflowDefinicionId)
        {   
            if (comentario.Length < 6)
            {
                return Json(new { data = Textos.Comentario_LargoMinimo_5 }, JsonRequestBehavior.AllowGet);
            }
            ResultadoCrear resultadoCrear = null;
            log.Info("Se va a forzar el paso por cero de la Balanza con Id: {0} por el usuario: {1}", balanzaId, datosUsuario.NombreUsuario);            
            var resultadoRepositorio = ActualizarEstadoBalanzaCereada(balanzaId);
            log.Info("Estado Actualizado para la balanza con Id {0} de forma {1}", balanzaId, resultadoRepositorio.HayErrores);
            if (instanciaWorkflow.HasValue)
            {
                log.Info("Se va a crear el Motivo Forzar Cero por el usuario: {0} , instanceId: {1}", datosUsuario.NombreUsuario, instanciaWorkflow.Value);
                resultadoCrear = comando.Ejecutar(new CrearMotivoForzarCero { BalanzaId = balanzaId, Fecha = DateTime.Now, Usuario = datosUsuario.NombreUsuario, InstanceId = instanciaWorkflow.Value, Motivo = comentario }) as ResultadoCrear;
                log.Info("Motivo Forzar Cero creado sin errores: {0}", !resultadoCrear.HayErrores);
            }           
            if (!resultadoRepositorio.HayErrores && workflowDefinicionId.HasValue && resultadoCrear != null && !resultadoCrear.HayErrores)
            {             
                var controlRecorrido = new ControlRecorridoDto
                {
                    Actividad = Textos.ActBalanzaACero,
                    ActividadXaml = "BalanzaACero",
                    WorkflowInstanceId = instanciaWorkflow.Value,
                    PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                    NombreUsuario = datosUsuario.NombreUsuario
                };
                var servicioWf = factory.CrearServicio(workflowDefinicionId.Value);
                servicioWf.BalanzaACero(instanciaWorkflow.Value, controlRecorrido);
                return Json(new { data = "true" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { data = !resultadoRepositorio.HayErrores ? (resultadoCrear != null && !resultadoCrear.HayErrores) ? "true" : resultadoCrear != null ? resultadoCrear.Errores.Values.FirstOrDefault() : Textos.Error_Generico : resultadoRepositorio.Errores.Values.FirstOrDefault() }, JsonRequestBehavior.AllowGet);
        }
    }
}
