using System;
using System.Collections.Generic;
using System.Globalization;
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
using Newtonsoft.Json;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AbmAsignacionDeRecorrido)]
    public class AsignacionDeRecorridoController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;
        private readonly IListaDeWorkflows workflows;
        private readonly IServicioActividadFactory<IAutorizarDescuentosEntregadorService> factoryDescuentos;
        private readonly IServicioActividadFactory<ICoordinacionService> factoryCoordinacion;

        public AsignacionDeRecorridoController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos, IListaDeWorkflows workflows, IServicioActividadFactory<IAutorizarDescuentosEntregadorService> factoryDescuentos, IServicioActividadFactory<ICoordinacionService> factoryCoordinacion)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
            this.workflows = workflows;
            this.factoryDescuentos = factoryDescuentos;
            this.factoryCoordinacion = factoryCoordinacion;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, string filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(filtro, pagina, ordenarPor, dirOrden, datosUsuario.CentroId);
            return View((object)filtro);
        }

        [AjaxOnly]
        [ActionName("Index")]
        [DatosUsuario]
        public ActionResult Listar(DatosUsuario datosUsuario, string filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(filtro, pagina, ordenarPor, dirOrden, datosUsuario.CentroId);
            return View("Listar", (object)filtro);
        }

        private void ListQuery(string filtro, int pagina, string ordenarPor, DirOrden dirOrden, int centroId)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);

            ViewBag.Items = servicio.ListarPaginadoAsignacionesDeRecorrido(centroId, filtro, paginacion);
        }

        [DatosUsuario]
        public ActionResult Crear(DatosUsuario datosUsuario)
        {
            CargarCombos(datosUsuario.CentroId);
            return View(new AsignacionDeRecorridoDto {FechaDesde = DateTime.Today, FechaHasta = DateTime.Today});
        }


        [DatosUsuario]
        [HttpPost]
        public ActionResult Crear(AsignacionDeRecorridoDto model, DatosUsuario datosUsuario)
        {
            ModelState.Remove("CalidadId");
            if (ModelState.IsValid)
            {
                model.CentroId = datosUsuario.CentroId;
                var resultado = servicioComandos.Ejecutar(new CrearAsignacionDeRecorrido { Dto = model });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            CargarCombos(datosUsuario.CentroId, model);
            return View(model);
        }

        [DatosUsuario]
        public ActionResult Modificar(int id, DatosUsuario datosUsuario)
        {
            var asignacionAModificar = servicio.ObtenerAsignacionDeRecorrido(id);
            CargarCombos(datosUsuario.CentroId, asignacionAModificar);
            return View(asignacionAModificar);

        }

        [DatosUsuario]
        [HttpPost]
        public ActionResult Modificar(AsignacionDeRecorridoDto model, DatosUsuario datosUsuario)
        {
            ModelState.Remove("CalidadId");
            if (ModelState.IsValid)
            {
                model.CentroId = datosUsuario.CentroId;
                var resultado = servicioComandos.Ejecutar(new ModificarAsignacionDeRecorrido { Dto = model });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            CargarCombos(datosUsuario.CentroId, model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarAsignacionDeRecorrido { Id = id });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        private void CargarCombos(int centroId, AsignacionDeRecorridoDto model = null)
        {
            if (model == null)
            {
                model = new AsignacionDeRecorridoDto {MaterialPorCentroId = -1};
            }

            ViewBag.Calidades =
                servicio.ListarCalidadesPorMaterialyCentro(model.MaterialPorCentroId)
                        .ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
            ViewBag.Workflows =
                servicio.ListarWorkflowsPorCentro(centroId).Where(w => w.Activo)
                        .ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
            ViewBag.Calles =
                servicio.ListarCalles(centroId)
                        .ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Nombre,
                                      model.CalleId.ToString(CultureInfo.InvariantCulture));
            var balanzas = servicio.ListarBalanzas(centroId, Dominio.Enums.TipoVehiculo.Camión);
            ViewBag.BalanzasBruto = balanzas.ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Nombre,
                                      (model.BalanzaBrutoId ?? 0).ToString(CultureInfo.InvariantCulture));
            ViewBag.BalanzasTara = balanzas.ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Nombre,
                                      (model.BalanzaTaraId ?? 0).ToString(CultureInfo.InvariantCulture));
            ViewBag.Almacenes =
                servicio.ListarAlmacenesPorCentro(centroId)
                        .ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion,
                                      model.AlmacenDestinoId.ToString(CultureInfo.InvariantCulture));
            ViewBag.Hidraulicas = new MultiSelectList(servicio.ListarHidraulicas(centroId,false), "Id", "Nombre");

        }

        public JsonResult CargarCalidades(int? materialPorCentroId)
        {
            if (materialPorCentroId.HasValue && materialPorCentroId != 0)
            {
                var calidades =
                    servicio.ListarCalidadesPorMaterialyCentro(materialPorCentroId.Value)
                            .ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
                return Json(new { calidades = calidades}, JsonRequestBehavior.AllowGet);
            }
            return Json(new { calidades = new List<SelectList>() }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public void AutomatizacionEtapas()
        {            
            try
            {
                var instacesWorflows = workflows.ListarWorflows();
                var fechaFiltro = DateTime.Now;

                log.Debug($"AutomatizacionEtapas: Inicio Fecha {fechaFiltro} ");

                // Obtengo la configuracion del reposito
                var configuracion = servicio.ObtenerConfiguracionAutomatizacionEtapas();

                //Obtengo las instancias disponibles y que cumplan los filtros.
                var instancias = instacesWorflows
                    .Where(w => configuracion.Any(c => c.CentroId == w.CentroId && c.Actividad?.ToLower().Trim() == w.ProximaAccion?.ToLower().Trim() && w.FechaUltimaModificacion != null && Convert.ToInt32((fechaFiltro - w.FechaUltimaModificacion).Value.TotalMinutes) >= c.MinutosEjecucion)).ToList();

                log.Debug($"AutomatizacionEtapas: Total Instancias {instancias?.Count} ");

                //Log Instance Worflow
                var instanciasLog = instancias.Select(s => new { Guid = s.Id, FechaUltimaModificacion = s.FechaUltimaModificacion, Minutos = Convert.ToInt32((fechaFiltro - s.FechaUltimaModificacion).Value.TotalMinutes), ProximaEtapa = s.ProximaAccion }).ToList();
                log.Debug($"AutomatizacionEtapas:  Instancias {JsonConvert.SerializeObject(instanciasLog)} ");

                for (int i = 0; i < instancias.Count; i++)
                {
                    var WorkflowIntancia = instancias[i];

                    try
                    {                        
                        log.Debug($"AutomatizacionEtapas: Procesando WorflowId: {WorkflowIntancia.Id} ");

                        var recorrido = servicio.ObtenerRecorridoPorGuid(WorkflowIntancia.Id);
                        if (recorrido is null)
                        {
                            log.Debug($"AutomatizacionEtapas: No se encontro el recorrido del WorflowId: {WorkflowIntancia.Id} ");
                            continue;
                        }

                        var resultado = new Resultado();

                        var modelRecorrido = new ControlRecorridoDto
                        {
                            WorkflowInstanceId = WorkflowIntancia.Id,
                            Comentario = $"Supera tiempo de espera en {WorkflowIntancia.ProximaAccion}.",
                            NombreUsuario = "AUTOMATICO",
                            Actividad = Textos.ActAutorizarDescuentosEntregador,
                            ActividadXaml = "AutorizarDescuentosEntregador",
                            Decision = true
                        };

                        log.Debug($"AutomatizacionEtapas: Se ejecutara la Acción {WorkflowIntancia.ProximaAccion} al WorflowId: {WorkflowIntancia.Id} ");

                        switch (WorkflowIntancia.ProximaAccion)
                        {
                            case "Coordinacion":

                                modelRecorrido.Actividad = Textos.ActCoordinacion;
                                modelRecorrido.ActividadXaml = "Coordinacion";
                                var serviciowfCoordinacion = factoryCoordinacion.CrearServicio(recorrido.WorkflowDefinicionId);
                                resultado = serviciowfCoordinacion.Coordinacion(WorkflowIntancia.Id, DecisionCoordinacion.Aceptar, modelRecorrido, new MuestraEnvioACamaraDto());
                                break;

                            case "AutorizarDescuentosEntregador":

                                modelRecorrido.Actividad = Textos.ActAutorizarDescuentosEntregador;
                                modelRecorrido.ActividadXaml = "AutorizarDescuentosEntregador";
                                var serviciowfAutorizarDescuentosEntregador = factoryDescuentos.CrearServicio(recorrido.WorkflowDefinicionId);
                                resultado = serviciowfAutorizarDescuentosEntregador.AutorizarDescuentosEntregador(WorkflowIntancia.Id, modelRecorrido);
                                break;

                            default:
                                log.Debug($"AutomatizacionEtapas: No existe configuracion de Automatizacion para la etapa : {WorkflowIntancia.ProximaAccion} WorflowId: {WorkflowIntancia.Id} ");
                                break;
                        }

                        if (configuracion.Any(a => WorkflowIntancia.ProximaAccion == a.Actividad))
                        {
                            if (!resultado.HayErrores)
                            {
                                log.Debug($"AutomatizacionEtapas: Se ejecuto la Acción {WorkflowIntancia.ProximaAccion} al WorflowId: {WorkflowIntancia.Id} Correctamente.");
                            }
                            else
                            {
                                log.Debug($"AutomatizacionEtapas: Ocurrio un error al ejecutara la Acción {WorkflowIntancia.ProximaAccion} al WorflowId: {WorkflowIntancia.Id} , Error : {resultado.Errores.FirstOrDefault().Key}|{resultado.Errores.FirstOrDefault().Value}");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error(e, "Ocurrio un error en el proceso de Automatizacion de etapas WorflowId : {0}.", WorkflowIntancia.Id);
                        continue;
                    }                    
                }
            }
            catch (Exception e)
            {
                log.Error(e, "Ocurrio un error no controlado al ejecutar el proceso de Automatizacion de etapas.");
            }
        }

    }
}
