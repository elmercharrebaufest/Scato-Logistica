using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;
using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadIngresoRemitoTerceros)]
    public class IngresoRemitoTercerosController : DocumentoIngresoController
    {
        private readonly IServicioActividadFactory<IIngresoRemitoTercerosService> factory;
        private readonly IListaDeWorkflows workflows;

        public IngresoRemitoTercerosController(ILogger log, IServicioRepositorio servicio, IServicioActividadFactory<IIngresoRemitoTercerosService> factory, IServicioComandos servicioComandos, IListaDeWorkflows workflows)
            : base(log, servicio, servicioComandos)
        {
            this.factory = factory;
            this.workflows = workflows;
        }

        [DatosUsuario]
        public ActionResult Index(string workflow, DatosUsuario datosUsuario, int cargaDeCupoId = 0)
        {
            if (!servicio.WorkflowActivoConDefinicionActiva(workflow))
            {
                return RedirectToAction("Index", "ListaDeCamiones");
            }

            var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflow);
            var ordenDeDescarga = servicio.ObtenerNumeroOrdenDeDescargaGenerado(datosUsuario.CentroId);
            SetearVista(workflowObj);
            var remito = new RemitoDto { OrdenDeDescarga = ordenDeDescarga, FechaOD = DateTime.Today, EsRemitoProveedor = true };
            if (cargaDeCupoId != 0)
            {
                var cupo = servicio.ObtenerCupoPorId(cargaDeCupoId);
                remito.PatenteCamion = cupo.Patente;
                remito.MaterialId = cupo.MaterialId.GetValueOrDefault();
                remito.Material = cupo.MaterialDescripcion;
            }
            return View(remito);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Index(string workflow, RemitoDto orden, DatosUsuario datosUsuario)
        {
            var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflow);
            ConsultarPagoTasaMunicipal(datosUsuario.CentroId, orden.PatenteCamion, orden.PatenteAcoplado, null, orden.TipoVehiculo, string.Empty, false,  orden.MaterialId);

            if (datosUsuario.CentroId == 0)
            {
                TempData["Alerta"] = Textos.SeleccionarCentro_Error;
                TempData["TipoAlerta"] = TipoAlerta.Advertencia;
                return RedirectToAction("Index", "ListaDeCamiones");
            }

            var remito = servicio.ObtenerRemitoPorNroRemito(orden.Remito);
            if (remito != null)
            {
                ModelState.AddModelError("Remito", Textos.IngresarRemito_Existente);
                SetearVista(workflowObj);
                return View(orden);
            }

            var otroRecorridoDelChofer = servicio.ObtenerOtroRecorridoDelChofer(orden.Chofer.Id);
            if (otroRecorridoDelChofer != null)
            {
                ModelState.AddModelError("", string.Format(Textos.Error_ChoferYaEstaEnPlanta, orden.Chofer.NombreCompleto, otroRecorridoDelChofer.NumeroDocumentoIngreso, otroRecorridoDelChofer.Patente));
                SetearVista(workflowObj);
                return View(orden);
            }

            if (workflows.ObtenerWorkflowPorPatente(orden.PatenteCamion) != null)
            {
                TempData["Alerta"] = Textos.OrdenCargaInterna_PatenteEnOtroWorkflow;
                TempData["TipoAlerta"] = TipoAlerta.Advertencia;
                return RedirectToAction("Index", "ListaDeCamiones");
            }

            var resultadoChofer = SetearChofer(orden.Chofer);
            if (resultadoChofer == false)
            {
                SetearVista(workflowObj);
                return View(orden);  
            }
            var transportistaId = orden.TransportistaId;
            var resultadoTransportista = SetearTransportista(ref transportistaId, orden.TipoComercialId, orden.EsTransportista);
            orden.TransportistaId = transportistaId;
            if (!resultadoTransportista)
            {
                SetearVista(workflowObj);
                return View(orden);
            }

            orden.EsRemitoProveedor = true;
            orden.OrigenCodigoSap = servicio.ObtenerProveedor(orden.OrigenId).CodigoSap;
            orden.TipoDeWorkflow = workflowObj.TipoDeWorkflow;
            orden.PatenteCamion = orden.PatenteCamion != null ? orden.PatenteCamion.ToUpper() : "";
            orden.PatenteAcoplado = orden.PatenteAcoplado != null ? orden.PatenteAcoplado.ToUpper() : "";
            orden.Remito = orden.Remito.Replace('-', 'R');
            
            int workflowDefinicionId = servicio.ObtenerUltimaWorkflowDefinicionPorCordigo(workflow);
            var servicioWf = factory.CrearServicio(workflowDefinicionId);

            var controlRecorrido = new ControlRecorridoDto
            {
                Actividad = Textos.ActIngresoRemitoTerceros,
                ActividadXaml = "IngresoRemitoTerceros",
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                NombreUsuario = datosUsuario.NombreUsuario
            };

            if (!Validar(orden, datosUsuario))
            {
                SetearVista(workflowObj);
                return View(orden);
            }

            var resultadoActividad = servicioWf.IngresoRemitoTerceros(orden, datosUsuario.CentroId, workflow, workflowDefinicionId, datosUsuario.NombreUsuario, controlRecorrido) as ResultadoCrearWorkflow;
            if (resultadoActividad.HayErrores)
            {
                ModelState.AgregarErrores(resultadoActividad);
                SetearVista(workflowObj);
                return View(orden);
            }

            if (ResultadoPagoTasaMunicipal != null && ResultadoPagoTasaMunicipal.IdPago != 0)
                ActualizarTasaMunicipal(resultadoActividad.InstanciaWorkflowId, ResultadoPagoTasaMunicipal.IdPago, ResultadoPagoTasaMunicipal.IdDiferenciaDePago ?? 0);

            return RedirectToAction("Index", "ListaDeCamiones", new { id = resultadoActividad.InstanciaWorkflowId });
        }

        private void SetearVista(WorkflowDto workflow)
        {
            var tiposComerciales = servicio.ListarTiposComercialesPorWfCodigo(workflow.Codigo);

            ViewBag.TiposComerciales = tiposComerciales.ToSelectList(f => f.Id.Value.ToString(CultureInfo.InvariantCulture), f => f.Descripcion);
            ViewBag.TiposComercialesTransportista = tiposComerciales.Where(x => !x.TransportistaEsProveedor).Select(y => y.Id.ToString()).ToList();
            ViewBag.TiposDocumentos = servicio.ListarTiposDocumentoIdentidad().ToSelectList(f => f.Id.ToString(), f => f.DescripcionCorta);
            ViewBag.Workflow = workflow.Codigo;
            ViewBag.EsIngreso = workflow.TipoDeWorkflow == TipoDeWorkflow.Ingreso;
            var almacenes = servicio.ListarAlmacenesPorCentro(workflow.CentroId);
            ViewBag.Almacenes = almacenes.GroupBy(x => x.Descripcion).Select(x => new AlmacenDto { Id = x.Select(y => y.Id)
                .FirstOrDefault(), Descripcion = x.Key }).ToSelectList(f => f.Id.ToString(), f => f.Descripcion);
            var calles = servicio.ListarCalles(workflow.CentroId);
            ViewBag.Calles = calles.GroupBy(x => x.Nombre).Select(x => new CalleDto { Id = x.Select(y => y.Id).FirstOrDefault(), Nombre = x.Key }).ToSelectList(f => f.Id.ToString(), f => f.Nombre,"3");
            var materiales = servicio.ListarMaterialesPorWorkflow(workflow.Id, workflow.CentroId);
            ViewBag.Materiales = materiales.ToSelectList(f => f.MaterialId.ToString(CultureInfo.InvariantCulture), f => f.MaterialDesc);
            ViewBag.MaterialesConAnexo = materiales.Where(x => x.RequiereAnexoInase).Select(y => y.MaterialId.ToString()).ToList();
        }
        protected virtual bool Validar(RemitoDto orden, DatosUsuario usuario)
        {
            var material = servicio.ObtenerMaterial(orden.MaterialId);

            if (material != null && material.Descripcion == "RESIDUOS ORGANICOS" && orden.Almacen_Id == null)
            {
                ModelState.AddModelError("Almacen_Id", Textos.OrdenInterna_AlmacenRequerido);
                return false;
            }
            return true;
        }
    }
}