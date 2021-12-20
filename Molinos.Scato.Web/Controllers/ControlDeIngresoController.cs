using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadControlDeIngreso)]
    public class ControlDeIngresoController : DocumentoIngresoController
    {
        private readonly IServicioActividadFactory<IControlDeIngresoService> factory;

        public ControlDeIngresoController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos, IServicioActividadFactory<IControlDeIngresoService> factory)
            : base(log, servicio, servicioComandos)
        {
            this.factory = factory;
        }

        public ActionResult Index(Guid id)
        {
            var recorrido = servicio.ObtenerRecorridoPorGuid(id);
            var model = new ModificarDocumentoDeIngresoDto
                {
                    NumeroDocumentoIngreso = recorrido.NumeroDocumentoIngreso,
                    TipoDocumentoIngreso = recorrido.TipoDocumentoIngreso
                };

            if (!recorrido.EsModificable)
            {
                TempData["Alerta"] = "NumeroDocumentoIngreso" + Textos.Error_NoModificacion;
                TempData["TipoAlerta"] = TipoAlerta.Advertencia;
                return RedirectToAction("Index", "ListaDeCamiones");
            }

            var documentoDeIngreso = servicio.ObtenerOrdenDeDescargaFasonPorNumeroDeOrden(model.NumeroDocumentoIngreso);

            if (documentoDeIngreso != null)
            {
                SetearVistaOrdenDeDescargaFason(documentoDeIngreso.Id, recorrido.Workflow, recorrido.Centro.Id, recorrido.WorkflowDefinicionId, id);
            }
            else
            {
                TempData["Alerta"] = "NumeroDocumentoIngreso" + Textos.Error_Invalido;
                TempData["TipoAlerta"] = TipoAlerta.Advertencia;
                return RedirectToAction("Index", "ListaDeCamiones");
            }

            var tipoComercial = servicio.ObtenerTipoComercial(documentoDeIngreso.TipoComercialId);
            if (tipoComercial.TransportistaEsProveedor)
            {
                var transportista = servicio.ObtenerTransportista(documentoDeIngreso.TransportistaId);
                var proveedor = servicio.ObtenerProveedorPorCuit(transportista.Cuit, new TiposProveedor {PR = true});
                if (proveedor != null)
                {
                    documentoDeIngreso.TransportistaId = proveedor.Id;
                    documentoDeIngreso.Transportista = proveedor.Descripcion;
                }                
            }

            ViewBag.SoloLectura = "false";
            return View("OrdenDeDescargaFason", documentoDeIngreso);
        }

        private void SetearVistaOrdenDeDescargaFason(int docId, WorkflowDto workflow, int centroId, int workflowDefinicionId, Guid id)
        {
            ViewBag.SoloLectura = "false";
            ViewBag.Guid = id;
            ViewBag.Workflow = workflow.Codigo;
            ViewBag.WorkflowDefinicionId = workflowDefinicionId;
            ViewBag.DocId = docId;
            
            var tiposComerciales = servicio.ListarTiposComercialesPorWfCodigo(workflow.Codigo);
            var materiales = servicio.ListarMaterialesPorWorkflow(workflow.Id, centroId);
            var pesoMaximoPorTipoVehiculo = servicio.ListarPesoMaximoPorTipoVehiculoPorCentro(centroId);

            ViewBag.TiposComerciales = tiposComerciales.ToSelectList(f => f.Id.Value.ToString(CultureInfo.InvariantCulture), f => f.Descripcion);
            ViewBag.TiposComercialesTransportista = tiposComerciales.Where(x => !x.TransportistaEsProveedor).Select(y => y.Id.ToString()).ToList();
            ViewBag.Materiales = materiales.ToSelectList(f => f.MaterialId.ToString(), f => f.MaterialDesc);
            ViewBag.TiposDocumentos = servicio.ListarTiposDocumentoIdentidad().ToSelectList(f => f.Id.ToString(), f => f.DescripcionCorta);
            ViewBag.Workflow = workflow.Codigo;
            ViewBag.EsIngreso = workflow.TipoDeWorkflow == TipoDeWorkflow.Ingreso;
            ViewBag.TiposVehiculo = pesoMaximoPorTipoVehiculo.Where(x => x.TipoVehiculo != TipoVehiculo.Tren).ToSelectList(f => f.TipoVehiculo.DisplayEnum(), f => f.TipoVehiculo.DisplayText());
        }

        [DatosUsuario]
        public ActionResult OrdenDeDescargaFason(string workflow, OrdenDeDescargaFasonDto orden, Guid guid, DatosUsuario datosUsuario, int docId)
        {
            orden.Id = docId;
            var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflow);
            var recorrido = servicio.ObtenerRecorridoPorGuid(guid);

            var resultadoChofer = SetearChofer(orden.Chofer);
            if (resultadoChofer == false)
            {
                SetearVistaOrdenDeDescargaFason(docId, workflowObj, workflowObj.CentroId, recorrido.WorkflowDefinicionId, guid);
                return View(orden);
            }
            var transportistaId = orden.TransportistaId;
            var resultadoTransportista = SetearTransportista(ref transportistaId, orden.TipoComercialId, orden.EsTransportista);
            orden.TransportistaId = transportistaId;
            if (!resultadoTransportista)
            {
                SetearVistaOrdenDeDescargaFason(docId, workflowObj, workflowObj.CentroId, recorrido.WorkflowDefinicionId, guid);
                return View(orden);
            }

            orden.TipoDeWorkflow = workflowObj.TipoDeWorkflow;

            orden.PatenteCamion = orden.PatenteCamion != null ? orden.PatenteCamion.ToUpper() : "";
            orden.PatenteAcoplado = orden.PatenteAcoplado != null ? orden.PatenteAcoplado.ToUpper() : "";

            var controlRecorrido = new ControlRecorridoDto
            {
                WorkflowInstanceId = guid,
                NombreUsuario = datosUsuario.NombreUsuario,
                Actividad = Textos.ActControlDeIngreso,
                ActividadXaml = "ControlDeIngreso",
                Decision = false,
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId
            };

            var workflowDefinicionId = servicio.ObtenerUltimaWorkflowDefinicionPorCordigo(workflow);
            var servicioWf = factory.CrearServicio(workflowDefinicionId);
            var resultadoActividad = servicioWf.ControlDeIngreso(orden, false, controlRecorrido, guid);

            if (resultadoActividad.HayErrores)
            {
                ModelState.AgregarErrores(resultadoActividad);
                SetearVistaOrdenDeDescargaFason(docId, workflowObj, workflowObj.CentroId, recorrido.WorkflowDefinicionId, guid);
                return View(orden);
            }
            return RedirectToAction("Index", "ListaDeCamiones");
        }

        [DatosUsuario]
        public ActionResult Rechazar(string codigoWf, int workflowDefinicionId, Guid instanceId, DatosUsuario datosUsuario)
        {
            ViewBag.Motivos = servicio.ListarMotivos().ToSelectList(x => x.Descripcion, x => x.Descripcion);
            ViewBag.Workflow = codigoWf;
            ViewBag.WorkflowDefinicionId = workflowDefinicionId;
            var controlRecorrido = new ControlRecorridoDto
            {
                WorkflowInstanceId = instanceId,
                NombreUsuario = datosUsuario.NombreUsuario,
                Actividad = Textos.ActControlDeIngreso + "/" + Textos.Rechazar,
                ActividadXaml = "ControlDeIngreso",
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
            };

            return View("_TransportistaRechazado", controlRecorrido);
        }

        [DatosUsuario]
        public ActionResult TransportistaRechazado(string workflow, int workflowDefinicionId, ControlRecorridoDto controlRecorrido)
        {
            controlRecorrido.Decision = true;
            var guid = controlRecorrido.WorkflowInstanceId;
            var orden = new OrdenDeDescargaFasonDto();
            var serivce = factory.CrearServicio(workflowDefinicionId);
            var resultado = serivce.ControlDeIngreso(orden, true, controlRecorrido, guid);
            if (!resultado.HayErrores)
            {
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            ModelState.AgregarErrores(resultado);
            return RedirectToAction("Index", new { id = controlRecorrido.WorkflowInstanceId });
        }

    }
}
