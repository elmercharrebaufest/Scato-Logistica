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
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Newtonsoft.Json;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadCargarOrdenDeDescargaFason)]
    public class CargarOrdenDeDescargaFasonController : DocumentoIngresoController
    {
        private readonly IServicioActividadFactory<ICargarOrdenDeDescargaFasonService> factory;
        private readonly IListaDeWorkflows workflows;

        public CargarOrdenDeDescargaFasonController(ILogger log, IServicioRepositorio servicio, IServicioActividadFactory<ICargarOrdenDeDescargaFasonService> factory, IServicioComandos servicioComandos, IListaDeWorkflows workflows)
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
            var ordenDeDescarga = servicio.ObtenerNumeroOrdenDeDescargaFasonGenerado(datosUsuario.CentroId);
            SetearVista(workflowObj, datosUsuario.CentroId);
            var orden = new OrdenDeDescargaFasonDto { Numero = ordenDeDescarga, FechaOD = DateTime.Today };
            if (cargaDeCupoId != 0)
            {
                var cupo = servicio.ObtenerCupoPorId(cargaDeCupoId);
                orden.PatenteCamion = cupo.Patente;
                orden.MaterialId = cupo.MaterialId;
                orden.Material = cupo.MaterialDescripcion;
            }
            return View(orden);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Index(string workflow, OrdenDeDescargaFasonDto orden, DatosUsuario datosUsuario)
        {
            var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflow);
            Validar(orden);

            if (datosUsuario.CentroId == 0)
            {
                TempData["Alerta"] = Textos.SeleccionarCentro_Error;
                TempData["TipoAlerta"] = TipoAlerta.Advertencia;
                return RedirectToAction("Index", "ListaDeCamiones");
            }

            var ordenDeDescargaFason = servicio.ObtenerOrdenDeDescargaFasonPorNumeroDeOrden(orden.Numero);
            if (ordenDeDescargaFason != null)
            {
                ModelState.AddModelError("OrdenDeDescargaFason", Textos.OrdenDeDescarga_Existente);
                SetearVista(workflowObj, datosUsuario.CentroId);
                return View(orden);
            }

            var ordenDeDescarga = servicio.ObtenerOrdenDeDescargaFasonPorNumeroDeOrdenYCliente(orden.Numero, orden.ClienteId);
            if (ordenDeDescarga != null)
            {
                ModelState.AddModelError("NumeroRemito", Textos.OrdenDeDescarga_Existente);
                SetearVista(workflowObj, datosUsuario.CentroId);
                return View(orden);
            }

            var ordenDeDescargaPorRemito = servicio.ObtenerOrdenDeDescargaFasonPorNumeroRemito(orden.NumeroRemito);
            if (ordenDeDescargaPorRemito != null)
            {
                ModelState.AddModelError("NumeroRemito", Textos.IngresarRemito_Existente);
                SetearVista(workflowObj, datosUsuario.CentroId);
                return View(orden);
            }

            var otroRecorridoDelChofer = servicio.ObtenerOtroRecorridoDelChofer(orden.Chofer.Id);
            if (otroRecorridoDelChofer != null)
            {
                ModelState.AddModelError("", string.Format(Textos.Error_ChoferYaEstaEnPlanta, orden.Chofer.NombreCompleto, otroRecorridoDelChofer.NumeroDocumentoIngreso, otroRecorridoDelChofer.Patente));
                SetearVista(workflowObj, datosUsuario.CentroId);
                return View(orden);
            }

            var consultaCPEAutomotor = servicioComandos.Ejecutar(new ConsultarCPEAutomotorDG
            {
                CentroId = datosUsuario.CentroId,
                Usuario = datosUsuario.NombreUsuario,
                NumeroCTG = Convert.ToInt64(orden.NumeroCTG)
            });

            var codigoSAP = servicio.ObtenerCentroCodigoSap(datosUsuario.CentroId);
            var camara = servicio.ListarVideoCamarasPuesto(datosUsuario.PuestoDeTrabajoId).FirstOrDefault();
            ResultadoConsultaCpeAutomotorDG consulta = (ResultadoConsultaCpeAutomotorDG)consultaCPEAutomotor;

            if(consultaCPEAutomotor != null )
            {
                servicioComandos.Ejecutar(new GuardarImagenDescarga
                {
                    Pdf = consulta.Pdf,
                    TipoImagen = TipoImagen.CPEDG,
                    CodigoCentroSap = codigoSAP,
                    NroCartaPorte = orden.Numero,
                    Patente = orden.PatenteCamion,
                    Etapa = string.Empty,
                    TipoVehiculo = orden.TipoVehiculo ,
                    RutaFotoCP = camara == null ? string.Empty : camara.Directorio

                });
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
                SetearVista(workflowObj, datosUsuario.CentroId);
                return View(orden);
            }
            var transportistaId = orden.TransportistaId;
            var resultadoTransportista = SetearTransportista(ref transportistaId, orden.TipoComercialId, orden.EsTransportista);
            orden.TransportistaId = transportistaId;
            if (!resultadoTransportista)
            {
                SetearVista(workflowObj, datosUsuario.CentroId);
                return View(orden);
            }
                orden.TipoDeWorkflow = workflowObj.TipoDeWorkflow;

                orden.PatenteCamion = orden.PatenteCamion != null ? orden.PatenteCamion.ToUpper() : "";
                orden.PatenteAcoplado = orden.PatenteAcoplado != null ? orden.PatenteAcoplado.ToUpper() : "";

                int workflowDefinicionId = servicio.ObtenerUltimaWorkflowDefinicionPorCordigo(workflow);
                var servicioWf = factory.CrearServicio(workflowDefinicionId);

                var controlRecorrido = new ControlRecorridoDto
                    {
                        Actividad = Textos.ActCargarOrdenDeDescargaFason,
                        ActividadXaml = "CargarOrdenDeDescargaFason",
                        PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                        NombreUsuario = datosUsuario.NombreUsuario
                    };

                var resultadoActividad = servicioWf.CargarOrdenDeDescargaFason(orden, datosUsuario.CentroId, workflow, workflowDefinicionId, datosUsuario.NombreUsuario, controlRecorrido) as ResultadoCrearWorkflow;
                if (resultadoActividad.HayErrores)
                {
                    ModelState.AgregarErrores(resultadoActividad);
                    SetearVista(workflowObj, datosUsuario.CentroId);
                    return View(orden);
                }

               

                return RedirectToAction("Index", "ListaDeCamiones", new { id = resultadoActividad.InstanciaWorkflowId });
        }

        private void Validar(OrdenDeDescargaFasonDto orden)
        {
            var material = servicio.ObtenerMaterial(orden.MaterialId);

            if (!material.EsDerivadoGranario && orden.ProcedenciaId == 0)
            {
                ModelState.AddModelError("Procedencia", string.Format(Textos.Error_Requerido, Textos.Procedencia));
            }
            if (material.EsDerivadoGranario && orden.DomicilioId == 0)
            {
                ModelState.AddModelError("Procedencia", string.Format(Textos.Error_Requerido, Textos.Procedencia));
            }
        }

        private void SetearVista(WorkflowDto workflow, int centroId)
        {
            SetearVista(workflow, servicio, this, centroId);
        }

        public static void SetearVista(WorkflowDto workflow, IServicioRepositorio servicio, ControllerBase controller, int centroId)
        {
            var tiposComerciales = servicio.ListarTiposComercialesPorWfCodigo(workflow.Codigo);
            var materiales = servicio.ListarMaterialesPorWorkflow(workflow.Id, centroId);

            controller.ViewBag.TiposComerciales = tiposComerciales.ToSelectList(f => f.Id.Value.ToString(CultureInfo.InvariantCulture), f => f.Descripcion);
            controller.ViewBag.TiposComercialesTransportista = tiposComerciales.Where(x => !x.TransportistaEsProveedor).Select(y => y.Id.ToString()).ToList();
            controller.ViewBag.Materiales = materiales.ToSelectList(f => f.MaterialId.ToString() , f => f.MaterialDesc);
            controller.ViewBag.TiposDocumentos = servicio.ListarTiposDocumentoIdentidad().ToSelectList(f => f.Id.ToString(), f => f.DescripcionCorta);
            controller.ViewBag.Workflow = workflow.Codigo;
            controller.ViewBag.WorkflowId = workflow.Id;
            controller.ViewBag.EsIngreso = workflow.TipoDeWorkflow == TipoDeWorkflow.Ingreso;
            controller.ViewBag.CentroId = centroId;
            controller.ViewBag.WorkflowId = workflow.Id;
            
            
            controller.ViewBag.TipoMateriales = JsonConvert.SerializeObject(materiales.Select(s => new { Id = s.MaterialId.ToString(), EsDerivadoGranario = s.EsDerivadoGranario }));

        }
    }
}