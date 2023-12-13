using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio;
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

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadIngresarOrdenCargaInterna)]
    public class IngresarOrdenCargaInternaController : DocumentoIngresoController
    {
        private readonly IServicioActividadFactory<IIngresarOrdenCargaInternaService> factory;
        private readonly IListaDeWorkflows workflows;

        public IngresarOrdenCargaInternaController(ILogger log, IServicioRepositorio servicio, IServicioActividadFactory<IIngresarOrdenCargaInternaService> factory, IServicioComandos servicioComandos, IListaDeWorkflows workflows)
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
            SetearVista(workflowObj, datosUsuario.CentroId);
            var numeroOrden = servicio.ObtenerNumeroDocumentoGenerado().ToString(CultureInfo.InvariantCulture).PadLeft(8, '0');
            var orden = new OrdenCargaInternaDto { FechaEmision = DateTime.Now, NumeroOrden = numeroOrden };
            if(cargaDeCupoId != 0)
            {
                var cupo = servicio.ObtenerCupoPorId(cargaDeCupoId);
                orden.PatenteCamion = cupo.Patente;
                orden.MaterialId = cupo.MaterialId;
                orden.MaterialDesc = cupo.MaterialDescripcion;
            }
            return View(orden);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Index(string workflow, OrdenCargaInternaDto orden, DatosUsuario datosUsuario)
        {
            var workflowObje = servicio.ObtenerWorkflowPorCodigo(workflow);

            if (orden.TipoYOrdenDestino != null)
            {
                var domicilio = orden.TipoYOrdenDestino.Split('-');
                orden.TipoDomicilioDestino = int.Parse(domicilio[0]);
                orden.OrdenDomicilioDestino = int.Parse(domicilio[1]);
            }

            Validar(orden);

            if (!ModelState.IsValid)
            {
                SetearVista(workflowObje, datosUsuario.CentroId);
                ViewBag.ErrorAfip = Textos.OrdenCarga_ErrorValidacion;
                return View(orden);
            }

            if (orden.PatenteCamion != null)
            {
                orden.PatenteCamion = orden.PatenteCamion.ToUpper();
            }
            if (orden.PatenteAcoplado != null)
            {
                orden.PatenteAcoplado = orden.PatenteAcoplado.ToUpper();
            }

            if (workflows.ObtenerWorkflowPorPatente(orden.PatenteCamion) != null)
            {
                TempData["Alerta"] = Textos.PatenteEnOtroWorkflow;
                TempData["TipoAlerta"] = TipoAlerta.Advertencia;
                return RedirectToAction("Index", "ListaDeCamiones");
            }

            var resultadoChofer = SetearChofer(orden.Chofer);
            if (resultadoChofer == false)
            {
                var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflow);
                SetearVista(workflowObj, datosUsuario.CentroId);
                return View(orden);
            }

            var transportistaId = orden.TransportistaId;
            var resultadoTransportista = SetearTransportista(ref transportistaId, orden.TipoComercialId, orden.EsTransportista);
            orden.TransportistaId = transportistaId;
            if (!resultadoTransportista)
            {
                var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflow);
                SetearVista(workflowObj, datosUsuario.CentroId);
                return View(orden);
            }

            //if(orden.DerivadoGranarioHabilitado && !(orden.Demorado || orden.Rechazado))
            //{
            //    var domicilio = orden.TipoYOrdenDestino.Split('-');
            //    orden.TipoDomicilioDestino = int.Parse(domicilio[0]);
            //    orden.OrdenDomicilioDestino = int.Parse(domicilio[1]);
            //    var dominios = new List<string> { orden.PatenteCamion };
            //    if (!string.IsNullOrEmpty(orden.PatenteAcoplado))
            //    {
            //        dominios.Add(orden.PatenteAcoplado);
            //    }
            //    ConfirmarCTGVencidos(datosUsuario.CentroId);
            //    var resultadoAltaDummy = servicioComandos.Ejecutar(new AutorizarCpeDGDummy
            //    {
            //        TipoVehiculo = orden.TipoVehiculo,
            //        CentroId = datosUsuario.CentroId,
            //        MaterialId = orden.MaterialId,
            //        DestinoId = orden.DestinoId,
            //        DestinoPlanta = orden.PlantaDGDestino ?? 0,
            //        DestinoDomicilioTipo = orden.TipoDomicilioDestino ?? 0,
            //        DestinoDomicilioOrden = orden.OrdenDomicilioDestino ?? 0,
            //        TransportistaId = orden.TransportistaId,
            //        Dominios = dominios.ToArray(),
            //        KmRecorrer = !string.IsNullOrEmpty(orden.KmARecorrer) ? int.Parse(orden.KmARecorrer) : 0,
            //        ChoferCuit = orden.Chofer.Cuil,
            //        PagadorFleteId = orden.PagadorFleteId ?? 0,

            //    }) as ResultadoCartaPorteElectronicaDummy;
            //    if (resultadoAltaDummy.HayErrores)
            //    {
            //        SetearVista(workflowObje, datosUsuario.CentroId);
            //        ViewBag.ErrorAfip = resultadoAltaDummy.Errores.Values.First();
            //        return View(orden);
            //    }

            //    var resultadoAnulacionDummy = servicioComandos.Ejecutar(new AnularCPEDGDummy
            //    {
            //        CentroId = datosUsuario.CentroId,
            //        NroOrden = (int)resultadoAltaDummy.NroOrden,
            //        Sucursal = resultadoAltaDummy.Sucursal,
            //        TipoCPE = (short)resultadoAltaDummy.TipoCPE,
            //    });
            //    if (resultadoAnulacionDummy.HayErrores)
            //    {
            //        SetearVista(workflowObje, datosUsuario.CentroId);
            //        ViewBag.ErrorAfip = resultadoAnulacionDummy.Errores.Values.First();
            //        return View(orden);
            //    }
            //}

            var controlRecorrido = new ControlRecorridoDto
            {
                Actividad = Textos.ActIngresarOrdenCargaInterna,
                ActividadXaml = "IngresarOrdenCargaInterna",
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                NombreUsuario = datosUsuario.NombreUsuario,
                Comentario = orden.Rechazado ? $"Vehiculo Rechazado. {orden.MotivoRechazo}" : (orden.Demorado ? $"Vehiculo Demorado. {orden.MotivoDemora}" : string.Empty)
            };

            int workflowDefinicionId = servicio.ObtenerUltimaWorkflowDefinicionPorCordigo(workflow);
            var servicioWf = factory.CrearServicio(workflowDefinicionId);
            var resultadoActividad = servicioWf.IngresarOrdenCargaInterna(orden, datosUsuario.CentroId, workflow, workflowDefinicionId, datosUsuario.NombreUsuario, controlRecorrido) as ResultadoCrearWorkflow;
            
            if (!resultadoActividad.HayErrores)
            {
                return RedirectToAction("Index", "ListaDeCamiones", new { id = resultadoActividad.InstanciaWorkflowId });
            }

            ModelState.AgregarErrores(resultadoActividad);
            SetearVista(workflowObje, datosUsuario.CentroId);
            return View(orden);
        }

        private void SetearVista(WorkflowDto workflow, int centroId)
        {
            SetearVista(workflow, centroId, servicio, this);
        }

        public static void SetearVista(WorkflowDto workflow, int centroId, IServicioRepositorio servicio, ControllerBase controller)
        {
            var tiposComerciales = servicio.ListarTiposComercialesPorWfCodigo(workflow.Codigo);
            var pesoMaximoPorTipoVehiculo = servicio.ListarPesoMaximoPorTipoVehiculoPorCentro(centroId);
            var materiales = servicio.ListarMaterialesPorWorkflow(workflow.Id, centroId);

            controller.ViewBag.TiposComerciales = tiposComerciales.ToSelectList(f => f.Id.Value.ToString(CultureInfo.InvariantCulture), f => f.Descripcion);
            controller.ViewBag.TiposComercialesTransportista = tiposComerciales.Where(x => !x.TransportistaEsProveedor).Select(y => y.Id.ToString()).ToList();
            controller.ViewBag.Materiales = materiales.ToSelectList(f => f.MaterialId.ToString(), f => f.MaterialDesc);
            controller.ViewBag.TiposDocumentos = servicio.ListarTiposDocumentoIdentidad().ToSelectList(f => f.Id.ToString(), f => f.DescripcionCorta);
            var almacenes = servicio.ListarAlmacenesPorCentro(centroId);
            controller.ViewBag.Almacenes = almacenes.GroupBy(x => x.Descripcion).Select(x => new AlmacenDto { Id = x.Select(y => y.Id).FirstOrDefault(), Descripcion = x.Key }).ToSelectList(f => f.Id.ToString(), f => f.Descripcion);
            var calles = servicio.ListarCalles(centroId);
            controller.ViewBag.Calles = calles.GroupBy(x => x.Nombre).Select(x => new CalleDto { Id = x.Select(y => y.Id).FirstOrDefault(), Nombre = x.Key }).ToSelectList(f => f.Id.ToString(), f => f.Nombre,"3");
            controller.ViewBag.Workflow = workflow.Codigo;
            controller.ViewBag.CentroId = centroId;
            controller.ViewBag.TiposVehiculo = pesoMaximoPorTipoVehiculo.Where(x => x.TipoVehiculo != TipoVehiculo.Tren ).ToSelectList(f => ((int)f.TipoVehiculo).ToString(), f => f.TipoVehiculo.DisplayText());
            controller.ViewBag.WorkflowId = workflow.Id;
            controller.ViewBag.WorkflowDescripcion = workflow.Descripcion;
            controller.ViewBag.MaterialesDerivadoGranario = materiales.Where(x => x.EsDerivadoGranario).Select(x => x.MaterialId).ToList();
        }

        [HttpGet]
        [DatosUsuario]
        public JsonResult ObtenerAlmacenesPorMaterial(int materialId, DatosUsuario datosUsuario)
        {
            var almacenes = servicio.ListarAlmacenesPorMaterial(datosUsuario.CentroId, materialId).OrderBy(c => c.Descripcion).Select(x => new AlmacenDto { Id = x.Id, Descripcion = x.Descripcion });
            var almacenesList = almacenes.ToSelectList(f => f.Id.ToString(), f => f.Descripcion);
            return Json(almacenes, JsonRequestBehavior.AllowGet);
        }

        private void Validar(OrdenCargaInternaDto orden)
        {
            var material = servicio.ObtenerMaterial(orden.MaterialId);
            orden.DerivadoGranarioHabilitado = material.EsDerivadoGranario;
            if (material != null && material.Descripcion == "RESIDUOS ORGANICOS" && orden.Almacen_Id == null)
            {
                ModelState.AddModelError("Almacen_Id", Textos.OrdenInterna_AlmacenRequerido);
            }

            if (material.EsDerivadoGranario && !orden.PlantaDGDestino.HasValue)
            {
                ModelState.AddModelError("PlantaDGDestino", string.Format(Textos.Error_Requerido, Textos.OrdenCarga_PlantaDGDestino));
            }

            if (material.EsDerivadoGranario && string.IsNullOrEmpty(orden.TipoYOrdenDestino))
            {
                ModelState.AddModelError("TipoYOrdenDestino", string.Format(Textos.Error_Requerido, Textos.OrdenCarga_TipoYOrdenDestino));
            }

            if (material.EsDerivadoGranario && (!orden.PagadorFleteId.HasValue || orden.PagadorFleteId <= 0)) 
            {
                ModelState.AddModelError("PagadorFlete", string.Format(Textos.Error_Requerido, Textos.OrdenCarga_CuitPagadorFlete));
            }
        }

        private void ConfirmarCTGVencidos(int centroId)
        {
            servicioComandos.Ejecutar(new ConfirmarCTGVencidas
            {
                CentroId = centroId,
                TipoPerfil = TipoPerfil.Solicitante,
            });
        }
    }
}