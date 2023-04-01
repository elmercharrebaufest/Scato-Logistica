using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Filtros;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadCoordinacion)]
    public class CoordinacionController : BaseController
    {
        private readonly IServicioActividadFactory<ICoordinacionService> factory;
        private readonly IConfiguracionProvider configuracion;
        private readonly ILogger log;
        private readonly IListaDeWorkflows workflows;
        private readonly IServicioComandos servicioComandos;

        public CoordinacionController(ILogger log, IServicioRepositorio servicio, IServicioActividadFactory<ICoordinacionService> factory, IConfiguracionProvider configuracion, IListaDeWorkflows workflows, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.factory = factory;
            this.configuracion = configuracion;
            this.workflows = workflows;
            this.servicioComandos = servicioComandos;
        }

        public ActionResult Index(Guid id, int pagina = 1, string ordenarPor = "AnalisisDeCalidad", DirOrden dirOrden = DirOrden.Asc)
        {
            var recorrido = servicio.ObtenerRecorridoPorGuid(id);

            Listar(recorrido, pagina, ordenarPor, dirOrden);
            return View(recorrido);
        }

        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(Guid id, int pagina = 1, string ordenarPor = "AnalisisDeCalidad", DirOrden dirOrden = DirOrden.Asc)
        {
            var recorrido = servicio.ObtenerRecorridoPorGuid(id);
            Listar(recorrido, pagina, ordenarPor, dirOrden);
            return View("Listar", recorrido);
        }

        private void Listar(RecorridoDto recorrido, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);
            var info = servicio.ObtenerInformacionCartaPorte(recorrido.Id);
            var caracteristicas = servicio.ListarAnalisisYCaladoPorCaracteristicaNoAceptables(recorrido.InstanciaWorkflow);
            var mensajes = servicio.ListarAnalisisYCaladoPorCaracteristicaConAdvertencia(recorrido.InstanciaWorkflow);
            var cupo = servicio.ObtenerCupoPorRecorrido(recorrido.Id);

            ViewBag.Caracteristicas = servicio.ListarPaginadoAnalisisYCaladoPorCaracteristica(recorrido.InstanciaWorkflow, paginacion);
            ViewBag.EsAceptable = !caracteristicas.Any();
            ViewBag.CaracteristicasNoAceptables = caracteristicas;
            ViewBag.CaracteristicasMensajes = mensajes;
            ViewBag.EsSojaSustentable = recorrido.Establecimiento != null;
            ViewBag.EsSojaEPA = recorrido.Establecimiento != null && recorrido.Establecimiento.EsSojaEPA;
            ViewBag.AgenteCompras = !string.IsNullOrEmpty(info.AgenteCompras) ? info.AgenteCompras : Textos.No;
            ViewBag.TrigoEspecial = info.TrigoEspecial;
            ViewBag.PatenteAcoplado = recorrido.Vehiculo != null ? recorrido.Vehiculo.PatenteAcoplado : string.Empty;
            ViewBag.FechaIngreso = string.Format(CultureInfo.CurrentCulture, "{0:d/M/yyyy}", recorrido.FechaInicio);
            ViewBag.TarjetaDeAcceso = recorrido.TarjetaDeAcceso;
            ViewBag.TitularCP = info.TitularCartaPorte;
            ViewBag.TieneEntregador = !string.IsNullOrEmpty(info.Entregador) ? info.Entregador : Textos.No;
            ViewBag.RtteComercial = !string.IsNullOrEmpty(info.RtteComercial) ? info.RtteComercial : Textos.No;
            ViewBag.CTG = !string.IsNullOrEmpty(info.CTG) ? info.CTG : Textos.No;
            ViewBag.Cupo = info.Cupo;
            ViewBag.CaracteristicasNoCorrenspodenEspecial = false;
            ViewBag.Comentario = recorrido.Calado.Comentario != null ? recorrido.Calado.Comentario : "";
            if (info.TrigoEspecial)
            {
                ViewBag.CaracteristicasNoCorrenspodenEspecial = servicio.ValoresNoCorrespondenAEspecial(recorrido.InstanciaWorkflow, recorrido.Material.Id, recorrido.Centro.Id);
            }

            var desdeEsMenor = recorrido.Centro.HorarioDesde < recorrido.Centro.HorarioHasta;
            var desde = recorrido.Centro.HorarioDesde;
            var hasta = recorrido.Centro.HorarioHasta;
            if (info.EnvioDirectoCamara && recorrido.Material.CodigoSAP == configuracion.AppSettings["CodigoSapSemillaSoja"] && recorrido.Centro.HorarioDesde.HasValue && recorrido.Centro.HorarioHasta.HasValue && ((desdeEsMenor && desde <= recorrido.Calado.FechaCreacion.Value.Hour && recorrido.Calado.FechaCreacion.Value.Hour < hasta) ||
                                            (!desdeEsMenor && !(hasta <= recorrido.Calado.FechaCreacion.Value.Hour && recorrido.Calado.FechaCreacion.Value.Hour < desde))))
            {
                ViewBag.EnvioDirectoACamara = string.Format(Textos.ProveedorRangoEnvioCamara);
            }
            ViewBag.CupoEsFabrica = false;
            if (cupo != null)
            {
                ViewBag.CupoEsFabrica = cupo.Camara == "03"; //03 es de fabrica (hasta el momento es el unico establecido)
            }
        }

        [DatosUsuario]
        [HttpParamAction]
        public ActionResult Rechazar(string codigoWf, int workflowDefinicionId, Guid instanceId, DatosUsuario datosUsuario)
        {
            CargarMotivos();
            ViewBag.Workflow = codigoWf;
            ViewBag.WorkflowDefinicionId = workflowDefinicionId;
            var controlRecorrido = new ControlRecorridoDto
            {
                WorkflowInstanceId = instanceId,
                NombreUsuario = datosUsuario.NombreUsuario,
                Actividad = Textos.ActCoordinacion + "/" + Textos.Rechazar,
                ActividadXaml = "Coordinacion",
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId
            };

            return View("_TransportistaRechazado", controlRecorrido);
        }

        [DatosUsuario]
        [HttpParamAction]
        public ActionResult Recalar(string codigoWf, int workflowDefinicionId, Guid instanceId, DatosUsuario datosUsuario)
        {
            CargarMotivos();
            ViewBag.Workflow = codigoWf;
            ViewBag.WorkflowDefinicionId = workflowDefinicionId;
            var controlRecorrido = new ControlRecorridoDto
            {
                WorkflowInstanceId = instanceId,
                NombreUsuario = datosUsuario.NombreUsuario,
                Actividad = Textos.ActCoordinacion + "/" + Textos.Coordinacion_Recalar,
                ActividadXaml = "Coordinacion",
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId
            };

            return View("_TransportistaRecalar", controlRecorrido);
        }

        [DatosUsuario]
        [HttpParamAction]
        public ActionResult EnviarACamara(string codigoWf, int workflowDefinicionId, Guid instanceId, int caladoId, int materialId, DatosUsuario datosUsuario)
        {
            var camaraExcepcion = servicio.ObtenerCamaraDeExcepcionDescuento(instanceId, materialId, datosUsuario.CentroId);

            ViewBag.Camaras = (camaraExcepcion != null ? new List<CamaraDto> { camaraExcepcion } : servicio.ListarCamaras()).ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
            var material = servicio.ObtenerMaterialPorCentroPorInstanceId(instanceId);
            ViewBag.Workflow = codigoWf;
            ViewBag.WorkflowDefinicionId = workflowDefinicionId;
            var caracteristicas = servicio.CaladoPorCaracteristicaQueSeEnvianACamara(caladoId);

            var caracteristicasDeCoordinacion = servicio.ListarCaracteristicasDeCalidadPorMaterialSinExceptuadas(instanceId, materialId, datosUsuario.CentroId).Where(x => x.SituacionEnvioACamara == EnvioACamara.EnCoordinacion).ToList();
            var muestraEnvioACamaraDto = new MuestraEnvioACamaraDto
            {
                NombreUsuario = datosUsuario.NombreUsuario,
                CaladoId = caladoId,
                CamaraId = camaraExcepcion != null ? camaraExcepcion.Id : (material != null && material.CamaraId.HasValue ? material.CamaraId.Value : 0),
                CaracteristicasDeCalidad = caracteristicasDeCoordinacion.Select(x => new CaracteristicaDeCalidadDto { Descripcion = x.Descripcion, Id = x.Id, SeEnviaACamara = caracteristicas.Any(b => b == x.Id), SituacionEnvioACamara = x.SituacionEnvioACamara }).ToList(),
                Actividad = codigoWf,
                WorkflowInstanceId = instanceId,
                CentroId = datosUsuario.CentroId,
                HuboExcepcion = camaraExcepcion != null
            };
            return View("_TransportistaEnviaACamara", muestraEnvioACamaraDto);
        }

        [DatosUsuario]
        [HttpPost]
        [HttpParamAction]
        public ActionResult Aceptar(string codigoWf, int workflowDefinicionId, Guid instanceId, int caladoId, DatosUsuario datosUsuario)
        {
            var instaceWorflow = workflows.ObtenerWorkflowPorGuid(instanceId);

            if (instaceWorflow.ProximaAccion == "Coordinacion")
            {
                var serviciowf = factory.CrearServicio(workflowDefinicionId);

                var recorrido = CrearControlRecorrido(codigoWf, workflowDefinicionId, instanceId, datosUsuario, true);
                var resultado = serviciowf.Coordinacion(instanceId, DecisionCoordinacion.Aceptar, recorrido, new MuestraEnvioACamaraDto());
                if (!resultado.HayErrores)
                {
                    EnviarAPreLote(caladoId, datosUsuario.NombreUsuario, codigoWf, instanceId, datosUsuario.CentroId);
                    return RedirectToAction("Index", "ListaDeCamiones");
                }
                ModelState.AgregarErrores(resultado);
                return RedirectToAction("Index", new { id = recorrido.WorkflowInstanceId });
            }
            else
            {
                TempData["FlagMostrarValidacionEtapaAutomatica"] = true;
                TempData["messageEtapaAutomatica"] = $"La tarea seleccionada se encuentra en otro etapa : {instaceWorflow.ProximaAccion}";
                return RedirectToAction("Index", "ListaDeCamiones");
            }
        }

        [HttpPost]
        public ActionResult TransportistaRechazado(ControlRecorridoDto controlRecorrido, string workflow, int workflowDefinicionId)
        {
            var serviciowf = factory.CrearServicio(workflowDefinicionId);
            controlRecorrido.Decision = false;
            var resultado = serviciowf.Coordinacion(controlRecorrido.WorkflowInstanceId, DecisionCoordinacion.Rechazar, controlRecorrido, new MuestraEnvioACamaraDto());
            if (!resultado.HayErrores)
            {
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            ModelState.AgregarErrores(resultado);
            return RedirectToAction("Index", new { id = controlRecorrido.WorkflowInstanceId });
        }

        [HttpPost]
        public ActionResult TransportistaRecalar(ControlRecorridoDto controlRecorrido, int workflowDefinicionId, string workflow)
        {
            var serviciowf = factory.CrearServicio(workflowDefinicionId);
            var resultado = serviciowf.Coordinacion(controlRecorrido.WorkflowInstanceId, DecisionCoordinacion.Recalar, controlRecorrido, new MuestraEnvioACamaraDto());
            if (!resultado.HayErrores)
            {
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            ModelState.AgregarErrores(resultado);
            return RedirectToAction("Index", new { id = controlRecorrido.WorkflowInstanceId });
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult TransportistaEnviarACamara(MuestraEnvioACamaraDto envioACamara, string workflow, int workflowDefinicionId, DatosUsuario datosUsuario)
        {
            var serviciowf = factory.CrearServicio(workflowDefinicionId);
            var control = new ControlRecorridoDto
            {
                WorkflowInstanceId = envioACamara.WorkflowInstanceId,
                NombreUsuario = envioACamara.NombreUsuario,
                Actividad = Textos.Actividad_Coordinacion + "/" + "MuestraEnvioACamaraDto",
                ActividadXaml = "Coordinacion",
                Decision = true,
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId
            };

            var resultado = serviciowf.Coordinacion(envioACamara.WorkflowInstanceId, DecisionCoordinacion.EnviarACamara, control, envioACamara);
            if (!resultado.HayErrores)
            {
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            ModelState.AgregarErrores(resultado);
            return RedirectToAction("Index", new { id = envioACamara.WorkflowInstanceId });
        }

        private ControlRecorridoDto CrearControlRecorrido(string codigoWf, int workflowDefinicionId, Guid instanceId, DatosUsuario datosUsuario, bool decision)
        {
            ViewBag.Workflow = codigoWf;
            ViewBag.WorkflowDefinicionId = workflowDefinicionId;
            return new ControlRecorridoDto
            {
                WorkflowInstanceId = instanceId,
                NombreUsuario = datosUsuario.NombreUsuario,
                Actividad = Textos.ActCoordinacion,
                ActividadXaml = "Coordinacion",
                Decision = decision,
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId
            };
        }

        private void CargarMotivos()
        {
            ViewBag.Motivos = servicio.ListarMotivos().ToSelectList(x => x.Descripcion, x => x.Descripcion);
        }

        private void EnviarAPreLote(int caladoId, string nombreUsuario, string codigoWf, Guid instanceId, int centroId)
        {
            var muestraEnvioACamara = servicio.ObtenerMuestraEnvioACamaraPorCalado(caladoId);
            if(muestraEnvioACamara == null)
            {
                var configuracionHorarioPreLote = servicio.ListarConfiguracionesGenerales(Constantes.ConfiguracionGeneral.Pantalla.PreLote);
                var diaActual = DateTime.Now;
                var confDesde = configuracionHorarioPreLote.FirstOrDefault(q => q.Nombre == Constantes.ConfiguracionGeneral.PreLote.HorarioNocturnoDesde)?.Valor;
                var congHasta = configuracionHorarioPreLote.FirstOrDefault(q => q.Nombre == Constantes.ConfiguracionGeneral.PreLote.HorarioNocturnoHasta)?.Valor;

                int desde = 0;
                int hasta = 0;
                if (int.TryParse(confDesde, out desde) && int.TryParse(congHasta, out hasta))
                {
                    if (diaActual.Hour >= desde || diaActual.Hour <= hasta)
                    {
                        var camaras = servicio.ListarCamaras();
                        var muestra = new MuestraEnvioACamaraDto
                        {
                            NombreUsuario = nombreUsuario,
                            CaladoId = caladoId,
                            CamaraId = camaras.FirstOrDefault().Id, // obetener alguna camara,
                            CaracteristicasDeCalidad = null,
                            Actividad = codigoWf,
                            WorkflowInstanceId = instanceId,
                            FechaDescarga = DateTime.Now,
                            CentroId = centroId,
                            HuboExcepcion = false,
                            EsPreLote = true
                        };
                        var resultado = servicioComandos.Ejecutar(new CrearEnvioACamara { Dto = muestra }) as ResultadoCrear;
                    }
                }

            }
        }
    }
}