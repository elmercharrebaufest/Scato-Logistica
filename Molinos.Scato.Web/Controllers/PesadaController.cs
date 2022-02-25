using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using BrockAllen.CookieTempData;
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
using Molinos.Scato.Web.Seguridad;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadPesada)]
    public class PesadaController : BaseController
    {
        protected readonly ILogger log;
        protected readonly IServicioComandos comando;
        private readonly IServicioActividadFactory<IPesadaService> factory;
        private readonly IServicioOrquestador orquestador;
        private readonly IListaDeWorkflows workflows;
        protected TipoPesada TipoPesada;
        protected string ActividadXaml;

        public PesadaController(ILogger log, IServicioRepositorio servicio,
                                IServicioActividadFactory<IPesadaService> factory, IServicioComandos comando,
                                IServicioOrquestador orquestador, IListaDeWorkflows workflows)
            : base(servicio)
        {
            this.log = log;
            this.factory = factory;
            this.comando = comando;
            this.orquestador = orquestador;
            this.workflows = workflows;
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            TempDataProvider = new CookieTempDataProvider();
        }

        [DatosUsuario]
        public virtual ActionResult Index(Guid id, DatosUsuario datosUsuario, bool automatizadoFull = false)
        {
            log.Debug("Puesto de Trabajo Index {0}", datosUsuario.PuestoDeTrabajoId);

            var recorrido = servicio.ObtenerRecorridoPorGuid(id);
            var centro = recorrido.Centro;
            SetearVista(recorrido, datosUsuario, automatizadoFull);

            // Chequea si la balanza que se asignó en puesto comando es la balanza del puesto 

            var almacen = new AlmacenDto();
            if (recorrido.Material != null && recorrido.Almacen == null)
            {
                var almacenPredeterminado = servicio.ObtenerAlmacenPredeterminado(datosUsuario.CentroId,
                                                                                  recorrido.Material.Id);
                almacen.Id = almacenPredeterminado != null ? almacenPredeterminado.Id : 0;
            }
            else if (recorrido.Almacen != null)
            {
                almacen = recorrido.Almacen;
            }

            var modelo = new Pesada
            {
                TipoPesada = TipoPesada,
                WorkflowInstanceId = recorrido.InstanciaWorkflow,
                Patente = centro.ReingresaPatenteAlPesar ? null : recorrido.Patente,
                PatenteOriginal = recorrido.Patente,
                AlmacenId = almacen.Id,
                AlmacenDesc = almacen.DescripcionCorta,
                BalanzaId = datosUsuario.BalanzaId,
                CalleDesc = recorrido.CalleDesc,
                HidraulicaDesc = recorrido.HidraulicasDesc,
                ActividadXaml = ActividadXaml,
                TipoVehiculo = recorrido.TipoVehiculo,
                Rechazado = recorrido.Rechazado,
                TipoDeWorkflow = recorrido.Workflow.TipoDeWorkflow
            };
            return View("~/Views/Pesada/Index.cshtml", modelo);
        }

        private bool BalanzaValida(RecorridoDto recorrido)
        {
            bool valida = true;
            IEnumerable<SelectListItem> balanzas = ViewBag.Balanzas;

            if (TipoPesada == TipoPesada.Bruto && recorrido.BalanzaBrutoId.HasValue && !recorrido.PesoBrutoFecha.HasValue)
            {
                valida = balanzas.Any(x => x.Value == recorrido.BalanzaBrutoId.Value.ToString(CultureInfo.InvariantCulture));
            }
            if (TipoPesada == TipoPesada.Tara && recorrido.BalanzaTaraId.HasValue && !recorrido.PesoTaraFecha.HasValue)
            {
                valida = balanzas.Any(x => x.Value == recorrido.BalanzaTaraId.Value.ToString(CultureInfo.InvariantCulture));
            }
            return valida;
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Index(Pesada pesada, string workflow, int workflowDefinicionId, DatosUsuario datosUsuario)
        {
            log.Debug("Puesto de Trabajo Post {0}", datosUsuario.PuestoDeTrabajoId);
            if (ModelState.IsValid) //Todos los datos obligatorios fueron ingresados
            {
                if (pesada.Patente.ToLower() != pesada.PatenteOriginal.ToLower()) //Patente ingresada incorrectamente
                {
                    TempData["Alerta"] = Textos.Pesada_PatenteInvalida;
                    TempData["TipoAlerta"] = TipoAlerta.Advertencia;
                    return RedirectToAction("Index", "ListaDeCamiones");
                }
                var balanza = servicio.ObtenerBalanza(pesada.BalanzaId);
                //if (string.IsNullOrEmpty(datosUsuario.NombrePc) || datosUsuario.NombrePc == "NoTienePuesto")
                //{
                //    log.Error("No se pudo identificar el puesto de trabajo");
                //    return new ContentResult { Content = Textos.Pesada_ErrorPuestoDeTrabajo };
                //}
                //if (datosUsuario.NombrePc != balanza.PuestoDeTrabajo)
                //{
                //    log.Error("El puesto de trabajo para la balanza con id {0} no coincide con el actual puesto de trabajo {1}", pesada.BalanzaId, datosUsuario.NombrePc);
                //    return new ContentResult { Content = Textos.Pesada_ErrorBalanzaPuestoDeTrabajo };
                //}
                if (balanza.EstaEnCero && pesada.Peso.HasValue) //Balanza en condiciones de pesar
                {
                    var centro = servicio.ObtenerCentro(datosUsuario.CentroId);
                    if (centro.ValidarLimiteMinimoDePeso && centro.LimiteMinimoDePeso.HasValue && pesada.Peso.Value <= centro.LimiteMinimoDePeso)
                    {
                        return new ContentResult { Content = Textos.ErrorPesoPesadaNoValido + centro.LimiteMinimoDePeso };
                    }
                    pesada.Rechazado = !string.IsNullOrEmpty(pesada.Mensaje);
                    var cookie = new CookieUsuario();
                    cookie.ActualizarValor("BalanzaId", pesada.BalanzaId.ToString(CultureInfo.InvariantCulture));
                    var controlRecorrido = new ControlRecorridoDto
                    {
                        Actividad = Textos.Pesada + " (" + pesada.TipoPesada.DisplayEnum() + ")" + (pesada.Rechazado ? "/Rechazo" : string.Empty),
                        ActividadXaml = pesada.ActividadXaml,
                        WorkflowInstanceId = pesada.WorkflowInstanceId,
                        PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                        NombreUsuario = datosUsuario.NombreUsuario,
                        Decision = pesada.Rechazado,
                        Comentario = pesada.Comentario,
                        Mensaje = pesada.Mensaje
                    };
                    var proximaAccion = workflows.ObtenerWorkflowProximaAccion(pesada.WorkflowInstanceId);
                    if (!proximaAccion.ProximaAccion.StartsWith("Pesada" + pesada.TipoPesada.ToString()) && proximaAccion.ProximaAccion != "PesadaCargaExportacion") //Controla la actividad actual con la pantalla actual
                    {
                        log.Error("El vehículo no se encuentra en la pesada: {0}", pesada.TipoPesada.ToString());
                        return new ContentResult { Content = Textos.Error_EtapaIncorrecta };
                    }
                    var servicioWf = factory.CrearServicio(workflowDefinicionId);
                    var resultadoActividad = servicioWf.Pesada(pesada.WorkflowInstanceId, pesada.Peso.Value, pesada.AlmacenId, pesada.HidraulicaId, pesada.CalleId, pesada.BalanzaId, pesada.ProximaBalanzaId, pesada.ControlPesada, DateTime.Now, controlRecorrido);
                    if (!resultadoActividad.HayErrores) //Peso tomado correctamente
                    {
                        comando.Ejecutar(new CrearBalanzaModificarModalidad { Dto = 
                            new BalanzaModificacionModalidadDto {
                                Modalidad = balanza.Modalidad,
                                BalanzaId = balanza.Id,
                                BalanzaNombre = balanza.Nombre,
                                Fecha = DateTime.Now,
                                Motivo = Textos.CambioModalidadBalanzaMotivo,
                                NombreUsuarioResponsable = datosUsuario.NombreUsuario
                            }
                        });
                        comando.Ejecutar(new ModificarBalanzaEstaEnCero
                        {
                            BalanzaId = pesada.BalanzaId,
                            EstaEnCero = false
                        });
                        return new ContentResult { Content = "OK" };
                    }
                    return new ContentResult { Content = resultadoActividad.Errores.First().Value };
                }
                //Balanza no está en cero
                return !balanza.EstaEnCero
                           ? new ContentResult { Content = Textos.Pesada_BalanzaNoCero }
                           : new ContentResult { Content = Textos.Pesada_Error };
            } //Hubieron errores
            return new ContentResult { Content = Textos.Pesada_Error };
        }

        [DatosUsuario]
        private void SetearVista(RecorridoDto recorrido, DatosUsuario datosUsuario, bool automatizadoFull)
        {
            if(PermisosHelper.Is(PermisosScato.VerBalanzasPesada))
            {
                ViewBag.Balanzas = automatizadoFull ?
                    (new List<BalanzaDto>() { servicio.ObtenerBalanzaPorPuestoDeTrabajoAutomatico(datosUsuario.PuestoDeTrabajoId) }).ToSelectList(f => f.Id.ToString(CultureInfo.InvariantCulture), f => f.Nombre) :
                    PermisosHelper.Is(PermisosScato.BalanzaAutomatica) ?
                    servicio.ListarBalanzasActivas(datosUsuario.CentroId, recorrido.TipoVehiculo).OrderBy(o => o.Nombre).ToSelectList(f => f.Id.ToString(CultureInfo.InvariantCulture), f => f.Nombre) :
                servicio.ListarBalanzasActivasPorNombrePc(datosUsuario.CentroId, datosUsuario.NombrePc, recorrido.TipoVehiculo).OrderBy(o => o.Nombre).ToSelectList(f => f.Id.ToString(CultureInfo.InvariantCulture), f => f.Nombre);
            } else
            {
                ViewBag.Balanzas = null;
            }
            ViewBag.ProximasBalanzas = servicio.ListarBalanzasActivas(datosUsuario.CentroId, recorrido.TipoVehiculo).OrderBy(o => o.Nombre).ToSelectList(f => f.Id.ToString(CultureInfo.InvariantCulture), f => f.Nombre);
            ViewBag.BalanzasObligatorias = servicio.BalanzasObligatoriasEnPuestoComando(datosUsuario.CentroId);

            ViewBag.Hidraulicas =
                servicio.ListarHidraulicas(datosUsuario.CentroId, recorrido.EsSustentable)
                        .ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Nombre);
            ViewBag.Calles =
                servicio.ListarCalles(datosUsuario.CentroId)
                        .ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Nombre);
            var almacenes = recorrido.Material != null
                                ? servicio.ListarAlmacenesPorMaterialYCentro(recorrido.Centro.Id, recorrido.Material.Id, recorrido.EsSustentable)
                                : servicio.ListarAlmacenesPorCentroYesSustentable(recorrido.Centro.Id, recorrido.EsSustentable);
            ViewBag.Almacenes = almacenes.ToSelectList(f => f.Id.ToString(CultureInfo.InvariantCulture),
                                                       f => f.DescripcionCorta);
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
            ViewBag.Motivos = servicio.ListarMotivos().ToSelectList(x => x.Descripcion, x => x.Descripcion);
            var Vehiculo = recorrido.TipoVehiculo;

            if (Vehiculo == TipoVehiculo.CamiónC || Vehiculo == TipoVehiculo.CamiónD || Vehiculo == TipoVehiculo.CamiónE)
            {
                ViewBag.Escalable = Textos.Escalable + '(' + Vehiculo + ')';
            }
            if (TipoPesada == TipoPesada.Bruto && recorrido.Calado != null)
            {
                ViewBag.TieneAnalisis = servicio.TieneAnalisisDeCalidad(recorrido.InstanciaWorkflow);
                ViewBag.TieneDescuentoDeHumedad = servicio.TieneDescuentoPorHumedad(recorrido.InstanciaWorkflow);
            }

            if (TipoPesada == TipoPesada.Bruto && recorrido.PesoTara == null && recorrido.BalanzaTaraId == null)
            {
                ViewBag.CargarProximaBalanza = true;
            }
            else if (TipoPesada == TipoPesada.Tara && recorrido.PesoBruto == null && recorrido.BalanzaBrutoId == null)
            {
                ViewBag.CargarProximaBalanza = true;
            }
            else
            {
                ViewBag.CargarProximaBalanza = false;
            }
            var puestosdetrabajo = servicio.ObtenerPuestoDeTrabajo(datosUsuario.PuestoDeTrabajoId);
            if (puestosdetrabajo != null)
            {
                ViewBag.Automatizado = puestosdetrabajo.Automatizado;
            }
            else
            {
                ViewBag.Automatizado = false;
            }
        }

        public ActionResult ValidarPatente(string patente, string patenteOriginal)
        {
            if (patente.ToLower() != patenteOriginal.ToLower())
            {
                TempData["Alerta"] = Textos.Pesada_PatenteInvalida;
                TempData["TipoAlerta"] = TipoAlerta.Advertencia;
                return Json(new { resultado = "ERROR", url = Url.Action("Index", "ListaDeCamiones") },
                            JsonRequestBehavior.AllowGet);
            }
            return Json(new { resultado = "OK", url = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerBalanza(int balanzaId)
        {
            var balanza = servicio.ObtenerBalanza(balanzaId);
            return Json(new { balanza.Color, Modalidad = (int?)balanza.Modalidad, balanza.EstaEnCero, balanza.PuestoDeTrabajo },
                        JsonRequestBehavior.AllowGet);
        }


        [DatosUsuario]
        public ActionResult TomarPeso(int balanzaid, DatosUsuario datosUsuario)
        {
            //Random random = new Random();
            //int randomNumber = random.Next(10000, 50000);
            //randomNumber = 10000;
            //return Json(randomNumber, JsonRequestBehavior.AllowGet);
            try
            {
                log.Info("Se tomará el peso en modalidad automática para la balanza con Id {0}", balanzaid);
                var balanza = servicio.ObtenerBalanza(balanzaid);
                //if (string.IsNullOrEmpty(datosUsuario.NombrePc) || datosUsuario.NombrePc == "NoTienePuesto")
                //{
                //    log.Error("No se pudo identificar el puesto de trabajo");
                //    return Json(Textos.Pesada_ErrorPuestoDeTrabajo, JsonRequestBehavior.AllowGet);
                //}
                //if (datosUsuario.NombrePc != balanza.PuestoDeTrabajo)
                //{
                //    var balanzaPorPuesto = servicio.ObtenerBalanzaPorPuestoDeTrabajo(datosUsuario.PuestoDeTrabajoId, TipoVehiculo.Camión);
                //    if (balanzaPorPuesto.Id != balanza.Id)
                //    {
                //        log.Error("El puesto de trabajo para la balanza con id {0} no coincide con el actual puesto de trabajo {1}", balanzaid, datosUsuario.NombrePc);
                //        return Json(Textos.Pesada_ErrorBalanzaPuestoDeTrabajo, JsonRequestBehavior.AllowGet);
                //    }
                //}
                log.Info("Se tomará el peso en modalidad automática para la balanza con Id {0}", balanzaid);

                var resultado = EjecutarPesaje(balanza.CodigoCabezal);

                if (resultado.Mensaje.Codigo != 0)
                {
                    log.Error("Error al tomar la pesada en balanza {0}. Mensaje: {1} - {2}",
                              balanzaid, resultado.Mensaje.Codigo, resultado.Mensaje.Descripcion);
                    return
                        Json(
                            "(" + Textos.Codigo + ":" + resultado.Mensaje.Codigo + ") " + Textos.Pesada_AutomaticaError +
                            "\r\n" + resultado.Mensaje.Descripcion, JsonRequestBehavior.AllowGet);
                }

                return Json(resultado.Valores["Pesaje"], JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error en tomar peso para la balanza con Id {0}", balanzaid);
                return Json(Textos.Pesada_AutomaticaError, JsonRequestBehavior.AllowGet);
            }

        }

        private ResultadoEjecutar EjecutarPesaje(string codigoCabezal)
        {
            ResultadoEjecutar resultado = null;
            var hayPesaje = false;
            var contador = 0;
            var ejecutarPesaje = new EjecutarPesaje { CodigoDispositivo = codigoCabezal };
            while (contador++ < 5 && !hayPesaje)
            {
                try
                {
                    resultado = orquestador.Ejecutar(ejecutarPesaje);
                    hayPesaje = resultado.Mensaje.Codigo == 0;
                    log.Debug("Llamada al orquestador exitosa. Hay peso en cabezal {0} = {1}", codigoCabezal, hayPesaje);
                    if (!hayPesaje)
                    {
                        //Esperar 50ms para hacer la próxima llamada
                        Thread.Sleep(50);
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex, "Error en tomar peso para la balanza con cabezal {0}", codigoCabezal);
                    resultado = null;
                }

            }
            return resultado;
        }

        [HttpPost]
        public ActionResult TomarPeso(Pesada pesada)
        {
            if (pesada.Peso.HasValue)
            {
                var content = new ContentResult { Content = pesada.Peso.Value.ToString(CultureInfo.InvariantCulture) };
                return content;
            }
            ModelState.AddModelError("peso", String.Format(Textos.Error_Requerido, Textos.Peso));
            return View("~/Views/Pesada/TomarPeso.cshtml", pesada);
        }
    }
}
