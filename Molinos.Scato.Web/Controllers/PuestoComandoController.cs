using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;
using Molinos.Scato.Web.Seguridad;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadPuestoComando, PermisosScato.PuestoDeComando_CaladoEnPlanta)]
    public class PuestoComandoController : BaseController
    {
        private readonly ILogger log;
        private readonly IListaDeWorkflows workflows;
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioActividadFactory<IPuestoComandoService> factory;
        private readonly IServicioActividadFactory<IEjecutarService> factoryejecutar;
        private readonly IServicioActividadFactory<IPesadaService> factoryPesada;

        public PuestoComandoController(ILogger log, IListaDeWorkflows workflows, IServicioRepositorio servicio, IServicioComandos servicioComandos, IServicioActividadFactory<IPuestoComandoService> factory,
            IServicioActividadFactory<IEjecutarService> factoryejecutar, IServicioActividadFactory<IPesadaService> factoryPesada)
            : base(servicio)
        {
            this.log = log;
            this.workflows = workflows;
            this.servicioComandos = servicioComandos;
            this.factory = factory;
            this.factoryejecutar = factoryejecutar;
            this.factoryPesada = factoryPesada;
        }
        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, FiltroListaDeWorkflowsDto filtro, int pagina = 1, string ordenarPor = "FechaInicio", DirOrden dirOrden = DirOrden.Desc)
        {
            if (string.IsNullOrEmpty(filtro.ProximaAccion))
            {
                filtro.ProximaAccion = "PuestoComando";
                filtro.CantidadDeResultados = CantidadDeResultados.Veinticinco;
            }
            ListQuery(datosUsuario, filtro, pagina, ordenarPor, dirOrden,true);

            ViewBag.SepararAlmacenSustentable = ConfigurationManager.AppSettings["SepararAlmacenSustentable"];

            return View(filtro);
        }
        [DatosUsuario]
        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(DatosUsuario datosUsuario, FiltroListaDeWorkflowsDto filtro, int pagina = 1, string ordenarPor = "FechaInicio", DirOrden dirOrden = DirOrden.Desc)
        {
            if (filtro.Patente != null)
            {
                filtro.Patente = filtro.Patente.ToUpper();
            }
            ListQuery(datosUsuario, filtro, pagina, ordenarPor, dirOrden,false);
            return View("Listar", filtro);
        }

        [DatosUsuario]
        public ActionResult ValidarAsignar(string instanceIds)
        {
            var respuesta = new RespuestaEstandarDto<AsignacionDto>();
            log.Debug("Obteniendo asignacion puesto comando para : {0}", instanceIds);
            var asignacion = servicio.ObtenerAsignacionDePuestoComando(instanceIds);
            asignacion.InstanceIds = instanceIds;
            asignacion.patentesInvalidas = new List<string>();
            foreach (var guid in instanceIds.Split(','))
            {
                if (!workflows.VerificarExistenciaDeWorkflowPorGuid(new Guid(guid)))
                {
                    var patente = servicio.ObtenerPatentePorGuid(new Guid(guid));
                    asignacion.FalloWF = true;
                    asignacion.patentesInvalidas.Add(patente);
                }
            }
            if (asignacion.FalloWF)
            {
                var mensaje = string.Format(@"No es posible asignar un Puesto de Comando para las siguientes patentes porque el Workflow falló: {0}", string.Join(",", asignacion.patentesInvalidas));
                respuesta.Mensajes.Add(new MensajeEstandarDto { Mensaje = mensaje, TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
            }
            else {
                respuesta.Data = asignacion;
            }

            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }


        [DatosUsuario]
        [HttpPost]
        public ActionResult MostrarAsignar(AsignacionDto asignacion, DatosUsuario datosUsuario)
        {
            SetearVista(datosUsuario, asignacion.MaterialId, asignacion.SonSustentables, asignacion.SustentableMixto, asignacion.SonSojaEPA);
            return View("_Asignar",asignacion);
        }

        [DatosUsuario]
        [HttpPost]
        public ActionResult Asignar(AsignacionDto model, bool balanzasObligatorias, DatosUsuario datosUsuario)
        {
            if (!PermisosHelper.Is(PermisosScato.ActividadPuestoComando))
            {
                ModelState.Remove("HidraulicasId");
            }
            if (ModelState.IsValid)
            {
                log.Debug("Iniciando Asignacion Puesto Comando");
                ResultadoPuestoComando resultado;
                if (PermisosHelper.Is(PermisosScato.ActividadPuestoComando))
                {
                    resultado = servicioComandos.Ejecutar(new ActualizarPuestocomando { Dto = model, BalanzasObligatorias = balanzasObligatorias }) as ResultadoPuestoComando;
                }
                else
                {
                    resultado = servicioComandos.Ejecutar(new ActualizarPuestoComandoCaladoEnPlanta { Dto = model, BalanzasObligatorias = balanzasObligatorias }) as ResultadoPuestoComando;
                }
                if (!resultado.HayErrores)
                {
                    servicioComandos.Ejecutar(new ActualizarAsignacionDeCalle { CalleId = model.CalleId, HidraulicasId = model.HidraulicasId });
                    AvanzarWorkflow(resultado, datosUsuario);
                    if (ModelState.IsValid)
                    {
                        return new AjaxEditSuccessResult();
                    }
                }
                log.Error("Hubo un error al asignar el puesto comando: {0}", resultado.Errores.FirstOrDefault());
                ModelState.AgregarErrores(resultado);

            }
            SetearVista(datosUsuario, model.MaterialId, model.SonSustentables, true, model.SonSojaEPA);
            return View("_Asignar",model);
        }

        private void AvanzarWorkflow(ResultadoPuestoComando resultado, DatosUsuario datosUsuario)
        {
            var camionesAceptados = new List<DatosDeWorkflowDto>();
            var patentesFallidas = new List<string>();
            log.Debug("Inicio Asignacion Puesto Comando sin errores");
            foreach (
                var workflow in
                    resultado.Workflows.Where(
                        workflow =>
                        workflows.ObtenerWorkflowProximaAccion(workflow.InstanciaWorkflow).ProximaAccion ==
                        "PuestoComando"))
            {
                try
                {
                    log.Debug("Intentando asignar el workflow {0}", workflow.InstanciaWorkflow);
                    var servicioWf = factory.CrearServicio(workflow.WorkflowDefId);
                    var control = new ControlRecorridoDto
                    {
                        Actividad = Textos.ActPuestoComando,
                        ActividadXaml = "PuestoComando",
                        Fecha = DateTime.Now,
                        NombreUsuario = datosUsuario.NombreUsuario,
                        PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                        WorkflowInstanceId = workflow.InstanciaWorkflow,
                        Decision = true
                    };
                    var r = servicioWf.PuestoComando(workflow.InstanciaWorkflow, control);
                    if (!r.HayErrores)
                    {
                        camionesAceptados.Add(workflow);
                    }
                    log.Debug("El workflow {0} fue asignado correctamente", workflow.InstanciaWorkflow);
                }
                catch (Exception e)
                {
                    log.Error(e, "Fallo la asignación del workflow {0}", workflow.InstanciaWorkflow);
                    patentesFallidas.Add(workflow.Patente);
                }
            }
            if (patentesFallidas.Count > 0)
            {
                ModelState.AddModelError("Error", String.Format("Fallo la Asignación de las patentes {0} en el Workflow, pero las demás se procesaron correctamente.",
                                                            string.Join(",", patentesFallidas)));
            }
            ImprimirResumenHojaDeRuta(camionesAceptados, datosUsuario);
        }

        private void ImprimirResumenHojaDeRuta(List<DatosDeWorkflowDto> camionesAceptados, DatosUsuario datosUsuario)
        {
            if (!ModelState.IsValid || camionesAceptados.FirstOrDefault() == null ||
                !camionesAceptados.FirstOrDefault().MaterialEsGrano ||
                (camionesAceptados.FirstOrDefault().EsSoja && camionesAceptados.All(x => !x.TieneDescuentos)))
            {
                return;
            }
            var codigo = "ImprimirResumenHojaDeRuta";
            var documento = servicio.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(codigo, datosUsuario.CentroId, datosUsuario.PuestoDeTrabajoId);
            if (documento == null)
            {
                return;
            }

            var resultadoImpresion = servicioComandos.Ejecutar(new ImprimirResumenHojaDeRuta
            {
                Dto = new ImpResumenHojaDeRutaDto
                {
                    DatosDeWorkflows = camionesAceptados.Where(x => !x.EsSoja || (x.EsSoja && x.TieneDescuentos)).ToList(),
                    Impresora = documento.ImpresoraDireccion ?? "",
                    Codigo = "ImprimirResumenHojaDeRuta",
                }
            });

            if (resultadoImpresion.HayErrores)
            {
                ModelState.AddModelError("Imp", "Error, no se pudo imprimir: " + resultadoImpresion.Errores.First().Value);
            }
        }

        private void ListQuery(DatosUsuario datosUsuario, FiltroListaDeWorkflowsDto filtro, int pagina, string ordenarPor, DirOrden dirOrden, bool EsPrimeraCarga)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, (int)filtro.CantidadDeResultados);
            filtro.CentroId = datosUsuario.CentroId;
            filtro.NombreUsuario = datosUsuario.NombreUsuario;
            var datosWorkflow = servicio.ListarWorkFlows(paginacion, filtro);

            foreach (var instancia in datosWorkflow.Workflows)
            {
                if (instancia.MaterialCodigoSap == ConfigurationManager.AppSettings["CodigoSapSemillaSoja"])
                {
                    instancia.EsSemillaSoja = true;
                }
            }


            ViewBag.Caracteristicas = servicio.ListarCaracteristicaConfiguracionDeTabla(datosUsuario.CentroId, filtro.MaterialId ?? 0, datosUsuario.NombreUsuario);
            ViewBag.Items = datosWorkflow.Workflows;

            if (EsPrimeraCarga == true)
            {
                ViewBag.Workflows = datosWorkflow.WorkflowsCentro.OrderBy(x => x.Descripcion).ToSelectList(x => x.Codigo, x => x.Descripcion);

                var actividades = workflows.ObtenerWorkflowProximasAcciones(datosUsuario.NombreUsuario, datosUsuario.CentroId);
                if (actividades.All(x => x != "PuestoComando"))
                {
                    actividades.Add("PuestoComando");
                }
                ViewBag.Estados = actividades.ToSelectList(x => x, x => Textos.ResourceManager.GetString("Act" + x));
                ViewBag.Calles = servicio.ListarTodasLasCalles(datosUsuario.CentroId).ToSelectList(x => x.Id.ToString(), x => x.Nombre);
                ViewBag.TiposComerciales = servicio.ListarTiposComercialesPorCentro(datosUsuario.CentroId).ToSelectList(x => x.Id.ToString(), x => x.Descripcion);
                ViewBag.Calidades = datosWorkflow.Calidades.OrderBy(c => c.Descripcion).ToSelectList(x => x.Descripcion, x => x.Descripcion);
            }
        }

        private void SetearVista(DatosUsuario datosUsuario, int? materialId, bool esSustentable, bool sustentableMixto, bool sojaEPA)
        {
            esSustentable = ConfigurationManager.AppSettings["SepararAlmacenSustentable"] == "false" ? false : esSustentable;
            ViewBag.BalanzasObligatorias = servicio.BalanzasObligatoriasEnPuestoComando(datosUsuario.CentroId);
            ViewBag.Calles = servicio.ListarCalles(datosUsuario.CentroId).ToSelectList(x => x.Id.ToString(), x => x.Nombre);
            ViewBag.Balanzas = servicio.ListarBalanzasActivas(datosUsuario.CentroId, TipoVehiculo.Camión).ToSelectList(x => x.Id.ToString(), x => x.Nombre);
            var almacenes = materialId.HasValue ?
                                sustentableMixto ? servicio.ListarAlmacenesPorMaterialYCentroSustentableMixto(datosUsuario.CentroId, materialId.Value)
                                    : servicio.ListarAlmacenesPorMaterialYCentro(datosUsuario.CentroId, materialId.Value, esSustentable)
                                : servicio.ListarAlmacenesPorCentroYesSustentable(datosUsuario.CentroId, esSustentable);

            almacenes = sojaEPA ? servicio.ListarAlmacenesPorMaterialYCentroEPA(datosUsuario.CentroId, materialId.Value) : almacenes;

            ViewBag.Almacenes = almacenes.ToSelectList(x => x.Id.ToString(), x => x.Descripcion);

            var hidraulicas = new List<PuestosDeCargaDescargaDto>();
            if (!PermisosHelper.Is(PermisosScato.HidraulicasEspeciales))
            {
                hidraulicas = servicio.ListarHidraulicasPorCriterioSustentable(datosUsuario.CentroId, esSustentable, sustentableMixto, true).ToList();
            } else
                hidraulicas = servicio.ListarHidraulicasPorCriterioSustentable(datosUsuario.CentroId, esSustentable, sustentableMixto).ToList();

            ViewBag.Hidraulicas = new MultiSelectList(hidraulicas, "Id", "Nombre");
        }

        public ActionResult ConfigurarTabla()
        {
            return View();
        }
        [DatosUsuario]
        public ActionResult Rechazar(string instancesId, DatosUsuario datosUsuario)
        {
            log.Debug("Rechazando : {0}", instancesId);
            var patentesInvalidas = new List<string>();
            if (instancesId != null)
            {
                foreach (var guid in instancesId.Split(',').Select(x => new Guid(x)))
                {
                    if (!workflows.VerificarExistenciaDeWorkflowPorGuid(guid))
                    {
                        var patente = servicio.ObtenerPatentePorGuid(guid);
                        patentesInvalidas.Add(patente);
                    }
                }
            }
            if (patentesInvalidas.Any())
            {
                ViewBag.Error = "No es posible rechazar las siguientes patentes porque el Workflow falló:";
                ViewBag.PatentesInvalidas = patentesInvalidas;
                log.Error("Rechazar - Workflow falló para las patentes: {0}", string.Join(",", patentesInvalidas.ToArray()));
            }

            var controlRecorrido = SetearVistaRechazar(instancesId, datosUsuario);
            return View("TransportistaRechazado", controlRecorrido);

        }

        private ControlRecorridoDto SetearVistaRechazar(string instancesId, DatosUsuario datosUsuario)
        {
            ViewBag.Motivos = servicio.ListarMotivos().ToSelectList(x => x.Descripcion, x => x.Descripcion);
            ViewBag.InstanceIds = instancesId;
            var controlRecorrido = new ControlRecorridoDto
            {
                NombreUsuario = datosUsuario.NombreUsuario,
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId
            };
            return controlRecorrido;
        }

        [DatosUsuario]
        [HttpPost]
        public ActionResult TransportistaRechazado(ControlRecorridoDto controlRecorrido, string instanceIds, DatosUsuario datosUsuario)
        {
            controlRecorrido.Decision = false;
            controlRecorrido.Fecha = DateTime.Now;
            var patentesInvalidas = new List<string>();

            //validar etapa actual
            if (instanceIds != null)
            {
                foreach (var camion in instanceIds.Split(',').Select(x => new Guid(x)))
                {
                    if (workflows.VerificarExistenciaDeWorkflowPorGuid(camion))
                    {
                        var accion = workflows.ObtenerWorkflowProximaAccion(camion).ProximaAccion;
                        var vehiculo = servicio.ObtenerVehiculoPorGuid(camion);
                        if (accion == "PesadaBruto" && (vehiculo.TipoVehiculo == TipoVehiculo.Tren || vehiculo.TipoVehiculo == TipoVehiculo.Bitren))
                        {
                            var workflow = servicio.ObtenerDatosDeInstanciaPorGuid(camion);
                            var servicioWf = factoryPesada.CrearServicio(workflow.WorkflowDefinicionId);

                            controlRecorrido.Actividad = Textos.ActPuestoComando;
                            controlRecorrido.ActividadXaml = accion;
                            controlRecorrido.WorkflowInstanceId = camion;
                            controlRecorrido.Automatizado = false;
                            controlRecorrido.Decision = true;

                            servicioWf.Pesada(camion,
                                vehiculo.PesoBrutoOrigen ?? 0, 0, null, null, 0, null, false, DateTime.Now, controlRecorrido);
                        }
                        else if (accion == "PuestoComando")
                        {
                            var workflow = servicio.ObtenerDatosDeInstanciaPorGuid(camion);
                            var servicioWf = factory.CrearServicio(workflow.WorkflowDefinicionId);

                            controlRecorrido.Actividad = Textos.ActPuestoComando;
                            controlRecorrido.ActividadXaml = "PuestoComando";
                            controlRecorrido.WorkflowInstanceId = (camion);

                            servicioWf.PuestoComando(camion, controlRecorrido);
                        }
                        else if (accion == "EnPlayaExterna")
                        {
                            var workflow = servicio.ObtenerDatosDeInstanciaPorGuid(camion);
                            var servicioWf = factoryejecutar.CrearServicio(workflow.WorkflowDefinicionId);

                            controlRecorrido.Actividad = Textos.ActEnPlayaExterna;
                            controlRecorrido.ActividadXaml = "EnPlayaExterna";
                            controlRecorrido.WorkflowInstanceId = (camion);

                            servicioWf.Ejecutar(camion, controlRecorrido);
                        }
                        else
                        {
                            var patente = servicio.ObtenerPatentePorGuid(camion);
                            patentesInvalidas.Add(patente + " - " + accion);
                            log.Error("Falló el rechazo del camión ya que no se encuentra en Puesto Comando ni en Playa Externa o vagón en Pesada bruto");
                        }
                    }
                }
                if (patentesInvalidas.Any())
                {
                    ViewBag.Error = "No es posible rechazar las siguientes patentes porque no se encuentran en Puesto Comando ni en Playa Externa o los vagones en Pesada bruto:";
                    ViewBag.PatentesInvalidas = patentesInvalidas;
                }
                else if (ModelState.IsValid)
                {
                    return new AjaxEditSuccessResult();
                }
            }
            SetearVistaRechazar(instanceIds, datosUsuario);
            return View(controlRecorrido);
        }



        [DatosUsuario]
        [HttpPost]
        public ActionResult ConfigurarTabla(ConfiguracionDeTablaDto model, DatosUsuario datosUsuario)
        {

            if (ModelState.IsValid)
            {
                model.CentroId = datosUsuario.CentroId;
                var resultado = servicioComandos.Ejecutar(new ActualizarConfiguracionDeTabla { Dto = model, Usuario = datosUsuario.NombreUsuario });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            return View(model);
        }

        [DatosUsuario]
        public JsonResult ListarCaracteristicasConfiguracion(DatosUsuario datosUsuario, int materialId)
        {
            var caracteristicas = servicio.ListarCaracteristicasDeCalidadPorConfiguracion(datosUsuario.CentroId, materialId, datosUsuario.NombreUsuario);

            return Json(caracteristicas.Select(s => new { Descripcion = s.CaracteristicaDeCalidadDesc, Id = s.CaracteristicaDeCalidadId, s.Visible }), JsonRequestBehavior.AllowGet);
        }

        private void CargarMotivos()
        {
            ViewBag.Motivos = servicio.ListarMotivos().ToSelectList(x => x.Descripcion, x => x.Descripcion);
        }
    }
}
